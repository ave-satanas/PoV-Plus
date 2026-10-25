# PoVPlus

## Engineering knowledge

Start substantial development and investigation at [Knowledge/INDEX.md](Knowledge/INDEX.md).
It links installed mod identities/dependencies, configuration evidence, diagnostics, investigations,
decisions, experiments, and session history. Open `Knowledge/` as an Obsidian vault or browse in
any Markdown editor. No Obsidian plugins, database, or external service is required.

```powershell
python Scripts/knowledge.py search 'povplus.core'
python Scripts/knowledge.py validate --strict
```

See [maintenance instructions](Knowledge/Development/DEV-002-Maintenance.md) before refreshing
installation snapshots. Ordinary validation reads repository files only. Root `AGENTS.md`
integration is [approved and applied](Knowledge/Decisions/DEC-002-Agent-integration.md).
This workspace is a Git repository on `main`, connected to
[ave-satanas/PoV-Plus](https://github.com/ave-satanas/PoV-Plus).

## Plugin

Minimal BepInEx 5 plugin for the Valheim development installation. Version 0.1.0
logs `PoVPlus 0.1.0 loaded.` during startup. It creates no configuration entries
and applies no Harmony patches or gameplay changes.

## Layout

- `Source/PoVPlus.sln`: Visual Studio solution.
- `Source/PoVPlus/`: .NET Framework 4.8 plugin project and source.
- `Scripts/Build.ps1`: local build and output verification.
- `Configs/`: reserved for reviewed compatibility configurations; untouched.
- `Documentation/`: reserved for additional development documentation.

## Requirements

Use Windows, a .NET SDK supporting `net48` SDK-style projects, and the .NET
Framework 4.8 targeting pack. The assessed machine has SDKs 8.0.405, 9.0.201,
and 10.0.100, Visual Studio 2022, and the 4.8 reference assemblies installed.
The SDK version is separate from the plugin's `net48` runtime target.

The default reference installation is `S:\Valheim-Modding\PoVPlus-Dev`, assessed
with BepInEx 5.4.23.5 and HarmonyX (`0Harmony.dll` assembly version 2.9.0.0).
References also include `assembly_valheim.dll`, `UnityEngine.dll`, and
`UnityEngine.CoreModule.dll`. HarmonyX and the game reference are available for
future work; this startup plugin does not call them. No third-party DLLs are
vendored or copied into build output, and no NuGet packages are required.

## Build

From the repository root in PowerShell:

```powershell
.\Scripts\Build.ps1
.\Scripts\Build.ps1 -Configuration Debug
.\Scripts\Build.ps1 -ValheimDevPath 'S:\Valheim-Modding\PoVPlus-Dev'
```

The script restores against a local directory only, builds, and checks that the
output contains no dependency DLLs. Release output is
`Source\PoVPlus\bin\Release\net48\PoVPlus.dll`.

Alternatively, open `Source\PoVPlus.sln` in Visual Studio and build Release.
For a different reference installation, pass `/p:ValheimDevPath=...` to MSBuild.
Missing reference DLLs produce an explicit build error. Do not replace installed
BepInEx or Harmony libraries to resolve a compilation problem.

## Safe development deployment

Use the standardized Windows PowerShell workflow. Building and diagnostics do not
authorize deployment, rollback, launching, or game configuration edits. A general
debugging request is not authorization to change the installation.

```powershell
.\Scripts\Dev.ps1 Build
.\Scripts\Dev.ps1 Status
.\Scripts\Dev.ps1 Deploy -WhatIf
.\Scripts\Dev.ps1 Deploy -DryRun
.\Scripts\Dev.ps1 Diagnose
.\Scripts\Dev.ps1 Rollback -WhatIf
.\Scripts\Dev.ps1 Launch -WhatIf
```

Only after separate explicit approval for each real operation:

```powershell
.\Scripts\Dev.ps1 Deploy -Authorized -Confirm
.\Scripts\Dev.ps1 Rollback -Authorized
.\Scripts\Dev.ps1 Launch -Authorized -Confirm
```

Rollback lists receipts, defaults to undoing the latest completed operation, and
requires an interactive confirmation. `-BackupId <id>` selects a specific compatible
receipt. `-DryRun` and `-WhatIf` do not write backups, reports, or installation files.
`-Authorized` records caller intent; it does not replace obtaining user approval.

Deployment validates the exact configured development path, plugin identity,
duplicate GUIDs, stopped processes, clean DLL output and hashes. It manages **only
PoVPlus.dll**; existing PDBs and configurations are left untouched. Backups, audit
receipts, staging and diagnostic reports stay in ignored `.dev-state/`. No operation
automatically deploys or launches another command. The repository and installation
must share a volume; junctions/reparse points are rejected.

**Save warning:** the development client may access the same character and world
saves as the normal game. Do not rely on `-savedir` for isolation. Launch uses the
development executable/working directory, windowed 2560x1440, popup window and console.

See [workflow and recovery instructions](Knowledge/Development/DEV-003-Workflow.md)
for crash recovery, diagnostic session selection, tests, and limitations.

```powershell
.\Scripts\tests\Test-DevWorkflow.ps1
python -B -m unittest discover -s Scripts/tests -v
python -B Scripts/knowledge.py validate --strict
```

There are no automated gameplay tests yet. Compilation and the build output
check validate this scaffold; startup behavior requires the approved runtime
smoke test. Existing compatibility configurations and third-party mods must
remain intact. Verify actual API signatures and licenses before future patches
or distribution.
