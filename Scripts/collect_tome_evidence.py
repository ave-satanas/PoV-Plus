"""Bounded, read-only INV-003 collection. Never enumerate caches or save directories."""
from __future__ import annotations

import argparse
import datetime as dt
import hashlib
import json
from pathlib import Path
import re

REPOSITORY = Path(__file__).resolve().parent.parent
INSTALLATION = Path("S:/Valheim-Modding/PoVPlus-Dev")
SESSION = re.compile(r"\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}\Z")
MAX_BYTES = 8 * 1024 * 1024
MARKER = "[PoVPlus.TomeDiagnostic]"
BRIDGE_MARKER = "[PoVPlus.TomeBridge]"
LIFECYCLE_MARKER = "[PoVPlus.TomeLifecycle]"


def redact(text: str) -> str:
    text = re.sub(r"(?i)[A-Z]:\\Users\\[^\\\s]+", "<profile>", text)
    text = re.sub(r"(?i)\b(password|token|secret|api_?key)\s*[=:]\s*[^\s,;]+", r"\1=<redacted>", text)
    return re.sub(r"\b\d{15,20}\b", "<identifier>", text)[:1600]


def read_source(path: Path, root: Path, result: dict) -> str | None:
    """Optional failures are evidence, not reasons to change permissions or abort all collection."""
    try:
        if not path.resolve().is_relative_to(root.resolve()):
            raise ValueError("Path escapes selected evidence root")
        before = path.stat()
        if before.st_size > MAX_BYTES:
            raise ValueError("Evidence exceeds bounded size limit")
        with path.open("rb") as stream:
            raw = stream.read(MAX_BYTES + 1)
        after = path.stat()
        if len(raw) > MAX_BYTES or (before.st_size, before.st_mtime_ns) != (after.st_size, after.st_mtime_ns):
            raise ValueError("Source changed during read; collect again explicitly")
        result["sources"].append(dict(path=str(path), bytes=len(raw),
            modified_utc=dt.datetime.fromtimestamp(after.st_mtime, dt.timezone.utc).isoformat(),
            sha256=hashlib.sha256(raw).hexdigest().upper()))
        return raw.decode("utf-8-sig", errors="replace")
    except (OSError, ValueError) as error:
        result["skipped"].append(dict(path=str(path), reason=type(error).__name__, detail=redact(str(error))))
        return None


