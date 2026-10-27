---
id: "EXP-006-Testing-policy-fixtures"
title: "DEV-008 offline controller and recovery falsification"
type: "experiment"
status: "partial"
created: "2026-10-27"
last_verified: "2026-10-27"
versions: {"Python": "3.14.0", "Windows": "11 Pro 25H2"}
evidence: ["Scripts/testing_policy.py", "Scripts/tests/test_testing_policy.py", "Scripts/tests/verify_testing_evidence.py", ".dev-state/dev-008/"]
related: ["[[DEV-008-Supervised-testing]]", "[[EVID-002-Persistence-and-control]]", "[[EXP-005-Passive-ownership-proof]]"]
confidence: "mixed"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# DEV-008 offline falsification and validation

## Scope

Scripts/testing_policy.py implements only necessary-condition decisions: relative existing-tree
fixture paths, immutable synthetic target/observation/action budgets, strict Level0 scenario
schema, explicit recovery prerequisites, fail-closed run phases and independent result scoring.
It has no game launcher, input synthesis, deployment, real recovery or scenario execution API.
The only recovery copier is a private unit-test fixture, constructed in a fresh TemporaryDirectory
with synthetic text files. No save discovery or installation defaults exist in that fixture.

The model's all-true evidence object can be fabricated by a caller. This limitation is explicitly
tested: it does not authenticate isolation/recovery or authorize a runtime transition. Production
needs an independent verifier behind a trusted adapter and actual OS boundaries. Do not reuse
these booleans as proof tokens. No cryptographic or ownership completeness claim is made.

## Adversarial coverage

The28 Python tests exercise multiple negative subcases:

| Area | Executed evidence / outcome |
|---|---|
| Target identity | Different PID, start identity, executable/hash, HWND or session rejected; target-title matching alone not used |
| Focus and observations | Foreground loss, locked flag, geometry/DPI change, unknown dialog, absent screenshot, consumed screenshot, stale/future/NaN/infinite clock rejected |
| Bounded actions | Exhausted/stopped budget, looping/held/drag/type requests rejected; one observation consumed once |
| Paths | Traversal, rooted/UNC/drive-relative/device-style/ADS, reserved names, empty/oversized/invalid segments, missing paths and hardlinks rejected |
| Links | Real Windows temporary junction escape rejected before target read; symlink lstat mode independently injected and rejected. Actual privileged symbolic-link creation not attempted |
| Recovery fixture | Exact independent SHA map, repeated new generations, baseline preservation, extra-file drift, hardlink inventory rejection, partial copy, corruption, prepublication interruption and postpublication/precommit interruption |
| Lease | Exclusive creation rejects a pre-existing stale/concurrent lock. No lock stealing. Cross-process/process-death/boot identity and global controller coordination not proven |
| Unknowns | Missing baseline/recovery condition, incomplete evidence object, unknown observation and missing journal commit cannot produce a valid result |
| Scenarios | Unknown fields, wrong schema/level/identity, booleans masquerading as numbers, unbounded values, missing baseline/evidence or recovery policy rejected |
| False success | Issued action without observation is Unknown; negative observation Fail; missing recovery Blocked even after positive gameplay observation |
| Stop/restart | Abort/timeout/focus/unexpected/emergency enters quarantine; no automatic resume. Retained partial generation and pending journal block a recovered claim |

All fixture writes are under newly allocated temporary roots. The path checker rejects reparse
ancestors and hard-linked files before reads/descent. It is a check-then-use policy, not an OS
capability. No claim that it resists a concurrent malicious filesystem actor is justified.
No writes are attempted through rejected paths. Fixture recovery uses only numeric generations;
no caller-provided restore destination, recursive deletion or in-place overwrite.

Four deliberately regressed in-memory policies are independently detected: issued-input-as-success,
missing-recovery-as-success, traversal allowed, and emergency-stop ignored. These are disposable
monkeypatches; original repository/installed code remains unchanged. They do not substitute for
audit of a future real input/recovery adapter.

## Preserved counterexamples and corrections

1. **Focus race:** a decision passes with correct foreground; a later observation shows another
   window. A second action is refused, but the first check cannot guarantee focus stayed stable
   before injection. No input was sent. This remains an architectural limitation.
2. **False evidence:** all-true caller declarations advance the synthetic model. They cannot prove
   a real baseline. Keep this counterexample; do not promote model state into runtime permission.
