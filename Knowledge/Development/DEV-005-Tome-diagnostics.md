---
id: "DEV-005-Tome-diagnostics"
title: "Opt-in Tome observation and offline validation"
type: "development"
status: "active"
created: "2026-10-25"
last_verified: "2026-10-25"
versions: {"PoVPlus": "0.1.0", "Valheim": "1.0.20", "BepInEx": "5.4.23.5"}
evidence: ["Source/PoVPlus/Diagnostics/DiagnosticSession.cs", "Source/PoVPlus/Diagnostics/TomeDiagnostic.cs", "Scripts/collect_tome_evidence.py", "Scripts/tests/TomeDiagnosticHarness.cs", "Scripts/tests/Test-TomeContracts.ps1"]
related: ["[[INV-003-Magic-book-tome-compatibility]]", "[[COMP-002-Tome-bridge-design]]", "[[DEV-003-Workflow]]", "[[DEV-001-Build]]", "[[ARCH-001-PoVPlus]]", "[[DEP-001-Environment]]", "[[DIAG-001-Logging]]"]
confidence: "mixed"
watch: ["src:Source/PoVPlus/Plugin.cs", "src:Source/PoVPlus/PoVPlus.csproj", "src:Source/PoVPlus/Diagnostics/DiagnosticSession.cs", "src:Source/PoVPlus/Diagnostics/TomeDiagnostic.cs", "src:Scripts/collect_tome_evidence.py"]
claims: {}
contradicts: []
supersedes: []
---

# Opt-in Tome observation and offline validation

## Boundary and ordinary builds

This is observation tooling, not an equipment bridge. Diagnostic sources are removed from normal
compilation by the csproj. EnableTomeDiagnostic=true explicitly includes them and defines
POVPLUS_TOME_DIAGNOSTIC; Plugin's observation entry point is compiled only then. No installed
configuration is created to enable it. Never deploy an opt-in artifact without specific approval.
The ordinary startup log and absence of gameplay patches remain the production baseline.

The local baseline is main at 9f6261f before this offline repair. The installed ordinary DLL remains
the exact restored binary recorded in INV-003. Builds here write only repository ignored outputs;
no game launch, deployment, save access or third-party config changes are part of offline checks.

## Identity and readiness

Expected names remain exactly PoVPlus_Test and POVPlus_Lab. Comparison is ordinal, case-sensitive;
case-only differences are logged but rejected. The probe does not accept filenames, normalized
aliases or guessed alternative casing. Name APIs validate labels, not unique save identities;
duplicate display names cannot be distinguished without further approved evidence. Manual
selection and the designated disposable-session boundary remain essential.

Game APIs verified locally: Player.GetPlayerName() reads ZDO s_playerName, returning empty for an
invalid view or "..." for a missing name; ZNet.GetWorldName() returns World.m_name or null.
SetPlayerID writes the supplied name. Neither getter normalizes case or returns a filename.
Game.SpawnPlayer sets the local player before loading profile data and calling OnSpawned; this
sequence alone does not prove Update can interleave within that synchronous method. Prior abort
causation remains unknown because the old probe omitted actual values/comparison outcomes.

The repaired probe waits for a valid ZNetView, player.m_isLoading == false (audited private read),
Game/ZNet presence and an open inventory before rejecting a mismatched initialized identity.
It logs expected/observed names, API names, availability, exact-match and case-only flags.
Only these display-name values are logged; controls are escaped and values capped at 96 characters.
No player IDs, Steam IDs, credentials, profile paths or save contents are queried. Matching names
are required before reading mod/UI/item state. Missing metadata defers; mismatches fail closed.

Once a local player appears, initialization is bounded to 180 seconds, including waiting for
inventory/descriptors/prefabs. Menu waiting remains inert. State changes log once, with a maximum
24 transitions; identical per-second polls do not log. Time regressions, timeout, API drift or
binding failure abort with a reason. A snapshot is attempted once, including failures.

Seven runtime DLL fingerprints (game, HarmonyX and five mods) pin audited implementations.
Unknown visible descriptors are logged but their predicates skipped. The known SeneaL wrapper's
closure, underlying MS predicates and invocation lists are validated; methods with active Harmony
owners are skipped as unaudited. This is conservative and may leave values unverified in a real
modpack. Direct prefab data is read; no items are constructed, registered, moved or equipped.

## Offline commands

From the repository root, in PowerShell:

```powershell
.\Scripts\Dev.ps1 Build
.\Scripts\tests\Test-TomeDiagnostic.ps1
.\Scripts\tests\Test-TomeOffline.ps1
.\Scripts\tests\Test-TomeContracts.ps1
python -B -m unittest discover -s Scripts/tests -v
python -B Scripts/knowledge.py validate --strict
```

Test-TomeOffline generates a package-free net48 harness under .dev-state/offline-tome/harness
and executes only the Unity-independent policy against synthetic observations. It does not
instantiate game types or read saves. Contract tests inspect installed metadata without executing
plugins; they fail on fingerprint/shape drift. Artifact safety checks inspect direct calls/field
writes; reflection purity still requires the documented IL audit.

