---
id: "MOD-valheimmodding-jotunn"
title: "ValheimModding-Jotunn"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"com.jotunn.jotunn": "2.30.2", "package": "2.30.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-yamldotnet]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/ValheimModding-Jotunn/Jotunn.dll", "pkg:BepInEx/plugins/ValheimModding-Jotunn/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# ValheimModding-Jotunn

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/ValheimModding-Jotunn/manifest.json`; package version **2.30.2**.
- Package publisher inferred from folder: **ValheimModding**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/Valheim-Modding/Jotunn
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2333`.

## Plugin identities

- **Jotunn 2.30.2** — GUID `com.jotunn.jotunn`; type `Jotunn.Main`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.bepis.bepinex.configurationmanager`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `com.maxsch.valheim.LocalizationCache`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/ValheimModding-Jotunn/Jotunn.dll` — assembly `Jotunn 2.30.2.0`; target `.NETFramework,Version=v4.6.2`; 110 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Valheim-Modding team` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-yamldotnet]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-yamldotnet]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
