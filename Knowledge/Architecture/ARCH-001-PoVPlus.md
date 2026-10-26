---
id: "ARCH-001-PoVPlus"
title: "Development architecture"
type: "architecture"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[DEV-001-Build]]", "[[DIAG-001-Logging]]", "[[DEC-001-Development-baseline]]", "[[DEP-001-Environment]]"]
confidence: "verified"
watch: ["src:Source/PoVPlus/Plugin.cs", "src:Source/PoVPlus/PoVPlus.csproj"]
claims: {}
contradicts: []
supersedes: []
---

# Development architecture

[[ARCH-002-Project-philosophy]] defines the project identity and priorities;
[[DEV-004-Intervention-framework]] governs intervention choices. The facts below describe the
initial implementation, whose startup-only behavior was a development baseline rather than a
limit on future compatibility, integration, customization, or original functionality.

## Verified boundaries

Source/PoVPlus.sln contains the SDK-style Source/PoVPlus/PoVPlus.csproj library, net48, C# 7.3.
Plugin.cs declares povplus.core, PoVPlus 0.1.0, derived from BaseUnityPlugin. The initial
Awake only logged; this historical baseline preceded the default-off bridge below. [[DEV-001-Build]] owns build details.

Offline review, 2026-10-25: this statement applies to the ordinary build. An explicit
EnableTomeDiagnostic=true build now includes a bounded read-only observation module; the csproj
excludes all Diagnostics sources by default. Default artifact exclusion and the opt-in compile
were verified offline; no deployment/runtime behavior is inferred. [[DEV-005-Tome-diagnostics]]
owns the guard, collector, tests and authorization limits. At that checkpoint no bridge existed.

The later offline source implementation adds a startup-bound default-false acceptance feature
and five soft dependency declarations. With the feature Off no patches apply. When explicitly
enabled, it validates and extends only the two MS Tome gates; MS retains all lifecycle ownership.
No runtime behavior is verified and nothing is deployed. [[COMP-002-Tome-bridge-design]] owns the
configuration, guards, tests and limitations. Ordinary builds still exclude diagnostic tooling.

References come from S:\Valheim-Modding\PoVPlus-Dev. All five explicit references use Private=false.
Unity Mono loads components through BepInEx 5. [[DIAG-001-Logging]] owns lifecycle/log behavior.

## Controlled development operations

Scripts/Dev.ps1 dispatches Build, Deploy, Launch, Diagnose, Rollback and Status.
Scripts/DevWorkflow.psm1 owns guarded metadata/planning/transaction and diagnostic functions.
Only PoVPlus.dll is managed; local backup/audit/report state lives under ignored .dev-state/.
See [[DEV-003-Workflow]] and [[DEC-003-Workflow-safety]] for authorization and recovery.

## Directory responsibilities

Source/: solution/code. Scripts/: build and knowledge tools. Configs/: reserved, currently empty.
Documentation/: existing empty area. Knowledge/: linked engineering memory and sanitized metadata.
Original game and saves are outside task scope. At bootstrap, no GitHub remote or .git metadata
was present. The repository has since been initialized; [[INDEX]] records the verified current remote/branch.
See [[DEC-001-Development-baseline]] for constraints and [[DEP-001-Environment]] for runtime scope.
