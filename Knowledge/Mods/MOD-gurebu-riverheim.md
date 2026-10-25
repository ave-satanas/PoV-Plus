---
id: "MOD-gurebu-riverheim"
title: "Gurebu-Riverheim"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"dev.gurebu.riverheim.stable": "1.2.1", "package": "1.2.1"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Gurebu-Riverheim/Riverheim.dll", "cfg:BepInEx/config/dev.gurebu.riverheim.stable.cfg", "pkg:BepInEx/plugins/Gurebu-Riverheim/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Gurebu-Riverheim

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Gurebu-Riverheim/manifest.json`; package version **1.2.1**.
- Package publisher inferred from folder: **Gurebu**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/ES3J7mu6kW
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2202`.

## Plugin identities

- **Riverheim 1.2.1** — GUID `dev.gurebu.riverheim.stable`; type `Riverheim.Plugin.RiverheimPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Gurebu-Riverheim/Riverheim.dll` — assembly `Riverheim 1.2.1.0`; target `.NETStandard,Version=v2.1`; 23 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Riverheim` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/dev.gurebu.riverheim.stable.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
