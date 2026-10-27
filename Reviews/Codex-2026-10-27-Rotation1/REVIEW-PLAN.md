# Engineering Rotation 1: independent adversarial preparation

Date: 2026-10-27. Reviewer: OpenAI Codex. **Preparation only; candidate NOT_RUN.**

Baseline is `b06cdfcac75c48349cfc26a09f5c4f1671947e83`, branch
`research/dev-008-supervised-testing`, primary worktree S:/Valheim-Modding/PoV-Plus.
The worktree was clean initially. Root AGENTS, Knowledge/INDEX, DEV-003/DEC-003,
DEV-008, the preserved cross-review and current workflow were consulted. Claude's stated
review checkpoint is3a56f5ac456222de3d012c8c57640e7b72f3c352; its current worktree,
uncommitted implementation and implementation design were **not inspected**. No assumptions
about its APIs or choice of locking primitive are made.

## Deliverables and navigation

- [THREAT-MODEL.md](THREAT-MODEL.md): resources, actors, baseline operation/lock/path inventory,
  trust boundaries, external writers and proof limits.
- [MATRIX.md](MATRIX.md): falsifiable invariants with independent oracles, severity, readiness
  and limitations. Every requested lock and path scenario is included.
- [ADAPTER-CONTRACT.md](ADAPTER-CONTRACT.md): future test seams, transport, authorization and
  evidence rules. This is a **test interface proposal**, not an invented production API.
- `run_fixtures.py`, `primitive_worker.py`, `path_cases.json`: runnable Windows-only
  self-controls and negative calibration. No production correction or candidate adapter.
- `source_inventory.py`: reproducible read-only inventory from immutable baseline Git blobs.
- [RESULTS.md](RESULTS.md): executions, known limitations and next-review questions.

## Scope and outcome rules

The mission is to challenge F9/F10 independently, not defend the existing implementation or
specify Claude's replacement. Requirements are behavioral. A named mutex, file handle or other
mechanism may be acceptable if its actual identity, lifetime, access and failure semantics meet
the declared contract. A lockfile's existence, a process ID, or an approval flag does not prove
ownership, resource identity, conversational authorization or recovery completion.

No current test result is a pass/fail of Claude's implementation. Self-control green means
only that the harness exercised a known primitive and its independent observations agreed.
Candidate testing requires a separately authorized immutable implementation checkpoint, source
review, an audited fixture adapter, and test seams that cannot hit live installation/processes.
If those seams are absent, mark the relevant test Blocked; do not silently substitute a model.

Status vocabulary:

| Status | Meaning |
|---|---|
| SELF_CONTROL_PASS | Harness/known Windows primitive behaved as expected; no candidate conclusion |
| MUTANT_DETECTED / negative-calibration pass | A deliberately broken fixture was caught; never a production failure |
| CANDIDATE_NOT_RUN | No candidate inspected/executed; default for every candidate invariant now |
| PASS / FAIL | Later authorized candidate result, tied to exact commit/adapter/host and oracle |
| BLOCKED / UNPROVEN / NOT_APPLICABLE | Missing authority/capability, unavailable evidence, or predeclared excluded support; none count as Pass |

## Acceptance and rejection policy

1. Before testing, require the implementation's supported identity, user/session, filesystem,
   alias, nesting, multi-resource and stale-state contract. Do not redefine it after failures.
2. Verify each safety-relevant entry point reaches the same required resource guard before
   side effects; public CLI flags, internal helper functions, exception recovery, no-op and
   preview paths are separately reviewed. Mock only I/O/process seams, never the guard under test.
3. Distinct worktree paths for the same accepted resource must not yield two simultaneous
   authorizations. An alias may be safely rejected or share the same resource identity; it
   cannot quietly become an independent writable resource. Unsupported identity must block.
4. On lock failure, timeout, cancellation or unknown state, zero prohibited side effects.
   Use external file inventories, protected sentinel hashes and mock-launch counters, not the
   candidate's `success`, `held` or receipt status. Assert failure reason and intended gate.
5. Positive controls are mandatory: a genuinely acquired synthetic lease must permit the
   designated effect, and release must permit reacquisition. "Refuse everything" is not a pass.
6. Missing observations, dropped events, worker death, unsupported operations, unexpected
   exceptions or incomplete cleanup mean Unknown/Blocked/Fail as scoped. Never treat silence
   or a generic exception as a successful rejection for the named threat.
7. Change-and-change-back, stale receipts, alias changes and same-count/file-set substitutions
   must be tested where claims require them. Two equal hashes cannot prove no intervening write.
8. A handle released after a controller crash is not proof the game stopped. A known running
   client or unknown external-process query still blocks protected operations after reacquisition.
