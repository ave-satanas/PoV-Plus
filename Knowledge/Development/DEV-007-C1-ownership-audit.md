---
id: "DEV-007-C1-ownership-audit"
title: "C1 ownership, observation purity and neutral-state audit"
type: "development"
status: "partial"
created: "2026-10-26"
last_verified: "2026-10-26"
versions: {"Magic Supremacy": "3.1.5", "SeneaL UI": "1.2.7", "PoVPlus": "0.1.0"}
evidence: ["Scripts/tests/Export-TomeOwnershipAudit.ps1", "Scripts/tests/Test-TomeOwnershipAudit.ps1", "Scripts/tests/Test-TomeNeutral.ps1", "Scripts/tests/Test-TomeLifecycleArtifact.ps1", ".dev-state/c1-audit-continuation-20261026/metadata.json"]
related: ["[[DEV-006-Tome-C1-preflight]]", "[[DEV-005-Tome-diagnostics]]", "[[INV-003-Magic-book-tome-compatibility]]", "[[COMP-002-Tome-bridge-design]]", "[[EXP-004-Tome-equipment-lifecycle-plan]]"]
confidence: "mixed"
watch: ["src:Source/PoVPlus/Diagnostics/TomeLifecycleObserver.cs", "src:Scripts/tests/Test-TomeLifecycleArtifact.ps1", "src:Scripts/tests/Export-TomeOwnershipAudit.ps1", "src:Scripts/tests/Test-TomeOwnershipAudit.ps1", "src:Scripts/tests/Test-TomeNeutral.ps1", "src:Scripts/tests/TomeNeutralHarness.cs"]
claims: {}
contradicts: []
supersedes: []
---

# C1 ownership and neutral-state evidence ledger

## Decision and recovered state

**Classification B: blocked for the intended equipment-lifecycle/neutral-state test.** The audit
is complete as an offline review, with unresolved claims explicitly identified. It does not
establish end-to-end runtime purity or complete ownership visibility. No runtime work is authorized.

Recovered main/HEAD d29b5e560eacdc32e740e00fe0be2524fd4bf199; 13 modified tracked and10
untracked paths, nothing staged. All belong to the preserved C1 preparation/implementation/audit.
Starting bytes and inventory are in .dev-state/c1-audit-continuation-20261026/recovery.json and
starting-work/. The earlier observer completion JSON and interrupted audit logs were retained.
Existing files are LifecycleEvidence.cs and TomeLifecycleObserver.cs; no TomeLifecycleEvidence.cs
exists. The previous interrupted audit had already changed the two foreign-static reads to
refusal and added the neutral fixture. This continuation independently verifies that correction;
it does not claim to have newly implemented it. No production bridge source was changed.

## Reproduction and evidence boundaries

From the repository root, first build the opt-in C1 artifact as in DEV-006, then:

```powershell
powershell -NoProfile -ExecutionPolicy Bypass -File Scripts/tests/Export-TomeOwnershipAudit.ps1 -DiagnosticDll .dev-state/c1-audit-continuation-20261026/builds/c1/PoVPlus.dll -Output .dev-state/c1-audit-continuation-20261026/metadata.json
powershell -NoProfile -ExecutionPolicy Bypass -File Scripts/tests/Test-TomeOwnershipAudit.ps1
powershell -NoProfile -ExecutionPolicy Bypass -File Scripts/tests/Test-TomeNeutral.ps1
```

The exporter uses installed Mono.Cecil, reads target PE metadata/IL without executing target
assemblies, and rehashes each source after reading. The JSON records absolute paths, assembly
identities, MVIDs, metadata tokens, fields, beforefieldinit flags and selected readable IL.
Only inspection tooling itself is executed. Detailed third-party IL remains ignored/local.
No ILSpy/ilspycmd or C# decompiler was located in the checked PATH/tool locations; no tool was
installed. Cecil disassembly succeeded for all10 inputs; a high-level C# decompilation was not
performed and is not represented as evidence. Native/InternalCall bodies cannot be resolved
from these managed images. This is an explicit proof boundary, not a successful native audit.

All tokens below are hexadecimal metadata tokens in the named pinned image. Assembly versions
are PE identities, not necessarily plugin/package versions. Relative installation paths below
are under **S:\Valheim-Modding\PoVPlus-Dev\**; JSON contains the complete absolute paths.

