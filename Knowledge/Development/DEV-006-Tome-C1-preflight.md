---
id: "DEV-006-Tome-C1-preflight"
title: "Stage C1 acquisition, observation and neutral-state preflight"
type: "development"
status: "partial"
created: "2026-10-25"
last_verified: "2026-10-26"
versions: {"MagicPlugin": "2.2.2", "Magic Supremacy": "3.1.5", "SeneaL UI": "1.2.7", "Valheim": "1.0.20"}
evidence: ["Scripts/tests/Test-TomeC1Contracts.ps1", "Installed MP PrefabsSetup tokens0x06000130-132; MS Awake IL35c9-363d", "Installed MS CustomSlotSystem tokens0x06000453,457,459,45b,464", "Installed SeneaLUI.Inv.CustomStore token0x06000a9b", "BepInEx/config/blacks7ar.MagicPlugin.cfg:83-114,1416-1447,2676-2707", "BepInEx/config/DataForge/recipes.reference.yml:4838-4844,4867-4873,4881-4887,5445-5451", "BepInEx/config/YouAreNotWorthy/items.reference.yml:917,921,923,1014", "Installed Terminal.InitTerminal token0x06000915 IL1028-1077; handler0x060026ee; spawn action0x0600093b"]
related: ["[[EXP-004-Tome-equipment-lifecycle-plan]]", "[[INV-003-Magic-book-tome-compatibility]]", "[[COMP-002-Tome-bridge-design]]", "[[DEV-005-Tome-diagnostics]]", "[[DEV-003-Workflow]]"]
confidence: "mixed"
watch: ["src:Source/PoVPlus/Diagnostics/LifecycleEvidence.cs", "src:Source/PoVPlus/Diagnostics/TomeLifecycleObserver.cs", "src:Source/PoVPlus/Plugin.cs", "src:Source/PoVPlus/PoVPlus.csproj", "src:Scripts/tests/Test-TomeLifecycle.ps1", "src:Scripts/tests/Test-TomeLifecycleArtifact.ps1", "src:Scripts/tests/TomeLifecycleHarness.cs", "src:Scripts/collect_tome_evidence.py"]
claims: {}
contradicts: []
supersedes: []
---

# Stage C1 offline preflight

## Scope and current readiness

Offline inspection only. No game/plugin code execution, launch, deployment, acquisition, save
access, configuration writes, server interaction or C1 trial. Stage B proved prefab acceptance,
not actual inventory equipment. No C1 runtime observer is implemented by this task. The new
`Test-TomeC1Contracts.ps1` executes 27 pinned static metadata/config checks without loading mods.
The lifecycle observer below still needs a separate implementation/review before deployment.

## Acquisition: verified definitions versus availability

All identifiers/counts below are internal resource prefab names, not translated item labels.
MP methods AdvanceBook/BeginnersBook/DruidsTome register these requirements through embedded
ItemManager. Disk station/cost settings agree; requireOnlyOneIngredient is Off for all three.
MS Awake registers the native recipe and craft amount1. DataForge recipes.reference corroborates
the registered quantities; it is a generated historical reference, not independent proof of the
current live recipe. Inspected DataForge recipes.yml is comments only, with no recipe entries;
no additional recipe override file or target match was found in the inspected configuration set.
Later server sync, other mods and runtime recipe availability remain unverified.

| Target prefab | Station / level | Crafting requirements | Progression reference (not observed on character) |
|---|---|---|---|
| BMP_BeginnersMagicBook | piece_workbench /1 | GreydwarfEye6, Resin4, Feathers6, Wood12 | No required key listed for item in YANW reference |
| BMP_AdvanceMagicBook | piece_workbench /1 | YmirRemains2, SurtlingCore5, Crystal52, AncientSeed12 | defeated_bonemass |
| BMP_DruidsTome | piece_workbench /1 | GoblinTotem3, LoxPelt12, LinenThread16, ElderBark8 | defeated_dragon |
| ArmorMushroomcallerTome_DO | MagicWeaver_DO /4 | PageArcane_DO4, SpiritMushroom_DO6, DimEitr_DO4, MushroomYellow4 | defeated_eikthyr |

