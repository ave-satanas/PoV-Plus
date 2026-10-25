---
id: "MOD-sighsorry-pathofvalheimancraftyboxesbridge"
title: "sighsorry-PathOfValheimanCraftyBoxesBridge"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.PathOfValheimanCraftyBoxesBridge": "1.0.1", "package": "1.0.1"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-azumatt-azucraftyboxes]]", "[[MOD-treextr-pathofvalheiman]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-PathOfValheimanCraftyBoxesBridge/PathOfValheimanCraftyBoxesBridge.dll", "pkg:BepInEx/plugins/sighsorry-PathOfValheimanCraftyBoxesBridge/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-PathOfValheimanCraftyBoxesBridge

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-PathOfValheimanCraftyBoxesBridge/manifest.json`; package version **1.0.1**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest dependency requirements: `treextr-PathOfValheiman-5.0.4`, `Azumatt-AzuCraftyBoxes-1.8.28`.

## Plugin identities

- **PathOfValheiman CraftyBoxes Bridge 1.0.1** — GUID `sighsorry.PathOfValheimanCraftyBoxesBridge`; type `PathOfValheimanCraftyBoxesBridge.Plugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `treextr.pathofvalheiman`; second constructor argument `2`; [[MOD-treextr-pathofvalheiman]].
  - BepInDependency `Azumatt.AzuCraftyBoxes`; second constructor argument `2`; [[MOD-azumatt-azucraftyboxes]].
  - BepInDependency `sighsorry.InventorySlots`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-PathOfValheimanCraftyBoxesBridge/PathOfValheimanCraftyBoxesBridge.dll` — assembly `PathOfValheimanCraftyBoxesBridge 1.0.1.0`; target `.NETFramework,Version=v4.8.1`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-azumatt-azucraftyboxes]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-azumatt-azucraftyboxes]], [[MOD-treextr-pathofvalheiman]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
