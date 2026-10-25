---
id: "SESSION-20261024-Workflow"
title: "Controlled workflow implementation and verification"
type: "session"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"PoVPlus": "0.1.0", "BepInEx": "5.4.23.5"}
evidence: ["Scripts/Dev.ps1; Scripts/DevWorkflow.psm1; Scripts/tests/Test-DevWorkflow.ps1; local command output 2026-10-24"]
related: ["[[DEV-003-Workflow]]", "[[DEC-003-Workflow-safety]]", "[[INDEX]]"]
confidence: "verified"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Controlled workflow implementation and verification

## Scope

Implemented Dev.ps1 and module with Build, Deploy, Launch, Diagnose, Rollback and Status,
exact development path/identity checks, duplicate scanning, stopped-process gates, previews,
DLL-only backups, atomic journal/replacement, audit, rollback and recognized-state recovery.
New PowerShell fixture tests mock identities/processes/launch; no third-party DLLs copied into tests.
Updated README, architecture/build/diagnostic/decision records, presented and appended requested
AGENTS authorization rules, and ignored local state. Existing Build.ps1 and plugin source unchanged.
Snapshot collector now fingerprints the three workflow source/test files. Accepted snapshot changes
are limited to reviewed repository fingerprints; game assembly/manifest/cfg entries equal HEAD and
an independently collected read-only candidate (.dev-state/verification/environment-current.json).

## Checks and observations

- Dev.ps1 Build: Release passes, 0 warnings/errors; bin contains only PoVPlus.dll and its PDB.
- New build SHA-256: 351E948AFC75D8B5BFBEF67F2F1FBABB5A8A645AA7A5D51E9E804F7D501DF548.
- Installed SHA-256 remains 667C5D5BB6175D99690E9DA18DF67066612260773B62ECBD93A3F40BDDC673B7.
- Both retain plugin version 0.1.0. New generated AssemblyInformationalVersion includes the Git
  revision; the byte difference is not itself a gameplay change or a reason to bypass deployment approval.
- Status: game stopped, no real workflow backups; BepInEx log last-write UTC remains
  2026-10-25T01:51:26.6361862Z, correlated profile remains 2026-10-24_18-48-04.log.
- Deploy -WhatIf and -DryRun show exact replacement/backup plan without executing it.
- Rollback -WhatIf reports no real backups. Temporary fixtures test actual restore and rollback
  WhatIf, first-deploy removal, tampering/drift, automatic recovery, junctions and unknown processes.
- Launch -WhatIf prints exact arguments/shared-saves warning without starting a process; launcher
  primitive itself is tested only with a mocked Start-Process.
- Diagnose generates two ignored Markdown reports (initial and refined parser). Latest:
  .dev-state/reports/20261025T0238026016648Z-6db1e8b2426e41489b8917cee6df8b46.md.
  It observes the historical own startup message and correlated profiler Information policy.
- PowerShell fixture suite: 14 cases pass. Existing Python suite: 12 tests pass.
- Strict knowledge validation: 110 records, zero errors/warnings; existing compatibility research remains an open queue.
- Git diff reviewed, whitespace checks pass, no plugin source or Build.ps1 changes; no staging/commit/push performed.

## Boundaries and next action

No real deployment, rollback, game launch, configuration/save/world edit, commit or push occurred.
Only own-plugin build outputs and ignored repository-local reports/verification metadata were generated.
First real deployment test awaits explicit user approval of the reviewed plan. That approval must not
be treated as permission to launch the client. User-reported shared-save behavior was not retested.

## Ignored local artifacts

- .dev-state/reports/20261025T0234190774819Z-ca94b86f46ec4a94bfca429f49f160a7.md
- .dev-state/reports/20261025T0238026016648Z-6db1e8b2426e41489b8917cee6df8b46.md
- .dev-state/verification/environment-current.json
- Regenerated Source/PoVPlus/bin/Release/net48/PoVPlus.dll and PoVPlus.pdb, with normal ignored obj build intermediates.

## Commit preparation ? 2026-10-24

The user authorized review, commit on main and push to origin/main with message
Establish verified PoVPlus development workflow. Reviewed workflow/scripts/tests, canonical
notes and historical deployment/runtime evidence. Final Release build passed with zero warnings/errors;
14 workflow fixture tests and 12 Python tests passed; strict validation passed for 112 records.
Added a general *.log ignore rule. Checked 49 existing generated/probe paths: all ignored, with
no tracked compiled assemblies, logs, Python caches, build outputs or .dev-state artifacts.
Documentation PNGs remain trackable. Snapshot diff contains only the authorized deployed PoVPlus
fingerprint and reviewed repository-source fingerprints; unrelated installation evidence is unchanged.
The fetched origin/main matched main before publication. No game installation writes, deployment
or launch occurred during commit preparation. Publication result is reported to the user after push.