| Image / PE version | Local path | SHA-256 |
|---|---|---|
| MS /3.1.5.0 | BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll | 8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787 |
| UI /0.1.0.0 | BepInEx/plugins/seneaL-SeneaL_UI/SeneaL-UI/SeneaL-UI.dll | 53B0F675BFFD0BAC5FCCA711AF04AA1EF00BB5BECD543D702EBAA146E5C487CD |
| API /1.0.0.0 | BepInEx/plugins/seneaL-SeneaL_API/SeneaL-API/SeneaL-API.dll | 2EF9EB8DD3AB7DC72E7F3A908EFF843C4C7FAD19570CB46C3565BC36841E3574 |
| PoV /1.0.0.0 | BepInEx/plugins/treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll | 83BD50F23D9C301FE469B5FBE2C430FD35C928D7A2CCFD99AF5A1040558C9D72 |
| Game /0.0.0.0 | valheim_Data/Managed/assembly_valheim.dll | 94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A |
| Unity Core /0.0.0.0 | valheim_Data/Managed/UnityEngine.CoreModule.dll | FBA3821AFB6867FE4D471DC50BF04BA21E6982B33AB6B5336B08A5C55119D990 |
| Mono mscorlib /4.0.0.0 | valheim_Data/Managed/mscorlib.dll | 5ED1180FC8CB409D57952296C7F573F2B1B3D3D2A2CD37258EF407498A17DD4D |
| HarmonyX /2.9.0.0 | BepInEx/core/0Harmony.dll | 1A21CC03424FC82C3DD1346905D16494536B9595AE4162228D99FB7C285C1031 |
| BepInEx /5.4.23.5 | BepInEx/core/BepInEx.dll | 36BDB9AF2B8F2EDA4C911D2A49C2A05A4F643637AD724B876D292A9A5FCD8AE6 |
| Corrected C1 PoVPlus /0.1.0.0 | Repository .dev-state/c1-audit-continuation-20261026/builds/c1/PoVPlus.dll | 7982D2F18C5CE1D404A3065630C8E5C7808DD154ECF100A36A74B79A95A4618E |

## Foreign initialization: confirmed defect and existing correction

**MS RuntimeData, field04000450:** CustomSlotSystem is beforefieldinit. Its TryClaimSystemOwner
(0600044e) uses AppDomain.GetData then SetData at IL0045. It has no ldsfld. Its Log(06000448)
is only ret. Thus calling this static method need not force the type initializer before the
marker becomes visible. This disproves the former marker-as-completed-initializer argument;
it does not prove that the live game's Mono actually deferred initialization in a past session.
CustomSlotSystem..cctor(06000473) allocates weak tables/lists/maps and initializes Harmony
reflection caches; IL0136-0145 creates RuntimeData/SlotDefinitions, IL014a-015e more stores.
Reflecting RuntimeData with FieldInfo.GetValue(null) could be the initializing operation.

**UI View, field04000245:** InventoryGuiPatches is also beforefieldinit; .cctor06000306 builds
reflection delegates and writes static caches. A root named SeneaLUI_Inventory is consistent
with InventoryView..ctor06000b3a but cannot prove its creator, the static holder's initialization,
or the identity/freshness of the View object. No claim of an observed spoof is made.

The preserved correction is TomeLifecycleObserver.InitializedField (source298-304;
compiled06000044): both whitelisted candidates call LifecycleRead.Field(06000019), which checks
FieldInfo.IsStatic and throws **before GetValue**. Observe catches that and records section
unknown with the refusal reason. Consequently the later MS ownership/saved-binding and UI link
reads in these methods are currently unreachable. Do not describe them as captured evidence.

Mono RuntimeFieldInfo.GetValue06004d33 delegates to native GetValueInternal06004d32 at IL0060;
its internal initialization implementation is unavailable here. The disposable .NET Framework
fixture demonstrates that the former reflective static-read pattern runs a fixture initializer;
the corrected policy refuses it without initialization. This verifies the regression on the
host CLR, not the exact runtime timing of the installed Mono. Neither foreign initializer ran.

## Essential observation ledger

Classifications: **A** demonstrably read-only within the inspected managed body; **B** conditional
on stated initialization/identity/collection/patch conditions; **C** potentially state-changing;
**D** unresolved or unavailable. Every row inherits its assembly/hash from the table above.
No A label claims arbitrary runtime detours or native implementations have been proven harmless.

