---
id: "MOD-bentog-missingpieces"
title: "BentoG-MissingPieces"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"com.Bento.MissingPieces": "2.3.2", "package": "2.3.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-jotunn]]", "[[MOD-valheimmodding-jsondotnet]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/BentoG-MissingPieces/MissingPieces.dll", "pkg:BepInEx/plugins/BentoG-MissingPieces/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# BentoG-MissingPieces

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/BentoG-MissingPieces/manifest.json`; package version **2.3.2**.
- Package publisher inferred from folder: **BentoG**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/OrianaVenture/Valheim-MissingPieces
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`, `ValheimModding-Jotunn-2.30.2`, `ValheimModding-JsonDotNET-13.0.4`.

## Plugin identities

- **MissingPieces 2.3.2** — GUID `com.Bento.MissingPieces`; type `MissingPieces.MissingPiecesPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/BentoG-MissingPieces/MissingPieces.dll` — assembly `MissingPieces 2.3.2.0`; target `.NETFramework,Version=v4.8`; 1 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jotunn]], [[MOD-valheimmodding-jsondotnet]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-jotunn]], [[MOD-valheimmodding-jsondotnet]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
