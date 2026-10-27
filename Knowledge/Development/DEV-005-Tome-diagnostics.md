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
watch: ["src:Source/PoVPlus/Plugin.cs", "src:Source/PoVPlus/PoVPlus.csproj", "src:Source/PoVPlus/Diagnostics/DiagnosticSession.cs", "src:Source/PoVPlus/Diagnostics/TomeDiagnostic.cs", "src:Scripts/collect_tome_evidence.py", "src:Source/PoVPlus/Diagnostics/TomeStageBProbe.cs"]
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
The startup log is retained. The later ordinary-build bridge is controlled separately by a
default-false startup configuration option: [[COMP-002-Tome-bridge-design]]. With that option Off
no bridge patches are applied; temporary diagnostic types remain absent from ordinary builds.

The historical offline-repair baseline was main at 9f6261f. The installed ordinary DLL remains
the exact restored binary recorded in INV-003. Builds here write only repository ignored outputs;
no game launch, deployment, save access or third-party config changes are part of offline checks.

## Identity and readiness

Expected names are exactly PoVPlus_Test and PoVPlus_Lab (world label corrected with explicit
user authorization after the prior mismatch). Comparison is ordinal, case-sensitive;
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

## INV-003 Stage A - Controlled Baseline Runtime Verification

This is the next development milestone, requiring separate runtime authorization. Keep
EnableMagicPluginTomeBridge false throughout Stage A; do not apply bridge patches. Use the
repaired opt-in Tome diagnostic and the standard deployment/launch/rollback workflow.

1. Verify the exact PoVPlus_Test / PoVPlus_Lab identity guard after manual user selection.
2. Confirm the actual active rendered Tome descriptor and equipment authority.
3. Compare BMP_BeginnersMagicBook, BMP_AdvanceMagicBook and BMP_DruidsTome against the native
   ArmorMushroomcallerTome_DO control, including classifications and descriptor acceptance.
4. Evaluate the internal MS acceptance gate through audited read-only evidence. The current
   probe records registry membership rather than invoking its potentially initializing helper;
   distinguish that evidence from a directly executed predicate and leave gaps unverified.
5. Collect session-specific read-only evidence; do not infer effects/equipping from predicates.
6. Ask for normal manual exit, verify stopped, restore the recorded original DLL through the
   standardized rollback workflow and verify its original hash. No save edits or automated inputs.

**Stage B is a separate later authorization** to enable and test the bridge, including both
acceptance gates, native regression controls, effects, exclusivity, persistence and multiplayer
where appropriate. Neither stage is authorized or executed by this repository checkpoint task.
Disabling/removing the bridge while a MagicPlugin book is equipped can invalidate upstream
bindings/equipment state; retain the manual-unequip and normal-exit precautions for Stage B.

## Earlier supervised plan details (subject to Stage A/B boundary)

1. Review guard tests and expected spellings. If displayed names differ, obtain an explicit
   authorization correction before changing the allowlist; do not guess casing from save paths.
2. Record ordinary deployed hash and preview/build/deploy a reviewed opt-in artifact using the
   standardized workflow and approved output configuration. Keep the original receipt/backup.
3. Launch only with separate approval. The user manually selects PoVPlus_Test / PoVPlus_Lab,
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


## First repaired Stage A attempt: observable abort

Session 2026-10-25_18-25-08 reached initialized player/inventory readiness but failed the exact
world comparison: expected POVPlus_Lab, observed ZNet.GetWorldName() PoVPlus_Lab. Character
PoVPlus_Test matched. The guard logged a case-only mismatch and aborted before any snapshot.
This confirms logging/strict rejection, not a successful authorization or equipment evaluation.
No literal/normalization change or automatic redeployment was made. Any subsequent attempt
must explicitly review the corrected exact world label; never silently accept aliases.
See [[INV-003-Magic-book-tome-compatibility]] for evidence and completed normal-exit/rollback.
The exact original DLL is restored, instrumentation inactive and bridge false. A subsequent
attempt requires a separately authorized exact-label review; no correction was applied here.
The documented DryRun plan was used after a Windows PowerShell WhatIf/Get-FileHash preview
error; it runs the same validation without WhatIf propagation. Workflow source is unchanged.


## Authorized exact-label correction for Stage A retry