def collect(installation: Path, session: str, receipts: list[Path], repository: Path = REPOSITORY) -> dict:
    if not SESSION.fullmatch(session):
        raise ValueError("Use an explicit YYYY-MM-DD_HH-MM-SS profiler session identifier")
    result = dict(schema=1, captured_utc=dt.datetime.now(dt.timezone.utc).isoformat(), session=session,
        status="incomplete", sources=[], skipped=[], diagnostic=[], errors=[], receipts=[], profile=[])
    log = read_source(installation / "BepInEx/LogOutput.log", installation, result)
    if log is not None:
        lines = log.splitlines()
        starts = [i for i, text in enumerate(lines) if re.match(r"^\[Message:\s*BepInEx\] BepInEx \d", text)]
        if starts:
            start = starts[-1]
            current = lines[start:]
            refs = [line for line in current if "Writing startup and connection profiles" in line]
            referenced = re.findall(r"\d{4}-\d{2}-\d{2}_\d{2}-\d{2}-\d{2}\.log", refs[-1]) if refs else []
            if referenced and referenced[-1] == session + ".log":
                result.update(status="matched", session_start_line=start + 1)
                for i, line in enumerate(current, start + 1):
                    bucket = "diagnostic" if MARKER in line or BRIDGE_MARKER in line or LIFECYCLE_MARKER in line else "errors" if line.startswith("[Error") else None
                    if bucket:
                        if len(result[bucket]) < 256:
                            result[bucket].append(dict(line=i, text=redact(line)))
                        else:
                            result["evidence_truncated"] = True
                profile_path = installation / "BepInEx/config/LoadTimeProfiler" / (session + ".log")
                profile = read_source(profile_path, installation, result)
                expected_time = session.replace("_", " ").rsplit(" ", 1)
                expected_time = expected_time[0] + " " + expected_time[1].replace("-", ":")
                if profile is not None:
                    if "Session: " + expected_time not in profile:
                        result["skipped"].append(dict(path=str(profile_path), reason="ProfilerSessionMismatch"))
                    else:
                        result["profile"] = [redact(line) for line in profile.splitlines()
                            if line.startswith(("Session:", "Result:", "Logging ", "Total:"))][:24]
            else:
                result["status"] = "session-mismatch"
        else:
            result["status"] = "no-session-boundary"
    for path in receipts:
        allowed = repository / ".dev-state/backups"
        if path.name != "manifest.json" or not path.resolve().is_relative_to(allowed.resolve()):
            result["skipped"].append(dict(path=str(path), reason="NotWorkflowReceipt"))
            continue
        text = read_source(path, allowed, result)
        if text is None:
            continue
        try:
            data = json.loads(text)
            selected = {key: data.get(key) for key in ("Id", "Operation", "CreatedUtc", "Status", "Restores")}
            for phase in ("Before", "After"):
                selected[phase] = {key: data.get(phase, {}).get(key) for key in ("Exists", "Hash", "Version")}
            result["receipts"].append(selected)
        except (ValueError, TypeError, AttributeError):
            result["skipped"].append(dict(path=str(path), reason="InvalidReceipt"))
    texts = [row["text"] for row in result["diagnostic"]]
    result["counts"] = {event: sum(MARKER + " " + event + " " in line for line in texts)
        for event in ("ARMED", "GUARD", "BEGIN", "END", "COMPLETE", "ABORT", "RENDERED", "VISIBLE_ACCEPT")}
    result["snapshot_complete"] = result["counts"]["END"] > 0 and result["counts"]["ABORT"] == 0
    result["lifecycle"] = {
        "observations": sum(LIFECYCLE_MARKER + " OBSERVATION " in line for line in texts),
        "stops": sum(LIFECYCLE_MARKER + " STOP " in line for line in texts),
        "interpretation": "Observation evidence only; STOP is not neutral-state or lifecycle success."
    }
    result["interpretation"] = "Observed messages only; false, skipped and unavailable are distinct; no gameplay diagnosis inferred."
    return result


def write_report(result: dict, output: Path, repository: Path = REPOSITORY) -> None:
    allowed = (repository / ".dev-state/reports").resolve()
    output = output.resolve()
    if not output.is_relative_to(allowed) or output == allowed:
        raise ValueError("Reports must remain under repository .dev-state/reports/")
    json_path, markdown_path = output.with_suffix(".json"), output.with_suffix(".md")
    if json_path.exists() or markdown_path.exists():
        raise FileExistsError("Refusing to overwrite an existing evidence report")
    output.parent.mkdir(parents=True, exist_ok=True)
    with json_path.open("x", encoding="utf-8") as stream:
        json.dump(result, stream, indent=2)
    with markdown_path.open("x", encoding="utf-8") as stream:
        stream.write(f"# INV-003 diagnostic evidence\n\nSession: {result['session']}\n\n")
        stream.write(f"Collected UTC: {result['captured_utc']}\n\nStatus: {result['status']}\n\n")
        stream.write("Counts: " + json.dumps(result["counts"]) + "\n\n")
        stream.write("Lifecycle: " + json.dumps(result.get("lifecycle", {})) + "\n\n")
        for bucket in ("sources", "skipped", "profile", "receipts", "diagnostic", "errors"):
            stream.write("## " + bucket + "\n\n")
            for item in result[bucket]:
                stream.write("- " + redact(json.dumps(item) if not isinstance(item, str) else item) + "\n")
            stream.write("\n")
        stream.write(result["interpretation"] + "\n")


def main() -> int:
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--session", required=True)
    parser.add_argument("--receipt", type=Path, action="append", default=[])
    parser.add_argument("--output", type=Path, required=True)
    args = parser.parse_args()
    result = collect(INSTALLATION, args.session, args.receipt)
    write_report(result, args.output)
    print(json.dumps({key: result[key] for key in ("status", "counts", "snapshot_complete", "lifecycle")}))
    return 0 if result["status"] == "matched" else 2


if __name__ == "__main__":
    raise SystemExit(main())
