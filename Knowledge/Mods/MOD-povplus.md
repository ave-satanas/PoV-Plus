---
id: "MOD-povplus"
title: "PoVPlus"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"povplus.core": "0.1.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/PoVPlus/PoVPlus.dll"]
claims: {}
contradicts: []
supersedes: []
---

# PoVPlus

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- No package manifest found in this group; do not infer package version or author.

## Plugin identities

- **PoVPlus 0.1.0** — GUID `povplus.core`; type `PoVPlus.Plugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/PoVPlus/PoVPlus.dll` — assembly `PoVPlus 0.1.0.0`; target `.NETFramework,Version=v4.8`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `PoVPlus` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Project-owned source/build: [[ARCH-001-PoVPlus]] and [[DEV-001-Build]]. Startup-only behavior,
compiled initialization path, and the later observed startup message are recorded in
[[INV-001-Startup-message]]. Filtering rules belong to [[DIAG-001-Logging]].
No gameplay patches or configuration entries are created by the current plugin source.
The later first authorized deployment is [[EXP-002-First-deployment]]; current DLL/build hashes
match and the original is backed up. The subsequent [[EXP-003-First-runtime-smoke]] passed
startup and clean close with one load/message and no new error/warning signatures; no gameplay test.
