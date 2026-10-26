---
id: "DEV-001-Build"
title: "Plugin source and build reference"
type: "development"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[ARCH-001-PoVPlus]]", "[[DEP-001-Environment]]", "[[DEC-001-Development-baseline]]"]
confidence: "verified"
watch: ["src:Source/PoVPlus/Plugin.cs", "src:Source/PoVPlus/PoVPlus.csproj", "src:Scripts/Build.ps1"]
claims: {}
contradicts: []
supersedes: []
---

# Plugin source and build reference

## Source and behavior

Source/PoVPlus/Plugin.cs declares BepInPlugin/constants and private void Awake with LogInfo.
The later offline bridge adds soft dependencies and a default-false startup config binding;
[[COMP-002-Tome-bridge-design]] owns its acceptance behavior and runtime limitations.
PoVPlus.csproj: net48, C# 7.3, no packages. BepInEx, 0Harmony, assembly_valheim, UnityEngine,
UnityEngine.CoreModule references all Private=false.

Earlier 2026-10-25 source review: ordinary behavior was unchanged at that checkpoint, while an explicit diagnostic-only
compile flag can include the repaired observation module. All Diagnostics sources are excluded
from the normal project compilation. See [[DEV-005-Tome-diagnostics]] for the opt-in output path,
offline harness and exclusion test. Its compile check does not deploy or launch anything.

```powershell
.\Scripts\Build.ps1
.\Scripts\Build.ps1 -Configuration Debug
.\Scripts\Build.ps1 -ValheimDevPath 'S:\Valheim-Modding\PoVPlus-Dev'
```

Release: Source/PoVPlus/bin/Release/net48/PoVPlus.dll and PDB. Script restores locally, builds,
checks dependency DLLs, never deploys/launches. Earlier session build passed 0 warnings/0 errors;
no plugin rebuild during knowledge creation. Initial validation-rule bug was corrected to ignore
framework references without HintPath. Root README owns manual deployment/rollback instructions.

## Workflow entry point

Use `Scripts/Dev.ps1 Build` to invoke the same Build.ps1. Deployment, rollback, launch and
diagnostics use the separate commands in [[DEV-003-Workflow]]; none runs automatically after build.
The controlled workflow replaces manual copy/rollback instructions. Historical build/hash observations
above remain historical; the workflow implementation task rebuilt Release successfully without deploying.

## Previously verified tooling

SDKs 8.0.405/9.0.201/10.0.100; Visual Studio Community/Build Tools 2022 17.13.5; .NET Framework
4.8 reference pack; VS Code command and Git. Current task verifies Python 3.14.0; no packages
installed. SDK version does not change Unity runtime/plugin TFM. Earlier ilspycmd absence on PATH
does not establish absence of every GUI decompiler.
