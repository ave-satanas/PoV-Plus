---
id: "MOD-shudnal-conditionalconfigsync"
title: "shudnal-ConditionalConfigSync"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"_shudnal.ConditionalConfigSync": "1.0.11", "package": "1.0.11"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/shudnal-ConditionalConfigSync/ConditionalConfigSync.dll", "dll:BepInEx/plugins/shudnal-ConditionalConfigSync/ConditionalConfigSync.Plugin.dll", "pkg:BepInEx/plugins/shudnal-ConditionalConfigSync/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# shudnal-ConditionalConfigSync

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/shudnal-ConditionalConfigSync/manifest.json`; package version **1.0.11**.
- Package publisher inferred from folder: **shudnal**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/shudnal/ConditionalConfigSync
- Manifest dependency requirements: none declared.

## Plugin identities

- **Conditional Config Sync 1.0.11** — GUID `_shudnal.ConditionalConfigSync`; type `ConditionalConfigSync.ConditionalConfigSyncPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/shudnal-ConditionalConfigSync/ConditionalConfigSync.dll` — assembly `ConditionalConfigSync 1.0.0.0`; target `.NETFramework,Version=v4.7.2`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `shudnal` (not proof of authorship).
- `BepInEx/plugins/shudnal-ConditionalConfigSync/ConditionalConfigSync.Plugin.dll` — assembly `ConditionalConfigSync.Plugin 1.0.11.0`; target `.NETFramework,Version=v4.7.2`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `shudnal` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
