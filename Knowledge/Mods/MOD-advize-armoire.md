---
id: "MOD-advize-armoire"
title: "Advize-Armoire"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"advize.Armoire": "1.2.2", "package": "1.2.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Advize-Armoire/Advize_Armoire.dll", "cfg:BepInEx/config/advize.Armoire.cfg", "pkg:BepInEx/plugins/Advize-Armoire/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Advize-Armoire

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Advize-Armoire/manifest.json`; package version **1.2.2**.
- Package publisher inferred from folder: **Advize**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://www.nexusmods.com/valheim/mods/3170
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`.

## Plugin identities

- **Armoire 1.2.2** — GUID `advize.Armoire`; type `Advize_Armoire.Armoire`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Advize-Armoire/Advize_Armoire.dll` — assembly `Advize_Armoire 1.2.2.0`; target `.NETFramework,Version=v4.8`; 27 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/advize.Armoire.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
