# Rotation 1 threat model and immutable baseline inventory

Scope: F9/F10 and directly related operation/evidence safety. Source baseline
b06cdfcac75c48349cfc26a09f5c4f1671947e83, not Claude's upcoming implementation.
`source_inventory.py` exports hashes and line locations from immutable baseline Git objects;
the generated inventory is discovery evidence, not an executable policy or completeness proof.

## Assets, actors and trust boundaries

Assets: the managed PoVPlus DLL and directory entry; unrelated installation files/configuration;
approved source artifact; independent backup/receipt; worktree-local evidence/build outputs;
resource/operation identity; and the ability to launch the exact approved executable once.
Personal saves, cloud accounts and the dedicated server are protected **outside scope**.

Threats include two well-intentioned controllers in different worktrees, a restarted controller,
an old still-running worker, partial/crashed operations, file watchers, concurrent builds, manual
launches, stale/wrong evidence, path aliases and ordinary filesystem errors. Deliberate synthetic
inputs/faults probe the stated guarantees; they do not imply a hostile administrator is part of
the supported production threat model. Administrator/native attackers or arbitrary adapter code
cannot be contained by a Python test root marker or a cooperative lock. Declare assumptions and
test enforceable boundaries instead of claiming universal confinement.

Five distinct boundaries must not be collapsed:

1. **Authority:** owner approval is conversational and operation-scoped. A CLI flag, receipt or
   Boolean acknowledgement does not create it.
2. **Identity:** strings supplied by a caller are not proof of a canonical physical resource,
   approved source/destination or actual process/window identity.
3. **Exclusion:** only the actual primitive/lease lifetime can establish cooperative ownership.
   A lockfile, metadata record or plausible PID can be stale or forged.
4. **Operation:** successful acquisition does not guarantee every write/launch passes the guard;
   helper bypass, exception recovery and post-check races must be reviewed separately.
5. **Evidence:** logs/receipts/hashes describe scoped observations. They do not establish
   continuous state, all-writer coverage, successful recovery, stopped clients or authorized use.

## Current operations, every lock site, and destinations

| Baseline operation | Reads / writes / launch | Existing guard and evidence boundary |
|---|---|---|
| Dev.ps1 context creation | Reads configured installation markers; repo from PSScriptRoot; fixed dev allowlist | New-DevContext checks lexical boundaries and existing reparse ancestors; no lock acquired |
| Build / Build.ps1 | Reads installed reference DLLs; writes own Source/bin and obj, normal SDK state | No installation writes or game launch. Local build races can invalidate an artifact selected for later deploy |
| Deploy planning | Reads build/plugin metadata, duplicate GUIDs, current target and process query | Planning is not ownership. Before/After hashes are observations that execution must revalidate |
| Invoke-DevTransition for Deploy/Rollback | Writes only target PoVPlus.dll/parent directories in installation; backups, staging, lockfile and prepared/completed/failure receipt in repo .dev-state | **Lock site1:** DevWorkflow.psm1:246 opens Context.State/workflow.lock with FileShare.None; released at299 finally. Context.State is repo/.dev-state at48; therefore per-worktree |
| Failure recovery inside transaction | May replace/delete managed target from recognized states, write recovery staging/receipt | Same held transaction handle; stopped/process/hash checks must remain on failure paths too. Unknown current bytes must not cause blind restore |
| Rollback planning | Reads repo backups/manifests; checks manifest.Repository matches context, before backup hash/current target | A shared lock alone cannot transfer backup authority between repositories; no silent "latest receipt anywhere" fallback |
| Start-DevGame | Starts installation/valheim.exe in installation working directory, writes local lockfile | **Lock site2:** at333, same FileShare.None path. Finally337 releases immediately after Start-Process returns, not at client exit |
| Status | Reads build/deployment/process/log timestamps | No coherent global snapshot/ownership guarantee; separate reads can race another controller |
| Diagnose | Reads BepInEx log/config and session-correlated profiler; Save-DevReport writes own .dev-state/reports | Session/filter limitations explicit; report status is not an acquired lease or current process proof |
| Other evidence exporters/tests | Read pinned metadata/local artifacts; write own or explicitly selected output paths | Absolute/cwd-dependent defaults can cross worktrees; see path inventory below. Do not execute unsafe defaults during preparation |

