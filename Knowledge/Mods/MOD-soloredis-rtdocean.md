---
id: "MOD-soloredis-rtdocean"
title: "Soloredis-RtDOcean"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"soloredis.rtdocean": "2.3.2", "package": "2.3.2"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]", "[[MOD-valheimmodding-jotunn]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Soloredis-RtDOcean/RtDOcean.dll", "cfg:BepInEx/config/soloredis.rtdocean.cfg", "pkg:BepInEx/plugins/Soloredis-RtDOcean/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Soloredis-RtDOcean

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Soloredis-RtDOcean/manifest.json`; package version **2.3.2**.
- Package publisher inferred from folder: **Soloredis**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/Soloredis
- Manifest dependency requirements: `ValheimModding-Jotunn-2.30.2`, `denikson-BepInExPack_Valheim-5.4.2351`.

## Plugin identities

- **RtDOcean 2.3.2** — GUID `soloredis.rtdocean`; type `RtDOcean.OceanPlugin`; base `BepInEx.BaseUnityPlugin`.
  - BepInDependency `com.jotunn.jotunn`; second constructor argument `1`; [[MOD-valheimmodding-jotunn]].

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Soloredis-RtDOcean/RtDOcean.dll` — assembly `RtDOcean 1.0.0.0`; target `.NETStandard,Version=v2.1`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `RtDOcean` (not proof of authorship).
  - Local mod assembly reference matches: [[MOD-valheimmodding-jotunn]].

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/soloredis.rtdocean.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]], [[MOD-valheimmodding-jotunn]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
