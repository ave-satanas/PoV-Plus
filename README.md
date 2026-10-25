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

Building does not deploy files or launch Valheim. Deployment and launching require
separate approval. After approval:

1. Exit the development game and server. Confirm the destination is
   `S:\Valheim-Modding\PoVPlus-Dev`, not a production installation.
2. If `BepInEx\plugins\PoVPlus` already exists, preserve its contents in a backup
   outside the plugin scan directory before replacing anything. Check for an
   older PoVPlus DLL elsewhere under `plugins` to avoid duplicate plugin GUIDs.
3. Create `BepInEx\plugins\PoVPlus` and copy only the built `PoVPlus.dll` there.
   The matching PDB is optional for debugging. Do not copy dependencies or configs.
4. Launch only after approval and check `BepInEx\LogOutput.log` for
   `PoVPlus 0.1.0 loaded.` and any new errors.
5. To roll back with the game stopped, remove only the newly deployed PoVPlus
   files and restore any previous PoVPlus version from the backup.

There are no automated gameplay tests yet. Compilation and the build output
check validate this scaffold; startup behavior requires the approved runtime
smoke test. Existing compatibility configurations and third-party mods must
remain intact. Verify actual API signatures and licenses before future patches
or distribution.