| Claim / source | Access chain and primary evidence | Mutation risk, contradiction and uncertainty | Conclusion / verification |
|---|---|---|---|
| Exact authorized identity; Game Player.GetPlayerName0600023f, ZNet.GetWorldName06001197 | Poll -> initialized view/m_isLoading -> getters; LifecycleWindow.Sample checks exact strings before capture | Getter chains read ZDO strings/World.m_name; initialization and effective transitive detours remain conditions. Equal labels do not uniquely identify saves | B, conditional; installed IL,18 identity cases and per-sample fixtures. No save identity proof |
| Main inventory; Game Humanoid.m_inventory040001fc, Inventory.m_inventory0400102b | Capture -> instance reflection -> existing List count/index -> Item | No inventory setters; malformed/list-change exceptions invalidate section. Custom stores can hold other items; main list completeness is not total inventory completeness | A for direct read, B for completeness/coherence; artifact IL and28 fixtures |
| Actual item/flag identity; Game ItemData.m_equipped04002bf7, m_customData04002bf5 | Item -> actual references -> observer aliases; existing string-key GUID TryGetValue; field stack/quality/shared/prefab data | m_equipped alone is not an ownership authority. Distinct objects can share GUIDs; aliases reset after restart; prefab-name read crosses Unity native body | B; raw field evidence is supported, global uniqueness/accounting unresolved. No test manifest reconciliation is implemented |
| MS runtime ownership; MS GetEquippedItem06000459, SetEquippedItem0600045a, EquippedItems04000458 | Intended: RuntimeData -> non-creating CWT.TryGetValue(player) -> dictionary.TryGetValue(tome) | Upstream GetEquippedItem uses GetRuntimeData06000453 -> GetOrCreateValue (creating). Current observer refuses the static table read, so does not reach intended lookup | Current Unknown, safe refusal supported by compiled IL and24 audit assertions; former read path C unless independent initialization proof |
| Owner versus flag precedence; MS EquipSlotInternal0600046f / UnequipSlotInternal0600046e | Equip sets map at IL0056 then flag at005d, saved binding0076; unequip removes effect0012, clears flag0019 then map0026 and binding0037 | Map, flags and binding are separate writes. They can temporarily disagree or diverge after an exception. MS lookup uses map; other frameworks have separate ownership | B for MS-local map authority only; no global precedence. Report conflicts, never repair/choose a convenient source |
| Non-creating weak-table read; Mono CWT.TryGetValue060046d8 | LifecycleRead.ExistingEntry0600001a resolves exact BCL method then reflection Invoke | Installed method locks and reads identity-hashed ephemerons; no factory/Add path. CLR locking/hash/reflection bookkeeping is not zero total process mutation. Current MS path cannot reach it | A for absence of equipment creation in inspected body; B for actual table/runtime identity. Fixture miss never creates an entry |
| Required saved binding; MS GetSavedEquippedGuid06000464, Game Player.m_customData04000329 | Intended direct existing dictionary.TryGetValue(DO_Equipped_tome); upstream uses definition.PlayerSaveKey | Transient in-memory key is not persisted save evidence. Current reader is after refused MS static access and is not reached. Disk persistence deliberately unobserved | Unknown; cannot certify saved neutral state from UI or flags; no saves read |
| Rendered descriptor; UI View04000245 -> InventoryView._links04000ded -> LinkCell.Def04002494,Slot04002495 -> Chip.Root04002498; API backing fields0400010e/10f/112 | Rendered first reads existing InventoryGui root, then attempts whitelisted static View | Static read refused. Even an active link would establish descriptor presentation only; TickLinked06000b6c invokes Source for item display, which observer does not call. Cache/root can be stale | Unknown; no present C1 rendered-cell or rendered-item proof. Historical Stage B remains separate runtime evidence |
| SeneaL custom inventory/cache; CustomStore.Claimed06000a9b | Current capture emits inventory.seneaLCustom=unknown; no Inv/Claimed getter | Claimed writes _plainBelt via stsfld and reaches cached/delegate paths. Custom inventory coverage is absent | C for forbidden helper; A for avoiding it; total inventory state Unknown. Installed contract checks cache-write instruction |
| Relevant Belt/Utility; Game Humanoid.m_utilityItem04000204; PoV BeltSlot.Get0600028a,worn040001f8; Gear.State.Item060025b2,Bound060025b1 | Vanilla utility field is observed; PoV route deliberately unknown | BeltSlot.Get uses a separate static weak table (initializing .cctor0600028f). State.Item routes Belt to it, other utility to fields/Cells.At; State .cctor060025cf creates collections and Hidden ItemData at IL0089 | A for vanilla field read, C/B for proposed static route. No global Belt/Utility exclusivity proof; PoV authority not silently omitted |
| Required residual effects; Game SEMan.m_statusEffects040006df | Effects reads existing Character.m_seman -> bounded list; existing names/types only | No Add/Remove/Update call. Unity names opaque; no independently complete baseline/test-effect manifest. Equal empty owner/flag values cannot exclude residual effects | B for list observation, Unknown for residual-effect clearance |
| Neutral certification; observer Capture06000037 and LifecycleEvidence | Always emits neutral.certification=unknown; no decision may trigger workflow actions | Missing custom inventories, owners, manifest, persistent bindings and settled reload remain mandatory. A STOP or stable-sample is not success | Supported refusal; fixture-only proposed rule below is not integrated into live observations |

