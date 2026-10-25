---
id: "MOD-odinplus-odinhorse"
title: "OdinPlus-OdinHorse"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Raelaziel.OdinHorse": "1.9.2", "package": "1.9.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/OdinPlus-OdinHorse/OdinHorse.dll", "cfg:BepInEx/config/Raelaziel.OdinHorse.cfg", "pkg:BepInEx/plugins/OdinPlus-OdinHorse/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# OdinPlus-OdinHorse

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/OdinPlus-OdinHorse/manifest.json`; package version **1.9.2**.
- Package publisher inferred from folder: **OdinPlus**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **OdinHorse 1.9.2** — GUID `Raelaziel.OdinHorse`; type `OdinHorse.OdinHorse`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Ygg.BetterRiding`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `OdinPlus.OdinMountedCombat`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/OdinPlus-OdinHorse/OdinHorse.dll` — assembly `OdinHorse 1.8.8.0`; target `.NETFramework,Version=v4.8`; 68 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Raelaziel.OdinHorse.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
