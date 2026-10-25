---
id: "MOD-sighsorry-servermanager"
title: "sighsorry-ServerManager"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"sighsorry.ServerManager": "1.2.3", "package": "1.2.3"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-jerekuusela-upgrade-world]]", "[[MOD-sighsorry-clan]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/sighsorry-ServerManager/ServerManager.dll", "cfg:BepInEx/config/sighsorry.ServerManager.cfg", "pkg:BepInEx/plugins/sighsorry-ServerManager/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# sighsorry-ServerManager

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/sighsorry-ServerManager/manifest.json`; package version **1.2.3**.
- Package publisher inferred from folder: **sighsorry**. This is not verified personal authorship.
- Manifest dependency requirements: none declared.

## Plugin identities

- **ServerManager 1.2.3** — GUID `sighsorry.ServerManager`; type `ServerManager.ServerManagerPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `sighsorry.Clan`; second constructor argument `2`; [[MOD-sighsorry-clan]].
  - BepInDependency `upgrade_world`; second constructor argument `2`; [[MOD-jerekuusela-upgrade-world]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/sighsorry-ServerManager/ServerManager.dll` — assembly `ServerManager 1.2.3.0`; target `.NETFramework,Version=v4.8`; 46 HarmonyPatch attributes.
  - AssemblyCompany metadata: `sighsorry` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/sighsorry.ServerManager.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-jerekuusela-upgrade-world]], [[MOD-sighsorry-clan]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
