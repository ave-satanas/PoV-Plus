---
id: "MOD-jerekuusela-structure-tweaks"
title: "JereKuusela-Structure_Tweaks"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"structure_tweaks": "1.37", "package": "1.37.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/JereKuusela-Structure_Tweaks/StructureTweaks.dll", "cfg:BepInEx/config/structure_tweaks.cfg", "pkg:BepInEx/plugins/JereKuusela-Structure_Tweaks/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# JereKuusela-Structure_Tweaks

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/JereKuusela-Structure_Tweaks/manifest.json`; package version **1.37.0**.
- Package publisher inferred from folder: **JereKuusela**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/JereKuusela/valheim-structure_tweaks
- Manifest dependency requirements: none declared.

## Plugin identities

- **Structure Tweaks 1.37** — GUID `structure_tweaks`; type `StructureTweaksPlugin.Plugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/JereKuusela-Structure_Tweaks/StructureTweaks.dll` — assembly `StructureTweaks 1.0.0.0`; target `.NETFramework,Version=v4.8`; 78 HarmonyPatch attributes.
  - AssemblyCompany metadata: `StructureTweaks` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/structure_tweaks.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
