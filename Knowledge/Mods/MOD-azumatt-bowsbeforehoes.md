---
id: "MOD-azumatt-bowsbeforehoes"
title: "Azumatt-BowsBeforeHoes"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Azumatt.BowsBeforeHoes": "2.0.2", "package": "2.0.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Azumatt-BowsBeforeHoes/BowsBeforeHoes.dll", "cfg:BepInEx/config/Azumatt.BowsBeforeHoes.cfg", "pkg:BepInEx/plugins/Azumatt-BowsBeforeHoes/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Azumatt-BowsBeforeHoes

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Azumatt-BowsBeforeHoes/manifest.json`; package version **2.0.2**.
- Package publisher inferred from folder: **Azumatt**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/pdHgy6Bsng
- Manifest dependency requirements: none declared.

## Plugin identities

- **BowsBeforeHoes 2.0.2** — GUID `Azumatt.BowsBeforeHoes`; type `BowsBeforeHoes.BowsBeforeHoesPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `yay.spikehimself.xportal`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `Azumatt.AzuExtendedPlayerInventory`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Azumatt-BowsBeforeHoes/BowsBeforeHoes.dll` — assembly `BowsBeforeHoes 2.0.2.0`; target `.NETFramework,Version=v4.8`; 95 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Azumatt` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Azumatt.BowsBeforeHoes.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