## Complete reachable-operation audit

| Code path | Class | Evidence and remaining boundary |
|---|---|---|
| Constructor, Pin, TypeFor | A/B | Reads loaded dependency metadata/files and hashes; allocates observer-owned dictionaries/lists/salt. Chainloader dictionary must be stable on main thread. Assembly.GetType/metadata lookup does not deliberately run inspected mod methods. OS reads and crypto/runtime caches have normal process effects |
| Poll: Time, Player.m_localPlayer, ZNet/Game singleton access | B/C | Time native clock; singleton getters are ldsfld/ret. Player..cctor06000393 writes collections/default fields; Game..cctor060016e0 and ZNet..cctor060011fa write globals. Reads are safe from additional initialization only if already initialized by the game. Typical load order is not proved by this observer. No new initialization gate was invented to hide this limit |
| RequirePlain / BridgePatches / PatchEvidence | B/C | Harmony.GetPatchInfo060000d9 -> PatchProcessor06000155 -> PatchManager060002c4 locks existing map -> GetValueSafe06000215 -> TryGetValue. But Harmony..cctor060000c3 calls StackTraceFixes.Install; PatchManager..cctor060002c0 allocates stores and subscribes resolvers. Prior Harmony use is an initialization assumption. Only selected top-level read methods are checked; nested/native detours and later patch changes are not exhaustively excluded |
| Capture and Observe wrappers | A/B | Each section catches exceptions and records unknown; partial child values may remain beside parent unknown and must be treated incomplete. No automatic retries/repairs. Duplicate evidence keys can escalate to bounded ObservationFailure, never empty |
| Inventory/Item/Effects/Scalar/Read | A/B/D | Existing instance field reads and bounded list/dictionary access. Null item reference means only that particular reference is empty; null inventory/store is unknown. Unity Object.name and component/UI reads reach injected native bodies; their implementations are unresolved |
| Unity equality/marshalling | A/D | Object.IsNativeObjectAlive060021ec reads GetCachedPtr; MarshalNotNull0600225c directly reads m_CachedPtr in this image (no cache write found). GetName0600223d and Transform.FindRelativeTransformWithPath06002596 reach native injected calls. Managed absence of writes does not prove native purity |
| Supremacy / Rendered | A for refusal; B/D for preliminary work | AppDomain.GetData and type/patch metadata, or native UI root lookup, precede refusal. No RuntimeData/View raw static read executes. All later map/link code is retained candidate code, not currently usable functionality |
| Alias/Digest/serialization | A | Writes only observer-owned collections; SHA/UTF8/string formatting; ordinal sorted evidence JSON. No raw GUID writes or save access. Salted GUID equality only within one observer lifetime |
| LifecycleWindow.Sample/Stop | A | Per-capture exact authorization, monotonic checks,1sec minimum cadence,120 emitted rows/600sec lifetime,64KiB payload; exception/budget permanently stops. stable-sample means structural equality only; no settling or neutral decision |
| Say/ReportStop | B/D | BepInEx ManualLogSource.LogInfo06000172 -> Log0600016d -> LogEvent.Invoke at IL0014; Logger.InternalLogEvent0600015d -> SendLogEvent06000308 -> arbitrary listener LogEvent. Expected log writes permitted; actual subscriber behavior/failures cannot be proved offline. Observer itself subscribes to no events |
| Plugin.Awake/Update integration | B | Immutable already-bound config Boolean avoids own Chainloader startup lookup. Existing ordinary bridge Start is separate; C1 observer neither enables it nor invokes acceptance predicates. Normal BepInEx Config.Bind may create/update own config at a later launch; no launch occurred. No diagnostic equipment patch, equipment callback, slot registration or ownership write found |

