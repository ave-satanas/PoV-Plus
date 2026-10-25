---
id: "INV-002-Compatibility-triage"
title: "Compatibility research queue"
type: "investigation"
status: "open"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[COMP-001-Warnings]]", "[[MOD-000-Inventory]]", "[[CFG-001-Configuration-map]]", "[[DEC-001-Development-baseline]]"]
confidence: "unverified"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Compatibility research queue

## Unresolved questions

1. Does the PoV Crossbows category warning cause duplicated mastery? Inspect effective tree
   config and lookup implementation; do not correct on warning alone.
2. Which installed APIs replace members missing from EpicLoot/PoV integrations, and are current
   fallbacks sufficient? Start at [[COMP-001-Warnings]], then inspect caller/callee IL.
3. Which patch targets overlap, with what priorities/conditions? Declarative metadata is indexed;
   dynamic patches and active runtime owner/order registry remain incompletely researched.
4. Which YAML/JSON/embedded overrides govern progression? Current coverage is cfg fingerprints
   and diagnostic settings. Effective precedence and ownership need targeted review.
5. Confirm actual authors/licenses from local licenses or authoritative source before redistribution.
6. Verify GitHub identity/history; no .git exists in this working directory.

Follow-up, 2026-10-24: item 6 records the earlier bootstrap observation. Local Git metadata now
exists; `git branch --show-current` returns `main`, and `git remote -v` reports origin
`https://github.com/ave-satanas/PoV-Plus.git`. Repository identity is verified locally; remote
availability and full history were not audited. See [[INDEX]] for current repository state.

## Research procedure

Apply [[DEV-004-Intervention-framework]] to each concern at substantive review: distinguish
verified defects/conflicts, preferences, missing integrations/features, redundancy, and unresolved
hypotheses. Existing questions above remain unresolved. A warning can establish a failed probe
without establishing harmful gameplay behavior. Preserve intentional author adaptations while
investigating; the framework permits justified replacement without predetermining this queue's outcome.

Choose one concern; verify involved hashes, exact signatures/config locators, distinguish optional
probes from required APIs, and propose the smallest reversible experiment. Runtime, deployment or
installation/config edits require authorization. Link focused child investigations when justified.
