---
id: "MOD-tastychickenlegs-nosmokestaylit"
title: "TastyChickenLegs-NoSmokeStayLit"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"TastyChickenLegs.NoSmokeStayLit": "2.4.3", "package": "2.4.3"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/TastyChickenLegs-NoSmokeStayLit/NoSmokeStayLit.dll", "cfg:BepInEx/config/TastyChickenLegs.NoSmokeStayLit.cfg", "pkg:BepInEx/plugins/TastyChickenLegs-NoSmokeStayLit/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# TastyChickenLegs-NoSmokeStayLit

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/TastyChickenLegs-NoSmokeStayLit/manifest.json`; package version **2.4.3**.
- Package publisher inferred from folder: **TastyChickenLegs**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.com/invite/PQMXbUS6FH
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2101`.

## Plugin identities

- **NoSmokeStayLit 2.4.3** — GUID `TastyChickenLegs.NoSmokeStayLit`; type `NoSmokeStayLit.NoSmokeStayLitPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/TastyChickenLegs-NoSmokeStayLit/NoSmokeStayLit.dll` — assembly `NoSmokeStayLit 2.4.3.0`; target `.NETFramework,Version=v4.8`; 27 HarmonyPatch attributes.
  - AssemblyCompany metadata: `TastyChickenLegs` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/TastyChickenLegs.NoSmokeStayLit.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
