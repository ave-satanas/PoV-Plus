---
id: "MOD-therzie-armory"
title: "Therzie-Armory"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Therzie.Armory": "1.4.3", "package": "1.4.3"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-therzie-warfare]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Therzie-Armory/Armory.dll", "cfg:BepInEx/config/Therzie.Armory.cfg", "pkg:BepInEx/plugins/Therzie-Armory/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Therzie-Armory

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Therzie-Armory/manifest.json`; package version **1.4.3**.
- Package publisher inferred from folder: **Therzie**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/AEqkrq2z4u
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2202`, `Therzie-Warfare-1.9.5`.

## Plugin identities

- **Armory 1.4.3** — GUID `Therzie.Armory`; type `Armory.ArmoryPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Therzie.Warfare`; second constructor argument `1`; [[MOD-therzie-warfare]].
  - BepInDependency `org.bepinex.plugins.jewelcrafting`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Therzie-Armory/Armory.dll` — assembly `Armory 1.4.3.0`; target `.NETFramework,Version=v4.8`; 32 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Therzie` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Therzie.Armory.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-therzie-warfare]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
