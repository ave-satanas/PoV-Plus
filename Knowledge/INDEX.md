---
id: "INDEX"
title: "PoVPlus engineering knowledge"
type: "index"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[KB-001-System]]"]
confidence: "verified"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# PoVPlus engineering knowledge

Read root AGENTS.md, this index, then relevant records before substantial work.
Open Knowledge/ as an Obsidian vault, or navigate/search in an editor. No Obsidian plugins required.

## Retrieval map

| Task | Canonical records |
|---|---|
| Project identity, priorities and engineering decisions | [[ARCH-002-Project-philosophy]], [[DEV-004-Intervention-framework]], [[DEC-004-Project-governance]] |
| Architecture and constraints | [[ARCH-001-PoVPlus]], [[DEC-001-Development-baseline]] |
| Build and reference APIs | [[DEV-001-Build]], [[DEP-001-Environment]] |
| Controlled deployment, launch, diagnostics and rollback | [[DEV-003-Workflow]], [[DEC-003-Workflow-safety]], [[EXP-002-First-deployment]], [[EXP-003-First-runtime-smoke]] |
| Mod GUIDs, versions, dependencies, configs | [[MOD-000-Inventory]], [[CFG-001-Configuration-map]] |
| Startup and missing messages | [[DIAG-001-Logging]], [[INV-001-Startup-message]], [[EXP-001-Logging-override]] |
| Warnings and patch triage | [[COMP-001-Warnings]], [[INV-002-Compatibility-triage]] |
| Magic books, Tome acceptance and equipment ownership | [[INV-003-Magic-book-tome-compatibility]] |
| Repaired offline diagnostics and experimental single-owner bridge | [[DEV-005-Tome-diagnostics]], [[COMP-002-Tome-bridge-design]] |
| Evidence and external research | [[EVID-001-Local-observations]], [[REF-001-External-docs]] |
| Record schema and maintenance | [[KB-001-System]], [[DEV-002-Maintenance]] |
| Session history and agent instructions | [[SESSION-20261024-Knowledge-bootstrap]], [[SESSION-20261024-Workflow]], [[SESSION-20261024-Philosophy]], [[DEC-002-Agent-integration]] |

## Current state

Next milestone: **INV-003 Stage A - Controlled Baseline Runtime Verification**, with the bridge
disabled. [[DEV-005-Tome-diagnostics]] defines the read-only baseline and original-DLL restoration;
Stage B bridge activation/gameplay testing requires separate later authorization. Neither stage
is part of the repository checkpoint.

The offline INV-003 follow-up repaired the opt-in guard/collector and added fixture/contract
tests; [[DEV-005-Tome-diagnostics]] owns reproducible commands. Ordinary builds exclude the
instrumentation and the installed DLL was not changed. [[COMP-002-Tome-bridge-design]] is a
partial source implementation: the initial acceptance bridge is now implemented offline and
disabled by default. No new runtime slot, acceptance or effects measurements have been obtained.

The 2026-10-25 book investigation, [[INV-003-Magic-book-tome-compatibility]], verifies
that MagicPlugin's disabled Tome classifier blocks PoV's native cell. The follow-up traces
the likely visible SeneaL/MS slot to `AzuEPI.emulated/$tome_DO`, with a separate prefab
restriction; the installed UI presentation lists omit PoV's native Tome cell. Confirm the
actual rendered descriptor with the documented read-only probe before changing settings.
Enabling MagicPlugin requests a distinct slot; author intent and runtime effects remain open.
The earlier preflight stopped before player/world access. The later explicitly authorized
experiment has now deployed a compile-gated diagnostic and launched the development client;
the user confirmed manual PoVPlus_Test / POVPlus_Lab inventory interaction, but the probe had
aborted at its exact identity guard before taking any snapshot. Runtime ownership/acceptance
remain unverified. At that historical cleanup checkpoint, the original DLL/source were restored, deployed
diagnostic instrumentation was absent, the client was stopped, and strict validation had no
warnings. The later offline repair reintroduces source tooling behind the opt-in build flag;
the original deployed DLL remains unchanged. Two third-party cfg files changed during the run and were left untouched by
cleanup; see INV-003 for hashes, evidence and the completed rollback receipt. The bridge source
was implemented later, remains default-off, and is not deployed.

Official philosophy and intervention policy are recorded in [[ARCH-002-Project-philosophy]]
and [[DEV-004-Intervention-framework]]. Design preferences and justified replacement are valid
project work; technical-conflict claims still require evidence. Operational authorization rules
remain in effect.

The first authorized on-disk deployment completed successfully; [[EXP-002-First-deployment]]
records build/deployment/backup hashes and the unchanged-file comparison. The subsequent
authorized startup/clean-close test passed: [[EXP-003-First-runtime-smoke]]. PoVPlus loaded once,
its own startup message appeared once, and no new warning/error signatures were observed.
The game is stopped. Real-installation rollback is now verified in INV-003; gameplay
compatibility and save isolation remain unverified.

The later 18:48:04 startup contains PoVPlus's own startup log. The earlier filtering hypothesis
is supported by the observed configuration/result comparison; see its experimental limitations.
Compatibility warnings remain triage items, not established gameplay defects.
The workspace is now a Git repository on `main`, with origin
`https://github.com/ave-satanas/PoV-Plus.git` (verified with `git branch --show-current`
and `git remote -v` on 2026-10-24). Earlier no-Git observations describe the bootstrap state.

After meaningful work, update canonical evidence, tests/outcomes and links; add a concise session
entry when useful and validate. Root AGENTS integration is approved and applied; future relevant sessions must follow this workflow.