Only those two lock acquisition sites exist in the baseline module; no whole-session owner,
cross-worktree shared store, reentrant/multi-resource protocol or persistent recovery lease is
implemented there. CLI approval/ShouldProcess gates are separate from these primitives.
`-WhatIf`/DryRun/no-op paths must not gain hidden mutation or launch side effects in a correction.

The module currently has no general game-configuration writer. Separately authorized own-config
changes are still shared writes that any eventual workflow must cover, not permission to introduce
such a command in this rotation. No third-party file becomes managed merely by adding a lock.

## Root/default/evidence contamination inventory

| Site | Current behavior / risk to test later |
|---|---|
| Dev.ps1 / Build.ps1 | Repository from script root; default ValheimDevPath is fixed development installation. Build also accepts its supplied reference path. Preserve allowlist; do not use CWD as another implicit repo |
| DevWorkflow.New-DevContext | State/backups/staging/reports keyed by repository, target by installation; exact drive-string comparison for same-volume assumption. Lexical GetFullPath is not file identity or volume identity |
| Export-TomeOwnershipAudit.ps1:3-4 | DiagnosticDll and Output default to primary worktree absolute .dev-state paths: can read author's artifact and write author's tree from another worktree |
| Test-TomeOwnershipAudit.ps1:2 | Metadata default selects primary-worktree previous export |
| Test-TomeLifecycleArtifact.ps1:2 | Dll default selects primary build, not invoking worktree's artifact |
| DEV-005:90 / DEV-006:244 / EXP-005:145 | Documented absolute OutputPath points to primary tree; a command run elsewhere can contaminate it |
| Export-TomeProvenanceEvidence.ps1 | Default output relative to current directory, not rooted via script location; invoke from unrelated CWD to challenge intended ownership |
| Test-TomeProvenanceArtifact EvidenceOutput | Checks textual repository .dev-state prefix; caller's evidence destination and ancestor substitutions require explicit review |
| collect_tome_evidence.py / other collectors | Explicit path boundaries, input timestamps/session tags and report provenance require checking independently; inclusion in this list is not a new demonstrated defect |

Also review every selected candidate diff, compiler OutputPath/BaseIntermediateOutputPath,
environment/current-directory influence, temporary path, backup ID and log-derived profiler
filename. A dated absolute path in historical prose is benign evidence; an implicit **current**
input/output that selects someone else's artifact is not. Keep those classifications separate.

## External processes and identity races

Get-DevProcesses queries client/server names; Get-DevRuntime filters known paths under the
configured development installation, and unknown executable paths block. A known client from
another installation is ignored even though the current shared-profile topology may share saves.
Steam is not queried by the lock. A file/mutex held by cooperative controllers cannot prevent a
manual or Steam-triggered launch between the final process query and a protected operation.

Use simulated process snapshots/query errors and a fake-launch recorder later; no real client,
Steam or dedicated-server probing is needed now. An external process outside the declared
resource boundary is not automatically a target to terminate or manipulate. Unknown ownership
must stop/quarantine, not authorize forced termination or recovery. After a controller crash,
an orphaned simulated client may remain alive despite an available OS lock.

## Proof classes and non-goals

| Evidence class | Can establish | Cannot establish |
|---|---|---|
| Static source/AST review | Acquisition/release/output/entry-point paths; branch coverage obligations | Actual OS exclusivity, absence of dynamic calls or races |
| Unit tests with controlled failures | Validation decisions and exception/receipt behavior for exercised branches | All protected operations actually use the guard |
| Cross-process synthetic tests | Observed exclusion, bounded waits, crash handle release and ownership behavior under tested user/session | Cross-user ACLs, reboot durability, real game stop/recovery or all schedules |
| Synthetic integration | No sentinel changes, exact fake target/source, mock launch counts, transaction/publication behavior under injected faults | Isolation of real saves/cloud or actual lifecycle correctness |
| Later operational validation | Chosen real storage/process/session boundaries under separate approval | Universal future behavior or equipment ownership completeness |

Cross-user/session cases are unexecuted: no accounts or sessions may be created/changed now.
Path check/use substitution is tested only in an owned private tree against a deliberately unsafe
writer. Candidate protection may require stable handles, checked identities and/or controlled
ACL assumptions. Do not prescribe a mechanism or mark a test passed because a helper returned
True. If a guarantee exceeds the threat model, narrow the **claim explicitly**, not the test.

Stage B acceptance remains verified; Stage C1 ownership, lifecycle, persistence, neutral state
and rollback safety remain unverified. Shared locking/path corrections cannot certify them.
