"""Rotation1 harness calibration, not a test/approval of Claude's implementation.

Only review-owned disposable paths/processes are used. No production module import,
game install defaults, installed-binary reads, GUI, accounts or VM operations.
"""
import argparse
import hashlib
import json
import os
from pathlib import Path
import queue
import stat
import subprocess
import sys
import threading
import time
import uuid

HERE = Path(__file__).resolve().parent
REPO = HERE.parents[1]


class Actor:
    def __init__(self, root, worker_command=None):
        self.root = root
        # Future reviewed adapters can reuse this transport without editing production files.
        # No candidate adapter is supplied or invoked in this preparation phase.
        command = worker_command or [sys.executable, '-B', str(HERE / 'primitive_worker.py')]
        self.trace = []
        self.process = subprocess.Popen([*command, str(root)],
            stdin=subprocess.PIPE, stdout=subprocess.PIPE, stderr=subprocess.PIPE, text=True, bufsize=1)
        self.events = queue.Queue()
        def pump():
            for line in self.process.stdout:
                try:
                    event = json.loads(line)
                    self.trace.append(dict(direction='from_actor', at=time.monotonic(), data=event))
                    self.events.put(event)
                except ValueError:
                    self.events.put({'event': 'protocol_error'})
        self.reader = threading.Thread(target=pump, daemon=True); self.reader.start()
        try:
            self.ready = self.event('ready')
            assert self.ready['pid'] == self.process.pid
        except Exception:
            self.process.kill(); self.process.wait(timeout=5)
            raise

    def send(self, op, **kwargs):
        identity = uuid.uuid4().hex
        message = dict(id=identity, op=op, **kwargs)
        self.trace.append(dict(direction='to_actor', at=time.monotonic(), data=message))
        self.process.stdin.write(json.dumps(message) + '\n')
        self.process.stdin.flush()
        return identity

    def event(self, expected, identity=None, timeout=8):
        value = self.events.get(timeout=timeout)
        assert value['event'] == expected, value
        if identity is not None:
            assert value['id'] == identity, value
        return value

    def acquire(self, path, **kwargs):
        identity = self.send('acquire', path=path, **kwargs)
        first = self.events.get(timeout=8)
        assert first.get('id') == identity, first
        if first['event'] == 'result':
            assert first['status'] == 'error', first  # preflight refusal is not an attempted acquisition
            return first
        assert first['event'] == 'attempt', first
        return self.event('result', identity)

    def request(self, op, **kwargs):
        identity = self.send(op, **kwargs)
        return self.event('result', identity)

    def close(self):
        if self.process.poll() is None:
            try:
                assert self.request('quit')['status'] == 'bye'
                self.process.wait(timeout=5)
            except Exception:
                # Only this owned, exact Popen fixture child; NEVER a process-name search.
                self.process.kill(); self.process.wait(timeout=5)
                raise
        self.reader.join(timeout=2)


def sha(path):
    return hashlib.sha256(path.read_bytes()).hexdigest()


def assert_single_owner(results):
    # Independent harness expectation, not a candidate-provided success Boolean.
    assert sum(result['status'] == 'acquired' for result in results) == 1, 'multiple announced owners'


def rejected(action):
    try:
        action()
    except AssertionError:
        return
    raise AssertionError('deliberately broken behavior escaped the harness oracle')


def junction(link, target, owned_root):
    assert link.absolute().is_relative_to(owned_root) and target.resolve().is_relative_to(owned_root)
    result = subprocess.run(['cmd', '/c', 'mklink', '/J', str(link), str(target)],
                            capture_output=True, text=True, timeout=5)
    assert result.returncode == 0, result.stderr


