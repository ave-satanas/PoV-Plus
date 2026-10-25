---
id: "EXP-003-First-runtime-smoke"
title: "First authorized runtime startup and clean close"
type: "experiment"
status: "resolved"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"PoVPlus": "0.1.0", "BepInEx": "5.4.23.5", "Valheim": "1.0.20", "LoadTimeProfiler": "1.3.2"}
evidence: [".dev-state/reports/runtime-smoke-2026-10-24_23-37-47.md", ".dev-state/runtime-smoke/before.json; after.json; launch.json; close.json; comparison.json", "BepInEx/config/LoadTimeProfiler/2026-10-24_23-37-47.log; BepInEx/LogOutput.log current session"]
related: ["[[EXP-002-First-deployment]]", "[[DEV-003-Workflow]]", "[[DIAG-001-Logging]]", "[[COMP-001-Warnings]]", "[[MOD-povplus]]"]
confidence: "verified"
watch: ["dll:BepInEx/plugins/PoVPlus/PoVPlus.dll", "cfg:BepInEx/config/sighsorry.LoadTimeProfiler.cfg"]
claims: {}
contradicts: []
supersedes: []
---

# First authorized runtime startup and clean close

## Conditions and outcome

User authorized one development launch/clean close, prohibiting character/world selection,
save access, config/assembly edits, unnecessary redeployment, commit and push.
Build/installed SHA-256 matched `351E948AFC75D8B5BFBEF67F2F1FBABB5A8A645AA7A5D51E9E804F7D501DF548` before/after.
Standard launcher started exact development executable PID 26720 at
2026-10-25T06:37:44.7466180Z, using the documented window/console flags.
Current session: **2026-10-24_23-37-47** (America/Los_Angeles).

**PASS:** BepInEx loaded povplus.core once, its exact own startup message appeared once,
88 plugins loaded with 0 skipped/failed, and Start To Lobby completed in 1 min 29.909s.
Current-session profiler policy allowed povplus.core Information at both boundaries.
Final log has zero error/exception lines; all 31 normalized unique warning/error signatures
match baseline. Existing API/compatibility/asset warnings remain; no new regression or
PoVPlus-attributed failure was observed. Normalize timestamps before treating a warning as new.

## Closure and constraints

A prepared local helper verified exact executable/PID and one visible UnityWndClass window,
then requested normal WM_CLOSE. No force termination or other UI/console input was used.
Process exited by 2026-10-25T06:39:48.3502602Z; shutdown log includes Stopping build thread and
Steam manager on destroy. Final Status is Stopped. Numeric exit code was unavailable, not assumed 0.
No character/world selection/load actions or direct save-file reads/writes were performed by tools.
The runtime's possible background save discovery was not audited; save isolation is not claimed.
245 assembly, 87 manifest and 99 cfg content hashes are identical before/after. No redeploy.

## Evidence and interpretation

Full ignored report: `.dev-state/reports/runtime-smoke-2026-10-24_23-37-47.md`.
Snapshots/checkpoints/helpers: `.dev-state/runtime-smoke/`.
Baseline log was 18:48:04; final log was 23:37:47. Both happen to be 68106 bytes, but hashes,
profile identity and timestamps differ: file size alone cannot establish session identity.
Own startup output confirms this deployed binary initialized; combined profiler timing alone does not.

## Readiness

Ready for controlled ordinary plugin iteration through standardized build/deploy/launch/diagnose
and cooperative close, with existing authorization gates. Gameplay/world entry, save isolation,
remaining third-party compatibility questions and real-installation rollback are untested.
No source/workflow script edits, game configuration edits, commit or push occurred.