The user explicitly corrected the expected world to PoVPlus_Lab. DiagnosticSession.ExpectedWorld
and the snapshot label now use that exact spelling; ExpectedCharacter remains PoVPlus_Test.
Ordinal comparisons, separate API logging, readiness, timeout and one-snapshot policy are
unchanged. Regression coverage now rejects observed POVPlus_Lab with case-only evidence and
zero snapshots. The historical failed-attempt values above are preserved; no saves were renamed
or inspected, and no bridge source/configuration behavior changed.


## Corrected retry snapshot: identity and rendered-cell proof

Session 2026-10-25_18-38-00 passed exact PoVPlus_Test / PoVPlus_Lab comparisons and completed
one snapshot (primary log 1448-1474). The bound active SeneaLUI_Link1 descriptor is
AzuEPI.emulated/$tome_DO with MS IsTomeItem callback. Three BMP books return false, native MS
control true, through direct descriptor and visible-wrapper queries. Registry membership was
read; private IsItemValidForSlot was not invoked, so direct internal-gate execution remains
unverified. No item/effect mutation was performed. [[INV-003-Magic-book-tome-compatibility]]
contains the matrix and source lines. Await user normal exit before rollback; bridge remains Off.


## Corrected retry cleanup verified

After manual normal exit, independent process/Status checks and the exact-receipt standard
rollback restored original SHA-256 351E948AFC75D8B5BFBEF67F2F1FBABB5A8A645AA7A5D51E9E804F7D501DF548.
Completed rollback receipt: 20261026T0144272444449Z-402d964125ea45bfa48137b6813caaf0.
All 343 retry DLL/config baseline hashes match; bridge false, native MP slot Off, diagnostic
not deployed and client stopped. Source correction and regression tests remain unstaged.
Final tests: 18 identity/policy, 22 bridge, 25 contracts, 24 Python, 14 workflow; ordinary build,
artifact/exclusion checks, strict knowledge and whitespace checks pass. No Stage B was started.
[[INV-003-Magic-book-tome-compatibility]] separates actual callback observations from private
internal-validator inference and records the remaining supervised testing requirements.


## Historical Stage B preparation (uncommitted at that checkpoint)

Stage A checkpoint 446f8e5e4a208e97bd430e1a937ca7f3ff5ef4e7 was pushed and fetched; main/origin
were clean/equal before this work. The current user explicitly authorizes only an enabled-bridge
acceptance test with the same manual character/world handoffs and no equipment/item operations.
At that checkpoint no Stage B change was authorized for commit or push; the later checkpoint task authorizes preservation after review.

EnableTomeStageBDiagnostic=true additionally requires EnableTomeDiagnostic=true; otherwise the
build fails. Ordinary builds exclude the Stage B type. This build option never changes the
bridge default or activates it by itself. Activation is a temporary single-value edit of only
BepInEx/config/povplus.core.cfg from false to true, after validation, with exact original bytes
backed up under .dev-state/stage-b/. Restore those bytes only after user normal exit/rollback.
No third-party configuration is changed; MP native Tome remains Off. The existing bridge code
and its atomic inactive-until-complete patch installation are unchanged.

The Stage B-only helper reads own configuration/activation, verifies exact patch MethodInfos,
owner povplus.core.magicplugin-tomes and one postfix per gate with no prefixes/transpilers/
finalizers/IL manipulators. It logs actual priority/index/before/after lists. Other patched
predicates/helpers remain refused. Existing Stage A's unpatched-only policy is unchanged unless
the extra Stage B compile flag is set. Startup dependency hashes and patch metadata may be read
before local-player authorization; no item/registry evaluation occurs until exact identities,
initialized UI, existing registry and native control pass the usual bounded single snapshot.

Pinned MS helper GetSharedSlotRegistry (0x0600044a) returns an existing dictionary at IL_0018-19;
only its missing-registry branch calls SetData at IL_0030. IsItemValidForSlot (0x0600044c) reads
null/prefab/slot validity and membership. Direct invocation is allowed only after observing a
correct-type existing tome registry/native control and verifying helper/target patch metadata.
Supporting ObjectDB lookup methods must be unpatched; installed IL reads dictionaries only.
Registry reference and contents are compared before/after. Do not initialize missing state to
force a result. The reviewed bridge postfix reads metadata and updates only its own activation/
bounded logging state; it performs no inventory, equipment, SE, stats, save or registry writes.

