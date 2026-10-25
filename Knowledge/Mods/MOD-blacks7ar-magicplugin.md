---
id: "MOD-blacks7ar-magicplugin"
title: "blacks7ar-MagicPlugin"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"blacks7ar.MagicPlugin": "2.2.2", "package": "2.2.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/blacks7ar-MagicPlugin/MagicPlugin.dll", "cfg:BepInEx/config/blacks7ar.MagicPlugin.cfg", "pkg:BepInEx/plugins/blacks7ar-MagicPlugin/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# blacks7ar-MagicPlugin

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/blacks7ar-MagicPlugin/manifest.json`; package version **2.2.2**.
- Package publisher inferred from folder: **blacks7ar**. This is not verified personal authorship.
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`.

## Plugin identities

- **MagicPlugin 2.2.2** — GUID `blacks7ar.MagicPlugin`; type `MagicPlugin.Plugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Azumatt.AzuExtendedPlayerInventory`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `org.bepinex.plugins.jewelcrafting`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/blacks7ar-MagicPlugin/MagicPlugin.dll` — assembly `MagicPlugin 2.2.2.0`; target `.NETStandard,Version=v2.1`; 67 HarmonyPatch attributes.
  - AssemblyCompany metadata: `blacks7ar` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/blacks7ar.MagicPlugin.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Initial inventory coverage was metadata only; authorship beyond metadata remains unreviewed.

2026-10-25: [[INV-003-Magic-book-tome-compatibility]] verifies the three book registrations,
equipment effects and config-gated Tome predicate. The observed disk toggle is Off. The
investigation distinguishes native PoV acceptance from SeneaL's redirected Azu/custom-slot
paths and records the related accessory family. Runtime ownership/effects remain open.
