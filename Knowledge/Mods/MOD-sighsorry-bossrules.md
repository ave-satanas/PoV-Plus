---
id: "MOD-sighsorry-bossrules"
title: "sighsorry-BossRules"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.BossRules": "1.1.8", "package": "1.1.8"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-sighsorry-youarenotworthy]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-BossRules/BossRules.dll", "cfg:BepInEx/config/sighsorry.BossRules.cfg", "pkg:BepInEx/plugins/sighsorry-BossRules/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-BossRules

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-BossRules/manifest.json`; package version **1.1.8**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **BossRules 1.1.8** — GUID `sighsorry.BossRules`; type `BossRules.BossRulesPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `sighsorry.YouAreNotWorthy`; second constructor argument `2`; [[MOD-sighsorry-youarenotworthy]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-BossRules/BossRules.dll` — assembly `BossRules 1.1.8.0`; target `.NETFramework,Version=v4.8`; 49 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.BossRules.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-sighsorry-youarenotworthy]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
