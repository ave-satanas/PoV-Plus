---
id: "EXP-001-Logging-override"
title: "Retrospective logging comparison"
type: "experiment"
status: "resolved"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[INV-001-Startup-message]]", "[[DIAG-001-Logging]]", "[[EVID-001-Local-observations]]"]
confidence: "verified"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Retrospective logging comparison

## Proposed test and later observed result

Earlier proposal: allow Errors, Warnings, Information for povplus.core under Logging.Mods,
then restart with unchanged DLL. This task performed neither edit nor launch/deployment.
It inspected the existing later result.

18:34:47 profile: mods=[]; Info absent in the previously examined BepInEx log.
18:48:04 profile: override present at both boundaries; current LogOutput.log line 184 contains
the exact startup message. Prior binary equality is preserved in the evidence ledger; current
snapshot fingerprints the deployed DLL.

Outcome supports filtering and verifies later startup logging. No contemporaneous record isolates
all changed variables or identifies the actor. This is a retrospective comparison, not an
experiment executed by Codex. [[INV-001-Startup-message]] records resolution and limitations.