The snapshot adds verified-at-runtime prefab Wood and a belt non-Tome identifier query, logging
whether belt is actually registered in MS. Its original registry expectation is compared with
the observed result; native positive overlap is preserved, not mislabeled as a conflict. All
three books and native control are inspected as existing prefab ItemData, never constructed.
Before-intervention reference values come from Stage A; no reverse patch/unpatch is used to
invoke original methods. Existing bridge first-extension logs prove the guarded false-to-true
path was visited; direct post-bridge values and membership are recorded separately.

Run Test-TomeStageB.ps1 (managed doubles, no real detours), Test-TomeStageBContracts.ps1 (pinned
installed purity contract), all prior tests and artifact audits. For the extra probe audit use
Test-TomeDiagnostic.ps1 -Enabled -StageB -Dll <artifact>. The evidence collector now retains
same-session TomeBridge lines alongside TomeDiagnostic events; snapshot counts still use only
the diagnostic prefix. No effects, persistence, exclusivity or multiplayer correctness is proven.


Stage B checkpoint: session 2026-10-25_19-09-09 reports the bridge active, but the temporary
probe aborted during Awake because its own Chainloader PluginInfo.Instance was not yet assigned.
No acceptance snapshot or direct internal-gate result was obtained. The attempt is stopped;
user normal exit, original-DLL rollback and exact own-config restoration are pending. Do not
repair/redeploy automatically or claim Stage B passed. [[INV-003-Magic-book-tome-compatibility]]
records the installed Chainloader ordering, local evidence and reviewed future repair options.

Stage B cleanup is now verified after user normal exit: standard rollback restored the exact
original non-diagnostic DLL and the exact original PoVPlus configuration bytes (bridge false).
All 343 baseline DLL/configuration hashes match; development client stopped, separate server
untouched. Evidence and experimental work remain preserved and uncommitted. The startup-probe
failure still requires a reviewed repair and separately authorized retry; no Stage B acceptance
or gameplay success is claimed. See [[INV-003-Magic-book-tome-compatibility]] for hashes/receipt.


## Authorized Stage B retry: startup snapshot repair (offline)

Previous startup failure is confined to the temporary probe's own Chainloader.Instance lookup
inside Awake. Installed Chainloader calls AddComponent before assigning PluginInfo.Instance.
Plugin.Awake now passes its already-bound boolean setting as an immutable nullable value snapshot
to TomeDiagnostic/CheckStartup; null or false fails closed with a useful message. No own
registration lookup or new global singleton is needed. Existing bridge-active, patch-owner,
registry-purity, exact identities, bounded readiness and one-snapshot guards remain unchanged.
The bridge implementation/default false are unchanged. Twenty executable Stage B managed
fixtures cover the two pre-registration cases, missing snapshot and prior safety cases; compiled
IL checks verify actual Awake-to-constructor wiring and no own Chainloader lookup. These fixtures
are doubles, not actual Harmony gameplay tests. Eighteen identity/policy, 22 bridge, 25 installed
contracts, 26 Python and 16 workflow cases pass; ordinary/Stage B builds and artifact audits pass.
File-lock policy and process-guard regression coverage are in [[DEV-003-Workflow]] and AGENTS.md.
Runtime retry evidence will be recorded separately in .dev-state/reports/INV-003-stage-b-retry.md;
no new acceptance, effects, persistence or multiplayer result is implied by this offline repair.


Stage B retry startup (2026-10-25_19-24-38) now succeeds after immutable-setting repair:
88 plugins loaded, none skipped/failed; bridge active; diagnostic verifies both exact owned
postfixes at priority400/index0, no other interceptors or before/after constraints. No startup
ABORT. Manual authorized character/world/inventory handoff is pending; no acceptance snapshot
or gameplay result is claimed. The development client is running with temporary own-config true;
normal user exit and original-DLL/config restoration are still required. Local evidence:
.dev-state/reports/INV-003-stage-b-retry-startup-evidence.md; receipt/state in
.dev-state/stage-b-retry/state.json. Earlier failure and cleanup remain historical evidence.


## Stage B retry acceptance verified; cleanup pending

