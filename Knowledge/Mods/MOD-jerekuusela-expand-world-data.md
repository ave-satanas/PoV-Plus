---
id: "MOD-jerekuusela-expand-world-data"
title: "JereKuusela-Expand_World_Data"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"expand_world_data": "1.79", "package": "1.79.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-yamldotnet]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/JereKuusela-Expand_World_Data/ExpandWorldData.dll", "cfg:BepInEx/config/expand_world_data.cfg", "pkg:BepInEx/plugins/JereKuusela-Expand_World_Data/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# JereKuusela-Expand_World_Data

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/JereKuusela-Expand_World_Data/manifest.json`; package version **1.79.0**.
- Package publisher inferred from folder: **JereKuusela**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/VFRJcPwUdm
- Manifest dependency requirements: `ValheimModding-YamlDotNet-16.3.1`.

## Plugin identities

- **Expand World Data 1.79** — GUID `expand_world_data`; type `ExpandWorldData.EWD`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/JereKuusela-Expand_World_Data/ExpandWorldData.dll` — assembly `ExpandWorldData 1.0.0.0`; target `.NETFramework,Version=v4.8`; 66 HarmonyPatch attributes.
  - AssemblyCompany metadata: `ExpandWorldData` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-yamldotnet]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/expand_world_data.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-yamldotnet]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
