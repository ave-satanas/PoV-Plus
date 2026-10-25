---
id: "MOD-shudnal-gammaofnightlights"
title: "shudnal-GammaOfNightLights"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"shudnal.GammaOfNightLights": "1.0.10", "package": "1.0.10"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-shudnal-conditionalconfigsync]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/shudnal-GammaOfNightLights/GammaOfNightLights.dll", "cfg:BepInEx/config/shudnal.GammaOfNightLights.cfg", "pkg:BepInEx/plugins/shudnal-GammaOfNightLights/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# shudnal-GammaOfNightLights

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/shudnal-GammaOfNightLights/manifest.json`; package version **1.0.10**.
- Package publisher inferred from folder: **shudnal**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/shudnal/GammaOfNightLights
- Manifest dependency requirements: `shudnal-ConditionalConfigSync-1.0.5`.

## Plugin identities

- **Gamma of Night Lights 1.0.10** — GUID `shudnal.GammaOfNightLights`; type `GammaOfNightLights.GammaOfNightLights`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `_shudnal.ConditionalConfigSync`; second constructor argument `1.0.5`; [[MOD-shudnal-conditionalconfigsync]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/shudnal-GammaOfNightLights/GammaOfNightLights.dll` — assembly `GammaOfNightLights 1.0.10.0`; target `.NETFramework,Version=v4.8`; 5 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-shudnal-conditionalconfigsync]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/shudnal.GammaOfNightLights.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-shudnal-conditionalconfigsync]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