Session2026-10-25_19-24-38 passed exact identities and completed one snapshot without ABORT.
Runtime primary1462-1492 confirms the visible AzuEPI.emulated/$tome_DO MS callback and three
books changing Stage A descriptor/visible False to Stage B True. Direct private internal tome
validation now also returns True for all three; native ArmorMushroomcallerTome_DO passes all
three queries, Wood fails all three. All five fail the nonregistered non-Tome belt identifier.
Registry membership remains unchanged (books absent, native present); no gameplay mutations.
Stage A never called the private validator, so its internal baseline remains static/inferred.
Both exact owned postfixes were verified (priority400/index0, no other interceptors). MP native
Tome slot remains Off. This supports acceptance integration only, not equipment effects,
exclusivity, actual inventory items, persistence or multiplayer. See
[[INV-003-Magic-book-tome-compatibility]] for matrix, line references, limits and the recurring
EpicLoot warning. Evidence: .dev-state/reports/INV-003-stage-b-retry-acceptance-evidence.md.
A Warfare cfg hash drift occurred during the session and is preserved, not automatically reverted.
Client still running; original DLL/exact own-config restoration awaits user normal exit. No
installation changes were made by collection and no experimental changes were committed/pushed.


Stage B retry cleanup completed after user normal exit: exact original non-diagnostic DLL and
own config bytes restored, bridge false, client stopped, separate server untouched. All343
monitored DLL/config fingerprints match baseline. Warfare.cfg changed transiently during the
run and returned to its baseline hash without cleanup writing it; preserve both observations
in [[INV-003-Magic-book-tome-compatibility]]. Successful acceptance evidence remains valid;
equipment behavior/effects/exclusivity/persistence/multiplayer remain unverified. Experimental
source/tests/docs remain uncommitted. Next: review checkpoint and separately authorize a scoped
equipment-lifecycle plan; no further runtime work is implied. Restoration hashes/receipt:
.dev-state/stage-b-retry/restoration-comparison.json.


## Stage B checkpoint review and Stage C proposal

Stage B source/tests and canonical summaries have been reviewed for Git preservation. Earlier
pending/uncommitted statements above describe those historical checkpoints. Acceptance and
original DLL/config restoration are verified; no equipment/effects/persistence claim is added.
[[EXP-004-Tome-equipment-lifecycle-plan]] defines the proposed, unexecuted Stage C procedure,
separate C1/C2/C3/C4 approvals, manual preparation, evidence/failure criteria and explicit
unequip-and-confirm-neutral-persistence before disablement/rollback. It grants no deployment,
launch, item/save or multiplayer permission. The existing dedicated server remains out of scope.


## Offline C1 preflight (no runtime trial)

[[DEV-006-Tome-C1-preflight]] owns the new acquisition/observation evidence and C1 matrix.
MP Workbench1 recipes and native MagicWeaver4 recipe are corroborated by installed code/config
and generated DataForge references; progression/availability and actual test possession remain
unverified. MS GetEquippedItem calls GetOrCreateValue indirectly; SeneaL Claimed writes a cache:
neither is an unconditional pure observation API. Proposed lifecycle reads must use pre-existing
state/TryGetValue and audited fields, distinguish missing from empty, correlate all inventories
and authorities, and require neutral save/reload before rollback. The new static contract suite
has27 checks; it executes no mod code. C1 runtime instrumentation is designed, not implemented.
Stage B remains acceptance-only evidence. No launch, deployment, config/save/item/server actions,
commit or push are authorized/performed by this offline task.


## C1 lifecycle observer offline implementation - 2026-10-26

[[DEV-006-Tome-C1-preflight]] now documents the implemented opt-in observer and executable/static
tests, superseding its earlier design-only status. New lifecycle build flag requires diagnostic
mode and excludes Stage B mode; ordinary artifacts exclude its code/polling. Bound immutable
setting, exact identity before every state capture, non-creating weak-table lookup and raw
existing field reads isolate it from production bridge behavior. Complete PoV/SeneaL owner paths,
rendered Source item, actual regen rate and neutral certification explicitly remain Unknown.
The observer is NOT runtime-ready for the full equipment/neutral-state matrix; no C1 gameplay
or purity is claimed from offline checks. Stage B evidence and original deployment remain intact.
