---
id: "MOD-venturevalheim-venture-floating-items"
title: "VentureValheim-Venture_Floating_Items"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"com.orianaventure.mod.VentureFloatingItems": "1.0.1", "package": "1.0.1"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-jotunn]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/VentureValheim-Venture_Floating_Items/VentureValheim.FloatingItems.dll", "cfg:BepInEx/config/com.orianaventure.mod.VentureFloatingItems.cfg", "pkg:BepInEx/plugins/VentureValheim-Venture_Floating_Items/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# VentureValheim-Venture_Floating_Items

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/VentureValheim-Venture_Floating_Items/manifest.json`; package version **1.0.1**.
- Package publisher inferred from folder: **VentureValheim**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/OrianaVenture/VentureValheim
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2351`, `ValheimModding-Jotunn-2.30.2`.

## Plugin identities

- **VentureFloatingItems 1.0.1** — GUID `com.orianaventure.mod.VentureFloatingItems`; type `VentureValheim.FloatingItems.FloatingItemsPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/VentureValheim-Venture_Floating_Items/VentureValheim.FloatingItems.dll` — assembly `VentureValheim.FloatingItems 1.0.1.0`; target `.NETFramework,Version=v4.8`; 1 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jotunn]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/com.orianaventure.mod.VentureFloatingItems.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-jotunn]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
