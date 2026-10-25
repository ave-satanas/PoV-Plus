---
id: "MOD-voidey-lich-king"
title: "Voidey-Lich_King"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Voidey.LichKing": "1.1.0", "package": "1.1.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Voidey-Lich_King/LichKing.dll", "cfg:BepInEx/config/Voidey.LichKing.cfg", "pkg:BepInEx/plugins/Voidey-Lich_King/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Voidey-Lich_King

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Voidey-Lich_King/manifest.json`; package version **1.1.0**.
- Package publisher inferred from folder: **Voidey**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://www.youtube.com/watch?v=HInmEcbFyF0&
- Manifest dependency requirements: none declared.

## Plugin identities

- **LichKing 1.1.0** — GUID `Voidey.LichKing`; type `LichKing.LichKingPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Voidey-Lich_King/LichKing.dll` — assembly `LichKing 1.1.0.0`; target `.NETFramework,Version=v4.8`; 21 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Voidey` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Voidey.LichKing.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
