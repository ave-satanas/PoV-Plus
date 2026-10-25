---
id: "MOD-jerekuusela-upgrade-world"
title: "JereKuusela-Upgrade_World"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"upgrade_world": "1.87", "package": "1.87.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/JereKuusela-Upgrade_World/UpgradeWorld.dll", "cfg:BepInEx/config/upgrade_world.cfg", "pkg:BepInEx/plugins/JereKuusela-Upgrade_World/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# JereKuusela-Upgrade_World

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/JereKuusela-Upgrade_World/manifest.json`; package version **1.87.0**.
- Package publisher inferred from folder: **JereKuusela**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/VFRJcPwUdm
- Manifest dependency requirements: `JereKuusela-Server_devcommands-1.112.0`.

## Plugin identities

- **Upgrade World 1.87** — GUID `upgrade_world`; type `UpgradeWorld.UpgradeWorld`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/JereKuusela-Upgrade_World/UpgradeWorld.dll` — assembly `UpgradeWorld 1.0.0.0`; target `.NETFramework,Version=v4.8`; 13 HarmonyPatch attributes.
  - AssemblyCompany metadata: `UpgradeWorld` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/upgrade_world.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
