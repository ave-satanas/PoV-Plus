---
id: "MOD-odinplus-odinskingdom"
title: "OdinPlus-OdinsKingdom"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"odinplus.plugins.odinskingdom": "1.6.3", "package": "1.6.3"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/OdinPlus-OdinsKingdom/OdinsKingdom.dll", "cfg:BepInEx/config/odinplus.plugins.odinskingdom.cfg", "pkg:BepInEx/plugins/OdinPlus-OdinsKingdom/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# OdinPlus-OdinsKingdom

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/OdinPlus-OdinsKingdom/manifest.json`; package version **1.6.3**.
- Package publisher inferred from folder: **OdinPlus**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **OdinsKingdom 1.6.3** — GUID `odinplus.plugins.odinskingdom`; type `OdinsKingdom.OdinsKingdom`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/OdinPlus-OdinsKingdom/OdinsKingdom.dll` — assembly `OdinsKingdom 1.0.0.0`; target `.NETFramework,Version=v4.8`; 17 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/odinplus.plugins.odinskingdom.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