3. **Publication is not completion:** verified pointer may be published while recovery journal
   remains pending after a fault. Journal+fresh verifier required, not pointer alone.
4. Initial fixture incorrectly unlinked a held lock on Windows: WinError32. Close owned handle
   before unlink; lock remains held throughout the operation. Initial output retained locally.
5. Initial mutation-test wrapper reused subTest assertions, which unittest records instead of
   propagating to assertRaises. Replace with direct independent assertions, preserving the same
   safety requirements; four intended mutants are now demonstrably killed.
6. Strengthened inventory to reject links before file reads/descent; no rglob-based link traversal
   is used for fixture verification. Recovery obligation checks name every declared field rather
   than interpreting a damaged empty __dict__ as vacuous success.

## Validation inventory and reproduction

```powershell
python -B -m unittest discover -s Scripts/tests -v
pwsh -NoProfile -File Scripts/tests/Test-DevWorkflow.ps1
pwsh -NoProfile -File Scripts/tests/Export-TestingArchitectureEvidence.ps1
python -B Scripts/tests/verify_testing_evidence.py .dev-state/dev-008/persistence-il.json
pwsh -NoProfile -File Scripts/Dev.ps1 Build
pwsh -NoProfile -File Scripts/tests/Test-TomeDiagnostic.ps1
pwsh -NoProfile -File Scripts/tests/Test-TomeBridgeArtifact.ps1
pwsh -NoProfile -File Scripts/tests/Test-TomeLifecycleArtifact.ps1
pwsh -NoProfile -File Scripts/tests/Test-TomeProvenanceArtifact.ps1 -Dll Source/PoVPlus/bin/Release/net48/PoVPlus.dll
python -B Scripts/knowledge.py validate --strict
git diff --check
```

Existing identity18, StageB20, bridge22, installed contracts25, C1 contracts27, lifecycle28,
neutral30/adversarial tests and StageB purity checks were rerun. Workflow16 tests use fake
metadata/process launch primitives only. Fresh ordinary Release has0warnings/0errors; compiled
ordinary diagnostic/C1/PoC exclusion and unchanged211-call bridge artifact audit pass.
EXP-005's frozen36 original cases, three original counterexamples, corrected53 cases and seven
fixture/compiled-IL regression mutants were rerun separately. No opt-in game diagnostic activated.
New code is Python/offline metadata tooling; production C# and diagnostic implementation unchanged.
The final Python discovery run passes56 tests (28 existing +28 DEV-008). Two additional real
exporter CLI negative checks reject an output outside .dev-state and a temporary output junction
before metadata reads; the outside synthetic sentinel is unchanged. Completed strict knowledge
validation passes127 records with0errors/0warnings. Git emits an informational LF/CRLF notice
for INDEX under core.autocrlf; whitespace checks have no errors.

Initial artifact invocation omitted the provenance auditor's mandatory Dll parameter; corrected
invocation passes without changing auditor/source. An initial shell-quoted helper-generation
command failed; replaced with direct PowerShell text handling. One read raced an in-progress
metadata export and saw the previous JSON schema; analysis/verification used the completed export.
An intermediate strict KB run saw links to the not-yet-created experiment and unlinked new records;
the completed linked record set is validated again. These are preserved invocation/in-progress
failures, not ignored passing tests. Existing neutral fixture CS0649 compiler warnings remain
visible; expected mutant/invalid-case failures are distinguished from unexpected failures.

Local .dev-state/dev-008 holds commands, logs, full metadata, integrity report and review evidence;
none is intended for Git. Final counts/results are in the task completion report. The accepted
installation/source snapshot was not refreshed. All343 baseline DLL/config fingerprints match;
original installed DLL/config match and bridge=false. No client/server/save/config operations.

## Explicitly unproven

No real restoration, crash/power-loss durability, reboot recovery, registry/profile transaction,
VM graphics/Steam provisioning, external-sync denial, runtime window identity, game input,
stop watchdog latency, desktop disconnect behavior, cloud mount, or actual save reload tested.
No trace collected. Fixture exceptions model interrupted stages, not a killed process or failed
disk. Exact hashes do not prove semantic world validity. No normal-save non-access claim derives
from these tests: normal saves were not accessed at all, and a future environment must enforce
that boundary independently. No safe environment or StageC1 readiness certificate is produced.
