---
id: "MOD-jerekuusela-server-devcommands"
title: "JereKuusela-Server_devcommands"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"server_devcommands": "1.116", "package": "1.116.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/JereKuusela-Server_devcommands/ServerDevcommands.dll", "cfg:BepInEx/config/server_devcommands.cfg", "pkg:BepInEx/plugins/JereKuusela-Server_devcommands/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# JereKuusela-Server_devcommands

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/JereKuusela-Server_devcommands/manifest.json`; package version **1.116.0**.
- Package publisher inferred from folder: **JereKuusela**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/VFRJcPwUdm
- Manifest dependency requirements: none declared.

## Plugin identities

- **Server Devcommands 1.116** — GUID `server_devcommands`; type `ServerDevcommands.ServerDevcommands`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `bruce.valheim.comfymods.gizmo`; second constructor argument `2`; target not mapped in this snapshot.
  - BepInDependency `m3to.mods.GizmoReloaded`; second constructor argument `2`; target not mapped in this snapshot.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/JereKuusela-Server_devcommands/ServerDevcommands.dll` — assembly `ServerDevcommands 1.0.0.0`; target `.NETFramework,Version=v4.8`; 121 HarmonyPatch attributes.
  - AssemblyCompany metadata: `ServerDevcommands` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/server_devcommands.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
