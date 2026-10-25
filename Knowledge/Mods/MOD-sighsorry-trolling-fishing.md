---
id: "MOD-sighsorry-trolling-fishing"
title: "sighsorry-Trolling_Fishing"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.TrollingFishing": "1.1.4", "package": "1.1.4"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-randyknapp-epicloot]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-Trolling_Fishing/TrollingFishing.dll", "cfg:BepInEx/config/sighsorry.TrollingFishing.cfg", "pkg:BepInEx/plugins/sighsorry-Trolling_Fishing/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-Trolling_Fishing

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-Trolling_Fishing/manifest.json`; package version **1.1.4**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **TrollingFishing 1.1.4** — GUID `sighsorry.TrollingFishing`; type `TrollingFishing.TrollingFishingPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `WackyMole.ItemRequiresSkillLevel`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `Detalhes.ItemRequiresSkillLevel`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `randyknapp.mods.epicloot`; second constructor argument `2`; [[MOD-randyknapp-epicloot]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-Trolling_Fishing/TrollingFishing.dll` — assembly `TrollingFishing 1.1.4.0`; target `.NETFramework,Version=v4.8`; 72 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.TrollingFishing.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-randyknapp-epicloot]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
