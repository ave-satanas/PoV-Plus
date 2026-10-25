---
id: "MOD-sighsorry-finedining"
title: "sighsorry-FineDining"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.FineDining": "1.1.8", "package": "1.1.8"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-azumatt-azucraftyboxes]]", "[[MOD-jerekuusela-expand-world-data]]", "[[MOD-treextr-pathofvalheiman]]", "[[MOD-valheimmodding-jotunn]]", "[[MOD-xutz-valheimcuisine]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-FineDining/FineDining.dll", "cfg:BepInEx/config/sighsorry.FineDining.cfg", "pkg:BepInEx/plugins/sighsorry-FineDining/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-FineDining

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-FineDining/manifest.json`; package version **1.1.8**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **FineDining 1.1.8** — GUID `sighsorry.FineDining`; type `FineDining.FineDiningPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `2`; [[MOD-valheimmodding-jotunn]].
  - BepInDependency `Azumatt.AzuExtendedPlayerInventory`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `sighsorry.InventorySlots`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `Azumatt.AzuCraftyBoxes`; second constructor argument `2`; [[MOD-azumatt-azucraftyboxes]].
  - BepInDependency `XutzBR.ValheimCuisine`; second constructor argument `2`; [[MOD-xutz-valheimcuisine]].
  - BepInDependency `expand_world_data`; second constructor argument `2`; [[MOD-jerekuusela-expand-world-data]].
  - BepInDependency `treextr.pathofvalheiman`; second constructor argument `2`; [[MOD-treextr-pathofvalheiman]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-FineDining/FineDining.dll` — assembly `FineDining 1.1.8.0`; target `.NETFramework,Version=v4.8`; 96 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.FineDining.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-azumatt-azucraftyboxes]], [[MOD-jerekuusela-expand-world-data]], [[MOD-treextr-pathofvalheiman]], [[MOD-valheimmodding-jotunn]], [[MOD-xutz-valheimcuisine]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
