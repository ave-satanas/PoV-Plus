---
id: "DEV-003-Workflow"
title: "Controlled development workflow"
type: "development"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"PoVPlus": "0.1.0", "BepInEx": "5.4.23.5"}
evidence: ["Scripts/Dev.ps1; Scripts/DevWorkflow.psm1; Scripts/tests/Test-DevWorkflow.ps1; local command results 2026-10-24"]
related: ["[[DEV-001-Build]]", "[[DEP-001-Environment]]", "[[DIAG-001-Logging]]", "[[DEC-003-Workflow-safety]]"]
confidence: "verified"
watch: ["src:Scripts/Dev.ps1", "src:Scripts/DevWorkflow.psm1", "src:Scripts/tests/Test-DevWorkflow.ps1"]
claims: {}
contradicts: []
supersedes: []
---

# Controlled development workflow

## Commands and authorization

Run Windows PowerShell from the repository root. Dev.ps1 validates the configured installation,
then dispatches one operation; nothing chains into automatic deployment or launch.

| Command | Behavior |
|---|---|
| Build | Calls existing Scripts/Build.ps1; Release by default, optional -Configuration Debug |
| Status | Displays paths, SHA-256s, deployed version, game/server state, latest receipt/log timestamps |
| Deploy | Validates and plans only PoVPlus.dll; requires explicit approval and -Authorized for actual writes |
| Launch | Separate approval and -Authorized; warns of shared saves; starts the exact development executable |
| Diagnose | Reads logs/settings and writes an ignored local Markdown report, never config edits |
| Rollback | Lists receipts; defaults to undoing latest completed operation; explicit approval, -Authorized and interactive confirmation |

All operations with side effects accept -DryRun, -WhatIf and -Confirm. Status only reads.
These switches do not authorize actions. A broad debugging request does not authorize installation changes.

```powershell
.\Scripts\Dev.ps1 Build
.\Scripts\Dev.ps1 Status
.\Scripts\Dev.ps1 Deploy -WhatIf
.\Scripts\Dev.ps1 Deploy -DryRun
.\Scripts\Dev.ps1 Diagnose
.\Scripts\Dev.ps1 Rollback -WhatIf
```

After explicit approval of each operation, separately:

```powershell
.\Scripts\Dev.ps1 Deploy -Authorized -Confirm
.\Scripts\Dev.ps1 Rollback -Authorized -BackupId '<receipt-id>'
.\Scripts\Dev.ps1 Launch -Authorized -Confirm
```

## Installation and identity boundaries

CLI accepts only S:\Valheim-Modding\PoVPlus-Dev, not a guessed path or production directory.
Repository/installation cannot contain each other, must share a volume for atomic replacement,
and cannot traverse junctions/reparse points. valheim.exe, BepInEx/core files, plugins and config
must exist. Internal module fixtures can supply a different allowlisted temporary root; CLI exposes no such override.
Metadata is read with the installed Mono.Cecil, without executing plugin code. Expected assembly
PoVPlus, one BepInPlugin identity povplus.core / PoVPlus, and BaseUnityPlugin inheritance are checked.
Every other plugin DLL is scanned for a renamed duplicate GUID; an unreadable DLL blocks rather
than assumes uniqueness. Unexpected DLLs anywhere below the build output block deployment.
A running development client/server, unknown executable path or failed process query blocks mutation.

## Managed files and transaction

Only BepInEx/plugins/PoVPlus/PoVPlus.dll is managed. PDB deployment is deliberately omitted;
existing PDBs/configs/other files remain untouched. Plans show target/source hashes and exact
backup, staging, lock and audit paths before writes. Identical bytes produce a no-op, not a backup.

Ignored .dev-state/backups/<id>/ contains the previous DLL (when present) and manifest.json.
The manifest records schema, operation, UTC, installation/repository, before/after hashes/versions,
managed filename, rollback parent and prepared/completed/failure state. Atomic journal updates
use manifest.json.tmp. .dev-state/staging/<id>/ holds the candidate; a repository-local exclusive
workflow.lock serializes deployment/rollback and the launch process check/start window. Staging and target share a volume: File.Replace
or first-install File.Move changes the managed directory entry atomically. Hashes are checked
before backup, after backup/staging, immediately before replacement, and after installation.
No whole-directory deletion or third-party assembly copy is performed.

Rollback restores the selected receipt's Before state only when current bytes match that receipt's
After state. Tampered backup or manually changed/foreign current DLL blocks restoration.
Each rollback itself makes an auditable backup, so undoing that rollback can restore its prior state.
A first deployment whose Before was absent can be rolled back by removing only that managed DLL.

## Failure and crash recovery

