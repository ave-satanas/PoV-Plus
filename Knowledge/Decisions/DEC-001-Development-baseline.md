---
id: "DEC-001-Development-baseline"
title: "Development constraints and choices"
type: "decision"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[ARCH-001-PoVPlus]]", "[[DEV-001-Build]]", "[[KB-001-System]]", "[[DEC-002-Agent-integration]]"]
confidence: "decision"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Development constraints and choices

## Existing instructions

Preserve PoV progression/balance and author compatibility patches. Prefer standalone patches;
verify assembly references/signatures, never invent APIs/keys, explain configuration changes,
preserve originals and check redistribution licenses. Overlap does not establish conflict.
Development installation is read-only unless authorized; original installation and saves are
outside scope. Builds do not authorize deployment or game launch.

## Existing implementation choices

- net48 matches PoV/PoVLink targets and installed 4.8 reference pack.
- Explicit local references with Private=false avoid copying third-party dependencies.
- Startup-only plugin establishes a minimal baseline before gameplay modifications.
- Build script uses local-only restore and checks output DLL names.

## Knowledge system decision

Use Markdown/YAML, stable IDs/wikilinks, sanitized JSON metadata, PowerShell collection and Python
standard-library validation. No database, service, package install or binary archive.
Tradeoff: structural graph/claim checks cannot prove prose semantics or runtime patch behavior.
Generated inventory blocks preserve manual notes. Root knowledge instructions were approved and applied; see:
[[DEC-002-Agent-integration]].
