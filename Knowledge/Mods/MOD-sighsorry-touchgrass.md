---
id: "MOD-sighsorry-touchgrass"
title: "sighsorry-TouchGrass"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.TouchGrass": "1.0.7", "package": "1.0.7"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-TouchGrass/TouchGrass.dll", "cfg:BepInEx/config/sighsorry.TouchGrass.cfg", "pkg:BepInEx/plugins/sighsorry-TouchGrass/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-TouchGrass

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-TouchGrass/manifest.json`; package version **1.0.7**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`.

## Plugin identities

- **TouchGrass 1.0.7** — GUID `sighsorry.TouchGrass`; type `TouchGrass.TouchGrassPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-TouchGrass/TouchGrass.dll` — assembly `TouchGrass 1.0.7.0`; target `.NETFramework,Version=v4.8`; 36 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.TouchGrass.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