Class C above means a possible initializing path exists, not that this observer has been observed
to mutate live equipment. The two targeted mod-static hazards are closed by refusal. This does
not establish absence of every game/framework initializer, arbitrary logging handler or native
side effect. The compiled artifact audit detects foreign stores/direct prohibited calls; it
cannot prove all indirect operations pure. No broad observer rewrite was made during continuation.

## Falsification results and explicit neutral rule

| Challenged proposition | Result |
|---|---|
| empty means the authoritative Tome slot is empty | Conditional only for an initialized, complete MS map for the exact player; current C1 cannot obtain it |
| missing ownership mapping means empty | False for missing table/player entry; unknown. Only a verified existing authoritative dictionary can support its own missing-key result |
| false equipped flag means no ownership | False: MS map, flags and binding are separate writes and authorities |
| empty rendered UI means empty equipment | Unsupported: presentation/caches are not ownership |
| transient UI/inventory proves saved binding | False; even an observed in-memory key is not persisted evidence |
| observer cannot initialize or mutate through getters | Unproven globally; two mod static reads now refused, game/Harmony startup/native/log boundaries remain conditional |
| two agreeing sources establish actual global state | False where both derive from the same source or omit another authority |
| complete-looking main inventory includes custom inventories | False; custom inventory is explicitly unknown |

The decision rule is a **fixture-only specification**, not a live certificate: each obligation
has explicit Neutral/NonNeutral/Unknown/Conflict evidence with provenance. Identity and fresh,
complete capture must be supported first. Any contradiction produces unknown. A trustworthy
positive occupied ownership/binding/flag/residual-effect observation establishes
confirmed-non-neutral even if another noncontradictory obligation is missing. Confirmed-neutral
requires **all** obligations positively satisfied: initialized MS owner and empty Tome mapping;
required saved binding empty; no bridged flags/ownership elsewhere; complete custom stores;
unique accounted test manifest; no required residual effects; consistent current UI; and settled
neutral confirmation/reload evidence. Missing/stale/interrupted/budget-exhausted capture or any
unknown mandatory condition blocks confirmed-neutral. Never infer missing fields from absent JSON.

The fixture uses11 named obligations including identity/freshness/settledReload. Its input
labels are synthetic assertions; there is no implementation claiming to gather every obligation.
The live observer therefore stays unknown. No assessment invokes rollback. Normal save/reload
validation and user recovery decisions remain the separate boundaries in EXP-004.

## Tests, counterexamples and full validation matrix

Fresh output/logs are under .dev-state/c1-audit-continuation-20261026/. Exact commands,
exit codes, warnings and paths are recorded in validation.json. All final mandatory offline
commands exit0; the deliberate mutant executable exits1 as required. An initial strict knowledge
check reported1 source-revalidation warning, subsequently resolved by reviewing this correction
and recording only the reviewed source fingerprints; installed snapshots were not rebaselined.

| Check | Result / limit |
|---|---|
| Ordinary Dev.ps1 Build; Stage A/B/C1 dotnet builds | All4 net48 Release variants compile with0warnings/0errors; no deployment |
| Lifecycle managed fixtures + independent JSON parser |28 passed; exact identity, existing/missing weak table, empty/occupied/unknown, exceptions, deterministic phases and bounds |
| Neutral/adversarial fixture |30 checks passed; includes missing dictionary/init, conflict, stale UI, missing/duplicate manifest identity, unknown custom inventory/effects, partial/interrupted capture and repeat evaluation |
| Unknown-to-empty mutation | Compiles; fails at `missing ownership dictionary is not empty`, exit1. Normal source unchanged; mutant output isolated. This kills the proposed-rule regression, not a live Unity implementation |
| Static initialization counterexample | Matching fixture marker exists while holder initializer count0; rejected field read keeps0; old raw reflection initializes holder to1. Fixture assembly only |
| Installed C1 / new ownership assertions |27 existing contracts +24 fresh hash/IL assertions pass, including both original helper hazards and corrected compiled refusal |
| Stage B / identity / bridge / installed contracts |20 /18 /22 /25 pass; additional Stage B purity contract passes |
| Python / workflow |28 /16 pass; fixture-only workflow filesystem/process changes remain isolated |
| Artifact separation/safety | Ordinary and installed DLL exclude all diagnostics/Update. Stage A498, B660, C1 generic1155 member calls audited; C1-specific657 calls; ordinary bridge211. No foreign stores/prohibited direct equipment calls/new diagnostic event subscription |
| Invalid build modes | C1 without diagnostic and C1+StageB both fail as intended; not passing builds |
| Knowledge / whitespace | Strict validation and Git diff checks pass after documented source review; semantic completeness still requires this audit |
| Installation integrity | All343 monitored DLL/config fingerprints match baseline; original DLL/config hashes match; bridge false; development client stopped. No saves or unmonitored files scanned |
| Runtime equipment/neutral state | Not executed; no fresh C1 runtime proof, no authority completeness or gameplay certification |

