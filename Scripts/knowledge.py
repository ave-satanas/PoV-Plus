"""Dependency-free knowledge tools. Frontmatter uses the JSON-compatible YAML subset."""
import argparse
import datetime as dt
import hashlib
import json
from pathlib import Path
import re
import sys

ROOT = Path(__file__).resolve().parents[1]
KNOWLEDGE = ROOT / "Knowledge"
REQUIRED = {"id", "title", "type", "status", "created", "last_verified", "versions",
            "evidence", "related", "confidence", "watch", "claims", "contradicts", "supersedes"}
TYPES = {"index", "architecture", "mod", "dependency", "configuration", "compatibility",
         "diagnostic", "investigation", "decision", "experiment", "development", "session", "reference"}
STATUSES = {"active", "partial", "open", "proposed", "resolved", "superseded", "historical"}
CONFIDENCE = {"verified", "mixed", "hypothesis", "unverified", "decision"}
LINK = re.compile(r"\[\[([^\]|#]+)(?:#[^\]|]+)?(?:\|[^\]]+)?\]\]")


def read_record(path):
    text = path.read_text(encoding="utf-8-sig")
    if not text.startswith("---\n"):
        raise ValueError("missing YAML frontmatter")
    head, body = text[4:].split("\n---\n", 1)
    meta = {}
    for line in head.splitlines():
        key, sep, value = line.partition(":")
        if not sep or key in meta:
            raise ValueError("invalid or duplicate frontmatter key: " + line)
        meta[key] = json.loads(value.strip())
    return meta, body, text


def write_record(path, meta, body):
    path.parent.mkdir(parents=True, exist_ok=True)
    path.write_text("---\n" + "\n".join(k + ": " + json.dumps(v, ensure_ascii=False)
                    for k, v in meta.items()) + "\n---\n\n" + body.lstrip("\n").rstrip() + "\n", encoding="utf-8")


def fingerprints(snapshot):
    result = {}
    for category, prefix in (("assemblies", "dll"), ("configurations", "cfg"), ("repository_sources", "src"), ("packages", "pkg")):
        for item in snapshot.get(category, []):
            result[prefix + ":" + item["path"]] = item["sha256"]
    return result


def validate(args):
    errors, warnings, records = [], [], {}
    for path in sorted(KNOWLEDGE.rglob("*.md")):
        try:
            meta, body, text = read_record(path)
            missing = REQUIRED - meta.keys()
            if missing:
                raise ValueError("missing metadata: " + ", ".join(sorted(missing)))
            if not re.fullmatch(r"[A-Za-z0-9][A-Za-z0-9-]*", meta["id"]):
                raise ValueError("invalid ID")
            if meta["id"].casefold() in {r[1]["id"].casefold() for r in records.values()}:
                raise ValueError("duplicate ID: " + meta["id"])
            if meta["type"] not in TYPES or meta["status"] not in STATUSES or meta["confidence"] not in CONFIDENCE:
                raise ValueError("invalid type/status/confidence")
            for field in ("evidence", "related", "watch", "contradicts", "supersedes"):
                if not isinstance(meta[field], list):
                    raise ValueError(field + " must be an array")
            for field in ("versions", "claims"):
                if not isinstance(meta[field], dict):
                    raise ValueError(field + " must be an object")
            if not meta["title"] or not meta["evidence"]:
                raise ValueError("title and evidence must be nonempty")
            created = dt.date.fromisoformat(meta["created"])
            verified = dt.date.fromisoformat(meta["last_verified"])
            if created > verified or verified > dt.date.today():
                raise ValueError("inconsistent verification dates")
            if (dt.date.today() - verified).days > args.max_age and meta["status"] not in {"historical", "superseded"}:
                warnings.append(f"STALE {path.stem}: last verified {verified}")
            records[path] = (path, meta, body, text)
        except (ValueError, KeyError, TypeError) as exc:
            errors.append(f"{path.relative_to(ROOT)}: {exc}")
    names = {}
    for path in records:
        for name in (path.stem, path.relative_to(KNOWLEDGE).with_suffix("").as_posix()):
            names.setdefault(name.casefold(), set()).add(path)
    edges = {p: set() for p in records}
    for path, (_, meta, body, text) in records.items():
        for target in LINK.findall(text):
            matches = names.get(target.casefold(), set())
            if len(matches) != 1:
                errors.append(f"{path.stem}: broken/ambiguous link [[{target}]]")
            else:
                edges[path].update(matches)
        for field in ("related", "contradicts", "supersedes"):
            for target in meta[field]:
                if not LINK.fullmatch(target):
                    errors.append(f"{path.stem}: {field} must contain wikilinks: {target}")
        if meta["status"] == "superseded" and not any(
                f"[[{path.stem}]]" in r[1]["supersedes"] for r in records.values()):
            warnings.append(f"{path.stem}: superseded without a replacement record")
        if meta["contradicts"]:
            warnings.append(f"CONTRADICTION REVIEW {path.stem}: {', '.join(meta['contradicts'])}")
    # All records must be reachable from INDEX, not just have a self/back link.
    seen, pending = set(), [KNOWLEDGE / "INDEX.md"]
    while pending:
        path = pending.pop()
        if path not in seen:
            seen.add(path)
            pending.extend(edges.get(path, set()) - seen)
    for path in records.keys() - seen:
        warnings.append("ORPHAN " + path.stem)
    claims = {}
    for path, (_, meta, _, _) in records.items():
        if meta["status"] in {"historical", "superseded"}:
            continue
        for key, value in meta["claims"].items():
            # A claim key includes its scope; differing versions do not silently reconcile.
            if key in claims and claims[key][0] != value:
                errors.append(f"contradictory active claim {key}: {claims[key][1]} vs {path.stem}")
            claims[key] = (value, path.stem)
    baseline_path = KNOWLEDGE / "Dependencies/environment-snapshot.json"
    baseline = json.loads(baseline_path.read_text(encoding="utf-8-sig"))
    old = fingerprints(baseline)
    for path, (_, meta, _, _) in records.items():
        for key in meta["watch"]:
            if key not in old:
                errors.append(f"{path.stem}: unknown watch fingerprint {key}")
    # Always check watched local source files without reading the game installation.
    changed = set()
    for key, value in old.items():
        if key.startswith("src:"):
            file = ROOT / key[4:]
            if not file.exists() or hashlib.sha256(file.read_bytes()).hexdigest().upper() != value:
                changed.add(key)
    if args.compare_snapshot:
        candidate = json.loads(Path(args.compare_snapshot).read_text(encoding="utf-8-sig"))
        new = fingerprints(candidate)
        changed.update(k for k in old.keys() | new.keys() if old.get(k) != new.get(k))
        if changed:
            warnings.append(f"ENVIRONMENT DRIFT: {len(changed)} added/removed/changed fingerprints; review inventory and affected records")
    for path, (_, meta, _, _) in records.items():
        affected = set(meta["watch"]) & changed
        if affected:
            warnings.append(f"REVALIDATE {path.stem}: {', '.join(sorted(affected))}")
    queue = [(p.stem, m["status"]) for p, (_, m, _, _) in records.items()
             if m["type"] in {"investigation", "experiment"} and m["status"] in {"open", "partial", "proposed"}]
    print(f"Validated {len(records)} Markdown records: {len(errors)} errors, {len(warnings)} warnings.")
    for item in errors:
        print("ERROR " + item)
    for item in warnings:
        print("WARNING " + item)
    for name, status in queue:
        print(f"QUEUE {status}: {name}")
    print("Semantic contradictions outside structured claims require human review. No live installation reads performed.")
    return 1 if errors or (args.strict and warnings) else 0


