---
id: "MOD-venturevalheim-venture-logout-tweaks"
title: "VentureValheim-Venture_Logout_Tweaks"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"com.orianaventure.mod.LogoutTweaks": "1.0.0", "package": "1.0.0"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/VentureValheim-Venture_Logout_Tweaks/VentureValheim.LogoutTweaks.dll", "pkg:BepInEx/plugins/VentureValheim-Venture_Logout_Tweaks/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# VentureValheim-Venture_Logout_Tweaks

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/VentureValheim-Venture_Logout_Tweaks/manifest.json`; package version **1.0.0**.
- Package publisher inferred from folder: **VentureValheim**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/OrianaVenture/VentureValheim
- Manifest dependency requirements: `denikson-BepInExPack_Valheim-5.4.2350`.

## Plugin identities

- **LogoutTweaks 1.0.0** — GUID `com.orianaventure.mod.LogoutTweaks`; type `VentureValheim.LogoutTweaks.LogoutTweaksPlugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/VentureValheim-Venture_Logout_Tweaks/VentureValheim.LogoutTweaks.dll` — assembly `VentureValheim.LogoutTweaks 1.0.0.0`; target `.NETFramework,Version=v4.8`; 2 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