The neutral harness currently emits9 CS0649 warnings for unused fields in the included shared
DiagnosticObservation policy type on each compile; no errors. These are recorded, not hidden.
Four actual plugin builds have no warnings. Previously reported28 tests describe lifecycle
fixtures only; they must not be conflated with the separate30 neutral checks or28 Python tests.

## Remaining blockers and exact next action

1. MS table/UI static holder initialization provenance: current reads always refuse. Need a
   demonstrably non-initializing source or independently captured initialized-instance evidence;
   review offline first. A marker, root name or typical load order alone is insufficient.
2. Full PoV Belt/Utility and SeneaL custom inventory ownership remains missing. Need audited
   existing-store paths and completeness/identity correlation; runtime correlation requires later
   authorization. Do not replace unknown with a convenience getter.
3. Main-thread game/Harmony initialization, transitive patches/native queries and logging
   subscriber assumptions remain conditional. Resolve the bounded startup assumptions and plan
   explicit observation evidence; no claim of all-runtime purity follows from Cecil.
4. No manifest reconciliation, trusted cross-reload identity, persistent-binding evidence or
   residual-effect clearance is currently implemented. These are necessary before a gameplay
   trial whose cleanup would disable the bridge. No disk/save access is authorized by this audit.

**Recommended next action:** review this ledger and decide a narrowly scoped offline route to
the missing ownership/initialization evidence. Keep C1 equipment testing blocked. A later,
separately authorized exploratory observation-only run could check bounded startup/unknown
reporting after manual exact-identity selection, with no item interaction and no neutral claim;
it would not substitute for the missing prerequisites or authorize this task to deploy/launch.
No additional implementation, runtime test or Git checkpoint is implicit in audit completion.


## Ownership feasibility closure - 2026-10-26

**Verdict: not demonstrably feasible with the current architecture. Recommendation C: stop
observer development and investigate an alternative verification architecture.** No observer,
bridge, build or test source changed during this feasibility task. The existing Unknown
results and full neutral predicate are retained. This closes the bounded investigation; it
neither asserts that no possible read path exists nor authorizes runtime exploration.

### Blocker disposition and minimum resolving evidence

Hashes/PE identities for MS, UI, API, PoV, Game and Mono are the pinned table above; targeted
new IL and assertions are local under .dev-state/c1-feasibility-20261026/. They match the
inspected files, not documentation-derived versions. Prior established hazards were reused.

| Required state / authority and exact chain | Disposition / uncertainty | Minimum evidence still needed |
|---|---|---|
| MS Tome owner: CustomSlotSystem.RuntimeData04000450 -> CWT player entry -> HumanoidCustomSlotData.EquippedItems04000458[tome] | Still unknown: initialization and root accessibility. Typed field/reference scan of the MS image finds RuntimeData only as a static root, read by GetRuntimeData06000453 and assigned by .cctor06000473. No typed instance field exposes per-player data. This bounded negative finding does not exclude reflection/opaque references elsewhere | Independently identifiable, already-existing table/player-data reference or trustworthy completed-initialization provenance, plus exact player entry and stable map. No such root is established by this inspection |
| SeneAL rendered descriptor: InventoryGuiPatches.View04000245 -> InventoryView._links04000ded -> LinkCell.Def/Item04002494/2496 | Conditional on future runtime verification: an instance route exists in constructor IL, but obtaining and authenticating that exact live instance is unverified. Identity, freshness and callbacks remain conditions | Existing handler/grid identity and current root/session correlation; read only retained fields, no callback invocation; distinguish cached cell from MS authority |
| SeneAL actual custom inventory: CustomStore.<Inv>k__BackingField04000d0c -> Inventory.m_inventory | Static root remains unproven; narrower instance alias is conditional. Completeness/identity cannot follow from visible contents | Already-bound current custom grid inventory, proof it belongs to this local player/current store and is not stale; enumerate its entire list and every linked provider owner without synchronizing |
| PoV Utility/Belt: Humanoid.m_utilityItem; BeltSlot.worn040001f8 -> Worn.Item; Gear.State.Item060025b2 -> Cells.At06002435 -> fallback rows or active provider | Direct vanilla field supported; complete ownership still unknown. Proposed Cells.At route is confirmed unsafe as an unconditional observation API: failure calls Cells.Fail06002434, which sets suspended. Belt/static provider roots still require initialization evidence | Existing Belt entry and active provider instance/contract/contents, provenance of initialized holders and an audited direct read that cannot run provider failure handling. Main list/flags cannot substitute |
| Game/Harmony/native/logging boundaries from prior ledger | Conditional, unchanged; not resolved by additional mod-store inspection | Bounded current runtime initialization and effective call/patch/listener evidence, or a passive observation mechanism that does not execute these paths. No runtime-purity claim from managed IL |
| Manifest, bindings, residual effects and settled persistence | Still unknown: completeness, identity and persistence. Player.m_customData[DO_Equipped_tome] is independently readable in memory but currently unreachable after the observer's MS refusal | Accounted unique live test instances, all ownership sources, required effect baseline and separately authorized neutral reload corroboration. In-memory binding absence cannot prove disk state; no save access performed |

