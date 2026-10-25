---
id: "MOD-sighsorry-feedlikegrandma"
title: "sighsorry-FeedLikeGrandma"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.FeedLikeGrandma": "1.1.8", "package": "1.1.8"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-jerekuusela-expand-world-data]]", "[[MOD-odinplus-odinhorse]]", "[[MOD-therzie-monstrum]]", "[[MOD-xutz-valheimcuisine]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-FeedLikeGrandma/FeedLikeGrandma.dll", "cfg:BepInEx/config/sighsorry.FeedLikeGrandma.cfg", "pkg:BepInEx/plugins/sighsorry-FeedLikeGrandma/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-FeedLikeGrandma

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-FeedLikeGrandma/manifest.json`; package version **1.1.8**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **FeedLikeGrandma 1.1.8** — GUID `sighsorry.FeedLikeGrandma`; type `FeedLikeGrandma.FeedLikeGrandmaPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Yggdrah.BetterRiding`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `Yggdrah.DragonRiders`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `expand_world_data`; second constructor argument `2`; [[MOD-jerekuusela-expand-world-data]].
  - BepInDependency `Therzie.Monstrum`; second constructor argument `2`; [[MOD-therzie-monstrum]].
  - BepInDependency `Therzie.MonstrumDeepNorth`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `Raelaziel.OdinHorse`; second constructor argument `2`; [[MOD-odinplus-odinhorse]].
  - BepInDependency `XutzBR.ValheimCuisine`; second constructor argument `2`; [[MOD-xutz-valheimcuisine]].
  - BepInDependency `sighsorry.STUWard`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-FeedLikeGrandma/FeedLikeGrandma.dll` — assembly `FeedLikeGrandma 1.1.8.0`; target `.NETFramework,Version=v4.8`; 74 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.FeedLikeGrandma.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-jerekuusela-expand-world-data]], [[MOD-odinplus-odinhorse]], [[MOD-therzie-monstrum]], [[MOD-xutz-valheimcuisine]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
