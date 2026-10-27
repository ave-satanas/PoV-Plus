"""DEV-008 synthetic-only falsification. Never opens a game/save directory."""
from dataclasses import replace
import hashlib
import importlib.util
import json
import os
from pathlib import Path
import shutil
import subprocess
import sys
import tempfile
import unittest
from unittest.mock import patch

SPEC = importlib.util.spec_from_file_location("testing_policy", Path(__file__).parents[1] / "testing_policy.py")
p = importlib.util.module_from_spec(SPEC)
sys.modules[SPEC.name] = p
SPEC.loader.exec_module(p)


def hashes(root):
    result = {}
    pending = [root]
    visited = 0
    while pending:
        directory = pending.pop()
        for entry in directory.iterdir():
            visited += 1
            if visited > 100:
                raise p.Blocked("fixture inventory bound")
            relative = str(entry.relative_to(root))
            checked = p.fixture_path(root, relative)  # refuse links before descent/read
            if checked.is_dir():
                pending.append(checked)
            elif checked.is_file() and checked.stat().st_size <= 2_000_000:
                result[relative] = hashlib.sha256(checked.read_bytes()).hexdigest()
            else:
                raise p.Blocked("unsupported fixture entry")
    return result


class FixtureRecovery:
    """Test fixture only: copy immutable synthetic baseline into new generations.

    No real-environment API, deletion, installation defaults or save discovery.
    Atomic pointer publication models a single-root transaction, not Windows
    power-loss durability or a coherent multi-root/profile snapshot.
    """
    def __init__(self, root):
        self.root = root
        self.baseline = root / "baseline"
        (self.baseline / "config").mkdir(parents=True)
        (self.baseline / "config/settings.txt").write_text("synthetic baseline")
        (self.baseline / "state.txt").write_text("synthetic state")
        self.expected = hashes(self.baseline)
        self.pointer = root / "current.json"
        self.pointer.write_text(json.dumps({"generation": None, "state": "blocked"}))
        self.journal = root / "recovery.json"
        self.journal.write_text(json.dumps({"state": "unvalidated"}))

    def recover(self, generation, fault=None):
        if not generation.isdecimal():
            raise p.Blocked("invalid fixture generation")
        lock = self.root / "fixture.lock"
        # Exclusive creation. Stale lock deliberately blocks; no age-based steal.
        with lock.open("x") as lock_handle:
            try:
                self.journal.write_text(json.dumps({"state": "pending", "generation": generation}))
                if hashes(self.baseline) != self.expected:
                    raise p.Blocked("baseline drift")
                target = self.root / ("generation-" + generation)
                target.mkdir()
                for index, relative in enumerate(sorted(self.expected)):
                    source = p.fixture_path(self.baseline, relative)
                    destination = target / relative
                    destination.parent.mkdir(parents=True, exist_ok=True)
                    shutil.copyfile(source, destination)
                    if index == 0 and fault == "partial":
                        raise OSError("injected interrupted copy")
                if fault == "corrupt":
                    (target / "state.txt").write_text("injected corruption")
                if hashes(target) != self.expected:
                    raise p.Blocked("restored hash mismatch")
                if fault == "prepublish":
                    raise OSError("injected interruption before publication")
                staged = self.root / "next.json"
                staged.write_text(json.dumps({"generation": generation, "state": "verified-fixture"}))
                os.replace(staged, self.pointer)
                if fault == "postpublish":
                    raise OSError("injected interruption before journal commit")
                self.journal.write_text(json.dumps({"state": "committed", "generation": generation}))
                return target
            finally:
                lock_handle.close()  # Windows refuses unlink while our handle is open.
                lock.unlink()


class TestingPolicyTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory(prefix="povplus-dev008-")
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.target = p.Target(42, "fixture-epoch", "C:/fixture/client.exe", "fixture-hash", 7, 2)
        self.observation = p.Observation(self.target, 7, 10, "synthetic_ready", True, (800, 600, 96), "fixture-screen")
        self.proof = p.RecoveryEvidence(*([True] * 8))

    def test_target_and_bounded_input(self):
        b = p.InputBudget(1).plan(self.target, self.observation, 10.1, (800, 600, 96), "single_key")
        self.assertEqual(b.remaining, 0)
        with self.assertRaises(p.Blocked):
            b.plan(self.target, self.observation, 10.1, (800, 600, 96), "single_key")

    def test_observation_consumed_once(self):
        b = p.InputBudget().plan(self.target, self.observation, 10.1, (800, 600, 96), "single_key")
        with self.assertRaises(p.Blocked):
            b.plan(self.target, self.observation, 10.2, (800, 600, 96), "single_key")

    def test_symlink_mode_rejected_before_resolve(self):
        # Actual junction separately tested; this injects file-symlink lstat metadata.
        import stat
        fake = type("Stat", (), {"st_mode": stat.S_IFLNK, "st_file_attributes": 0})()
        with patch.object(Path, "lstat", return_value=fake), self.assertRaises(p.Blocked):
            p.fixture_path(self.root, "link")

    def test_wrong_target_each_identity_component(self):
        for field, value in dict(pid=43, started="reused", executable="other", executable_hash="other", window=8, session=3).items():
            with self.subTest(field=field), self.assertRaises(p.Blocked):
                p.InputBudget().plan(self.target, replace(self.observation, target=replace(self.target, **{field: value})), 10.1, (800, 600, 96), "single_key")

    def test_foreground_lock_geometry_screen_rejection(self):
        for field, value in dict(foreground=8, unlocked=False, geometry=(800, 600, 144), screen="unexpected_dialog", screenshot="").items():
            with self.subTest(field=field), self.assertRaises(p.Blocked):
                p.InputBudget().plan(self.target, replace(self.observation, **{field: value}), 10.1, (800, 600, 96), "single_click")

    def test_timeouts_future_nan_and_emergency(self):
        for now in [9, 11, float("nan"), float("inf")]:
            with self.subTest(now=now), self.assertRaises(p.Blocked):
                p.InputBudget().plan(self.target, self.observation, now, (800, 600, 96), "single_click")
        with self.assertRaises(p.Blocked):
            p.InputBudget(stopped=True).plan(self.target, self.observation, 10.1, (800, 600, 96), "single_click")

    def test_reject_loops_and_held_inputs(self):
        for action in ["loop", "hold_key", "type_text", "drag", "click_until_found"]:
            with self.subTest(action=action), self.assertRaises(p.Blocked):
                p.InputBudget().plan(self.target, self.observation, 10.1, (800, 600, 96), action)

    def test_race_is_not_solved_by_precheck(self):
        # Counterexample retained: foreground may change AFTER a pure decision.
        budget = p.InputBudget().plan(self.target, self.observation, 10.1, (800, 600, 96), "single_key")
        late = replace(self.observation, foreground=9)
        self.assertEqual(budget.remaining, 4)
        with self.assertRaises(p.Blocked):
            budget.plan(self.target, late, 10.2, (800, 600, 96), "single_key")
        # No actual input is issued. Adapter must acknowledge remaining race.

    def test_existing_bounded_path(self):
        (self.root / "a").mkdir()
        (self.root / "a/b.txt").write_text("fixture")
        self.assertEqual(p.fixture_path(self.root, "a/b.txt"), self.root / "a/b.txt")
        self.assertEqual(p.fixture_path(self.root, "a\\b.txt"), self.root / "a/b.txt")

    def test_path_escape_syntax(self):
        for relative in ["../other", "a/../b", "a//b", "./a", "C:foo", "C:/foo", "\\\\host\\share", "/abs", "a:b", "a.", "a ", "CON.txt", "NUL", "a\x00b", "a/*", "", "a" * 241]:
            with self.subTest(path=relative), self.assertRaises(p.Blocked):
                p.fixture_path(self.root, relative)

    def test_missing_path_not_assumed_safe(self):
        with self.assertRaises(p.Blocked):
            p.fixture_path(self.root, "absent")

    def test_hardlink_rejected(self):
        original = self.root / "original"
        original.write_text("fixture")
        os.link(original, self.root / "alias")
        with self.assertRaises(p.Blocked):
            p.fixture_path(self.root, "alias")

    def test_recovery_inventory_refuses_link_before_read(self):
        fixture = FixtureRecovery(self.root)
        sentinel = self.root / "sentinel"
        sentinel.write_text("outside baseline fixture")
        os.link(sentinel, fixture.baseline / "escape")
        with patch.object(Path, "read_bytes", side_effect=AssertionError("must not read any file")):
            with self.assertRaises(p.Blocked):
                # Directly inspect link first; ordering independent from filesystem enumeration.
                p.fixture_path(fixture.baseline, "escape")
        with self.assertRaises(p.Blocked):
            fixture.recover("1")
        self.assertEqual(sentinel.read_text(), "outside baseline fixture")

    def test_actual_junction_escape_rejected(self):
        outside = self.root / "outside"
        inside = self.root / "inside"
        outside.mkdir(); inside.mkdir()
        (outside / "sentinel").write_text("protected synthetic fixture")
        link = inside / "escape"
        if os.name == "nt":
            result = subprocess.run(["cmd", "/c", "mklink", "/J", str(link), str(outside)], capture_output=True)
            self.assertEqual(result.returncode, 0, result.stderr)
        else:
            link.symlink_to(outside, target_is_directory=True)
        try:
            with self.assertRaises(p.Blocked):
                p.fixture_path(inside, "escape/sentinel")
            self.assertEqual((outside / "sentinel").read_text(), "protected synthetic fixture")
        finally:
            if os.name == "nt":
                link.rmdir()  # removes only this checked fixture junction
            else:
                link.unlink()

    @staticmethod
    def scenario():
        return dict(schema=1, id="offline-fixture", level=0, baseline="synthetic-v1", character="PoVPlus_Test", world="PoVPlus_Lab", max_actions=5, timeout_seconds=30, evidence=["before", "after", "recovery"], recovery="discard-generation")

    def test_scenario_valid(self):
        self.assertTrue(p.validate_scenario(self.scenario()))

    def test_scenario_invalid(self):
        changes = [dict(level=1), dict(level=True), dict(schema=True), dict(max_actions=True), dict(max_actions=100), dict(timeout_seconds=0), dict(character="personal"), dict(world="wrong"), dict(baseline=""), dict(evidence=["before"]), dict(evidence=[{}]), dict(recovery="assume-empty"), dict(execute="arbitrary-code")]
        for change in changes:
            with self.subTest(change=change), self.assertRaises(p.Blocked):
                p.validate_scenario({**self.scenario(), **change})

    def test_each_unknown_recovery_obligation_blocks(self):
        self.assertFalse(p.recovery_complete(None))
        self.assertFalse(p.recovery_complete(type("Empty", (), {})()))
        for field in vars(self.proof):
            with self.subTest(field=field):
                self.assertFalse(p.recovery_complete(replace(self.proof, **{field: False})))
                with self.assertRaises(p.Blocked):
                    p.RunModel().step("validate_baseline", evidence=replace(self.proof, **{field: None}))

    def test_incomplete_evidence_object_not_vacuously_complete(self):
        damaged = p.RecoveryEvidence()
        damaged.__dict__.clear()
        self.assertFalse(p.recovery_complete(damaged))

    def test_no_baseline_or_false_success(self):
        with self.assertRaises(p.Blocked):
            p.RunModel().step("begin_fixture")
        with self.assertRaises(p.Blocked):
            p.RunModel().step("validate_baseline")
        self.assertEqual(p.score_experiment(issued=True, observed=None, recovery=True), "unknown")
        self.assertEqual(p.score_experiment(issued=True, observed=False, recovery=True), "fail")
        self.assertEqual(p.score_experiment(issued=True, observed=True, recovery=None), "blocked")

    def test_state_machine_stops_not_resumable(self):
        ready = p.RunModel().step("validate_baseline", evidence=self.proof)
        running = ready.step("begin_fixture")
        for reason in ["abort", "timeout", "focus_lost", "unexpected", "emergency_stop"]:
            stopped = running.step(reason)
            self.assertEqual(stopped.phase, "quarantined")
            with self.assertRaises(p.Blocked):
                stopped.step("begin_fixture")
        self.assertEqual(running.step("seal_evidence").step("verify_recovery", evidence=self.proof).phase, "complete")

    def test_false_proofs_remain_untrusted_boundary(self):
        # A caller can lie about every boolean. This model cannot authenticate it.
        self.assertTrue(p.recovery_complete(self.proof))
        self.assertEqual(p.RunModel().step("validate_baseline", evidence=self.proof).phase, "ready")
        # This is a documented limitation, not real recovery certification.

    def test_fixture_recovery_exact_and_repeated(self):
        fixture = FixtureRecovery(self.root)
        a = fixture.recover("1")
        (a / "state.txt").write_text("experimental mutation")
        b = fixture.recover("2")
        self.assertEqual(hashes(b), fixture.expected)
        self.assertEqual(hashes(fixture.baseline), fixture.expected)
        self.assertNotEqual(hashes(a), fixture.expected)

    def test_fixture_partial_copy_corrupt_and_interruption(self):
        fixture = FixtureRecovery(self.root)
        fixture.recover("0")
        previous = fixture.pointer.read_bytes()
        for generation, fault in [("1", "partial"), ("2", "corrupt"), ("3", "prepublish")]:
            with self.subTest(fault=fault), self.assertRaises((OSError, p.Blocked)):
                fixture.recover(generation, fault)
            self.assertEqual(fixture.pointer.read_bytes(), previous)
            self.assertEqual(json.loads(fixture.journal.read_text())["state"], "pending")
        # Restart-equivalent new attempt uses a new generation; incomplete dirs stay evidence.
        self.assertEqual(hashes(fixture.recover("4")), fixture.expected)

    def test_publish_without_commit_is_not_recovered(self):
        fixture = FixtureRecovery(self.root)
        with self.assertRaises(OSError):
            fixture.recover("1", "postpublish")
        self.assertEqual(json.loads(fixture.pointer.read_text())["state"], "verified-fixture")
        self.assertEqual(json.loads(fixture.journal.read_text())["state"], "pending")
        self.assertFalse(p.recovery_complete(replace(self.proof, journal_committed=False)))

    def test_stale_and_concurrent_locks_block(self):
        fixture = FixtureRecovery(self.root)
        lock = self.root / "fixture.lock"
        lock.write_text("stale or concurrent; identity is unknown")
        with self.assertRaises(FileExistsError):
            fixture.recover("1")
        self.assertTrue(lock.exists())

    def test_baseline_hash_and_extra_file_drift_block(self):
        fixture = FixtureRecovery(self.root)
        (fixture.baseline / "unexpected.txt").write_text("external sync")
        with self.assertRaises(p.Blocked):
            fixture.recover("1")

    def test_mutants_are_killed_by_independent_expectations(self):
        # Disposable in-memory regressions, never edit repository or installed code.
        mutations = {
            "issued-is-success": (lambda **kw: "pass", lambda: self.assertEqual(p.score_experiment(issued=True, observed=None, recovery=True), "unknown"), "score_experiment"),
            "missing-recovery-is-success": (lambda evidence: True, lambda: self.assertFalse(p.recovery_complete(p.RecoveryEvidence())), "recovery_complete"),
        }
        for label, (mutant, check, member) in mutations.items():
            with self.subTest(mutant=label), patch.object(p, member, mutant):
                with self.assertRaises(AssertionError):
                    check()

    def test_path_and_input_mutants_killed(self):
        with patch.object(p, "fixture_path", lambda root, relative: Path(root) / relative):
            with self.assertRaises(AssertionError):
                with self.assertRaises(p.Blocked):
                    p.fixture_path(self.root, "../escape")
        with patch.object(p.InputBudget, "plan", lambda self, *args: self):
            with self.assertRaises(AssertionError):
                with self.assertRaises(p.Blocked):
                    p.InputBudget(stopped=True).plan(self.target, self.observation, 10.1, (800, 600, 96), "single_click")


if __name__ == "__main__":
    unittest.main()