def main():
    parser = argparse.ArgumentParser()
    parser.add_argument('--self-test', action='store_true', required=True)
    parser.parse_args()
    if os.name != 'nt':
        raise SystemExit('Windows-only; no silently substituted filesystem/lock semantics')
    source_hashes = {name:sha(HERE / name) for name in ['run_fixtures.py', 'primitive_worker.py', 'path_cases.json']}
    assert subprocess.check_output(['git', 'rev-parse', '--show-toplevel'], cwd=REPO, text=True).strip().replace('\\', '/').lower() == REPO.as_posix().lower()
    root = REPO / '.dev-state/rotation1' / uuid.uuid4().hex
    cursor = Path(root.anchor)
    for part in root.parts[1:]:
        cursor /= part
        if cursor.exists():
            info = cursor.lstat()
            assert not stat.S_ISLNK(info.st_mode) and not getattr(info, 'st_file_attributes', 0) & 0x400, 'scratch reparse point'
    root.mkdir(parents=True)
    run = root.name
    (root / '.rotation1-fixture').write_text(json.dumps(dict(kind='synthetic', run=run)))
    for name in ['locks', 'effects', 'repo-a', 'repo-b', 'resource-a', 'resource-b', 'protected']:
        (root / name).mkdir()
    sentinel = root / 'protected/sentinel.txt'; sentinel.write_text('untouched synthetic canary')
    baseline = sha(sentinel)
    actors = []
    results = []
    def actor():
        value = Actor(root); actors.append(value); return value
    def check(name, body, *, kind='primitive-control'):
        start = time.monotonic()
        try:
            body()
            assert sha(sentinel) == baseline, 'protected synthetic canary changed'
        except Exception as error:
            results.append(dict(id=name, status='SELF_CONTROL_FAIL', kind=kind, error=type(error).__name__))
            raise
        results.append(dict(id=name, status='SELF_CONTROL_PASS', kind=kind, canary_sha256=sha(sentinel),
                            elapsed_ms=round((time.monotonic() - start) * 1000, 3)))
        print('PASS', name, flush=True)
    try:
        a = actor(); b = actor()
        def exclusion():
            first = a.acquire('locks/shared.lock'); assert first['status'] == 'acquired'
            other = b.acquire('locks/shared.lock'); assert other['status'] == 'timeout'
            assert_single_owner([first, other])
            assert a.request('release', ticket=first['ticket'])['status'] == 'released'
        check('L01-explicit-same-lock-excludes-process', exclusion)

        def distinct():
            one = a.acquire('locks/resource-a.lock'); two = b.acquire('locks/resource-b.lock')
            assert one['status'] == two['status'] == 'acquired'
            a.request('release', ticket=one['ticket']); b.request('release', ticket=two['ticket'])
        check('L02-distinct-explicit-locks-independent', distinct)

        def race():
            for _ in range(5):
                ia = a.send('acquire', path='locks/race.lock', timeout_ms=200)
                ib = b.send('acquire', path='locks/race.lock', timeout_ms=200)
                a.event('attempt', ia); b.event('attempt', ib)
                ra = a.event('result', ia); rb = b.event('result', ib)
                assert_single_owner([ra, rb])
                winner, result = (a, ra) if ra['status'] == 'acquired' else (b, rb)
                winner.request('release', ticket=result['ticket'])
        check('L03-five-cross-process-acquisition-races', race)

        def crash():
            child = actor(); held = child.acquire('locks/crash.lock'); assert held['status'] == 'acquired'
            identity = child.send('crash'); child.event('crashing', identity)
            assert child.process.wait(timeout=5) == 17
            replacement = b.acquire('locks/crash.lock'); assert replacement['status'] == 'acquired'
            assert replacement['epoch'] != held['epoch']
            b.request('release', ticket=replacement['ticket'])
        check('L04-owned-process-death-releases-handle-not-recovery-proof', crash)

        def timeout():
            held = a.acquire('locks/timeout.lock'); start = time.monotonic()
            failure = b.acquire('locks/timeout.lock', timeout_ms=120)
            elapsed = time.monotonic() - start
            assert failure['status'] == 'timeout' and 0.08 <= elapsed < 3
            a.request('release', ticket=held['ticket'])
        check('L05-bounded-timeout', timeout)

        def cancellation():
            held = a.acquire('locks/cancel.lock')
            identity = b.send('acquire', path='locks/cancel.lock', timeout_ms=5000, cancel='cancel.flag')
            b.event('attempt', identity)  # deterministic injection point, not a guessed sleep
            (root / 'cancel.flag').write_text('stop only this synthetic acquisition')
            assert b.event('result', identity)['status'] == 'cancelled'
            a.request('release', ticket=held['ticket'])
        check('L06-cancel-after-attempt-barrier', cancellation)

        def exception():
            held = a.acquire('locks/exception.lock')
            assert a.request('work', ticket=held['ticket'], path='effects/never.marker', fail=True)['status'] == 'error'
            assert not (root / 'effects/never.marker').exists()
            next_owner = b.acquire('locks/exception.lock'); assert next_owner['status'] == 'acquired'
            b.request('release', ticket=next_owner['ticket'])
        check('L07-exception-finally-release', exception)

        def no_effect_without_lock():
            held = a.acquire('locks/no-effect.lock')
            assert b.acquire('locks/no-effect.lock')['status'] == 'timeout'
            assert b.request('work', ticket=held['ticket'], path='effects/forbidden.marker')['status'] == 'not_owner'
            assert not (root / 'effects/forbidden.marker').exists()
            assert b.request('release', ticket=held['ticket'])['status'] == 'not_owner'
            assert b.acquire('locks/no-effect.lock')['status'] == 'timeout'
            a.request('release', ticket=held['ticket'])
        check('L08-failed-or-foreign-lease-no-side-effect', no_effect_without_lock)

        def nested():
            outer = a.acquire('locks/nested.lock')
            assert a.acquire('locks/nested.lock')['status'] == 'timeout'
            assert b.acquire('locks/nested.lock')['status'] == 'timeout'
            a.request('release', ticket=outer['ticket'])
        check('L09-nonreentrant-control-keeps-outer-handle', nested)

        def case_alias():
            held = a.acquire('locks/CaseAlias.lock')
            assert b.acquire('locks/casealias.lock')['status'] == 'timeout'
            a.request('release', ticket=held['ticket'])
        check('L10-observed-Windows-case-alias', case_alias)

        def restart_identity():
            held = a.acquire('locks/restart.lock'); replacement = actor()
            assert replacement.acquire('locks/restart.lock')['status'] == 'timeout'
            a.request('release', ticket=held['ticket'])
            new = replacement.acquire('locks/restart.lock'); assert new['status'] == 'acquired'
            assert new['pid'] == replacement.process.pid and new['epoch'] == replacement.ready['epoch']
            assert new['epoch'] != held['epoch'] and new['ticket'] != held['ticket']
            replacement.request('release', ticket=new['ticket'])
        check('L11-controller-restart-does-not-inherit-old-token', restart_identity)

        def invalid_parent():
            assert a.acquire('absent-parent/no-fallback.lock')['status'] == 'error'
            assert not (root / 'absent-parent').exists()
        check('L12-missing-parent-is-error-not-contention-or-fallback', invalid_parent)

        def stale_file():
            target = root / 'locks/stale.lock'
            target.write_text('{"pid":1,"state":"running","claimed_recovered":true}')
            result = a.acquire('locks/stale.lock'); assert result['status'] == 'acquired'
            a.request('release', ticket=result['ticket'])
            assert target.read_text().endswith('true}')
        check('L13-file-existence-is-not-handle-or-recovery-authority', stale_file)

        def corroborated_effect():
            held = a.acquire('locks/effect.lock')
            assert a.request('work', ticket=held['ticket'], path='effects/positive.marker')['status'] == 'worked'
            actual = json.loads((root / 'effects/positive.marker').read_text())
            assert actual == dict(pid=a.process.pid, epoch=a.ready['epoch'], ticket=held['ticket'])
            a.request('release', ticket=held['ticket'])
        check('L14-permitted-side-effect-corroborated-by-file-and-Popen', corroborated_effect)

        def broken_worktree_model():
            left = a.acquire('repo-a/workflow.lock'); right = b.acquire('repo-b/workflow.lock')
            assert left['status'] == right['status'] == 'acquired'
            rejected(lambda: assert_single_owner([left, right]))
            a.request('release', ticket=left['ticket']); b.request('release', ticket=right['ticket'])
        check('N01-per-worktree-same-resource-oracle-detects-overlap', broken_worktree_model, kind='negative-calibration')

        def negative_oracle():
            before = sha(sentinel)
            sentinel.write_text('deliberately violated SYNTHETIC canary')
            rejected(lambda: require(sha(sentinel) == before))
            sentinel.write_text('untouched synthetic canary')
        check('N02-canary-oracle-detects-false-success', negative_oracle, kind='negative-calibration')

        def paths():
            corpus = json.loads((HERE / 'path_cases.json').read_text())
            assert corpus['schema'] == 1 and len(corpus['blocked']) == 14
            for bad in [entry['path'] for entry in corpus['blocked']] + ['x'*241]:
                assert a.acquire(bad)['status'] == 'error'
            assert not (root / 'escape.lock').exists()
        check('P01-actor-containment-corpus-not-candidate-path-proof', paths)

        def junction_refusal():
            link = root / 'repo-a/junction'
            junction(link, root / 'protected', root)
            try:
                assert a.acquire('repo-a/junction/escape.lock')['status'] == 'error'
                assert not (root / 'protected/escape.lock').exists()
            finally:
                link.rmdir()  # known synthetic junction only; no recursive deletion
        check('P02-real-junction-refused-by-actor-containment', junction_refusal)

        def hardlink_refusal():
            link = root / 'locks/hardlinked.lock'
            os.link(sentinel, link)
            assert a.acquire('locks/hardlinked.lock')['status'] == 'error'
            link.unlink()
        check('P03-hardlink-refused-by-actor-containment', hardlink_refusal)

        def check_use_counterexample():
            # Deterministic fault schedule for a deliberately unsafe lexical-check writer.
            allowed = root / 'repo-a/output'; allowed.mkdir()
            candidate = allowed / 'race-canary.txt'
            assert candidate.absolute().is_relative_to(allowed.absolute())  # unsafe check
            retained = root / 'repo-a/retained-original'
            assert allowed.resolve().is_relative_to(root) and retained.absolute().is_relative_to(root)
            allowed.rename(retained)
            target = root / 'protected/race-canary.txt'; target.write_text('synthetic original')
            original = sha(target)
            junction(allowed, root / 'protected', root)
            try:
                candidate.write_text('synthetic redirected write')
                rejected(lambda: require(sha(target) == original))
            finally:
                allowed.rmdir()  # removes only this synthetic junction
            assert retained.is_dir()
        check('N03-check-use-substitution-caught-by-external-canary', check_use_counterexample, kind='negative-calibration')
    finally:
        cleanup_errors = []
        for value in actors:
            try:
                value.close()
            except Exception as error:
                cleanup_errors.append(type(error).__name__)
        result = dict(schema=1, kind='harness-calibration', run=run,
            candidate_commit=None, candidate_verdict='NOT_RUN', cases=results, cleanup_errors=cleanup_errors,
            source_hashes=source_hashes,
            fixture_files_retained=True, private_scratch=str(root.relative_to(REPO)),
            limits=['No candidate implementation inspected', 'Explicit-file primitive only; no resource identity proof',
                    'No cross-user/session, live-process or malicious-filesystem containment proof'])
        (root / 'result.json').write_text(json.dumps(result, indent=2)+'\n')
        with (root / 'actions.jsonl').open('w', encoding='utf-8') as stream:
            for value in actors:
                for event in value.trace:
                    stream.write(json.dumps(dict(actor_pid=value.process.pid, **event))+'\n')
        print(json.dumps(dict(cases=len(results), candidate='NOT_RUN', result=str((root/'result.json').relative_to(REPO)))))
        if cleanup_errors:
            raise RuntimeError('fixture teardown incomplete; inspect retained scratch')


def require(condition):
    assert condition, 'independent canary invariant violated'


if __name__ == '__main__':
    main()
