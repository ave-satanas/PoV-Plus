---
id: "MOD-sighsorry-dataforge"
title: "sighsorry-DataForge"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.DataForge": "1.4.0", "package": "1.4.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-azumatt-azucraftyboxes]]", "[[MOD-blacks7ar-magicplugin]]", "[[MOD-mschmoecker-vnei]]", "[[MOD-xutz-valheimcuisine]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-DataForge/DataForge.dll", "cfg:BepInEx/config/sighsorry.DataForge.cfg", "pkg:BepInEx/plugins/sighsorry-DataForge/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-DataForge

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-DataForge/manifest.json`; package version **1.4.0**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **DataForge 1.4.0** — GUID `sighsorry.DataForge`; type `DataForge.DataForgePlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `blacks7ar.MagicPlugin`; second constructor argument `2`; [[MOD-blacks7ar-magicplugin]].
  - BepInDependency `Azumatt.AzuCraftyBoxes`; second constructor argument `2`; [[MOD-azumatt-azucraftyboxes]].
  - BepInDependency `XutzBR.ValheimCuisine`; second constructor argument `2`; [[MOD-xutz-valheimcuisine]].
  - BepInDependency `com.maxsch.valheim.vnei`; second constructor argument `2`; [[MOD-mschmoecker-vnei]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-DataForge/DataForge.dll` — assembly `DataForge 1.4.0.0`; target `.NETFramework,Version=v4.8`; 61 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.DataForge.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-azumatt-azucraftyboxes]], [[MOD-blacks7ar-magicplugin]], [[MOD-mschmoecker-vnei]], [[MOD-xutz-valheimcuisine]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
