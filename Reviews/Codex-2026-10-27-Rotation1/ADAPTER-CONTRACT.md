# Future adapter and evidence contract

**No candidate adapter exists in this preparation package.** The interfaces below describe
reviewer-owned test seams, not actual installed PoVPlus APIs and not a prescription for Claude's
implementation. Initial preparation must not load or inspect its WIP. A future separately
authorized reviewer supplies the small wrapper after verifying the immutable candidate SHA.

## Binding a later candidate without editing production files

`run_fixtures.Actor(root, worker_command=None)` is an importable JSON-line process transport.
The default command runs only the bundled explicit-file primitive actor. A later reviewed
wrapper may be passed as `worker_command=[host, wrapper, candidate-module-argument, ...]`;
the transport appends the private fixture root as the final argument. There is deliberately
no current `--candidate` switch that could accidentally import Claude's implementation.

The wrapper can import an explicitly approved committed module read-only at its original path,
or inspect immutable Git objects; it must not switch/copy/merge Claude's branch into this one,
write in that worktree or use its `.dev-state`. Record path + commit + source hash. No prototype
API is assumed available. If actual module loading creates production-default directories,
locks, services or caches before overrides, stop before import; static review first.

The wrapper must redirect ALL effects to fixture-owned roots: repository A/B, synthetic
installation A/B, lock storage/namespace, backup/staging/report directories, process snapshots,
clock/cancellation controls and launch recorder. Each destination and root is supplied by the
reviewer; there is no fallback to a live installation, user's Temp/profile saves, ProgramData,
another worktree or a global production mutex name. Named-object tests must use a unique
explicit fixture namespace. Cross-user namespace/ACL behavior stays unproven until approved.

Mock final file/process I/O or use genuine disposable files, but do not mock the candidate
canonicalizer, identity selector, acquisition guard, ownership/fencing decision or revalidation
being tested. If direct helper paths can bypass the guard, integrated tests must reach those
paths and observe the real decision. Keep test hooks outside production files; inability to
inject safely is a Blocked test and a question for the implementer, not permission to bypass.

An adapter is not a security sandbox. Arbitrary module/native code can ignore its root arguments;
audited source + explicit authority + appropriate execution environment remain prerequisites.
Passing `--authorized`, a wrapper flag or an evidence Boolean cannot establish that authority.

## Calibration protocol already implemented

Worker launch: existing Python with `-B primitive_worker.py <fresh-root>`. Fresh root must
contain `.rotation1-fixture` with synthetic kind and random run ID. All paths are fixture-relative.
The actor performs no resource-ID algorithm: parent picks **explicit lock-file paths**.

| Request / event | Meaning |
|---|---|
| ready {pid, epoch, run, backend} | Parent verifies PID against its actual Popen object; this is fixture identity, not production authorization |
| acquire {id, path, timeout_ms, cancel?} | Attempt explicit file handle; bounded0..5000ms. Unsupported paths can return error BEFORE attempt |
| attempt {id,pid} | Coordination point before acquisition; parent can inject cancellation without guessed delay |
| result {id,status,ticket?,pid?,epoch?,run?} | acquired / timeout / cancelled / error; ticket is opaque and local to that actor |
| release {id,ticket} | Release only this actor's handle; foreign/missing ticket returns not_owner |
| work {id,ticket,path,fail?} | If actually held, create only designated synthetic marker; fail injection releases in finally before any marker |
| crash {id} -> crashing | Deliberate os._exit(17) of exact synthetic actor, no game/process-name termination |
| quit {id} | Close owned handles and normal fixture exit |

Errors carry exception **types**, not user paths/secrets. A preflight error is not an acquisition
attempt, and a timeout is not access denied. Parent deadlines and worker deadlines are distinct.
The harness refuses wrong event IDs, malformed output, wrong ready PID and unexpected statuses.
The root marker/containment helper exists to keep these tests safe; it is not a solution to F10
or hostile reparse races. Actual links are refused except deliberately unsafe writer calibration,
which is isolated from the primitive worker and confined to another private canary directory.

