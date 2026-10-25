---
id: "MOD-sighsorry-nosnowybiome"
title: "sighsorry-NoSnowyBiome"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.NoSnowyBiome": "1.1.2", "package": "1.1.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-jerekuusela-expand-world-data]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-NoSnowyBiome/NoSnowyBiome.dll", "cfg:BepInEx/config/sighsorry.NoSnowyBiome.cfg", "pkg:BepInEx/plugins/sighsorry-NoSnowyBiome/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-NoSnowyBiome

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-NoSnowyBiome/manifest.json`; package version **1.1.2**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.com/invite/VFRJcPwUdm
- Manifest dependency requirements: none declared.

## Plugin identities

- **NoSnowyBiome 1.1.2** — GUID `sighsorry.NoSnowyBiome`; type `NoSnowyBiome.NoSnowyBiomePlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `expand_world_data`; second constructor argument `2`; [[MOD-jerekuusela-expand-world-data]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-NoSnowyBiome/NoSnowyBiome.dll` — assembly `NoSnowyBiome 1.1.2.0`; target `.NETFramework,Version=v4.8`; 13 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.NoSnowyBiome.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-jerekuusela-expand-world-data]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
