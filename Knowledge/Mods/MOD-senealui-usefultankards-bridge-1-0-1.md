---
id: "MOD-senealui-usefultankards-bridge-1-0-1"
title: "SenealUI-UsefulTankards-Bridge-1.0.1"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"avesatanas.senealui.usefultankards.bridge": "1.0.1"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-seneal-seneal-api]]", "[[MOD-seneal-seneal-ui]]", "[[MOD-sighsorry-usefultankards]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/SenealUI-UsefulTankards-Bridge-1.0.1/SenealUI-UsefulTankards-Bridge/SenealUI.UsefulTankards.Bridge.dll"]
claims: {}
contradicts: []
supersedes: []
---

# SenealUI-UsefulTankards-Bridge-1.0.1

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- No package manifest found in this group; do not infer package version or author.

## Plugin identities

- **SeneaLUI UsefulTankards Bridge 1.0.1** — GUID `avesatanas.senealui.usefultankards.bridge`; type `SenealUI.UsefulTankards.Bridge.Plugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `sighsorry.UsefulTankards`; second constructor argument `1.0.9`; [[MOD-sighsorry-usefultankards]].
  - BepInDependency `seneaL.valheim.api`; second constructor argument `1.2.0`; [[MOD-seneal-seneal-api]].
  - BepInDependency `seneaL.valheim.ui`; second constructor argument `1.2.5`; [[MOD-seneal-seneal-ui]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/SenealUI-UsefulTankards-Bridge-1.0.1/SenealUI-UsefulTankards-Bridge/SenealUI.UsefulTankards.Bridge.dll` — assembly `SenealUI.UsefulTankards.Bridge 1.0.1.0`; target `.NETFramework,Version=v4.8`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `SenealUI.UsefulTankards.Bridge` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-seneal-seneal-api]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-seneal-seneal-api]], [[MOD-seneal-seneal-ui]], [[MOD-sighsorry-usefultankards]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