MS config Content - Armor / Enable Mushroomcaller Set is On. Recipe discovery, materials,
stations/upgrades, personal keys and possession were NOT checked in the test saves. YANW's
progression.yml explicitly describes recipe-inherited personal-key requirements; item references
are consistent with ingredient tiers. Do not infer that a spawned item bypasses equipment gates,
that a global key satisfies a personal key, or that a reference entry proves the live gate.

Least invasive sequence, subject to future approval:

1. User manually checks existing ordinary items on only PoVPlus_Test; do not assume possession.
   Establish a manifest before equipping. Do not transfer items from personal saves.
2. If already eligible and supplied in PoVPlus_Lab, manually craft normally using the verified
   station/resources. Record quantities/resources/quality and progression conditions. Do not
   construct high-tier infrastructure, farm bosses or change keys merely to satisfy this test
   without an explicit expanded scope. Unavailable cases remain Blocked, not silently skipped.
3. If crafting is impractical, request approval for an explicitly test-only manual console
   acquisition. Installed Valheim registers cheat command `spawn`; handler parses prefab,
   amount(default1), level(default1), radius(default0.5), and optional p/e/i flags. Exact examples:
   `spawn BMP_BeginnersMagicBook 1 1`, `spawn BMP_AdvanceMagicBook 1 1`,
   `spawn BMP_DruidsTome 1 1`, `spawn ArmorMushroomcallerTome_DO 1 1`.
   These commands are documented candidates, NOT executed or approved. They create world objects;
   default command does not request Pickup/UseItem, but ordinary game auto-pickup may still act.
   The e flag reaches UseItem/equipment; p/i can trigger pickup. Exclude all suffixes, wildcards,
   field overrides, stacks and repeated commands from a first approved acquisition protocol.
   User must separately approve cheat-mode activation and pickup/world writes, verify the exact
   local session/prefab registration and count, and preserve the pre-test cheat state.
   Installed command registration/IL is verified; modpack interception/permission restrictions,
   cheat enablement and successful runtime creation are NOT verified. Do not guess a mod-specific
   give command or call Inventory.AddItem/Instantiate through diagnostic code.
4. If progression blocks use, stop and obtain a decision; no defeat-key grants, config disabling,
   save editing, bridge broadening or upstream safeguard bypass is included.

The console action invokes normal OnCreateNew/SetQuality and may still yield metadata different
from canonical prefabs. Both crafted and spawned actual instances need guarded state observation.
Existing-item availability and a specific acquisition choice remain user decisions.

## Observation contracts: safe candidates and traps

| Installed contract | Static finding | Proposed use |
|---|---|---|
| MS GetRuntimeData(Humanoid),0x0453 | ConditionalWeakTable.GetOrCreateValue | Never call; missing data must stay missing |
| MS GetEquippedItem,0x0459; IsItemEquippedInCustomSlot,0x045b | Both call GetRuntimeData | Not unconditional read-only APIs; do not invoke |
| MS GetDefinitionBySlotId,0x0457 | Calls EnsureDefinitionsInitialized | Read existing definitions field, never initialize for inspection |
| MS RuntimeData; nested HumanoidCustomSlotData.EquippedItems | Existing weak table and string-to-ItemData ownership dictionary | Reflect existing table, call framework TryGetValue(localPlayer,out data), then dictionary TryGetValue; never GetValue with factory/GetOrCreateValue |
| MS GetSavedEquippedGuid,0x0464 | Reads Player.m_customData.TryGetValue(def.PlayerSaveKey) | Prefer direct existing dictionary/key read; absent versus empty preserved, no GUID creation or setters |
| SeneaL CustomStore.Claimed,0x0a9b | Writes _plainBelt cache; calls frame-refreshed LinkedShown and delegates | Never invoke for observation or equate true with actual ownership |
| SeneaL CustomStore backing Inv, Last and existing UI LinkCell fields | Existing inventory/cache/view references | Read/correlate only; caches are supporting UI evidence, not sole authority |
| Humanoid.GetInventory0x01df; Inventory.GetAllItems()0x0e84 | Return existing field/list | Read/copy scalars without editing returned list; inspect effective Harmony chain or use audited direct fields |
| Inventory.GetEquippedItems0x0e81 | Builds own list from m_equipped flags | Flags alone do not identify MS slot owner; compare original live item references |
| Player.GetEitr/GetMaxEitr0x02c5/2c6; SEMan.GetStatusEffects0x05aa | Direct field getters | Scalar/read-only copied effect view; do not modify returned SE objects/list or invoke SE lifecycle/update methods |

