# Preparation results and continuation boundary

Date2026-10-27. **Harness calibrated; candidate CANDIDATE_NOT_RUN.** No Claude implementation
was inspected, imported or executed. These outcomes must not be used as its acceptance report.

## Inventory

| File | Purpose |
|---|---|
| REVIEW-PLAN.md | Scope, acceptance/rejection policy, review progression and twelve implementation questions |
| THREAT-MODEL.md | Assets/actors/boundaries, baseline operations and two lock sites, root/evidence contamination inventory |
| MATRIX.md |23 lock/ownership invariants,23 path/evidence invariants,7 methodology controls; methods, oracles, outcomes, severity/readiness/limits |
| ADAPTER-CONTRACT.md | Review-owned future adapter/process transport and evidence format; no invented production API |
| primitive_worker.py | Windows explicit-file FileShare.None actor; no resource mapper, game or production adapter |
| run_fixtures.py | Bounded independent-process controls, synthetic canaries, deterministic fault schedules and negative calibration |
| path_cases.json |14 invalid syntactic path vectors; oversized input is generated separately |
| source_inventory.py | Immutable-baseline script hash/line inventory; no current candidate/shared-resource reads |
| RESULTS.md | This executed-results/limits/continuation record |

Generated files are ignored under `.dev-state/rotation1/`. No binaries, private game logs,
credentials, personal save data or full user-profile paths are in this review directory.

## Commands executed

```powershell
python -B Reviews/Codex-2026-10-27-Rotation1/source_inventory.py
python -B Reviews/Codex-2026-10-27-Rotation1/run_fixtures.py --self-test
python -B Scripts/knowledge.py validate --strict
git diff --check
```

The inventory matched18 baseline scripts. This is a reproducible discovery query, not a claim
that a regex proves all relevant effects. Source review separately identified Deploy/Rollback,
exception recovery, Launch, local Build/Status/Diagnose and exporter/evidence paths.

Final corpus ran twice in fresh roots:

- `.dev-state/rotation1/7f290e1fa1b6440f8a2a95bf5f129bd4/result.json`
- `.dev-state/rotation1/486879d99d094d80ac0ece8f2661c696/result.json`

Both report20 SELF_CONTROL_PASS, candidate_commit=null, candidate_verdict=NOT_RUN and no
cleanup errors. Full action/observation transcripts are in sibling actions.jsonl. Each run has
five cross-process race trials. Observed controls cover same explicit lock exclusion, distinct
explicit locks, crash handle release, timeout/cancel, exception release, foreign-token refusal,
nonreentrant outer hold, case aliases, controller restart, missing parents, stale-file limitations,
an allowed effect with independently checked bytes/Popen identity, invalid paths, a real junction
and hardlink refusal. A crashed process is the exact synthetic actor using os._exit(17), not any
game/client/server. All handles/owned children close; exact fixture junctions are removed. Other
synthetic state is deliberately retained, not recursively cleaned or restored over real data.

Three negative calibrations demonstrably detect:

1. Same declared resource routed to two worktree-local lock files: both announce ownership;
   independent single-owner assertion rejects this broken model.
2. Changed synthetic protected bytes despite a possible success claim: independent SHA oracle
   rejects it. The main sentinel is restored only inside its newly generated fixture root.
3. Lexical check followed by parent-directory substitution: a deliberately unsafe writer follows
   a real synthetic junction to a protected test canary; its changed hash is detected. This is
   a test of the oracle and TOCTOU counterexample, not a candidate exploit or race-proof fix.

## Investigation failures and corrections

The first runner expected an `attempt` event for every acquisition request. The actor correctly
rejected a missing parent **before** attempting acquisition and returned a correlated error;
the runner wrongly flagged a protocol failure after11 controls. Corrected the transport to allow
only an early error result, while still refusing an early acquired/success response. No candidate
or production code changed and no safety requirement was weakened. Partial initial fixture
files remain ignored. Later runner revisions record failing-case/partial evidence in finally.

An intermediate corpus passed19 controls. Added an independently corroborated positive effect
control (L14), parent action transcripts and retained failure/teardown reporting; final corpus
passes20. Added14 declarative path vectors plus oversized input; the two final runs used the
same final corpus. No compilation, external game library or software installation was needed.
Final containment hardening checks existing scratch ancestors for reparse points, restricts
actors to direct run directories in their own worktree's scratch, and records source/corpus
hashes. The two runs cited above include these checks. This is test containment under controlled
fixtures, not a general race-proof sandbox or a candidate path-safety implementation.

Strict knowledge validation:127 records,0errors/0warnings. Existing open/partial research queues
remain unresolved; nothing was edited to suppress warnings. Review Python/JSON syntax, whitespace,
immutable baseline references and final Git scope are checked separately at completion.

## What remains untested

All46 candidate lock/path rows await an authorized immutable implementation and audited adapter.
Current executable controls deliberately do not test canonical resource selection, resource-set
identity, candidate receipt/fencing semantics, stale-client reconciliation, all final I/O entry
points, global lease scope, real exporter defaults, fake-process integrated Launch/Deploy/Rollback,
partial transaction/publication handling, multi-resource deadlocks or arbitrary filesystem actors.
The matrix defines concrete tests for these rather than treating a self-test green as completion.

Cross-user/session permissions, real process/Steam interactions, reboot/power-loss durability,
actual Windows access-denied configuration, VM/account setup, live save/cloud isolation and
game equipment/recovery remain separately gated. No ACL/account/VM/Steam change or GUI action.
No live installation or protected server/saves accessed. No game launch, deployment, config write,
merge, cherry-pick, commit or push. Cooperative exclusion does not prove external writers absent.

## Next handoff

Baseline HEAD remains b06cdfcac75c48349cfc26a09f5c4f1671947e83 on
research/dev-008-supervised-testing. Only this new review-preparation directory is intended
to remain untracked; existing tracked files/index and previous research stay unchanged.
No canonical knowledge record or prior review was silently amended.

When Claude reports completion: **stop and await separate owner authorization** to inspect
that immutable implementation. Then request the contract/test seams listed in REVIEW-PLAN.md,
bind a reviewed adapter and execute the applicable matrix. Do not infer its APIs, inspect WIP,
or begin testing it merely because the preparation harness is ready.
