---
id: "MOD-sighsorry-loadtimeprofiler"
title: "sighsorry-LoadTimeProfiler"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"package": "1.3.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/patchers/sighsorry-LoadTimeProfiler/LoadTimeProfiler.dll", "pkg:BepInEx/plugins/sighsorry-LoadTimeProfiler/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-LoadTimeProfiler

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-LoadTimeProfiler/manifest.json`; package version **1.3.2**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

No BepInPlugin attributes found. This may be a preloader patcher, library, or content package; do not infer it is an ordinary plugin.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/patchers/sighsorry-LoadTimeProfiler/LoadTimeProfiler.dll` — assembly `LoadTimeProfiler 1.3.2.0`; target `.NETFramework,Version=v4.8`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

The installed logger ownership/filter hooks and effective configuration are documented in
[[DIAG-001-Logging]] and [[CFG-001-Configuration-map]]. The observed PoVPlus case is
[[INV-001-Startup-message]], with retrospective outcome in [[EXP-001-Logging-override]].
These manual notes survive inventory rendering. Other patcher features remain unreviewed.