Tokens are within pinned installed assemblies; full signatures/layout are checked by the static
contract script. Raw reflection and getter names are not a runtime purity guarantee. Inspect
actual effective Harmony patches on every invoked game/mod read, inherited behavior and remaining
UI/PoV routes before use. Unsupported/creating getters are skipped as Unknown, not called.
The PoV slot store and complete SeneaL linked-source/utility occupancy read route still need
additional purity/binding review. Thus full exclusivity cannot yet be certified by an observer.

## Proposed C1 observer (design only)

A future explicitly opt-in lifecycle build flag must require diagnostic compilation, remain false
by default and be checked absent alongside Plugin.Update/support types in ordinary artifacts.
Use an independent lifecycle observer, not the one-shot Stage B prefab probe. Pass immutable own
configuration from Awake; never query own unassigned Chainloader.Instance. Preserve dependency
pins, exact ordinal PoVPlus_Test / PoVPlus_Lab guard and manual confirmation. Revalidate identity
before EVERY observation, including after logout/reload; menu/mismatch becomes inert/aborted.
No new Harmony patches, slot registration, equipment method invocation, predicate broadening,
status/stat writes, timers that trigger gameplay, save calls or console-command execution.

Observe on Unity main thread at bounded checkpoints after the user finishes an action; e.g.
sample at most once/sec, log changed fingerprints only, cap to120 changes/10min per authorized
world entry. Treat transitions separately from settled results; require at least two successive
consistent samples and bounded settling timeout. On cap/timeout/missing authority stop the test,
never label silence a neutral result. This bounded observer is a proposal, not present code.

Capture actual local-player item graph: main inventory, existing SeneaL custom inventory, all
existing MS equipped mappings and relevant vanilla/PoV equipment references. Preserve object
reference identity; assigning observer-local aliases is allowed. A single object in inventory
and in MS ownership is normal, not duplication. Detect distinct objects sharing an existing
item GUID, owner conflicts, missing items, unexpected stack/count/quality changes. Never create
GUIDs or mutate custom data to track items. Log only test item aliases or session-local GUID
fingerprints, never raw save/account/player IDs. Missing stable identity after reload remains
Unknown unless a verified manifest comparison resolves it.

Each checkpoint records session/UTC/case/transition, identity authorization, authoritative tome
item or explicit empty/missing, whole MS mapping, item flags and locations, rendered descriptor
binding/source, item count conservation, original shared/equipSE identities, observed active
book/native SEs, max/current Eitr and environment/food baselines. No prefab ItemData substitutes
for owned inventory items. Proposed extra owner reads remain unverified until implemented/audited.
If UI and authority disagree, report both and stop; do not refresh/synchronize equipment to fix it.

Bracket empty/native/book/empty samples to account for food decay and transient buffs. Installed
MP disk Eitr bonuses are24/48/62 and original shared regen modifiers .15/.25/.25; numeric player
expectations still require runtime overrides and food correction. Those modifiers are not flat
Eitr/sec. Use an independently approved safe expenditure method, verified regen delay, identical
recovery range and at least3 trials if regeneration is included. MP classifier remains Off; its
own regen hook may omit the contribution. Observe absence/double effects, do not implement fixes.

## Complete C1 matrix (proposed manual gameplay; not executed)

Prerequisite to every row: authorized initialized identity, audited bridge mode, enough free cells,
settled baseline/manifest, new case marker and explicit user completion. Repeat transitions only
when prior state is consistent. No full-bag, combat, death/grave, destructive or multiplayer cases.