def new(args):
    path = KNOWLEDGE / args.folder / (args.id + "-" + args.slug + ".md")
    if path.exists() or not re.fullmatch(r"[A-Za-z0-9-]+", args.id + args.slug):
        raise ValueError("destination exists or ID/slug invalid")
    if path.resolve().is_relative_to(KNOWLEDGE.resolve()) is False:
        raise ValueError("destination must stay under Knowledge")
    today = dt.date.today().isoformat()
    meta = dict(id=args.id, title=args.title, type=args.type, status="open", created=today,
                last_verified=today, versions={}, evidence=["Pending: cite evidence before marking verified"],
                related=["[[INDEX]]"], confidence="unverified", watch=[], claims={}, contradicts=[], supersedes=[])
    write_record(path, meta, f"# {args.title}\n\n## Question\n\nState scope and expected outcome.\n\n## Evidence\n\nNot collected.\n\n## Hypotheses and unresolved questions\n\nPending investigation.\n\n## Next action\n\nIdentify the smallest safe verification step.\n")
    print(path.relative_to(ROOT))
    print("Add an inbound link from an existing record, then validate.")


def search(args):
    for path in sorted(KNOWLEDGE.rglob("*.md")):
        meta, body, text = read_record(path)
        if args.query.casefold() in text.casefold():
            print(f"{path.relative_to(ROOT)} | {meta['status']} | {meta['confidence']} | {meta['title']}")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    commands = parser.add_subparsers(dest="command", required=True)
    check = commands.add_parser("validate")
    check.add_argument("--compare-snapshot")
    check.add_argument("--max-age", type=int, default=90)
    check.add_argument("--strict", action="store_true")
    find = commands.add_parser("search")
    find.add_argument("query")
    create = commands.add_parser("new")
    create.add_argument("id")
    create.add_argument("slug")
    create.add_argument("--title", required=True)
    create.add_argument("--folder", default="Investigations")
    create.add_argument("--type", choices=sorted(TYPES), default="investigation")
    args = parser.parse_args()
    try:
        if args.command == "validate":
            return validate(args)
        if args.command == "search":
            search(args)
        else:
            new(args)
        return 0
    except (ValueError, OSError, KeyError) as exc:
        print("ERROR " + str(exc), file=sys.stderr)
        return 1


if __name__ == "__main__":
    sys.exit(main())
