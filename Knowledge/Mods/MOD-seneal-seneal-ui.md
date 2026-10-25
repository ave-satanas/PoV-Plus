---
id: "MOD-seneal-seneal-ui"
title: "seneaL-SeneaL_UI"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"seneaL.valheim.ui": "1.2.7", "package": "1.2.7"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-seneal-seneal-api]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/seneaL-SeneaL_UI/SeneaL-UI/SeneaL-UI.dll", "cfg:BepInEx/config/seneaL.valheim.ui.cfg", "pkg:BepInEx/plugins/seneaL-SeneaL_UI/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# seneaL-SeneaL_UI

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/seneaL-SeneaL_UI/manifest.json`; package version **1.2.7**.
- Package publisher inferred from folder: **seneaL**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/xPQkzYJYc
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2202`, `seneaL-SeneaL_API-1.2.0`.

## Plugin identities

- **SeneaL UI 1.2.7** — GUID `seneaL.valheim.ui`; type `SeneaLUI.Plugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `seneaL.valheim.api`; second constructor argument `1`; [[MOD-seneal-seneal-api]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/seneaL-SeneaL_UI/SeneaL-UI/SeneaL-UI.dll` — assembly `SeneaL-UI 0.1.0.0`; target `.NETFramework,Version=v4.7.2`; 312 HarmonyPatch attributes.
  - AssemblyCompany metadata: `seneaL` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-seneal-seneal-api]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/seneaL.valheim.ui.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-seneal-seneal-api]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