9. Live resource safety, permissions across actual users/sessions, Steam/Valheim behavior,
   isolation, durability and recovery remain unproven by fixture results.

High-severity mutual-exclusion, wrong-root write, unsafe fallback, failed-acquisition side
effect and wrong-artifact verification failures block affected operational use. A reproducibility
or availability defect is scored separately, without downgrading a safety claim to make it pass.
Preserve intentional upstream compatibility safeguards and the current bridge acceptance policy.

## Safe execution now

From this worktree on Windows, using existing Python; no installs or target assemblies:

```powershell
python -B Reviews/Codex-2026-10-27-Rotation1/source_inventory.py
python -B Reviews/Codex-2026-10-27-Rotation1/run_fixtures.py --self-test
python -B Scripts/knowledge.py validate --strict
git diff --check
```

The runner starts only its own `primitive_worker.py` Python children, with `-B` and a fresh
marker-bearing directory under this worktree's ignored `.dev-state/rotation1/<run>/`.
It does not import operational scripts or invoke installed game binaries. Every mutation,
junction/hardlink and abrupt-exit test concerns freshly created synthetic resources. It never
searches for or kills processes by name. Bounded child timeout cleanup applies only to exact
owned Popen objects; it does not authorize killing a game. Synthetic files are retained for
evidence; junctions are removed by exact link path, without recursive deletion.

No accounts, VMs, ACLs, Steam settings, GUI, saves, real installation or server are touched.
This harness is not an OS sandbox for arbitrary future adapter code. Later adapters require
source review and explicit scoped approval before execution.

## Later review sequence, after separate authorization

1. Receive immutable candidate SHA, parent, exact files, API/entry-point map, claimed support,
   test results and concrete no-runtime reproduction instructions. Do not inspect WIP now.
2. Read committed diff via Git, including dependency/init paths, fallback and cleanup branches.
   Identify all writes, launches, lease acquisition/release, authoritative evidence inputs and
   path defaults. Verify unchanged gameplay/config authorization boundaries.
3. Bind an adapter to that committed implementation read-only, with all resources/lock storage,
   process enumeration and launch redirected to the synthetic fixture. No checkout/cherry-pick
   or production source edits required. Importing another tree must not make its outputs default
   there. If the implementation cannot run without shared writes, stop rather than bypass it.
4. Execute unit/path cases, then coordinated independent-process cases, then synthetic integrated
   Deploy/Rollback/Launch/exporter paths. Never launch Valheim; launch is a recorder stub only.
5. Inject the declared failure barriers. Deliberately disable or misroute a guard in a disposable
   test double to prove the independent oracle catches wrong effects. Do not alter candidate
   production files. Check reason-specific kills, compilation/host errors and receipt provenance.
6. Report each matrix row with evidence and limitations; separate source inspection, unit,
   cross-process, synthetic integration and unauthorized runtime obligations. No automatic
   integration, commit, approval, deployment or runtime permission.

## Questions for Claude when its checkpoint is available

1. What exact resources are protected: installation directory, profile, deployment target,
   desktop session, or a declared set? Are two installs sharing a profile actually disjoint?
2. How is resource identity established across casing, aliases, reparse paths, drive aliases,
   missing parents, inaccessible metadata and replacement of the directory at the same name?
3. Where does the lock live; what user/session scope and ACL assumptions apply? Can fixtures
   inject storage/namespace without using a production global/default path?
4. What primitive is authoritative, and what metadata is merely diagnostic? What survives a
   crash/reboot? How are orphaned clients and pending receipts reconciled without blind stealing?
5. Does acquisition cover the entire launched session and cleanup? Who owns the handle if the
   invoking shell exits while the synthetic/launched child is still alive?
6. What timeout, cancellation, reentrancy and multi-resource ordering policies are guaranteed?
7. Which operations/entry points require ownership, including recovery and unrecognized states?
   Can a low-level callable helper mutate without an acquired lease/authorization context?
8. How do external clients/Steam affect safety? What happens on unknown process identity/query
   failure, another install using the same profile, or an external start after the last query?
9. How are owner identity, epoch/nonce, PID/start/boot identity, operation, authorization reference,
   source artifact and target resource bound to receipts? Can stale release affect a new owner?
10. Which output defaults derive from script location versus CWD? How do overrides prevent
    another worktree's artifact/evidence from silently satisfying this run?
11. What binds source and destination to stable identities through staging/publication? Which
    TOCTOU protection is implemented, and which threats are explicitly outside its assumptions?
12. How are partial writes, stale `.tmp` files, same-file aliases, final hash mismatch and failed
    receipt publication surfaced? What independently blocks the next operation after uncertainty?

Stop at preparation. Once implementation completion is reported, await separate owner
authorization before examining or testing the actual changes.
