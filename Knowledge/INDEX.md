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
| Architecture and constraints | [[ARCH-001-PoVPlus]], [[DEC-001-Development-baseline]] |
| Build and reference APIs | [[DEV-001-Build]], [[DEP-001-Environment]] |
| Mod GUIDs, versions, dependencies, configs | [[MOD-000-Inventory]], [[CFG-001-Configuration-map]] |
| Startup and missing messages | [[DIAG-001-Logging]], [[INV-001-Startup-message]], [[EXP-001-Logging-override]] |
| Warnings and patch triage | [[COMP-001-Warnings]], [[INV-002-Compatibility-triage]] |
| Evidence and external research | [[EVID-001-Local-observations]], [[REF-001-External-docs]] |
| Record schema and maintenance | [[KB-001-System]], [[DEV-002-Maintenance]] |
| Session history and agent instructions | [[SESSION-20261024-Knowledge-bootstrap]], [[DEC-002-Agent-integration]] |

## Current state

The later 18:48:04 startup contains PoVPlus's own startup log. The earlier filtering hypothesis
is supported by the observed configuration/result comparison; see its experimental limitations.
Compatibility warnings remain triage items, not established gameplay defects.
The workspace is now a Git repository on `main`, with origin
`https://github.com/ave-satanas/PoV-Plus.git` (verified with `git branch --show-current`
and `git remote -v` on 2026-10-24). Earlier no-Git observations describe the bootstrap state.

After meaningful work, update canonical evidence, tests/outcomes and links; add a concise session
entry when useful and validate. Root AGENTS integration is approved and applied; future relevant sessions must follow this workflow.
