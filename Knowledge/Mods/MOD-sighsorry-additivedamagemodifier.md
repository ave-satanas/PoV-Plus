---
id: "MOD-sighsorry-additivedamagemodifier"
title: "sighsorry-AdditiveDamageModifier"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.AdditiveDamageModifier": "1.2.4", "package": "1.2.4"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-AdditiveDamageModifier/AdditiveDamageModifier.dll", "cfg:BepInEx/config/sighsorry.AdditiveDamageModifier.cfg", "pkg:BepInEx/plugins/sighsorry-AdditiveDamageModifier/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-AdditiveDamageModifier

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-AdditiveDamageModifier/manifest.json`; package version **1.2.4**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/mbkPcvu9ax
- Manifest dependency requirements: none declared.

## Plugin identities

- **AdditiveDamageModifier 1.2.4** — GUID `sighsorry.AdditiveDamageModifier`; type `AdditiveDamageModifier.AdditiveDamageModifierPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-AdditiveDamageModifier/AdditiveDamageModifier.dll` — assembly `AdditiveDamageModifier 1.2.4.0`; target `.NETFramework,Version=v4.8`; 28 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.AdditiveDamageModifier.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
