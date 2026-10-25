---
id: "MOD-sighsorry-warfaretweaks"
title: "sighsorry-WarfareTweaks"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.WarfareTweaks": "1.0.5", "package": "1.0.5"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-therzie-warfare]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-WarfareTweaks/WarfareTweaks.dll", "cfg:BepInEx/config/sighsorry.WarfareTweaks.cfg", "pkg:BepInEx/plugins/sighsorry-WarfareTweaks/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-WarfareTweaks

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-WarfareTweaks/manifest.json`; package version **1.0.5**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: `Therzie-Warfare-1.9.0`.

## Plugin identities

- **WarfareTweaks 1.0.5** — GUID `sighsorry.WarfareTweaks`; type `WarfareTweaks.WarfareTweaksPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Therzie.Warfare`; second constructor argument `2`; [[MOD-therzie-warfare]].
  - BepInDependency `Therzie.WarfareFireAndIce`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `org.bepinex.plugins.jewelcrafting`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-WarfareTweaks/WarfareTweaks.dll` — assembly `WarfareTweaks 1.0.5.0`; target `.NETFramework,Version=v4.8`; 49 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.WarfareTweaks.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-therzie-warfare]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
