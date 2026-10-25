---
id: "MOD-vapok-bettersleepbruh"
title: "Vapok-BetterSleepBruh"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"vapok.mods.BetterSleepBruh": "2.1.2", "package": "2.1.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-jotunn]]", "[[MOD-valheimmodding-yamldotnet]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Vapok-BetterSleepBruh/BetterSleepBruh.dll", "cfg:BepInEx/config/vapok.mods.BetterSleepBruh.cfg", "pkg:BepInEx/plugins/Vapok-BetterSleepBruh/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Vapok-BetterSleepBruh

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Vapok-BetterSleepBruh/manifest.json`; package version **2.1.2**.
- Package publisher inferred from folder: **Vapok**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/Vapok/BetterSleepBruh
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2351`, `ValheimModding-Jotunn-2.30.2`, `ValheimModding-YamlDotNet-16.3.1`.

## Plugin identities

- **BetterSleepBruh 2.1.2** — GUID `vapok.mods.BetterSleepBruh`; type `BetterSleepBruh.BetterSleepBruh`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].
  - BepInDependency `com.ValheimModding.YamlDotNetDetector`; second constructor argument `1`; [[MOD-valheimmodding-yamldotnet]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Vapok-BetterSleepBruh/BetterSleepBruh.dll` — assembly `BetterSleepBruh 2.1.2.0`; target `.NETFramework,Version=v4.8`; 23 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Vapok Gaming` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jotunn]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/vapok.mods.BetterSleepBruh.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-jotunn]], [[MOD-valheimmodding-yamldotnet]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
