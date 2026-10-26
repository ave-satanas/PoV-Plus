import importlib.util
import json
from pathlib import Path
import tempfile
import unittest
from unittest.mock import patch

SPEC = importlib.util.spec_from_file_location("tome_evidence", Path(__file__).parents[1] / "collect_tome_evidence.py")
collector = importlib.util.module_from_spec(SPEC)
SPEC.loader.exec_module(collector)
SESSION = "2026-10-25_01-01-03"


class EvidenceTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.addCleanup(self.temp.cleanup)
        self.root = Path(self.temp.name)
        self.dev = self.root / "dev"
        self.repo = self.root / "repo"
        self.log = self.dev / "BepInEx/LogOutput.log"
        self.profile = self.dev / "BepInEx/config/LoadTimeProfiler" / (SESSION + ".log")
        self.log.parent.mkdir(parents=True)
        self.profile.parent.mkdir(parents=True)
        self.log.write_text(self.session_log(SESSION), encoding="utf-8")
        self.profile.write_text("Session: 2026-10-25 01:01:03 -07:00\nResult: completed\n", encoding="utf-8")

    @staticmethod
    def session_log(session):
        return ("[Message: BepInEx] BepInEx 5.4.23.5\n"
            f"[Info : Profiler] Writing startup and connection profiles to C:/dev/{session}.log.\n"
            "[Info : PoVPlus] [PoVPlus.TomeDiagnostic] BEGIN authorized\n"
            "[Info : PoVPlus] [PoVPlus.TomeDiagnostic] RENDERED key=AzuEPI.emulated/$tome_DO\n"
            "[Info : PoVPlus] [PoVPlus.TomeDiagnostic] END snapshotComplete=true\n")

    def collect(self, receipts=None):
        return collector.collect(self.dev, SESSION, receipts or [], self.repo)

    def test_matching_session_and_snapshot(self):
        data = self.collect()
        self.assertEqual(data["status"], "matched")
        self.assertTrue(data["snapshot_complete"])
        self.assertEqual(data["diagnostic"][0]["line"], 3)
        self.assertEqual(len(data["sources"]), 2)
        self.assertIn("sha256", data["sources"][0])

    def test_bridge_activation_and_intervention_rows_preserved_without_inflating_snapshot_counts(self):
        with self.log.open("a", encoding="utf-8") as stream:
            stream.write("[Info : PoVPlus] [PoVPlus.TomeBridge] Both acceptance extensions active\n")
            stream.write("[Info : PoVPlus] [PoVPlus.TomeBridge] Extended gate=equipment prefab=BMP_DruidsTome\n")
        data = self.collect()
        self.assertEqual(sum(collector.BRIDGE_MARKER in row["text"] for row in data["diagnostic"]), 2)
        self.assertEqual(data["counts"]["BEGIN"], 1)
        self.assertTrue(data["snapshot_complete"])

    def test_prior_session_bridge_rows_excluded(self):
        old = self.session_log("2026-10-24_01-01-03") + "[Info : PoVPlus] [PoVPlus.TomeBridge] old activation\n"
        self.log.write_text(old + self.session_log(SESSION), encoding="utf-8")
        self.assertFalse(any(collector.BRIDGE_MARKER in row["text"] for row in self.collect()["diagnostic"]))

    def test_ignores_prior_launch(self):
        self.log.write_text(self.session_log("2026-10-24_01-01-03") + self.session_log(SESSION), encoding="utf-8")
        self.assertEqual(self.collect()["counts"]["BEGIN"], 1)

    def test_wrong_session_never_attaches_profiler(self):
        self.log.write_text(self.session_log("2026-10-24_01-01-03"), encoding="utf-8")
        data = self.collect()
        self.assertEqual(data["status"], "session-mismatch")
        self.assertEqual(data["diagnostic"], [])
        self.assertEqual(len(data["sources"]), 1)

    def test_optional_locked_profiler_nonfatal(self):
        original = Path.open
        def open_path(path, *args, **kwargs):
            if path == self.profile:
                raise PermissionError("fixture lock")
            return original(path, *args, **kwargs)
        with patch.object(Path, "open", open_path):
            data = self.collect()
        self.assertEqual(data["status"], "matched")
        self.assertEqual(data["skipped"][0]["reason"], "PermissionError")
        self.assertEqual(data["counts"]["RENDERED"], 1)

    def test_no_cache_or_save_enumeration(self):
        with patch.object(Path, "rglob", side_effect=AssertionError("No recursion allowed")):
            self.collect()

    def test_unavailable_log_records_failure(self):
        self.log.unlink()
        self.assertEqual(self.collect()["status"], "incomplete")

    def test_profiler_content_boundary_checked(self):
        self.profile.write_text("Session: 2000-01-01 00:00:00\n", encoding="utf-8")
        data = self.collect()
        self.assertFalse(data["profile"])
        self.assertEqual(data["skipped"][0]["reason"], "ProfilerSessionMismatch")

    def test_only_workflow_receipts(self):
        receipt = self.repo / ".dev-state/backups/fixture/manifest.json"
        receipt.parent.mkdir(parents=True)
        receipt.write_text(json.dumps(dict(Id="fixture", Status="completed", Error="password=private")), encoding="utf-8")
        data = self.collect([receipt, self.root / "personal.json"])
        self.assertEqual(data["receipts"][0]["Id"], "fixture")
        self.assertNotIn("Error", data["receipts"][0])
        self.assertEqual(data["skipped"][0]["reason"], "NotWorkflowReceipt")

    def test_size_limit_skipped_without_read(self):
        with patch.object(collector, "MAX_BYTES", 1):
            self.assertEqual(self.collect()["status"], "incomplete")

    def test_invalid_session_rejected(self):
        with self.assertRaises(ValueError):
            collector.collect(self.dev, "../../personal", [])

    def test_abort_is_not_success(self):
        with self.log.open("a", encoding="utf-8") as stream:
            stream.write("[Info : PoVPlus] [PoVPlus.TomeDiagnostic] ABORT SnapshotException\n")
        self.assertFalse(self.collect()["snapshot_complete"])

    def test_reports_never_overwrite_and_redact(self):
        data = self.collect()
        output = self.repo / ".dev-state/reports/test"
        collector.write_report(data, output, self.repo)
        with self.assertRaises(FileExistsError):
            collector.write_report(data, output, self.repo)
        with self.assertRaises(ValueError):
            collector.write_report(data, self.root / "outside", self.repo)
        self.assertNotIn("private", collector.redact("password=private"))


if __name__ == "__main__":
    unittest.main()
