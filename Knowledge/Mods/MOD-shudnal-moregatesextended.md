---
id: "MOD-shudnal-moregatesextended"
title: "shudnal-MoreGatesExtended"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"shudnal.MoreGatesExtended": "1.0.5", "package": "1.0.5"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-jotunn]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/shudnal-MoreGatesExtended/MoreGatesExtended.dll", "cfg:BepInEx/config/shudnal.MoreGatesExtended.cfg", "pkg:BepInEx/plugins/shudnal-MoreGatesExtended/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# shudnal-MoreGatesExtended

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/shudnal-MoreGatesExtended/manifest.json`; package version **1.0.5**.
- Package publisher inferred from folder: **shudnal**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://www.nexusmods.com/valheim/mods/1087
- Manifest dependency requirements: `ValheimModding-Jotunn-2.30.1`.

## Plugin identities

- **More Gates Extended 1.0.5** — GUID `shudnal.MoreGatesExtended`; type `MoreGatesExtended.MoreGatesExtended`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/shudnal-MoreGatesExtended/MoreGatesExtended.dll` — assembly `MoreGatesExtended 1.0.5.0`; target `.NETFramework,Version=v4.8`; 2 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jotunn]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/shudnal.MoreGatesExtended.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-jotunn]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
