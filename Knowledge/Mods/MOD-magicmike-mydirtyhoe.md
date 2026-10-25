---
id: "MOD-magicmike-mydirtyhoe"
title: "MagicMike-MyDirtyHoe"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"MagicMike.MyDirtyHoe": "2.3.2", "package": "2.3.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-jotunn]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/MagicMike-MyDirtyHoe/MyDirtyHoe.dll", "cfg:BepInEx/config/MagicMike.MyDirtyHoe.cfg", "pkg:BepInEx/plugins/MagicMike-MyDirtyHoe/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# MagicMike-MyDirtyHoe

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/MagicMike-MyDirtyHoe/manifest.json`; package version **2.3.2**.
- Package publisher inferred from folder: **MagicMike**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/mnr36G6J4x
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`, `ValheimModding-Jotunn-2.30.2`.

## Plugin identities

- **MyDirtyHoe 2.3.2** — GUID `MagicMike.MyDirtyHoe`; type `MyDirtyHoe.MyDirtyHoeMod`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/MagicMike-MyDirtyHoe/MyDirtyHoe.dll` — assembly `MyDirtyHoe 2.3.2.0`; target `.NETFramework,Version=v4.8`; 51 HarmonyPatch attributes.
  - AssemblyCompany metadata: `MagicMike` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jotunn]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/MagicMike.MyDirtyHoe.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-jotunn]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
