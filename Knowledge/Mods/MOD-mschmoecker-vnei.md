---
id: "MOD-mschmoecker-vnei"
title: "MSchmoecker-VNEI"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"com.maxsch.valheim.vnei": "0.17.6", "package": "0.17.6"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-randyknapp-epicloot]]", "[[MOD-valheimmodding-jotunn]]", "[[MOD-valheimmodding-yamldotnet]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/MSchmoecker-VNEI/VNEI.dll", "cfg:BepInEx/config/com.maxsch.valheim.vnei.cfg", "pkg:BepInEx/plugins/MSchmoecker-VNEI/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# MSchmoecker-VNEI

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/MSchmoecker-VNEI/manifest.json`; package version **0.17.6**.
- Package publisher inferred from folder: **MSchmoecker**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/MSchmoecker/VNEI
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`, `ValheimModding-Jotunn-2.30.0`, `ValheimModding-YamlDotNet-16.3.1`.

## Plugin identities

- **VNEI 0.17.6** — GUID `com.maxsch.valheim.vnei`; type `VNEI.Plugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].
  - BepInDependency `WackyMole.WackysDatabase`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/MSchmoecker-VNEI/VNEI.dll` — assembly `VNEI 0.17.6.0`; target `.NETFramework,Version=v4.8.1`; 10 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-randyknapp-epicloot]], [[MOD-valheimmodding-jotunn]], [[MOD-valheimmodding-yamldotnet]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/com.maxsch.valheim.vnei.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-randyknapp-epicloot]], [[MOD-valheimmodding-jotunn]], [[MOD-valheimmodding-yamldotnet]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
