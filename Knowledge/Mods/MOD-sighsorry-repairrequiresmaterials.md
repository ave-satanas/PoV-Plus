---
id: "MOD-sighsorry-repairrequiresmaterials"
title: "sighsorry-RepairRequiresMaterials"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.RepairRequiresMaterials": "1.1.2", "package": "1.1.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-azumatt-azucraftyboxes]]", "[[MOD-dreanegade-artisan-mastery]]", "[[MOD-treextr-pathofvalheiman]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-RepairRequiresMaterials/RepairRequiresMaterials.dll", "cfg:BepInEx/config/sighsorry.RepairRequiresMaterials.cfg", "pkg:BepInEx/plugins/sighsorry-RepairRequiresMaterials/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-RepairRequiresMaterials

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-RepairRequiresMaterials/manifest.json`; package version **1.1.2**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **RepairRequiresMaterials 1.1.2** — GUID `sighsorry.RepairRequiresMaterials`; type `RepairRequiresMaterials.RepairRequiresMaterialsPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Azumatt.AzuCraftyBoxes`; second constructor argument `2`; [[MOD-azumatt-azucraftyboxes]].
  - BepInDependency `Dreanegade.Artisan_Mastery`; second constructor argument `2`; [[MOD-dreanegade-artisan-mastery]].
  - BepInDependency `treextr.pathofvalheiman`; second constructor argument `2`; [[MOD-treextr-pathofvalheiman]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-RepairRequiresMaterials/RepairRequiresMaterials.dll` — assembly `RepairRequiresMaterials 1.1.2.0`; target `.NETFramework,Version=v4.8`; 32 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.RepairRequiresMaterials.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-azumatt-azucraftyboxes]], [[MOD-dreanegade-artisan-mastery]], [[MOD-treextr-pathofvalheiman]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
