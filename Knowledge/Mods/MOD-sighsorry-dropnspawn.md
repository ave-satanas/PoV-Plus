---
id: "MOD-sighsorry-dropnspawn"
title: "sighsorry-DropNSpawn"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.DropNSpawn": "1.4.8", "package": "1.4.8"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-jerekuusela-expand-world-data]]", "[[MOD-randyknapp-epicloot]]", "[[MOD-sighsorry-creaturemanager]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-DropNSpawn/DropNSpawn.dll", "cfg:BepInEx/config/sighsorry.DropNSpawn.cfg", "pkg:BepInEx/plugins/sighsorry-DropNSpawn/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-DropNSpawn

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-DropNSpawn/manifest.json`; package version **1.4.8**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.com/invite/VFRJcPwUdm
- Manifest dependency requirements: none declared.

## Plugin identities

- **DropNSpawn 1.4.8** — GUID `sighsorry.DropNSpawn`; type `DropNSpawn.DropNSpawnPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `expand_world_data`; second constructor argument `2`; [[MOD-jerekuusela-expand-world-data]].
  - BepInDependency `sighsorry.CreatureManager`; second constructor argument `2`; [[MOD-sighsorry-creaturemanager]].
  - BepInDependency `randyknapp.mods.epicloot`; second constructor argument `2`; [[MOD-randyknapp-epicloot]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-DropNSpawn/DropNSpawn.dll` — assembly `DropNSpawn 1.4.8.0`; target `.NETFramework,Version=v4.8`; 96 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-jerekuusela-expand-world-data]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.DropNSpawn.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-jerekuusela-expand-world-data]], [[MOD-randyknapp-epicloot]], [[MOD-sighsorry-creaturemanager]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
