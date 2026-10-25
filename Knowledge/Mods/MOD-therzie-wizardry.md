---
id: "MOD-therzie-wizardry"
title: "Therzie-Wizardry"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Therzie.Wizardry": "1.2.3", "package": "1.2.3"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/Therzie-Wizardry/Wizardry.dll", "cfg:BepInEx/config/Therzie.Wizardry.cfg", "pkg:BepInEx/plugins/Therzie-Wizardry/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# Therzie-Wizardry

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/Therzie-Wizardry/manifest.json`; package version **1.2.3**.
- Package publisher inferred from folder: **Therzie**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://discord.gg/AEqkrq2z4u
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2202`.

## Plugin identities

- **Wizardry 1.2.3** — GUID `Therzie.Wizardry`; type `WarfareWizardry.WizardryPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/Therzie-Wizardry/Wizardry.dll` — assembly `Wizardry 1.2.3.0`; target `.NETFramework,Version=v4.8`; 34 HarmonyPatch attributes.
  - AssemblyCompany metadata: `Therzie` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

- `BepInEx/config/Therzie.Wizardry.cfg` (filename matches plugin GUID; association heuristic, not proof of ownership).

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