| Case | Manual action / coverage | Expected observation / evidence | Failure criterion |
|---|---|---|---|
| C1-00 | Open inventory, touch no items | Exact names; MS owner; rendered AzuEPI.emulated/$tome_DO; all planned items accounted | Unknown/mismatch, missing item/route, no baseline |
| C1-01 | Native Tome equip then unequip,3 cycles | One MS tome owner; flag and visible state agree; native SE applies/removes once; count unchanged | Native regression, stuck flags/SE, any loss/duplicate |
| C1-02a/b/c | Each MP book equip then unequip,3 cycles per book | Correct actual instance, original type/SE retained; one owner; neutral after removal | Prefab accepted but real instance refused, metadata drift, residual effect or bonus |
| C1-03 | Every ordered MP pair,6 swaps: beginner->advance/druid, advance->beginner/druid, druid->beginner/advance | Only destination owns Tome; old flag/SE removed, old item remains accounted | Double owner/SE, unaccounted item, inconsistent UI |
| C1-04 | Native->each book,3 swaps | Native removed cleanly; one destination owner/effect | Native stale state or failed incoming instance |
| C1-05 | Each book->native,3 swaps | Book removed cleanly; native behavior preserved | Residual book SE/bonus, duplicate item |
| C1-06 | For each book, approved actual Belt/Utility route attempts before/after Tome placement; repeat reverse ordering | No same-instance double ownership; intentional distinct-item legal combinations preserved | Competing actual owners; forced eligibility, uncertain routing; unknown routes Blocked |
| C1-07 | Wood visible-Tome rejection; native control in verified invalid route | Invalid move rejected, original item stays accounted, no SE/slot changes | Material accepted, forced route, item moved/lost unexpectedly |
| C1-08 | Compare empty/native/each-book/empty max Eitr, effects; optional separately approved regen trials | Correct expected deltas once after accounting food/overrides; log all4 book/control SE identities | Unexplained bonus, missing/double regen, no numeric evidence => unresolved/Blocked, never invented pass |
| C1-09 | Unequip final Tome manually; open inventory and wait for settling | MS tome empty, every bridged instance unflagged, all items accounted, baseline SE/stats restored | Any Unknown, missing item, owner/flag mismatch or timeout blocks rollback |
| C1-10 | Approved normal save/logout, reload SAME exact identities with SAME enabled build; open inventory without equipping | Neutral state persists; no saved bridged binding/ghost SE, item manifest agrees | Restored worn book/stale binding, lost identity or incomplete evidence blocks rollback |

C1-06 initially requires reviewing which live Belt/utility routes actually exist and their
intended allowed combinations. Stage B's unregistered belt identifier is NOT this coverage.
Do not pick a random unrelated item for invalid routes: first verify its current rejection.
For native gameplay preservation, an optional separately approved bridge-Off native-control
session with the same read-only observer provides a direct baseline; comparing On native behavior
alone verifies its function, not absence of all regressions against an unmeasured Off session.
C1-10 is mandatory safe cleanup, not the broad C2 worn-equipment persistence campaign. Its normal
save/reload permission must be included even if full C2 is deferred.

## Neutral-state gate and recovery

Neutral requires ALL known, not merely absent log output: exact authorized initialized player;
pre-existing MS authority/data found; tome ownership entry absent/null; all approved books and
native control physically accounted with no test Tome flagged equipped in any existing inventory;
no other known slot owns the same book; rendered slot empty and consistent; no book/native SE or
unexplained max-Eitr/regen remainder compared with matched baseline; no pending equip/swap queue
where observable; test binding key absent/empty, not an unreadable dictionary. Observe two settled
samples, then user normal save/logout and enabled-bridge reload. Repeat the same neutral checks
and manifest comparison after reload. Do not delete item GUID metadata; MS may legitimately retain
it on an unequipped item. Empty binding is distinct from missing/unknown MS runtime data.

If any additional slot store cannot be audited, the observer cannot certify complete exclusivity;
require corroborating manual evidence or leave neutral Unknown and obtain a recovery decision.
Do not quietly omit an equipment authority from the neutral gate.

Only after neutral reload passes: ask user to exit normally, wait for confirmation, independently
verify dev client stopped, standard Dev.ps1 receipt rollback, exact own config byte restoration,
SHA256 checks, diagnostic exclusion and fingerprint comparison. Keep any generated third-party
changes as evidence, never auto-revert. No DLL/config writes while running; server out of scope.
If failed unequip, unknown binding, stale flags/SE, item loss, unexpected writes, wrong identity
or incomplete evidence: stop new interactions, preserve logs/state and the enabled build/config,
request an explicit recovery decision. Do not disable/rollback or automatically retry/redeploy.
An approved relaunch of the same enabled binary to manually unequip or user-managed test-only
backup restoration may be chosen later. Do not force equipment/save state through reflection.

## Pending decisions and validation boundary

