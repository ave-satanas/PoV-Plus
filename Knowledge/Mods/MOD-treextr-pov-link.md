---
id: "MOD-treextr-pov-link"
title: "treextr-PoV_Link"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"treextr.povlink": "0.3.0", "package": "0.3.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-treextr-pathofvalheiman]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/treextr-PoV_Link/PoVLink/PoVLink.dll", "pkg:BepInEx/plugins/treextr-PoV_Link/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# treextr-PoV_Link

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/treextr-PoV_Link/manifest.json`; package version **0.3.0**.
- Package publisher inferred from folder: **treextr**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/XZsYRF2YtQ
- Manifest dependency requirements: `treextr-PathOfValheiman-4.13.0`.

## Plugin identities

- **PoV Link 0.3.0** — GUID `treextr.povlink`; type `PoVLink.LinkPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `treextr.pathofvalheiman`; second constructor argument `1`; [[MOD-treextr-pathofvalheiman]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/treextr-PoV_Link/PoVLink/PoVLink.dll` — assembly `PoVLink 1.0.0.0`; target `.NETFramework,Version=v4.8`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `PoVLink` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-treextr-pathofvalheiman]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-treextr-pathofvalheiman]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
