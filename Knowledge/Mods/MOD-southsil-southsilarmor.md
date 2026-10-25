---
id: "MOD-southsil-southsilarmor"
title: "southsil-SouthsilArmor"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"southsil.SouthsilArmor": "3.2.2", "package": "3.2.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-yamldotnet]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/southsil-SouthsilArmor/SouthsilArmor.dll", "cfg:BepInEx/config/southsil.SouthsilArmor.cfg", "pkg:BepInEx/plugins/southsil-SouthsilArmor/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# southsil-SouthsilArmor

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/southsil-SouthsilArmor/manifest.json`; package version **3.2.2**.
- Package publisher inferred from folder: **southsil**. This is not verified personal authorship.
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.1900`.

## Plugin identities

- **SouthsilArmor 3.2.2** — GUID `southsil.SouthsilArmor`; type `Southsil.Armor.SouthsilArmor`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/southsil-SouthsilArmor/SouthsilArmor.dll` — assembly `SouthsilArmor 3.2.2.0`; target `.NETFramework,Version=v4.8`; 16 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-yamldotnet]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/southsil.SouthsilArmor.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-yamldotnet]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
