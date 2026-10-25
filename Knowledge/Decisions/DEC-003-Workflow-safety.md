---
id: "DEC-003-Workflow-safety"
title: "Workflow safety boundaries"
type: "decision"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"PoVPlus": "0.1.0", "BepInEx": "5.4.23.5"}
evidence: ["Scripts/Dev.ps1; Scripts/DevWorkflow.psm1; Scripts/tests/Test-DevWorkflow.ps1; local command results 2026-10-24"]
related: ["[[DEV-003-Workflow]]", "[[DEC-001-Development-baseline]]", "[[ARCH-001-PoVPlus]]"]
confidence: "decision"
watch: ["src:Scripts/Dev.ps1", "src:Scripts/DevWorkflow.psm1", "src:Scripts/tests/Test-DevWorkflow.ps1"]
claims: {}
contradicts: []
supersedes: []
---

# Workflow safety boundaries

## Decision and rationale

Implement Scripts/Dev.ps1 as the ordinary entry point; reuse Build.ps1 rather than duplicate compiler logic.
Keep parsing/transaction primitives in DevWorkflow.psm1 with temporary-fixture tests. Scripts protect
against mistakes, not a hostile local administrator or arbitrary external filesystem races.

Use an exact installation allowlist, fail-closed process/metadata checks, no junction traversal,
DLL-only managed state, repository-local hash-verified backups/journals and atomic same-volume replacement.
PDB deployment is omitted to avoid assuming symbol matching. Never manage third-party mods, configs or saves.
Backups cannot be placed under BepInEx discovery. All local state/reports stay ignored.

Builds and read-only diagnostics are ordinary development actions. Deploy/rollback/launch/config changes
require explicit user authorization; -Authorized is an acknowledgement, not evidence of conversational
approval. Rollback also requires an interactive confirmation; preview paths remain nonmutating.
A general debugging request must not be interpreted as authorization. AGENTS was presented and appended
as explicitly requested in this task, preserving earlier instructions.

Diagnostics use session boundaries and correlated profiler references, distinguish current config from
recorded policy, and do not promote an absent Info event into an initialization failure. No automatic
logging configuration changes or warning-driven gameplay fixes are permitted.

No real deployment, rollback or launch occurred during implementation. Before the first actual
transaction, obtain approval for the concrete plan and preserve the current installed DLL in the new
backup workflow. User reports client saves remain shared despite earlier -savedir testing; no retest here.
See [[DEV-003-Workflow]] for limits, recovery procedure, tests and exact commands.

Follow-up: [[EXP-002-First-deployment]] records the later explicitly approved first deployment.
It passed on-disk backup/receipt/hash verification without launching or changing unrelated files.