User must choose acquisition for each unavailable item, approve progression/cheat consequences
if any, establish user-managed disposable recovery, authorize normal save/neutral reload, select
any Eitr expenditure, and review the future lifecycle observer implementation before deployment.
The existing dedicated server and multiplayer require their own disposable environment/approval.
No headless bridge compatibility or network correctness is established here. Installed static
checks are27; run the ordinary build/exclusion/bridge artifact checks, existing policy/Python/
workflow suites and strict knowledge validation before review. No Stage C1 runtime success is claimed.


## Completed offline validation and diff review

Ordinary Release build passed with0warnings/errors; diagnostic type/polling exclusion and bridge
artifact audit passed. Suites:27 C1 static contracts,20 Stage B fixtures,18 identity/policy,
22 bridge fixtures,25 installed contracts, conditional-purity contract,26 Python and16 workflow
fixtures all passed. Strict validation:121 records,0errors/0warnings. Whitespace check passed.
All343 previously monitored installation fingerprints still match and installed bridge setting
remains false. No C# gameplay/plugin source or project configuration changed. The reviewed diff
is five linked documentation updates plus this new preflight and the new static contract script.
Generated IL inspection evidence stays ignored under .dev-state/stage-c1. Nothing staged,
committed, pushed, deployed or launched. Actual equipment/lifecycle observation remains unimplemented.


## C1 observer implemented offline - 2026-10-26 (supersedes earlier design-only status)

The original seven preparation files were preserved for review; their prior text above describes
that preparation checkpoint. The observer now exists in Source/PoVPlus/Diagnostics/
TomeLifecycleObserver.cs and LifecycleEvidence.cs. Production TomeBridgePolicy and
MagicPluginTomeBridge are unchanged. This is an offline implementation, not a successful C1 run.
**Not runtime-ready for the complete equipment/neutral-state matrix:** the unresolved owner and
inventory paths below deliberately prevent a complete neutral/exclusivity certification.

### Activation and isolation

Two explicit MSBuild settings are required: EnableTomeDiagnostic=true and
EnableTomeLifecycleDiagnostic=true. The lifecycle flag defaults false and is mutually exclusive
with EnableTomeStageBDiagnostic=true. Invalid combinations fail the build. In C1 mode Plugin.Awake
passes the already-bound bridge Boolean directly to the lifecycle observer; it does not create
the old one-shot TomeDiagnostic. C1 does not activate or change the bridge setting. The existing
bridge may be Off for a separately approved native baseline. No own Chainloader.Instance lookup,
new mutable singleton, new config entry or diagnostic Harmony patch is added.

Offline compile/audit example, after normal restore (no deployment implied):

```powershell
dotnet build Source/PoVPlus/PoVPlus.csproj --no-restore -c Release -p:EnableTomeDiagnostic=true -p:EnableTomeLifecycleDiagnostic=true -p:EnableTomeStageBDiagnostic=false -p:OutputPath=S:/Valheim-Modding/PoV-Plus/.dev-state/c1-observer/builds/c1/ --nologo
powershell -NoProfile -ExecutionPolicy Bypass -File Scripts/tests/Test-TomeLifecycleArtifact.ps1 -Enabled -Dll .dev-state/c1-observer/builds/c1/PoVPlus.dll
powershell -NoProfile -ExecutionPolicy Bypass -File Scripts/tests/Test-TomeLifecycle.ps1
```

Ordinary Dev.ps1 Build omits every diagnostic type and Plugin.Update. Fixture outputs, opt-in
artifacts and inspection evidence stay ignored; never deploy them without separate approval.

### Implemented sources and conditional purity evidence