1. Stop the development client/server. Do not manually copy DLLs or delete state to bypass checks.
2. Run Status, inspect .dev-state/backups/<id>/manifest.json and the preserved backup hash.
3. A caught failure automatically restores Before only from a recognized After state; otherwise
   it preserves evidence and marks manual recovery required. A prepared receipt may indicate interruption.
4. Preview `Rollback -BackupId '<id>' -WhatIf`. Explicit IDs permit recovery from prepared/failed
   receipts if current bytes match known Before/After and the preserved DLL verifies. If already
   restored, this is a no-op. Obtain approval before the real rollback, then use -Authorized.
5. If metadata is unreadable, hashes unrecognized, or a lock remains actively held, stop and
   investigate; do not force replacement. Power loss, disk failure and external races cannot be
   guaranteed recoverable. Receipt/backups remain evidence for a separately approved repair.

## Launch and save behavior

Start-Process uses valheim.exe in the exact development working directory, with:
-screen-fullscreen 0 -screen-width 2560 -screen-height 1440 -popupwindow -console.
It checks for an existing development process before launch and never launches after another operation.
Warning is mandatory: the development client may use normal local characters/worlds. The user
reports earlier -savedir testing did not isolate client saves; this task did not retest it.
Do not assume installation separation or window flags isolate saves; no save files are inspected or modified.

## Diagnostics and session boundaries

Reports under .dev-state/reports/ record collection UTC, source paths and log last-write time.
Only the final BepInEx startup header is selected. A profiler is attached only if that session's
log references its timestamped filename; latest-by-time files are never silently mixed in.
Current cfg is distinguished from recorded startup/end filter policy. Up to 10 error/exception,
10 initialization and 10 warning excerpts are prioritized, with original line numbers and counts.
Basic user-path/secret redaction is not a guarantee; review reports before sharing.
No header means unverified session; absent startup Info is not failed Awake. Only a named BepInEx
loading error is classified as load-failure evidence; other exceptions need attribution. A profile's
combined construction/Awake/OnEnable timing is not instruction-level proof. Warnings are not automatically conflicts.
See [[DIAG-001-Logging]] and [[INV-001-Startup-message]]. Reports may describe a prior completed run.

## Verification and limits

```powershell
.\Scripts\tests\Test-DevWorkflow.ps1
.\Scripts\tests\Test-TomeDiagnostic.ps1
.\Scripts\tests\Test-TomeOffline.ps1
.\Scripts\tests\Test-TomeContracts.ps1
.\Scripts\tests\Test-TomeBridge.ps1
.\Scripts\tests\Test-TomeBridgeArtifact.ps1
python -B -m unittest discover -s Scripts/tests -v
python -B Scripts/knowledge.py validate --strict
```

PowerShell tests use temporary directories, JSON fake DLL identities and mocked metadata/process
probes; they never access the real installation. They cover path/identity/duplicate guards,
transaction/rollback hashes, failed-write recovery, drift/tampering, WhatIf, junctions and session parsing.
Real Status and Deploy -WhatIf exercise installed metadata; all 14 fixture tests pass.
A later authorized real deployment passed hash, receipt, backup and unrelated-file verification:
[[EXP-002-First-deployment]]. The later authorized launch/clean-close smoke test also passed:
[[EXP-003-First-runtime-smoke]]. Rollback planning was verified with -WhatIf; no real-installation
rollback or gameplay/world-entry test has been performed.
Before the first real test, the Release rebuild differed in hash from the installed DLL while
retaining version 0.1.0. After the authorized deployment, build and installed hashes match.
That difference is not evidence of a runtime defect; do not overwrite the historical deployed hash.

Follow-up, 2026-10-25: [[INV-003-Magic-book-tome-compatibility]] records the first verified
real-installation rollback after the user closed the game normally. The standard command,
confirmation prompt, completed receipt and original deployed hash were verified. The earlier
untested-rollback statement describes the previous checkpoint. Temporary diagnostic source was
archived outside compilation; ordinary build and installed DLL both exclude instrumentation.
The fresh normal rebuild differs in hash from the exact original DLL restored from backup;
it was not redeployed. Third-party config content drift during the run is documented in INV-003.

The later offline repair adds reproducible guard/harness tests, installed contract checks and a
focused collector: [[DEV-005-Tome-diagnostics]]. Those commands do not launch or deploy. The old
recursive one-off installation collector is retained as local history, not the runtime evidence
collection path; inaccessible optional evidence is recorded without touching cache permissions.


The later default-off bridge is source-only and covered by the bridge fixture/artifact checks.
[[COMP-002-Tome-bridge-design]] records startup-only configuration, no hot disablement, and
manual unequip/exit requirements before an authorized rollback. No real deployment is implied.
