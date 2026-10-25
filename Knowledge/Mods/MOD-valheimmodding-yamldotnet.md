---
id: "MOD-valheimmodding-yamldotnet"
title: "ValheimModding-YamlDotNet"
type: "mod"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"com.ValheimModding.YamlDotNetDetector": "1.0.0", "package": "16.3.1"}
evidence: ["Dependencies/environment-snapshot.json: assemblies/packages selected by path"]
related: ["[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[MOD-000-Inventory]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/ValheimModding-YamlDotNet/YamlDotNet.dll", "dll:BepInEx/plugins/ValheimModding-YamlDotNet/YamlDotNetDetector.dll", "pkg:BepInEx/plugins/ValheimModding-YamlDotNet/manifest.json"]
claims: {}
contradicts: []
supersedes: []
---

# ValheimModding-YamlDotNet

<!-- inventory:start -->
Metadata verified from the accepted snapshot; behavior coverage is **partial**.

Source: `Dependencies/environment-snapshot.json`, select entries by `path`.

- Manifest: `BepInEx/plugins/ValheimModding-YamlDotNet/manifest.json`; package version **16.3.1**.
- Package publisher inferred from folder: **ValheimModding**. This is not verified personal authorship.
- Manifest-declared website (not independently endorsed/verified): https://github.com/Valheim-Modding/CommonPackages
- Manifest dependency requirements: none declared.

## Plugin identities

- **YamlDotNet Detector 1.0.0** — GUID `com.ValheimModding.YamlDotNetDetector`; type `YamlDotNetDetector.Plugin`; base `BepInEx.BaseUnityPlugin`.

Dependency enum overload: `1` = hard, `2` = soft; version-string overloads are version requirements, not flags. Manifest requirements and assembly references are separate dependency layers.

## Assemblies and patch declarations

- `BepInEx/plugins/ValheimModding-YamlDotNet/YamlDotNet.dll` — assembly `YamlDotNet 16.0.0.0`; target `.NETFramework,Version=v4.7`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `absent` (not proof of authorship).
- `BepInEx/plugins/ValheimModding-YamlDotNet/YamlDotNetDetector.dll` — assembly `YamlDotNetDetector 1.0.0.0`; target `.NETFramework,Version=v4.8`; 0 HarmonyPatch attributes.
  - AssemblyCompany metadata: `YamlDotNetDetector` (not proof of authorship).

The snapshot stores exact assembly references and declarative patch targets. Attribute counts are not counts of active runtime patches; dynamic patches and patch order need separate inspection.

## Configuration mapping

No GUID-named cfg matched. Embedded/custom/YAML configuration remains possible.

Related: [[CFG-001-Configuration-map]], [[COMP-001-Warnings]], [[DEP-001-Environment]], [[MOD-000-Inventory]].
<!-- inventory:end -->

## Engineering notes

Integration behavior and authorship beyond metadata remain unreviewed. Add verified evidence here; this section survives inventory rendering.