| Observation | Implemented read | Evidence / safety boundary |
|---|---|---|
| Identity and initialization | Audited unpatched Player.GetPlayerName, ZNet.GetWorldName, ZNetView.IsValid/GetZDO, ZDO.GetString; player.m_isLoading; singleton getters | Existing installed contracts/identity IL; effective read interceptors refused; names revalidated before EVERY state capture |
| Main inventory | Existing Humanoid.m_inventory -> Inventory.m_inventory, list count and entries | Direct field reads only; list bound256; no GetAllItems overload, collection modification or item construction |
| Live item identity | Reference aliases, prefab name, stack, quality, equipped flag, item type, original equip SE, shared regen modifier | Actual ItemData, not ObjectDB prefab stand-ins; no setters or GUID helper; null metadata explicitly unknown |
| Existing GUID identity | m_customData.TryGetValue(DO_CustomSlotGuid), salted hash | Observer-local salt and aliases only; no raw GUID logged, no GUID created. Hashes are comparable ONLY within one observer lifetime, not across process restart |
| MS authority | Existing AppDomain owner marker, then RuntimeData weak table.TryGetValue(player) -> existing EquippedItems | Pinned TryClaimSystemOwner0x044e sets marker after its class initializer has run; absent/foreign marker refuses static reads. TryClaimSystemOwner is NOT invoked and must be unpatched. Weak-table TryGetValue metadata also checked unpatched; no creating getter |
| Tome and other MS owners | Existing dictionary.TryGetValue(tome) plus bounded map entries | Observed map null/absent tome is Empty only after an actual existing player entry is found. Missing weak-table entry is Unknown. Mapping/flag and MS-versus-vanilla reference multi-claim evidence is limited scope, not a full conflict/exclusivity claim |
| Saved Tome binding | Existing Player.m_customData.TryGetValue(DO_Equipped_tome) | In-memory evidence only; absent/empty binding versus unavailable dictionary remain distinct; no disk/save reads, setters or clear operation |
| Rendered descriptor | Existing active root/SeneaLUI_Inventory, then InventoryGuiPatches.View._links and LinkCell fields; SlotDef auto-property backing fields | Pinned InventoryView constructor creates this root through the initialized UI patch flow; positive existing-root evidence is required before reading that static View. No Source/Accepts/Visible/Put/Take callbacks or cached getters invoked. Missing root/cell => Unknown, not a hidden empty slot |
| Vanilla utility | Existing Humanoid.m_utilityItem | Direct field read; not a claim to cover all mod utility systems |
| Active effects | Existing Character.m_seman -> SEMan.m_statusEffects | Bounded128; read names/types of existing effects only, no update/application/removal calls |
| Eitr | Existing m_maxEitr, m_eitr, m_eitrRegenDelay, m_eitrRegenTimer and shared item modifiers | Direct fields, not invoking equipment/stat calculation hooks; actual regeneration rate remains Unknown |
| Bridge context | Immutable startup setting and Harmony.GetPatchInfo for both MS gates | Owners/interceptor counts are metadata evidence only, NOT proof of active bridge transaction or successful equipping |

Game, Harmony, five mod dependencies, Unity CoreModule and the actual Mono mscorlib are hash-pinned.
Unity property reads are restricted to audited name/transform/gameObject/activeInHierarchy/Find;
relevant read patches are checked before capture. No mod property getter is executed. Generic
field access refuses ALL static fields and properties; only two separately whitelisted static
reads (MS.RuntimeData and UI.View) exist at their initialization-evidence-gated call sites.
Installed Mono ConditionalWeakTable.TryGetValue has no factory/creation call; the fixture proves
a miss leaves its weak table untouched. Proof remains conditional on the actual installed flow
and runtime patch/data evidence: static checks alone are not a runtime purity certification.

### Explicit unknowns and neutral-state limits

C1 always records unknown for SeneaL custom inventory, complete PoV utility ownership, the item
returned by the rendered Source delegate, full equipment exclusivity, actual regeneration rate,
and neutral-state certification. Those paths were not fully proven non-initializing; invoking
CustomStore.Claimed, lazy store getters, PoV's creating static initializer or UI Source callbacks
merely to fill evidence is prohibited. The observer reports observed main/MS/vanilla state without
silently treating these missing authorities as empty. Parent-section Unknown also invalidates
missing child fields; it is never an implicit empty result. A partially collected section retains
its explicit failure reason. Corroboration and additional audited reads are still required before
an equipment trial can safely rely on observer-complete item accounting or rollback approval.

A UI binding identifies the rendered descriptor, not its displayed item or successful equip.
Stable-sample means two identical equipment fingerprints, not neutral state or no pending swap.
The observer cannot certify clean persistence or queue completion; actual safe cleanup still
requires the manual manifest, full known owner graph, matched effects/stats and approved neutral
save/reload. No raw save or broader player data is dumped. Session-local GUID fingerprints do
not establish cross-process persistence identity.

### Bounds and evidence retrieval