### Evidence-supported candidates and rejected shortcuts

1. **Already-existing Player/Inventory fields: supported as non-mutating reads under stated
   conditions.** Actual Humanoid.m_inventory -> Inventory.m_inventory and m_utilityItem; existing
   Player.m_customData.TryGetValue(DO_Equipped_tome). These require a valid authorized current
   player, stable collections and no writes. They establish main-list contents, a vanilla
   reference, and an in-memory binding only. Relocating the latter read would add partial
   coverage, not fix a demonstrated defect or unlock neutral certification; no code was added.
2. **Existing SeneAL view through an instance callback: conditional.** UI InventoryView..ctor
   06000b3a IL0544-0578 adds an input handler to the existing trash control and installs bound
   pointer-enter/exit actions targeting `this`. Thus a genuine already-created handler's
   m_onPointerEnter -> delegate.m_target -> InventoryView -> _links is an instance-only managed
   field path. Callback field is UIInputHandler04000060 in assembly_guiutils0.0.0.0, absolute
   path S:\Valheim-Modding\PoVPlus-Dev\valheim_Data\Managed\assembly_guiutils.dll,
   SHA256 52846FCAE0C535D733711C13FDB8F214D303603B5DBF23D05B1ED6A8E475C9DD.
   Mono Delegate.m_target04001701 is an instance field; get_Target0600199e is ldfld/ret.
   Do not invoke the action or add a handler. Scene/component acquisition crosses Unity/native
   APIs; a replaced/multicast callback, stale view or wrong root must yield Unknown. Constructor
   retention is statically established; this is not a verified live bootstrap procedure.
3. **Existing custom grid inventory: conditional narrower read.** UI BuildCustom06000b71
   IL0012-0022 stores _cGrid04000de9 and names it SeneaLUI_CustomGrid. TickCustom06000b6b
   IL0000 obtains CustomStore.Inv, passes that same local to InventoryGrid.UpdateInventory at
   IL01e1, and retains it in _cInv04000deb at IL0277. Game UpdateInventory060009cc IL0002
   stores its argument in instance field m_inventory04000c18; GetInventory060009e5 is a direct
   instance-field getter. An independently obtained existing grid therefore exposes the inventory
   last bound to it without a CustomStore static read. Current-player/current-store identity and
   freshness are not proved by the name or by contents. Never call BuildCustom, CloneGrid,
   TickCustom or UpdateInventory for inspection: they create or change UI state.
4. **MS existing table with non-creating lookup: conditional only after independent acquisition.**
   The already-audited CWT.TryGetValue -> EquippedItems.TryGetValue path remains non-creating.
   This task found no verified instance bootstrap to that table. Calling GetEquippedItem merely
   to discover whether its GetOrCreateValue path would create an entry is not a safe substitute.
5. **CustomStore getter is not itself lazy, but is not unconditional safety evidence.** UI
   get_Inv06000a92 is exactly ldsfld/ret. Its beforefieldinit holder .cctor06000abd allocates
   stores/caches. Create06000aa6 constructs SeneaCustom Inventory (8x2), attaches m_onChanged and
   clears Last; Load06000aa8 loads items and calls Sync06000aaf. Do not invoke these helpers.
   Claimed remains cache-writing. No reflection bypass of initialization is proposed.

No candidate above is implemented or authorized for runtime use. Direct managed field reads
have a narrower proof than native scene lookup or a complete ownership certificate.

### Inventory completeness: not established

Installed data flow distinguishes physical collections from aliases and projections:

