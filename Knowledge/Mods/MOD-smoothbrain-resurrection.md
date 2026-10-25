---
id: "MOD-smoothbrain-resurrection"
title: "Smoothbrain-Resurrection"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"org.bepinex.plugins.resurrection": "1.0.15", "package": "1.0.15"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Smoothbrain-Resurrection/Resurrection.dll", "cfg:BepInEx/config/org.bepinex.plugins.resurrection.cfg", "pkg:BepInEx/plugins/Smoothbrain-Resurrection/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Smoothbrain-Resurrection

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Smoothbrain-Resurrection/manifest.json`; package version **1.0.15**.
- Package publisher inferred from folder: **Smoothbrain**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/blaxxun-boop/Resurrection
- Manifest dependency requirements: none declared.

## Plugin identities

- **Resurrection 1.0.15** — GUID `org.bepinex.plugins.resurrection`; type `Resurrection.Resurrection`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `org.bepinex.plugins.groups`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `org.bepinex.plugins.guilds`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Smoothbrain-Resurrection/Resurrection.dll` — assembly `Resurrection 1.0.0.0`; target `.NETFramework,Version=v4.8.1`; 25 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/org.bepinex.plugins.resurrection.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