Prefix is [PoVPlus.TomeLifecycle]. ARMED records bounds and startup setting. OBSERVATION carries
schema1, sequence, UTC, initial/transition/stable-sample phase and ordinal-sorted fields. Every
field has observed/occupied/empty/unknown status, source and optional explicit reason. Known
empty is distinct from unavailable. Changes emit at most once/sec; identical payloads/phase have
only a10sec heartbeat. Collection stops permanently at120 emitted records or600sec from first
Poll (menu waiting counts), on invalid clock, identity mismatch, uncaught observation failure,
alias bound1024, or oversized payload64KiB. No automatic reset/retry/redeploy occurs; Stop is NOT
success or a rollback permit. Main inventory256, MS map64, UI links64 and effects128 are bounded.
Plan any further recording window/restart separately; do not weaken identity or bounds mid-run.

Read-only collection uses the existing collect_tome_evidence.py with the explicit profiler session
and unique output path. It now preserves the lifecycle prefix and separate lifecycle observation/
stop counts, without inflating Stage A/B snapshot_complete. Sources, session boundaries and
receipt checks are unchanged. A C1-only report does not claim a completed Stage B snapshot or
neutral lifecycle result merely because collection reached a limit.

### Offline coverage and remaining gate

Test-TomeLifecycle executes28 policy/read fixtures plus independent JSON parsing/determinism:
exact identities, every-sample revalidation, unavailable versus empty, no creating weak-table
lookup, refusal of properties/static initialization, native fixture preservation, finite budgets,
no retry, culture/escaping and state phases. These fixtures execute no Unity/mod code. The compiled
artifact audit verifies exclusive C1 startup wiring, immutable setting handoff, no compatibility
behavior calls, no foreign field stores, no mutating/creating APIs or diagnostic Harmony installs.
Existing bridge fixtures cover native/default-Off preservation; they do not prove native gameplay.

Full matrix requires offline audit/implementation of remaining owners/custom inventory or an
explicitly narrower evidence scope, reviewed C1 artifact and dependency/read patch inspection,
user-approved acquisition, recovery backups, normal saves/neutral reload and manual gameplay
permission. C1 equipment testing, effects, exclusivity, persistence and multiplayer remain unverified.
No runtime deployment, launch, game configuration/save/item/server action, commit or push occurred.


Final implementation verification (2026-10-26): ordinary, Stage A, Stage B and C1 builds completed
with0warnings/errors; C1 compiled read-only and generic diagnostic audits
passed. Invalid C1-alone and combined StageB/C1 modes were refused. Final suites passed:28 new
lifecycle fixtures plus independent JSON parsing,27 existing C1 static contracts,20 Stage B,
18 identity,22 bridge,25 installed contracts,conditional-purity check,28 Python and16 workflow.
Static audits distinguish fixture preservation from actual native gameplay. Source fingerprints
were reviewed only for the conditional startup/build/collector changes; installed snapshots and
production acceptance source were preserved. Final knowledge/whitespace/install-integrity results
are recorded in .dev-state/reports/INV-003-c1-observer-offline.json. No C1 runtime trial occurred.


## Completed independent C1 audit continuation - 2026-10-26

[[DEV-007-C1-ownership-audit]] is the canonical evidence ledger and current readiness decision.
It supersedes the initialization-proof assertions in the earlier implemented-sources table:
MS is beforefieldinit; its marker-setting method and empty Log do not force its initializer.
A named active UI root likewise does not prove the static View holder's initialization. The
interrupted audit's existing correction is independently verified: both foreign-static reads
now route through the rejecting field policy and emit section Unknown. MS map/saved-binding
and UI link code after those refusals is unreachable, not successfully observed evidence.
The earlier implementation narrative is retained as history, not a current purity claim.

Fresh checks reproduce28 lifecycle fixtures and30 separate neutral/adversarial checks; the
unknown-to-empty mutant is killed at the missing-dictionary assertion. The fixture-only neutral
rule is not wired to runtime evidence. Native calls, transitive patches, game/Harmony startup
initialization and logging subscribers remain conditional/unresolved. Full owner/custom-store/
manifest/persistent-binding coverage is missing. Classification B: blocked for equipment/neutral
C1 testing; no deployment or launch is authorized. See the ledger for exact commands, hashes,
IL tokens, complete operation classifications, validation limits and next offline action.
