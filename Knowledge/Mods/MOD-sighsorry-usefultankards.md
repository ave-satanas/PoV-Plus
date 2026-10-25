---
id: "MOD-sighsorry-usefultankards"
title: "sighsorry-UsefulTankards"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.UsefulTankards": "1.0.9", "package": "1.0.9"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-randyknapp-epicloot]]", "[[MOD-xutz-valheimcuisine]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-UsefulTankards/UsefulTankards.dll", "cfg:BepInEx/config/sighsorry.UsefulTankards.cfg", "pkg:BepInEx/plugins/sighsorry-UsefulTankards/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-UsefulTankards

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-UsefulTankards/manifest.json`; package version **1.0.9**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **UsefulTankards 1.0.9** — GUID `sighsorry.UsefulTankards`; type `UsefulTankards.UsefulTankardsPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `XutzBR.ValheimCuisine`; second constructor argument `2`; [[MOD-xutz-valheimcuisine]].
  - BepInDependency `randyknapp.mods.epicloot`; second constructor argument `2`; [[MOD-randyknapp-epicloot]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-UsefulTankards/UsefulTankards.dll` — assembly `UsefulTankards 1.0.9.0`; target `.NETFramework,Version=v4.8`; 42 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.UsefulTankards.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-randyknapp-epicloot]], [[MOD-xutz-valheimcuisine]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
