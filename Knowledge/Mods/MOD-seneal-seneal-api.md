---
id: "MOD-seneal-seneal-api"
title: "seneaL-SeneaL_API"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"__seneaL.valheim.api.early": "1.2.0", "seneaL.valheim.api": "1.2.0", "package": "1.2.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/seneaL-SeneaL_API/SeneaL-API/SeneaL-API.dll", "cfg:BepInEx/config/seneaL.valheim.api.cfg", "pkg:BepInEx/plugins/seneaL-SeneaL_API/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# seneaL-SeneaL_API

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/seneaL-SeneaL_API/manifest.json`; package version **1.2.0**.
- Package publisher inferred from folder: **seneaL**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/xPQkzYJYc
- Manifest dependency requirements: none declared.

## Plugin identities

- **SeneaL API Early 1.2.0** — GUID `__seneaL.valheim.api.early`; type `SeneaL.API.EarlyPlugin`; base `BepInEx.BaseUnityPlugin`.
- **SeneaL API 1.2.0** — GUID `seneaL.valheim.api`; type `SeneaL.API.Plugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/seneaL-SeneaL_API/SeneaL-API/SeneaL-API.dll` — assembly `SeneaL-API 1.0.0.0`; target `.NETFramework,Version=v4.7.2`; 9 HarmonyPatch attributes.
  - AssemblyCompany metadata: `seneaL` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/seneaL.valheim.api.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Initial inventory coverage was metadata only; authorship beyond metadata remains unreviewed.

2026-10-25: [[INV-003-Magic-book-tome-compatibility]] verifies installed StubBridge behavior
and existing log evidence of Azu API redirection in MagicPlugin and Magic Supremacy. A missing
real AzuEPI plugin does not establish that their Azu-gated equipment hooks are inactive.


## Offline Tome integration review

See [[COMP-002-Tome-bridge-design]] for the installed two-gate/lifecycle contracts and
[[DEV-005-Tome-diagnostics]] for the opt-in observational probe. These are static findings and
proposed integration boundaries; visible-slot runtime ownership remains unverified. No bridge
or third-party configuration change was implemented. Historical findings above remain applicable.
