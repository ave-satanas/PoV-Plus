---
id: "MOD-mathiasdecrock-planbuild"
title: "MathiasDecrock-PlanBuild"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"marcopogo.PlanBuild": "0.20.0", "package": "0.20.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-azumatt-azucraftyboxes]]", "[[MOD-valheimmodding-jotunn]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/MathiasDecrock-PlanBuild/PlanBuild/PlanBuild.dll", "cfg:BepInEx/config/marcopogo.PlanBuild.cfg", "pkg:BepInEx/plugins/MathiasDecrock-PlanBuild/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# MathiasDecrock-PlanBuild

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/MathiasDecrock-PlanBuild/manifest.json`; package version **0.20.0**.
- Package publisher inferred from folder: **MathiasDecrock**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/sirskunkalot/PlanBuild
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`, `ValheimModding-Jotunn-2.30.1`.

## Plugin identities

- **PlanBuild 0.20.0** — GUID `marcopogo.PlanBuild`; type `PlanBuild.PlanBuildPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `2.30.1`; [[MOD-valheimmodding-jotunn]].
  - BepInDependency `aedenthorn.CraftFromContainers`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `Azumatt.AzuCraftyBoxes`; second constructor argument `2`; [[MOD-azumatt-azucraftyboxes]].
  - BepInDependency `bruce.valheim.comfymods.gizmo`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `BepIn.Sarcen.ValheimRAFT`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `mkz.itemdrawers`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/MathiasDecrock-PlanBuild/PlanBuild/PlanBuild.dll` — assembly `PlanBuild 0.20.0.0`; target `.NETFramework,Version=v4.8`; 47 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-azumatt-azucraftyboxes]], [[MOD-valheimmodding-jotunn]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/marcopogo.PlanBuild.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-azumatt-azucraftyboxes]], [[MOD-valheimmodding-jotunn]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
