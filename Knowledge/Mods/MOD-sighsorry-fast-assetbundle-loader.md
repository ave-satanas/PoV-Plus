---
id: "MOD-sighsorry-fast-assetbundle-loader"
title: "sighsorry-Fast_AssetBundle_Loader"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"package": "1.0.7"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/patchers/sighsorry-Fast_AssetBundle_Loader/FastAssetBundleLoader.dll", "pkg:BepInEx/plugins/sighsorry-Fast_AssetBundle_Loader/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-Fast_AssetBundle_Loader

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-Fast_AssetBundle_Loader/manifest.json`; package version **1.0.7**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`.

## Plugin identities

No BepInPlugin attributes found. This may be a preloader patcher, library, or content package; do not infer it is an ordinary plugin.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/patchers/sighsorry-Fast_AssetBundle_Loader/FastAssetBundleLoader.dll` — assembly `FastAssetBundleLoader 1.0.7.0`; target `.NETStandard,Version=v2.1`; 2 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