After the ordinary package-free restore, an offline opt-in compile check can be performed with:

```powershell
dotnet build Source/PoVPlus/PoVPlus.csproj --no-restore --configuration Release -p:EnableTomeDiagnostic=true -p:OutputPath=S:/Valheim-Modding/PoV-Plus/.dev-state/offline-tome/diagnostic/ --nologo
.\Scripts\tests\Test-TomeDiagnostic.ps1 -Enabled -Dll .dev-state/offline-tome/diagnostic/PoVPlus.dll
```

This is a compile check, not deployment. Run the ordinary Build/exclusion check afterward.
Do not copy the ignored opt-in output into the game, and do not redistribute dependencies.

## Focused evidence collector

Scripts/collect_tome_evidence.py supersedes the ignored one-off recursive installation collector
for runtime diagnostic evidence. It reads only BepInEx/LogOutput.log, the explicitly referenced
LoadTimeProfiler session and explicitly supplied repository-local workflow manifests. No recursive
cache/save enumeration, permission changes or cleanup. Missing/inaccessible optional sources are
nonfatal skipped entries with reasons; a missing main log or wrong session is incomplete/error.
It bounds reads to 8 MiB, detects file changes during read, preserves original line numbers,
timestamps/hashes and limits excerpts. The last BepInEx boundary and profiler reference must match
the requested session; the profiler's own session header is checked as well.

```powershell
python -B Scripts/collect_tome_evidence.py --session 2026-10-25_01-01-03 --receipt .dev-state/backups/20261025T0811496603929Z-b12efed6cf6049c59dad83532f265822/manifest.json --output .dev-state/reports/INV-003-offline-recollection
```

Use a new output stem: reports never overwrite earlier evidence and remain under .dev-state/reports.
The example refers to the historical failed run, not new runtime proof. Reports distinguish
observed false results, skipped predicates, errors and unavailable sources. A completed snapshot
does not prove equipment gameplay. Redaction is bounded/basic; review excerpts before sharing.
For installation-integrity auditing use separately planned stopped-process targeted fingerprints;
this focused collector deliberately does not claim whole-installation coverage.

## Next supervised session â€” planning only

1. Review guard tests and expected spellings. If displayed names differ, obtain an explicit
   authorization correction before changing the allowlist; do not guess casing from save paths.
2. Record ordinary deployed hash and preview/build/deploy a reviewed opt-in artifact using the
   standardized workflow and approved output configuration. Keep the original receipt/backup.
3. Launch only with separate approval. The user manually selects PoVPlus_Test / POVPlus_Lab,
   waits for loading and opens inventory within the bounded observation window. No automatic inputs.
4. After user confirmation, collect the exact new session; require a bound active RENDERED
   descriptor and a valid MS positive control before drawing acceptance conclusions. Inspect
   GUARD and skipped/error rows rather than assuming missing output is an equipment failure.
5. Distinguish descriptor acceptance from the internal MS lifecycle gate: the current probe
   reports the shared Tome registry membership as static-contract evidence for the latter, not
   a direct runtime call to its private validator. Verify that gate only through an audited,
   non-mutating observation; otherwise leave it unverified. Collect read-only lifecycle evidence.
   Lifecycle/effect inspection here remains static. Any actual equip/unequip, stats, persistence,
   grave recovery or multiplayer regression tests require a separately approved expanded scope.
6. Ask for normal manual exit, verify stopped, preview/execute the exact rollback and verify the
   original DLL hash. Verify ordinary builds exclude tooling. Never restore while the game runs.

No steps in this runtime plan were executed during the offline repair.


For the separately authorized supervised session, the existing Build wrapper inherits MSBuild
environment properties. Build the opt-in artifact into the normal managed output only during
that session; clear the property even on failure, then audit before any approved deployment:

```powershell
$previous = $env:EnableTomeDiagnostic
try {
    $env:EnableTomeDiagnostic = 'true'
    .\Scripts\Dev.ps1 Build
} finally { $env:EnableTomeDiagnostic = $previous }
.\Scripts\tests\Test-TomeDiagnostic.ps1 -Enabled
.\Scripts\Dev.ps1 Deploy -WhatIf
# Inspect the plan; only with specific authorization:
.\Scripts\Dev.ps1 Deploy -Authorized
.\Scripts\Dev.ps1 Launch -WhatIf
# Only after launch authorization: Dev.ps1 Launch -Authorized
```

Record the new deployment receipt, rather than assuming a historical backup ID. After manual
exit and stopped-process verification, preview Rollback -BackupId <that-deployment-id> -WhatIf,
then execute it with -Authorized and the normal confirmation. Verify the original hash. Finally
run an ordinary Build and the default exclusion audit. These commands are a future plan only;
no opt-in normal-output build or deployment was performed in this offline task. Launch warnings
about shared saves continue to apply. Use [[DIAG-001-Logging]] to interpret per-plugin filtering;
missing informational output alone does not prove failed execution.
