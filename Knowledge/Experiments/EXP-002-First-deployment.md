---
id: "EXP-002-First-deployment"
title: "First authorized development deployment"
type: "experiment"
status: "resolved"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"PoVPlus": "0.1.0", "BepInEx": "5.4.23.5"}
evidence: [".dev-state\\reports\\first-deployment-20261025T0632207882287Z-34ce09a537e5477e9a0b55fe9b4089df.md", ".dev-state\\backups\\20261025T0632207882287Z-34ce09a537e5477e9a0b55fe9b4089df\\manifest.json", ".dev-state/verification/first-deployment/comparison.json; user deployment authorization 2026-10-24"]
related: ["[[DEV-003-Workflow]]", "[[DEC-003-Workflow-safety]]", "[[DEP-001-Environment]]", "[[MOD-povplus]]"]
confidence: "verified"
watch: ["dll:BepInEx/plugins/PoVPlus/PoVPlus.dll"]
claims: {}
contradicts: []
supersedes: []
---

# First authorized development deployment

## Outcome

User explicitly authorized this deployment; Valheim launch, saves/world edits, commit and push
were prohibited. Reviewed the CLI/module's confirmation, path and rollback checks without code
changes. Release build passed with 0 warnings/errors. Preview showed only managed DLL replacement.
Standardized `Dev.ps1 Deploy -Authorized -Confirm:$false` completed at 2026-10-25T06:32:21.7715026Z.
The confirmation flag acknowledges existing explicit approval; other safety checks remained enabled.

Installed/build SHA-256: `351E948AFC75D8B5BFBEF67F2F1FBABB5A8A645AA7A5D51E9E804F7D501DF548`.
Original/backup SHA-256: `667C5D5BB6175D99690E9DA18DF67066612260773B62ECBD93A3F40BDDC673B7`.
Both plugin versions remain 0.1.0. Backup receipt ID: `20261025T0632207882287Z-34ce09a537e5477e9a0b55fe9b4089df`.
Local backup: `.dev-state/backups/20261025T0632207882287Z-34ce09a537e5477e9a0b55fe9b4089df/PoVPlus.dll`.
Receipt is schema 1, Deploy, completed, no error. Verified original backup and new installed hashes.
`Rollback -BackupId 20261025T0632207882287Z-34ce09a537e5477e9a0b55fe9b4089df -WhatIf` produces a valid restore plan. No real rollback executed.

## Scope verification

Before/after read-only fingerprints cover 2,921 non-save files (6,156,829,319 bytes each).
Exactly BepInEx/plugins/PoVPlus/PoVPlus.dll changed; zero additions/removals. Other 2,920 file
hashes, sizes, modification times and attributes match. Save directories were excluded and never
traversed; normal save locations and production installation were not accessed. Final game state Stopped.

The full ignored report is `.dev-state/reports/first-deployment-20261025T0632207882287Z-34ce09a537e5477e9a0b55fe9b4089df.md`; before/after/comparison JSON
are under `.dev-state/verification/first-deployment/`. Accepted snapshot changed only the known
PoVPlus DLL fingerprint after this comparison; historical hashes in earlier evidence remain intact.

## Limitations and next step

This establishes on-disk deployment, backup integrity and rollback planning, not runtime behavior
of the new DLL. Existing startup logs predate deployment. A separately authorized later launch/smoke test is now recorded in [[EXP-003-First-runtime-smoke]].
The no-launch statements above describe this deployment-only experiment. No deployment-script/source
modifications, configuration/save edits, commit or push occurred during that experiment.
