"""DEV-008 offline policy model. No launch, input, deployment or recovery executor.

Evidence arguments are synthetic/untrusted at the adapter boundary. These checks
test necessary conditions, not attestation of real Windows or Valheim isolation.
"""
from dataclasses import dataclass, fields, replace
import math
from pathlib import Path, PureWindowsPath
import re
import stat


class Blocked(ValueError):
    pass


def fixture_path(root, relative):
    """Existing-tree policy check, not a race-safe filesystem capability.

    No writes. Caller must own a quiescent fixture tree. A future real executor
    needs handle-based Windows checks and ACL isolation, not check-then-open.
    """
    if type(relative) is not str or not relative or len(relative) > 240:
        raise Blocked("invalid relative path")
    win = PureWindowsPath(relative)
    parts = re.split(r"[\\/]", relative)
    if win.drive or win.root or any(
        not p or p in (".", "..") or p.endswith((" ", ".")) or
        re.search(r'[<>:"|?*\x00-\x1f]', p) or
        p.split(".")[0].upper() in {"CON", "PRN", "AUX", "NUL", *(
            f"{prefix}{i}" for prefix in ("COM", "LPT") for i in range(1, 10))}
        for p in parts
    ):
        raise Blocked("unsupported path syntax")
    root = Path(root).absolute()
    current = Path(root.anchor)
    for part in (*root.parts[1:], *parts):
        current /= part
        try:
            info = current.lstat()
        except FileNotFoundError:
            raise Blocked("fixture path must already exist") from None
        if stat.S_ISLNK(info.st_mode) or getattr(info, "st_file_attributes", 0) & 0x400:
            raise Blocked("symlink/reparse path")
        if stat.S_ISREG(info.st_mode) and info.st_nlink != 1:
            raise Blocked("hard-linked file")
    if not current.resolve(strict=True).is_relative_to(root.resolve(strict=True)):
        raise Blocked("outside fixture root")
    return current


@dataclass(frozen=True)
class Target:
    pid: int
    started: str
    executable: str
    executable_hash: str
    window: int
    session: int


@dataclass(frozen=True)
class Observation:
    target: Target
    foreground: int
    observed_at: float
    screen: str
    unlocked: bool
    geometry: tuple
    screenshot: str


@dataclass(frozen=True)
class InputBudget:
    remaining: int = 5
    stopped: bool = False
    last_screenshot: str = ""

    def plan(self, expected, observed, now, geometry, action):
        # Pure decision only. There is deliberately no SendInput/cua adapter.
        if self.stopped or type(self.remaining) is not int or not 0 < self.remaining <= 20:
            raise Blocked("stopped or exhausted")
        if action not in {"single_click", "single_key"}:
            raise Blocked("unbounded or unsupported action")
        if (expected != observed.target or expected.pid <= 0 or
                expected.window <= 0 or not expected.started or
                not expected.executable_hash or not expected.executable):
            raise Blocked("target identity mismatch")
        if (not math.isfinite(now) or not math.isfinite(observed.observed_at) or
                not 0 <= now - observed.observed_at <= 0.5):
            raise Blocked("stale/invalid observation")
        if (not observed.unlocked or observed.foreground != expected.window or
                observed.geometry != geometry or not observed.screenshot or
                observed.screenshot == self.last_screenshot or
                observed.screen not in {"synthetic_ready", "synthetic_result"}):
            raise Blocked("uncertain desktop state")
        return replace(self, remaining=self.remaining - 1, last_screenshot=observed.screenshot)


def validate_scenario(data):
    required = {"schema", "id", "level", "baseline", "character", "world",
                "max_actions", "timeout_seconds", "evidence", "recovery"}
    if type(data) is not dict or len(data) != len(required) or set(data) != required:
        raise Blocked("scenario schema/unknown field")
    if data["schema"] != 1 or type(data["schema"]) is not int:
        raise Blocked("scenario schema version")
    if data["level"] != 0 or type(data["level"]) is not int:
        raise Blocked("offline model cannot authorize runtime")
    for field in ("id", "baseline"):
        if type(data[field]) is not str or not re.fullmatch(r"[a-z0-9-]{1,64}", data[field]):
            raise Blocked("invalid identifier")
    if data["character"] != "PoVPlus_Test" or data["world"] != "PoVPlus_Lab":
        raise Blocked("scenario names mismatch; names alone are not isolation")
    for field, cap in (("max_actions", 20), ("timeout_seconds", 300)):
        if type(data[field]) is not int or not 1 <= data[field] <= cap:
            raise Blocked("invalid bound")
    if (type(data["evidence"]) is not list or
            len(data["evidence"]) != 3 or
            any(type(x) is not str for x in data["evidence"]) or
            set(data["evidence"]) != {"before", "after", "recovery"} or
            data["recovery"] != "discard-generation"):
        raise Blocked("missing independent evidence/recovery policy")
    return True


@dataclass(frozen=True)
class RecoveryEvidence:
    baseline_hashes_match: bool = False
    complete_scope: bool = False
    writer_exclusion: bool = False
    stopped: bool = False
    evidence_sealed: bool = False
    journal_committed: bool = False
    restored_exact: bool = False
    external_sync_absent: bool = False


def recovery_complete(evidence):
    # Assertions are NOT independently authenticated by this model.
    return type(evidence) is RecoveryEvidence and all(
        getattr(evidence, field.name, None) is True for field in fields(RecoveryEvidence))


@dataclass(frozen=True)
class RunModel:
    phase: str = "unprepared"

    def step(self, event, *, evidence=None):
        if event in {"abort", "timeout", "focus_lost", "unexpected", "emergency_stop"}:
            return replace(self, phase="quarantined")
        transitions = {
            ("unprepared", "validate_baseline"): "ready",
            ("ready", "begin_fixture"): "observing",
            ("observing", "seal_evidence"): "recovering",
            ("recovering", "verify_recovery"): "complete",
        }
        target = transitions.get((self.phase, event))
        if target is None:
            raise Blocked("invalid transition")
        if event in {"validate_baseline", "verify_recovery"} and (
            type(evidence) is not RecoveryEvidence or not recovery_complete(evidence)
        ):
            raise Blocked("missing/unknown recovery prerequisite")
        return replace(self, phase=target)


def score_experiment(*, issued, observed, recovery):
    """Issued actions are audit history, never the success oracle."""
    if recovery is not True:
        return "blocked"
    if observed is True:
        return "pass"
    if observed is False:
        return "fail"
    return "unknown"