## Candidate protocol additions to negotiate later

Preserve transport/correlation, but explicitly add:

- `candidate_commit`, candidate/adapter source hashes and host version;
- `repository_origin`, `resource_path`/declared resource set and approved fixture lock store;
- mode (shared read/exclusive effect if supported), operation and owner/authorization reference;
- resolved identity, lease epoch/fencing token, actual owner lifetime and refusal reason;
- barriers for after-resolution, after-acquire, after-final-process-check, after-source-hash,
  before-stage, before-final-publication, after-publication/before-receipt;
- safe process-provider and launch-recorder hooks; simulated external client remains alive
  independently of lease-owner crash; no actual Start-Process of Valheim;
- requested source/destination/evidence paths and adapter trace of actual selected inputs;
- unsupported/blocked capability reporting instead of assuming method names or defaults.

Barrier notifications are test observations, not proof the protected code actually uses the
reported object. Combine source/data-flow review and final-I/O interception with independent
filesystem/process oracles. Wrong barriers, unexecuted guards or a no-op adapter must fail controls.
Nested/multiple-resource operations require explicit candidate support or refusal policy.

## Independent oracles and evidence format

The parent owns the schedule, known fixture contents, disjoint/alias relationships and exact
subprocess objects. Assertions use actual bytes/file sets, generated canaries, real file-sharing
outcomes or mock-launch counters. They must not call the candidate's normalization, hash,
resource-selection or receipt-validation helper to decide its expected answer.

Record one outcome per matrix row with this shape (example is UNEXECUTED):

```json
{
  "schema": 1,
  "kind": "candidate-review",
  "run": "reviewer-generated-id",
  "baseline_commit": "b06cdfcac75c48349cfc26a09f5c4f1671947e83",
  "candidate_commit": null,
  "adapter_sha256": null,
  "invariant": "K01",
  "phase": "cross-process",
  "status": "CANDIDATE_NOT_RUN",
  "expected": "one owner of the same accepted canonical resource",
  "observed": null,
  "oracle": "parent barrier, actual subprocess identities and external canary",
  "artifacts": [],
  "limitations": ["no implementation inspected"]
}
```

Include host/tool versions; SHA256 of each source/adapter/corpus; relative artifact paths;
attempt/acquire/release/effect/exit events with request IDs and parent monotonic timestamps;
before/after canary hashes and directory-entry inventories; injected failure and exact expected
refusal; raw exit/protocol/compile errors; timeout/cancel and cleanup outcomes. Known absent files
and unreadable files are distinct. Do not call a snapshot empty after a failed read.

Current `--self-test` produces `result.json` with kind=harness-calibration,
candidate_commit=null and candidate_verdict=NOT_RUN, plus `actions.jsonl`. On a failing case it
records SELF_CONTROL_FAIL, preserves partial observations and closes owned children in finally.
Negative-calibration cases have a distinct kind, not a claimed candidate failure. All private
scratch remains ignored; do not copy binaries, logs, raw personal paths or large trace files
into the review package. Reviewed summaries can later be preserved with owner authorization.

Sequence numbers and hashes detect only particular recorded/changed data; they do not prove
complete history, immutable provenance, all filesystem writes or current external process state.
Keep evidence gaps visible. Fake receipts or all-true flags must not turn an unknown result green.

## Teardown and stop conditions

Only exact owned synthetic Popen processes may be timed out/killed. No process-name kill,
real client, Steam or server interaction. Reparse fixtures are removed by their known exact
link path; no recursive deletion/move into an unchecked directory. Scratch is retained, with
successful child closure and exception status reported. If cleanup cannot be verified, stop
and preserve paths for review rather than force-recovering anything.

Actual candidate access, cross-user/session experiments, accounts/VMs, live launch/deploy,
Steam/profile/save manipulation, runtime cleanup and commits all remain separately gated.
