---
id: "MOD-randyknapp-epicloot"
title: "RandyKnapp-EpicLoot"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"randyknapp.mods.epicloot": "0.15.1", "package": "0.15.1"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-jotunn]]", "[[MOD-valheimmodding-jsondotnet]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/RandyKnapp-EpicLoot/EpicLoot.dll", "cfg:BepInEx/config/randyknapp.mods.epicloot.cfg", "pkg:BepInEx/plugins/RandyKnapp-EpicLoot/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# RandyKnapp-EpicLoot

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/RandyKnapp-EpicLoot/manifest.json`; package version **0.15.1**.
- Package publisher inferred from folder: **RandyKnapp**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/ZNhYeavv3C
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2351`, `ValheimModding-JsonDotNET-13.0.4`, `ValheimModding-Jotunn-2.30.2`.

## Plugin identities

- **Epic Loot 0.15.1** — GUID `randyknapp.mods.epicloot`; type `EpicLoot.EpicLoot`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].
  - BepInDependency `com.ValheimModding.NewtonsoftJsonDetector`; second constructor argument `1`; [[MOD-valheimmodding-jsondotnet]].
  - BepInDependency `randyknapp.mods.auga`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `vapok.mods.adventurebackpacks`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `kg.ValheimEnchantmentSystem`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `org.bepinex.plugins.steadyregeneration`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `MidnightsFX.ItemFavoriteFramework`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/RandyKnapp-EpicLoot/EpicLoot.dll` — assembly `EpicLoot 0.13.0.0`; target `.NETFramework,Version=v4.8.1`; 491 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Randy Knapp Mods` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jotunn]], [[MOD-valheimmodding-jsondotnet]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/randyknapp.mods.epicloot.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-jotunn]], [[MOD-valheimmodding-jsondotnet]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
