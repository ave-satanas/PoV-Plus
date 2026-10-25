---
id: "MOD-crystal-comfortable"
title: "Crystal-Comfortable"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"dev.crystal.comfortable": "1.2.3.0", "package": "1.2.3"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-shudnal-conditionalconfigsync]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Crystal-Comfortable/Comfortable.dll", "cfg:BepInEx/config/dev.crystal.comfortable.cfg", "pkg:BepInEx/plugins/Crystal-Comfortable/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Crystal-Comfortable

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Crystal-Comfortable/manifest.json`; package version **1.2.3**.
- Package publisher inferred from folder: **Crystal**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/CrystalFerrai/ValheimMods
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`, `shudnal-ConditionalConfigSync-1.0.6`.

## Plugin identities

- **Comfortable 1.2.3.0** — GUID `dev.crystal.comfortable`; type `Comfortable.ComfortablePlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `_shudnal.ConditionalConfigSync`; second constructor argument `1`; [[MOD-shudnal-conditionalconfigsync]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Crystal-Comfortable/Comfortable.dll` — assembly `Comfortable 1.2.3.0`; target `.NETFramework,Version=v4.8`; 9 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Crystal` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-shudnal-conditionalconfigsync]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/dev.crystal.comfortable.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-shudnal-conditionalconfigsync]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
