---
id: "MOD-therzie-warfare"
title: "Therzie-Warfare"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Therzie.Warfare": "1.9.5", "package": "1.9.5"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Therzie-Warfare/Warfare.dll", "cfg:BepInEx/config/Therzie.Warfare.cfg", "pkg:BepInEx/plugins/Therzie-Warfare/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Therzie-Warfare

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Therzie-Warfare/manifest.json`; package version **1.9.5**.
- Package publisher inferred from folder: **Therzie**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/AEqkrq2z4u
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2202`.

## Plugin identities

- **Warfare 1.9.5** — GUID `Therzie.Warfare`; type `Warfare.WarfarePlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `org.bepinex.plugins.jewelcrafting`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Therzie-Warfare/Warfare.dll` — assembly `Warfare 1.9.5.0`; target `.NETFramework,Version=v4.8`; 116 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Therzie` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Therzie.Warfare.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
