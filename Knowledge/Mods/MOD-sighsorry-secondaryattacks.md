---
id: "MOD-sighsorry-secondaryattacks"
title: "sighsorry-SecondaryAttacks"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.SecondaryAttacks": "1.2.20", "package": "1.2.20"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-blacks7ar-magicplugin]]", "[[MOD-dreanegade-magic-supremacy]]", "[[MOD-randyknapp-epicloot]]", "[[MOD-sighsorry-creaturemanager]]", "[[MOD-therzie-wizardry]]", "[[MOD-treextr-pathofvalheiman]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-SecondaryAttacks/SecondaryAttacks.dll", "cfg:BepInEx/config/sighsorry.SecondaryAttacks.cfg", "pkg:BepInEx/plugins/sighsorry-SecondaryAttacks/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-SecondaryAttacks

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-SecondaryAttacks/manifest.json`; package version **1.2.20**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **SecondaryAttacks 1.2.20** — GUID `sighsorry.SecondaryAttacks`; type `SecondaryAttacks.SecondaryAttacksPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Dreanegade.Magic_Supremacy`; second constructor argument `2`; [[MOD-dreanegade-magic-supremacy]].
  - BepInDependency `Therzie.Wizardry`; second constructor argument `2`; [[MOD-therzie-wizardry]].
  - BepInDependency `radamanto.Vikings_Magic`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `Dreanegade.Hunter_Legacy`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `randyknapp.mods.epicloot`; second constructor argument `2`; [[MOD-randyknapp-epicloot]].
  - BepInDependency `treextr.pathofvalheiman`; second constructor argument `2`; [[MOD-treextr-pathofvalheiman]].
  - BepInDependency `blacks7ar.MagicPlugin`; second constructor argument `2`; [[MOD-blacks7ar-magicplugin]].
  - BepInDependency `shudnal.Quickstep`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `vapok.mods.shieldmebruh`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `sighsorry.CreatureManager`; second constructor argument `2`; [[MOD-sighsorry-creaturemanager]].
  - BepInDependency `org.bepinex.plugins.creaturelevelcontrol`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `MidnightsFX.StarLevelSystem`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-SecondaryAttacks/SecondaryAttacks.dll` — assembly `SecondaryAttacks 1.2.20.0`; target `.NETFramework,Version=v4.8`; 98 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.SecondaryAttacks.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-blacks7ar-magicplugin]], [[MOD-dreanegade-magic-supremacy]], [[MOD-randyknapp-epicloot]], [[MOD-sighsorry-creaturemanager]], [[MOD-therzie-wizardry]], [[MOD-treextr-pathofvalheiman]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
