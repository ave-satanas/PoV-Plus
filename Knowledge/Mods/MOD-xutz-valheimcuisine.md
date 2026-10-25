---
id: "MOD-xutz-valheimcuisine"
title: "Xutz-ValheimCuisine"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"XutzBR.ValheimCuisine": "2.3.2", "package": "2.3.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Xutz-ValheimCuisine/ValheimCuisine.dll", "cfg:BepInEx/config/XutzBR.ValheimCuisine.cfg", "pkg:BepInEx/plugins/Xutz-ValheimCuisine/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Xutz-ValheimCuisine

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Xutz-ValheimCuisine/manifest.json`; package version **2.3.2**.
- Package publisher inferred from folder: **Xutz**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://www.nexusmods.com/valheim/mods/2155?tab=description
- Manifest dependency requirements: none declared.

## Plugin identities

- **ValheimCuisine 2.3.2** — GUID `XutzBR.ValheimCuisine`; type `ValheimCuisine.ValheimCuisinePlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Xutz-ValheimCuisine/ValheimCuisine.dll` — assembly `ValheimCuisine 2.3.2.0`; target `.NETFramework,Version=v4.8`; 74 HarmonyPatch attributes.
  - AssemblyCompany metadata: `XutzBR` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/XutzBR.ValheimCuisine.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
