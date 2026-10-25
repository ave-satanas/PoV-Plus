---
id: "MOD-treextr-pathofvalheiman"
title: "treextr-PathOfValheiman"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"treextr.pathofvalheiman": "5.3.2", "package": "5.3.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-randyknapp-epicloot]]", "[[MOD-valheimmodding-jotunn]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll", "cfg:BepInEx/config/treextr.pathofvalheiman.cfg", "pkg:BepInEx/plugins/treextr-PathOfValheiman/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# treextr-PathOfValheiman

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/treextr-PathOfValheiman/manifest.json`; package version **5.3.2**.
- Package publisher inferred from folder: **treextr**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/XZsYRF2YtQ
- Manifest dependency requirements: `ValheimModding-Jotunn-2.30.2`.

## Plugin identities

- **Path of Valheiman 5.3.2** — GUID `treextr.pathofvalheiman`; type `PassiveTree.Plugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `randyknapp.mods.epicloot`; second constructor argument `2`; [[MOD-randyknapp-epicloot]].
  - BepInDependency `ymiron.valheim.itemdrawers`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `kg.ItemDrawers`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll` — assembly `PathOfValheiman 1.0.0.0`; target `.NETFramework,Version=v4.8`; 1707 HarmonyPatch attributes.
  - AssemblyCompany metadata: `PathOfValheiman` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jotunn]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/treextr.pathofvalheiman.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-randyknapp-epicloot]], [[MOD-valheimmodding-jotunn]].
<!-- inventory:end -->

## Engineering notes

Initial inventory coverage was metadata only; authorship beyond metadata remains unreviewed.

2026-10-25: [[INV-003-Magic-book-tome-compatibility]] traces installed `utility.tome`
acceptance through Utility routing and MagicPlugin's predicate, and virtual utility status
effects/regen. It verifies public Equipment signatures against the installed DLL and records
the unresolved distinction between the native cell and SeneaL/Magic Supremacy's Tome route.
