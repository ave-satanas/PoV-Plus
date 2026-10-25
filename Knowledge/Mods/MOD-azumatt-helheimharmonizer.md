---
id: "MOD-azumatt-helheimharmonizer"
title: "Azumatt-HelheimHarmonizer"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Azumatt.HelheimHarmonizer": "1.0.11", "package": "1.0.11"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Azumatt-HelheimHarmonizer/HelheimHarmonizer.dll", "cfg:BepInEx/config/Azumatt.HelheimHarmonizer.cfg", "pkg:BepInEx/plugins/Azumatt-HelheimHarmonizer/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Azumatt-HelheimHarmonizer

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Azumatt-HelheimHarmonizer/manifest.json`; package version **1.0.11**.
- Package publisher inferred from folder: **Azumatt**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/pdHgy6Bsng
- Manifest dependency requirements: none declared.

## Plugin identities

- **HelheimHarmonizer 1.0.11** — GUID `Azumatt.HelheimHarmonizer`; type `HelheimHarmonizer.HelheimHarmonizerPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Azumatt-HelheimHarmonizer/HelheimHarmonizer.dll` — assembly `HelheimHarmonizer 1.0.11.0`; target `.NETFramework,Version=v4.8`; 25 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Azumatt` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Azumatt.HelheimHarmonizer.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
