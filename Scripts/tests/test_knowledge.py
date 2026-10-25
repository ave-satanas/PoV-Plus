"""Behavior checks for the knowledge validator; fixtures stay in temporary directories."""
import argparse
import contextlib
import copy
import datetime as dt
import hashlib
import importlib.util
import io
import json
from pathlib import Path
import tempfile
import unittest

spec = importlib.util.spec_from_file_location("knowledge", Path(__file__).parents[1] / "knowledge.py")
kb = importlib.util.module_from_spec(spec)
spec.loader.exec_module(kb)


class ValidationTests(unittest.TestCase):
    def setUp(self):
        self.temp = tempfile.TemporaryDirectory()
        self.old_root, self.old_knowledge = kb.ROOT, kb.KNOWLEDGE
        kb.ROOT = Path(self.temp.name)
        kb.KNOWLEDGE = kb.ROOT / "Knowledge"
        (kb.KNOWLEDGE / "Dependencies").mkdir(parents=True)
        today = dt.date.today().isoformat()
        self.meta = dict(id="INDEX", title="Test", type="index", status="active", created=today,
                         last_verified=today, versions={}, evidence=["Test fixture"], related=[],
                         confidence="verified", watch=[], claims={}, contradicts=[], supersedes=[])
        self.snapshot = dict(assemblies=[], configurations=[], repository_sources=[])
        self.save_snapshot()
        self.note("INDEX", self.meta, "# Test\n")

    def tearDown(self):
        kb.ROOT, kb.KNOWLEDGE = self.old_root, self.old_knowledge
        self.temp.cleanup()

    def save_snapshot(self):
        (kb.KNOWLEDGE / "Dependencies/environment-snapshot.json").write_text(json.dumps(self.snapshot))

    def note(self, name, meta, body):
        kb.write_record(kb.KNOWLEDGE / (name + ".md"), meta, body)

    def check(self, compare=None):
        output = io.StringIO()
        with contextlib.redirect_stdout(output):
            code = kb.validate(argparse.Namespace(max_age=90, strict=True, compare_snapshot=compare))
        return code, output.getvalue()

    def test_valid_minimal_vault(self):
        code, text = self.check()
        self.assertEqual(code, 0, text)

    def test_roundtrip_does_not_accumulate_blank_lines(self):
        path = kb.KNOWLEDGE / "INDEX.md"
        before = path.read_bytes()
        meta, body, _ = kb.read_record(path)
        kb.write_record(path, meta, body)
        self.assertEqual(before, path.read_bytes())

    def test_broken_link(self):
        self.note("INDEX", self.meta, "[[Missing]]")
        self.assertIn("broken/ambiguous", self.check()[1])

    def test_duplicate_id(self):
        self.note("Other", self.meta, "[[INDEX]]")
        self.assertIn("duplicate ID", self.check()[1])

    def test_missing_metadata(self):
        del self.meta["evidence"]
        self.note("INDEX", self.meta, "test")
        self.assertIn("missing metadata", self.check()[1])

    def test_orphan_detected_even_with_self_link(self):
        meta = dict(self.meta, id="OTHER")
        self.note("Other", meta, "[[Other]]")
        self.assertIn("ORPHAN Other", self.check()[1])

    def test_scoped_claim_conflict(self):
        self.meta["claims"] = {"installed.example": "1"}
        self.note("INDEX", self.meta, "[[Other]]")
        self.note("Other", dict(self.meta, id="OTHER", claims={"installed.example": "2"}), "[[INDEX]]")
        self.assertIn("contradictory active claim", self.check()[1])

    def test_historical_claim_does_not_conflict(self):
        self.meta["claims"] = {"installed.example": "1"}
        self.note("INDEX", self.meta, "[[Other]]")
        self.note("Other", dict(self.meta, id="OTHER", status="historical", claims={"installed.example": "2"}), "[[INDEX]]")
        self.assertEqual(self.check()[0], 0)

    def test_candidate_drift_revalidation(self):
        self.snapshot["assemblies"] = [dict(path="BepInEx/core/test.dll", sha256="OLD")]
        self.save_snapshot()
        self.meta["watch"] = ["dll:BepInEx/core/test.dll"]
        self.note("INDEX", self.meta, "test")
        candidate = copy.deepcopy(self.snapshot)
        candidate["assemblies"][0]["sha256"] = "NEW"
        path = kb.ROOT / "candidate.json"
        path.write_text(json.dumps(candidate))
        code, text = self.check(str(path))
        self.assertEqual(code, 1)
        self.assertIn("REVALIDATE INDEX", text)

    def test_watched_local_source_drift(self):
        source = kb.ROOT / "source.cs"
        source.write_text("old")
        self.snapshot["repository_sources"] = [dict(path="source.cs", sha256=hashlib.sha256(source.read_bytes()).hexdigest().upper())]
        self.save_snapshot()
        self.meta["watch"] = ["src:source.cs"]
        self.note("INDEX", self.meta, "test")
        source.write_text("new")
        self.assertIn("REVALIDATE INDEX", self.check()[1])

    def test_open_investigation_is_queue_not_error(self):
        self.note("INDEX", dict(self.meta, type="investigation", status="open"), "test")
        code, text = self.check()
        self.assertEqual(code, 0)
        self.assertIn("QUEUE open", text)

    def test_new_refuses_existing_record(self):
        args = argparse.Namespace(folder="", id="INDEX", slug="", title="test", type="investigation")
        (kb.KNOWLEDGE / "INDEX-.md").write_text("preserve")
        with self.assertRaises(ValueError):
            kb.new(args)
        self.assertEqual((kb.KNOWLEDGE / "INDEX-.md").read_text(), "preserve")


if __name__ == "__main__":
    unittest.main()
