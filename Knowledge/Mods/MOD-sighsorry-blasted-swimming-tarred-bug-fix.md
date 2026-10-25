---
id: "MOD-sighsorry-blasted-swimming-tarred-bug-fix"
title: "sighsorry-Blasted_Swimming_Tarred_Bug_Fix"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.blastedswimmingtarredbugfix": "1.2.6", "package": "1.2.6"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-blacks7ar-magicplugin]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-Blasted_Swimming_Tarred_Bug_Fix/BlastedSwimmingTarredBugFix.dll", "cfg:BepInEx/config/sighsorry.blastedswimmingtarredbugfix.cfg", "pkg:BepInEx/plugins/sighsorry-Blasted_Swimming_Tarred_Bug_Fix/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-Blasted_Swimming_Tarred_Bug_Fix

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-Blasted_Swimming_Tarred_Bug_Fix/manifest.json`; package version **1.2.6**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/jkcJCq2sK5
- Manifest dependency requirements: none declared.

## Plugin identities

- **BlastedSwimmingTarredBugFix 1.2.6** — GUID `sighsorry.blastedswimmingtarredbugfix`; type `BlastedSwimmingTarredBugFix.LiquidStuckFixPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `Azumatt.MyPitsDontLeak`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `redseiko.valheim.scenic`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `blacks7ar.MagicPlugin`; second constructor argument `2`; [[MOD-blacks7ar-magicplugin]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-Blasted_Swimming_Tarred_Bug_Fix/BlastedSwimmingTarredBugFix.dll` — assembly `BlastedSwimmingTarredBugFix 1.2.6.0`; target `.NETStandard,Version=v2.1`; 22 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.blastedswimmingtarredbugfix.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-blacks7ar-magicplugin]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
