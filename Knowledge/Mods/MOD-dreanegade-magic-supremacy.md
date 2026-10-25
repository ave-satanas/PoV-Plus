---
id: "MOD-dreanegade-magic-supremacy"
title: "Dreanegade-Magic_Supremacy"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Dreanegade.Magic_Supremacy": "3.1.5", "package": "3.1.5"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll", "cfg:BepInEx/config/Dreanegade.Magic_Supremacy.cfg", "pkg:BepInEx/plugins/Dreanegade-Magic_Supremacy/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Dreanegade-Magic_Supremacy

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Dreanegade-Magic_Supremacy/manifest.json`; package version **3.1.5**.
- Package publisher inferred from folder: **Dreanegade**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/T7JdRBwF4S
- Manifest dependency requirements: none declared.

## Plugin identities

- **Magic_Supremacy 3.1.5** — GUID `Dreanegade.Magic_Supremacy`; type `Magic_Supremacy.Magic_SupremacyPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Azumatt.AzuExtendedPlayerInventory`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll` — assembly `Magic_Supremacy 3.1.5.0`; target `.NETFramework,Version=v4.8`; 103 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Dreanegade` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Dreanegade.Magic_Supremacy.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