- Main physical list: Humanoid.m_inventory -> Inventory.m_inventory. PoV fallback EquipRows
  retains this same Inventory: Attach0600245b IL0001/0011; At06002459 reads coordinates in it.
  Equipment rows are not thereby a second physical collection. Pos/Owns metadata still needs
  an initialized holder before assigning ownership meaning to those coordinates.
- Separate physical collection: SeneAL CustomStore.Inv (SeneaCustom). The current observer
  omits it. Existing _cGrid/_cInv aliases could expose its last-bound contents conditionally.
- Ownership references, not inventory copies: MS EquippedItems; vanilla Utility; PoV Belt
  weak table; active external storage provider. A reference can also occur in a physical list.
- Presentation projections: UI TickLinked06000b6c IL0157-0162 calls API CustomSlots.Get060001f6;
  IL01c2 caches the returned ItemData in LinkCell.Item. CustomSlots.Read060001f8 invokes Source
  and catches failure into null at IL0026. Therefore cached null can represent a failed source,
  not proven empty ownership. Reading LinkCell.Item avoids re-execution but cannot prove freshness.
- PoV Cells.At selects fallback rows or active IStorageProvider.ItemIn060000ee; it subsequently
  checks main-inventory membership and acceptance. Its caught-exception path calls Fail, setting
  suspended at IL0009 and returning null. Calling it can both change state and hide failure as
  absence. The concrete active provider is a runtime fact; its interface does not guarantee purity.

A complete Tome-relevant manifest would require the same authorized player/session, every
physical store and owner reference, verified provider identity, no pending transition or failed
section, identity-preserving deduplication, and reconciliation of every manually tracked test
instance/count. All visible items or two matching lists do not prove those conditions. The exact
missing links are the non-initializing MS table root, current custom-store alias, initialized
Belt entry/provider state and current per-instance manifest. No claim to enumerate all world,
chest or save contents is needed or made; those are outside the controlled test scope.

### Neutral feasibility and stopping decision

A. Statically established: store/alias relationships above; direct instance reads; creating or
cache-writing helper hazards; provider failure mutation; source failure-to-null conversion.

B. Requires future corroboration: authenticated live UI/grid references, exact current player
and provider binding, coherent/fresh item instances, runtime initialization/patch assumptions
and an independently reconciled test manifest.

C. Not safely observable by the current observer: MS complete ownership/saved-binding section,
full Belt/provider/custom-inventory ownership, trusted settled/reload persistence and required
residual-effect clearance. The observer's deliberate refusal prevents treating these as empty.

There is no evidence-backed minimal correction that resolves the full neutral predicate.
A narrower `last-bound UI inventory` or `main inventory list` claim is feasible conditionally;
neither establishes rollback safety. Keep neutral.certification Unknown. **Recommend C only:**
stop extending this observer and investigate an alternative, independently verifiable passive
ownership/provenance architecture before another equipment test. That future investigation must
first demonstrate how it would expose existing roots without executing foreign initialization;
no debugger attachment, memory dump, hook, deployment or runtime exploration is performed or
implied here. The remaining proof cannot be obtained by repeatedly inspecting these same getters.

### Focused verification and integrity

17 new disposable hash/IL assertions passed (Verify.ps1 in the local evidence directory).
Existing focused suites passed:24 ownership assertions,27 C1 contracts,28 lifecycle fixtures
with independent JSON parsing,30 neutral/adversarial checks and the deliberately killed
unknown-to-empty mutant. Ordinary diagnostic exclusion and corrected C1 artifact audit passed.
No source/project/test changes require a new plugin build; prior ordinary/A/B/C1 build evidence
is reused only with matching source/artifact fingerprints. Fixtures do not validate live Unity.
Strict knowledge, whitespace, installation and final preservation results are recorded in the
local closure report. No source fingerprints or accepted installed snapshots are changed.


## Separate alternative architecture investigation

[[ARCH-003-Tome-transition-evidence]] is the subsequent design investigation requested after
this closure. It assesses naturally returned ownership roots plus mutation provenance, rather
than retrying static field reads. Its future offline PoC recommendation does not change this
observer's blocked status, neutral predicate or absence of runtime authorization. No observer
implementation was changed by the architecture investigation.


## Alternative PoC outcome

[[EXP-005-Passive-ownership-proof]] falsifies the alternative reader's concrete-collection
assumption and preserves ABA/coverage counterexamples. Verdict B concerns that separate minimal
experiment; it does not reopen this closed static-root observer or establish neutral/rollback
safety. No C1 runtime permission follows.
