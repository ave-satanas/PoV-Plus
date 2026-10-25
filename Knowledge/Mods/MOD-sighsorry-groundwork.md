---
id: "MOD-sighsorry-groundwork"
title: "sighsorry-Groundwork"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.Groundwork": "1.1.20", "package": "1.1.20"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-jerekuusela-expand-world-data]]", "[[MOD-treextr-pathofvalheiman]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-Groundwork/Groundwork.dll", "cfg:BepInEx/config/sighsorry.Groundwork.cfg", "pkg:BepInEx/plugins/sighsorry-Groundwork/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-Groundwork

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-Groundwork/manifest.json`; package version **1.1.20**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **Groundwork 1.1.20** — GUID `sighsorry.Groundwork`; type `Groundwork.GroundworkPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `advize.PlantEverything`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `treextr.pathofvalheiman`; second constructor argument `2`; [[MOD-treextr-pathofvalheiman]].
  - BepInDependency `org.bepinex.plugins.jewelcrafting`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `ZenDragon.ZenBeehive`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `expand_world_data`; second constructor argument `2`; [[MOD-jerekuusela-expand-world-data]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-Groundwork/Groundwork.dll` — assembly `Groundwork 1.1.20.0`; target `.NETFramework,Version=v4.8`; 85 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.Groundwork.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-jerekuusela-expand-world-data]], [[MOD-treextr-pathofvalheiman]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
