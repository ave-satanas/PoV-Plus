---
id: "MOD-blacks7ar-regenwhilesitting"
title: "blacks7ar-RegenWhileSitting"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"blacks7ar.RegenWhileSitting": "1.0.8", "package": "1.0.8"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/blacks7ar-RegenWhileSitting/RegenWhileSitting.dll", "cfg:BepInEx/config/blacks7ar.RegenWhileSitting.cfg", "pkg:BepInEx/plugins/blacks7ar-RegenWhileSitting/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# blacks7ar-RegenWhileSitting

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/blacks7ar-RegenWhileSitting/manifest.json`; package version **1.0.8**.
- Package publisher inferred from folder: **blacks7ar**. This is not verified personal authorship.
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`.

## Plugin identities

- **RegenWhileSitting 1.0.8** — GUID `blacks7ar.RegenWhileSitting`; type `RegenWhileSitting.Plugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/blacks7ar-RegenWhileSitting/RegenWhileSitting.dll` — assembly `RegenWhileSitting 1.0.8.0`; target `.NETStandard,Version=v2.1`; 13 HarmonyPatch attributes.
  - AssemblyCompany metadata: `blacks7ar` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/blacks7ar.RegenWhileSitting.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
