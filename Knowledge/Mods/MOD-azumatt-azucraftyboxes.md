---
id: "MOD-azumatt-azucraftyboxes"
title: "Azumatt-AzuCraftyBoxes"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Azumatt.AzuCraftyBoxes": "1.8.28", "package": "1.8.28"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-randyknapp-epicloot]]", "[[MOD-valheimmodding-jsondotnet]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Azumatt-AzuCraftyBoxes/AzuCraftyBoxes.dll", "cfg:BepInEx/config/Azumatt.AzuCraftyBoxes.cfg", "pkg:BepInEx/plugins/Azumatt-AzuCraftyBoxes/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Azumatt-AzuCraftyBoxes

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Azumatt-AzuCraftyBoxes/manifest.json`; package version **1.8.28**.
- Package publisher inferred from folder: **Azumatt**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/pdHgy6Bsng
- Manifest dependency requirements: none declared.

## Plugin identities

- **AzuCraftyBoxes 1.8.28** — GUID `Azumatt.AzuCraftyBoxes`; type `AzuCraftyBoxes.AzuCraftyBoxesPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `kg.ItemDrawers`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `org.bepinex.plugins.backpacks`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `org.bepinex.plugins.jewelcrafting`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `randyknapp.mods.epicloot`; second constructor argument `2`; [[MOD-randyknapp-epicloot]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Azumatt-AzuCraftyBoxes/AzuCraftyBoxes.dll` — assembly `AzuCraftyBoxes 1.8.28.0`; target `.NETStandard,Version=v2.1`; 68 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Azumatt` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jsondotnet]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Azumatt.AzuCraftyBoxes.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-randyknapp-epicloot]], [[MOD-valheimmodding-jsondotnet]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
