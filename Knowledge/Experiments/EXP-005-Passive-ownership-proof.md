---
id: "EXP-005-Passive-ownership-proof"
title: "Offline passive ownership PoC and falsification"
type: "experiment"
status: "partial"
created: "2026-10-26"
last_verified: "2026-10-26"
versions: {"Magic Supremacy": "3.1.5", "PoVPlus": "0.1.0", "fixture CLR": ".NET Framework 4.8"}
evidence: ["Scripts/tests/Export-TomeProvenanceEvidence.ps1", "Scripts/tests/Test-TomeProvenance.ps1", "Scripts/tests/Test-TomeProvenanceArtifact.ps1", ".dev-state/arch003-poc-20261026/primary.json", ".dev-state/arch003-poc-20261026/fixtures/normal/counterexample.txt"]
related: ["[[ARCH-003-Tome-transition-evidence]]", "[[DEV-007-C1-ownership-audit]]", "[[DEV-006-Tome-C1-preflight]]", "[[EXP-004-Tome-equipment-lifecycle-plan]]", "[[INV-003-Magic-book-tome-compatibility]]", "[[COMP-002-Tome-bridge-design]]"]
confidence: "mixed"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Passive ownership experiment: falsification checkpoint

## Verdict and stop

**B — Offline PoC exposes a specific correctable design defect.** The experimental reader checks
field declarations but not the concrete runtime collection class. A disposable derived Dictionary
reimplements IDictionary.GetEnumerator; observation executes it twice and still marks the snapshot
complete. The pinned upstream constructor creates a plain Dictionary, so this is an invalid-object
counterexample, not a claim that the installed game currently supplies that subclass. It disproves
unconditional non-interference for invalid/replaced objects. Do not wire this reader into a runtime
observer. Exact runtime collection validation is the smallest necessary correction, not sufficient
proof of overall architecture viability. Stop implementation here; preserve the failed experiment.

Two additional limits are reproduced: equal endpoint copies miss an intervening ownership/flag/
binding ABA roundtrip; caller-attested coverage can promote an unobserved roundtrip into a
CorroboratedNoOp history. Neither a Boolean attestation nor contiguous local sequence numbers
establishes actual quiescence or complete hook coverage. These are falsified proof assumptions,
not repaired runtime guarantees. No runtime test, neutral-state or rollback permission follows.

## Primary installed evidence

Root: S:/Valheim-Modding/PoVPlus-Dev. Magic_Supremacy.dll at
BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll has SHA-256
8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787,
PE identity Magic_Supremacy3.1.5.0, MVID ad51107e-c7b5-4d1e-a9f6-03711dacdf13.
Fresh Cecil reads, never reconstructed C#, establish:

| Entity / token | Exact evidence and consequence |
|---|---|
| GetRuntimeData06000453 | private static HumanoidCustomSlotData GetRuntimeData(Humanoid); IL0000 ldsfld RuntimeData,0005 ldarg.0,0006 CWT.GetOrCreateValue,000b ret; no handlers or method annotations. A normal return can supply the actual object; invoking it for observation can create state |
| RuntimeData04000450 | static initonly CWT<Humanoid,CustomSlotSystem/HumanoidCustomSlotData>; beforefieldinit holder .cctor06000473 builds this and other caches. Do not read it for bootstrap |
| HumanoidCustomSlotData020000fc | nested public sealed beforefieldinit instance type; constructor06000477 IL0001 new Dictionary<string,ItemData>,0006 stfld EquippedItems; no separate ownership getter needed |
| EquippedItems04000458 | public initonly instance Dictionary<string,ItemDrop/ItemData>; readonly reference does not make contents immutable |
| ItemData fields | game m_equipped04002bf7 and m_customData04002bf5; Player.m_customData04000329; inspected by existing pinned ownership exporter. Existing DO_CustomSlotGuid and DO_Equipped_tome are in-memory binding evidence, not durable persistence |
| GetRuntimeData callers | GetEquippedItem06000459 IL0014; SetEquippedItem0600045a IL0013; IsItemEquippedInCustomSlot0600045b IL000f; Humanoid_UnequipItem_CustomSlots.Prefix0600047c IL001e |
| Map mutations | setter0600045a Remove at0023 / set_Item0032; constructor field assignment06000477. Bounded full-MS member-reference scan found these; it cannot exclude reflection/external writers |
| Enclosing paths | UnequipSlotInternal0600046e setter0026; EquipSlotInternal0600046f setter0056 after swap removal0049; ValidateSingleSlot06000472 setter0090 and00da without ordinary equip helper |
| Partial transactions | equip map->flag->GUID/binding->effect->refresh; unequip effect->flag->map->binding. Restore has direct map/flag/effect paths; exceptions can interrupt between writes. No MS equip finalizer found in declared metadata |

Export-TomeProvenanceEvidence.ps1 reproduces29 selected bodies and14 member references with tokens,
attributes, handlers, annotations and IL, rehashing the image afterward. Fresh root-il.txt and
root-callers.txt were captured before implementation. Fresh ownership-metadata.json covers10
pinned sources and passes24 independent assertions. ARCH-003 owns the larger transition topology.
The previous declared-patches.json covers the inspected MS/UI/API/PoV annotations; none targets
GetRuntimeData there. This is **not** a scan proving absence of all dynamic/other-plugin patches.
Actual Harmony owners, original-call execution, final return replacement, inlining and Mono timing
remain unverified. No target or plugin assembly was executed in a fixture.

## Small implementation and separation

PassiveOwnershipProof.cs is guarded by POVPLUS_TOME_PROVENANCE_POC. The existing csproj already
excludes Diagnostics in ordinary builds; no project/startup/bridge source was changed. An extra
explicit DefineConstants opt-in compiles the seven PoC types. No installer, Harmony attributes,
TargetMethod resolver, event subscription or activation path exists. ObserveReturn is an independent
object-valued callback simulation, not an installed or Harmony-tested postfix. A future integration
would need separately verified original argument/result provenance; its supplied Boolean is not
such verification. We deliberately did not add a Harmony fixture once the reader proof failed.
Thus ARCH-003's proposed host-Harmony discovery/initialization/inlining experiment remains unrun.

ReturnedRoot holds only weak owner/root references. Repeated capture cannot renew its60sec lease;
owner/root replacement, lost root, invalid clock or disposal invalidates the witness. Deferred
Borrow temporarily strengthens a reference until the bounded reader returns. There is no timer
or polling; callers must enforce the window. Weak handles/metadata can outlive targets, but do not
root them. Alias tracking uses at most64 weak references and never recycles IDs. A fixture forces
collection of a captured otherwise-unreferenced object. This proves the host fixture behavior,
not live Unity lifetime/timing. Type/FieldInfo references retain metadata, not player/data objects.

Reader inputs are exact instance types and audited FieldInfos; it refuses statics/properties,
creating getters, missing fields, null roots/maps/entries, missing binding dictionaries and unknown
initialization/quiescence. Reads use existing map, tracked-item flag/GUID and existing player binding.
Two bounded copies compare counts, keys, value reference identities, flags and binding values.
Exceptions/visible deltas reject the snapshot. A missing Tome key is recorded as map absence,
never a global empty-ownership or safe-removal certificate. The derived-collection and ABA failures
above show why the Complete flag is **conditional and experimentally inadequate**, not a purity
certificate. Do not infer stable enumeration from absence of exceptions.

Snapshot and Transition are readonly evidence containers, retaining scalars/local aliases rather
than items. Records carry source, caller-supplied sequence, phase, transaction, kind, item alias,
state and provenance. Classification/Reason express conditional confidence. The ledger retains
only current state and a pending immutable transition, processes at most32 records, and refuses
nested/swap intermediates rather than modeling an entire equipment engine. It is not an archive
or a trustworthy event source. No global ordering, timestamp synchronization or complete-writer
coverage is invented. Supplied transaction/provenance strings are trusted fixture labels, not
hardened byte-bounded transport. Source/epoch authentication and health-to-ledger wiring remain
unimplemented. No runtime-safe or untrusted-input claim is made for this failed experiment.

Unknown baseline/coverage cannot heal through later apparent success. Fresh complete observation
may support a new scoped epoch but not recover lost history; an existing ledger refuses rebasing.
Returned-root capture is not independent initialization proof. CorroboratedEquip/Unequip/Restore
labels exist only in fixtures with an independently truthful initial/coverage oracle. They do not
mean effects completed, all owners reconciled, persistence established or rollback safe.

## Falsification and adversarial results

All36 normal policy/reader fixtures pass. This establishes their narrow assertions, not all three
mission propositions. --falsify deliberately exits2 and emits the three counterexamples above.
The validation runner requires that outcome and labels the failed proof explicitly. Two disposable
source mutants compile, then exit1 at independent assertions: assumed-initial is killed at
`root became baseline`; assumed-coverage at `coverage invented`. Normal source is unchanged.

| Cases | Result / scope |
|---|---|
| Valid/missing/duplicate/populated root | Existing game-simulated return preserved; no added getter calls; duplicate lease not renewed; late root does not grant initial state |
| Null/uninitialized/invalid/incomplete | Missing root/owner/item/maps/bindings, null map entries and absent readiness yield Unknown; malicious derived collection exposes the defect |
| Equip/unequip attempts and return-only no-op | Remain attempted or incomplete without independently matching map/flag/binding; tested successful scoped equip/removal separately |
| Swap and exception | Partial swap preserves exact exception object and independent driver's empty map; nested intermediate is rejected, not guessed atomic |
| Restore and missed mutation | Explicit paired Restore distinguishable; unexpected owner delta at next Begin invalidates history; invisible ABA remains undetectable without truthful coverage |
| Duplicate/out-of-order/missing events | Fail closed; repeated sequence numbers cannot silently apply twice |
| Conflicting item identity / map/flag / UI agreement | Reject completion despite a true flag and matching UI; UI is not counted as a second ownership authority |
| Changed/interrupted collection | Visible delta or interrupted read rejected without repairing state; two-pass ABA still falsely satisfies conditional reader |
| Bounds |60sec weak-root lease,64 entries/aliases,32 processed records; exhausted/interrupted evidence stays incomplete |
| Unknown initial / missing coverage | Later matching successes stay UnknownHistory; assumed-empty and assumed-coverage mutants killed |
| Binding/persistence | Missing/mismatched in-memory binding rejects equip; disk/save/load/cross-session persistence remains entirely unproven |

Fixture driver directly owns truth, controls actual maps/flags/bindings and injects faults; the
ledger's reconstructed state is not the oracle. Fields/counts/references before and after normal
reads agree. Cold fixture metadata/reader construction leaves its holder initializer count0;
only the driver calls its creating getter. No real game, MS, UI or bridge code executes in fixtures.

## Reproduction and compiled boundaries

Run from the repo with PowerShell7 / installed net48 SDK tooling:

```powershell
.\Scripts\tests\Export-TomeProvenanceEvidence.ps1
.\Scripts\tests\Test-TomeProvenance.ps1
# The runner expects --falsify exit2 and both mutant exit1 results; these are proof failures.
dotnet build Source/PoVPlus/PoVPlus.csproj --no-restore -c Release -p:EnableTomeDiagnostic=true '-p:DefineConstants=POVPLUS_TOME_DIAGNOSTIC%3BPOVPLUS_TOME_PROVENANCE_POC' -p:OutputPath=S:/Valheim-Modding/PoV-Plus/.dev-state/arch003-poc-20261026/builds/poc/ --nologo
.\Scripts\tests\Test-TomeProvenanceArtifact.ps1 -Enabled -Dll .dev-state/arch003-poc-20261026/builds/poc/PoVPlus.dll
.\Scripts\Dev.ps1 Build
.\Scripts\tests\Test-TomeProvenanceArtifact.ps1 -Dll Source/PoVPlus/bin/Release/net48/PoVPlus.dll
```

PoC artifact:7 types /163 member calls; no fixture hook, static root read, foreign field store,
creating getter, reflection Invoke/SetValue, patch installer or caller outside the experiment.
Foreign reflection GetValue exists only in Reader.Read; source policy is reviewed separately.
The derived collection counterexample demonstrates why this green static audit is insufficient.
Ordinary and installed artifacts contain no PoC types. Ordinary/A/B/C1/PoC builds compile without
warnings/errors; generic audits A498/B660/C11155 calls, C1-specific657, ordinary bridge211 pass.
Invalid C1-alone and combined B/C1 builds fail as intended. No gameplay acceptance source changed.

Existing suites:18 identity,20 Stage B,22 bridge,25 installed contracts,27 C1 contracts, Stage B
purity contract,28 lifecycle plus independent deterministic JSON,30 neutral checks plus its killed
unknown-to-empty mutant,24 fresh ownership assertions,28 Python and16 workflow fixtures pass.
Independent PowerShell JSON parsing of the new actual ledger export checks UnknownHistory /
historyComplete=false; synthetic output is explicitly not runtime evidence.

Warnings/failures are preserved, not hidden: neutral harness emits9 existing CS0649 warnings per
compile (normal and mutant builds); initial Windows PowerShell5 invocations of four suites failed
because Get-FileHash was unavailable in that host's module environment. The same unchanged tests
pass under PowerShell7. The first new artifact assertion incorrectly expected9 types; inspecting
the actual seven named types corrected the test inventory, not the implementation. An installed
original-DLL bridge audit was inapplicable and failed because that pre-bridge binary lacks bridge
classes; current ordinary bridge audit passes and installed no-diagnostic audits pass. Initial
runner native mutant exit leakage was corrected by explicitly returning0 only after all expected
exit codes are checked. Architecture counterexamples remain failures of proof, not suppressed tests.
An initial knowledge validation used unsupported status `blocked`, producing one schema error
and four dependent link errors; the new record now uses the existing `partial` status, retaining
the blocked verdict in its body. No validator or accepted source/installation snapshot was
changed to obtain green checks.

## Remaining proof obligations and next action

A: passive object receipt works in simulation; actual Harmony target interception and unmodified
original-return/owner provenance remain unproved. B: normal existing-object reads preserve tested
state, but arbitrary runtime collection dispatch defeats the reader's type assumption. C: the
ledger rejects declared/observable gaps and truthful unknowns; it cannot discover an invisible
unreported roundtrip under a falsely supplied coverage assertion. Full proposition conjunction
is not satisfied. Verdict B applies to this experiment only; C1 observer closure stays intact.

Before reconsidering runtime feasibility, a separate offline review must define exact concrete
collection validation and a non-creating, independently justified coherence/coverage/initial-state
protocol. Do not merely set the attestation Booleans true, count matching endpoints or add more
patches. Known MS mutation routes are mapped; external/reflection writes, reentrant scheduling,
patch replacements and other owners remain outside the proven coverage. Authenticating a root,
late capture and settled baseline still require evidence. No complete Eitr/multiplayer work is added.

This task stopped at falsification. Do not request/perform a runtime experiment on this prototype.
Neutral certification, safe removal and rollback safety remain Unknown. No launch, deployment,
config/save/item action, server interaction, ZIP, commit or push occurred. Full starting/final Git
inventories, preservation verification and343 targeted installation fingerprints are in the local
completion report under .dev-state/arch003-poc-20261026; no save/unmonitored-file audit is implied.


## Correction continuation — 2026-10-26 (supersedes the implementation verdict above)

**Verdict A — corrected experimental PoC, limited to local observations.** The original three
failures were reproduced before editing. They remain reproducible from the frozen source/harness
under Scripts/tests/Fixtures/EXP005Original, whose manifest pins their exact starting bytes.
Original36 fixtures pass, and original --falsify exits2 with all three failures. Earlier verdict B
and evidence above remain historical, not rewritten as successes. No production integration,
Harmony installation, runtime authorization or neutral/rollback certification follows from A.

The corrected mechanism is useful for typed, bounded, non-mutating local samples and explicit
comparisons. It cannot authenticate a game return, establish atomic capture, complete transition
coverage, prove initial authoritative state, or authorize removal/rollback. Further offline work
merely to turn these unknowns into positives would be unjustified. The next action is a design
review deciding whether that limited observation contract is useful for a separately scoped
feasibility investigation; it is not a request to run the game or reopen C1 equipment testing.

### Original failure reproduction and exact corrections

| Original failure / cause | Reproduction and correction |
|---|---|
| Declared Dictionary type trusted; interface dispatch invoked derived enumerator twice | Frozen original reproduces complete=True with foreign enumeration2. Corrected Copy accepts object+expected Type, compares exact runtime type before any Count/enumeration, then permits only the known BCL interface implementation. The same restriction covers ownership, item metadata and player bindings; derived Count/enumerator counters both remain0 |
| Two copies agree after an ownership/flag/binding ABA roundtrip | Driver still performs two writes and restores endpoints. Corrected result reports only LocalReadValid/EndpointsAgree/SampledValuesAgree; Atomicity, Continuity and AuthoritativeState stay unknown. Complete/Agrees were removed to avoid their former overstatement |
| Caller coverage flag and contiguous sequence upgraded history | _initial/_coverage promotion removed. Baseline's two claims are retained only in an explicitly unverified reason. HistoryComplete always false; coverage/initial/neutral/rollback getters are literal unknown. Valid returns yield Equip/Unequip/RestoreEndpointAgreement, never completed operations or complete history. Equal retained values yield NoChangeInRecordedProjection |

Snapshot consistency has two separate meanings: EndpointsAgree is equality of the bounded copies
and flag reads **within one collection attempt**; SampledValuesAgree is the relationship between
the sampled exact Tome entry, tracked reference/flag and in-memory binding. Neither means the
whole graph existed atomically at one instant. Across records, only the retained scalar projection
is compared; other map values and raw GUIDs are not retained. NoChangeInRecordedProjection does
not assert unchanged raw bindings, unchanged other owners, or absence of intermediate mutations.
A missing exact `tome` key is an observed key absence, not verified empty equipment.

Two additional defects were independently exposed and corrected within the same component:

- Independent readers assign equal numeric aliases to distinct items. A regression first failed
  with `unrelated readers with alias 1 were correlated`. Snapshots now carry an observer-owned
  scope marker; reference equality must hold before publishing a ledger record. The reader binds
  one root/owner using weak references and refuses replacement/loss. This marker distinguishes
  recorder identity only: **it supplies no coverage, initialization or ownership authority**.
- Public Count does not bound enumeration of a heavily deleted dictionary. A96-insertion/95-removal
  fixture failed despite public Count1. Copy now checks the supported BCL instance storage-count
  field before/after traversal, caps it at64, caps keys at128 and string values at4096. Source
  comparers are never called. These are structural work/input budgets in the stable fixture
  model, not real-time deadlines, thread-safety, or a new mutation-monitoring subsystem.

The ledger retains at most32 immutable input records and returns a cloned array. Transaction and
provenance labels are capped at96/160 characters. Its count/order checks reject observable gaps,
duplicates, wrong source, conflicting/missing aliases, invalid enums and interruptions; they do
not detect an event that never reached it. Existing64 weak-alias and60sec root-lease limits remain.
Snapshot scope markers have no back-reference to a reader/player/data object. GC fixtures retain
the snapshot while demonstrating that root, owner and tracked item remain collectable. Temporary
copies and borrowed references can keep objects alive during a read; a lease cannot revoke a
strong reference retained by a caller. No live thread/scheduling/lifetime guarantee is asserted.

### Primary evidence and compiled trace

Fresh evidence is under .dev-state/exp005-correction-20261026, not a refreshed accepted snapshot.
MS SHA256 remains8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787;
MVID ad51107e-c7b5-4d1e-a9f6-03711dacdf13. Export-TomeProvenanceEvidence.ps1 rehashes sources
before/after metadata inspection and now includes complete selected BCL bodies and game fields/
constructors. No Valheim/plugin assembly is executed in fixtures.

- MS GetRuntimeData06000453 still returns HumanoidCustomSlotData through GetOrCreateValue;
  observation never invokes it. EquippedItems04000458 is public initonly instance
  Dictionary<string,ItemData>. Whole-image typed field-reference inspection finds its only
  field assignment in constructor06000477 IL0001(new dictionary)/0006(stfld). Setter0600045a
  mutates entries; restore06000472 also calls it outside ordinary equip helpers. This does not
  exclude external reflection writers or runtime patches.
- Game SHA94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A:
  Player.m_customData04000329 and ItemData.m_customData04002bf5 are Dictionary<string,string>;
  ItemData.m_equipped04002bf7 is Boolean. Player constructor06000392 IL03d0/03d5 and ItemData
  constructor0600289c IL0030/0035 allocate/assign plain string dictionaries. No constructor is
  invoked for observation; a real object's construction history is not proved by its shape.
- Mono mscorlib SHA5ED1180FC8CB409D57952296C7F573F2B1B3D3D2A2CD37258EF407498A17DD4D:
  Dictionary.get_Count060060ac reads _count-_freeCount. IDictionary.GetEnumerator060060dc
  constructs/boxes its own Enumerator. MoveNext060060df compares stored version, scans to
  dictionary._count, reads entry fields and updates the observer-created enumerator, not the
  ownership map. get_Entry060060e4 reads the current pair. No source comparer callback occurs
  on these managed enumeration paths. The host CLR fixture's corresponding storage field is
  named count; installed Mono uses _count. Only these explicit instance Int32 layouts are
  accepted; unknown layout is rejected. No version tracking was added to the observer.
- Frozen compiled Copy accepted IDictionary: ICollection.get_Count preceded any runtime-type
  validation and IDictionary.GetEnumerator dispatched to the derived fixture. Corrected Copy
  validates object.GetType against the expected closed Dictionary type before interface reads.
  compiled-corrected.json preserves the exact current method tokens, offsets, bodies, references,
  MVID and SHA; compiled-original.json preserves the earlier dispatch. Tokens are artifact-local.

The compiled audit checks control-flow dominance, six Copy calls using validated metadata field
types, no alternate interface enumeration path, fixed unsupported-claim getters, scope equality
before record publication, and absence of fixture hooks/activation/foreign stores/creating calls.
Roslyn lowers ReferenceEquals to a CLI beq here; the audit follows that actual reference-equality
branch, not a nonexistent method call. Copy's non-interface enumerations inspect observer-owned
copies. BCL-private storage reads are separately constrained to the explicit instance-field shape.
Raw field reflection remains conditional on trusted exact metadata descriptors and an authenticated
future binding protocol. This is not a sandbox for forged FieldInfo objects or hostile runtime
patches. Framework metadata/cache allocation and native execution timing are not zero-process-
mutation claims. Source comparer semantics are not authenticated: observations use literal exact
keys and never invoke the source comparer to infer equipment behavior.

### Five evidence boundaries

| Claim | Corrected result |
|---|---|
| A: local observation validity | LocalReadValid means supported exact types, bounds, completed field/copy reads and no detected mismatch; origin and atomicity remain unknown. It is not an authenticated live-game snapshot certificate |
| B: sampled consistency | EndpointsAgree and SampledValuesAgree retain their narrow meanings above; contradictory flags/bindings are preserved as disagreement/incomplete evidence |
| C: transition coverage | Always unknown. Numbers describe received records only. Caller claims cannot establish missing-writer completeness |
| D: initial state | Always unknown. Baseline is a local comparison point, not proof of initialization, original emptiness or authoritative startup state |
| E: neutral/rollback | Always unknown. No inventories/all-owner graph, required effects, durable bindings, persistence/reload or cleanup preconditions are established |

Readiness/original-return Booleans are negative admission gates and untrusted reports only; true
cannot authenticate anything. ReturnedRoot.OriginValidity and Snapshot.OriginValidity remain
unknown. Snapshot.Atomicity and Continuity remain unknown even when no instability is detected.
Null maps, unavailable metadata/readiness, unsupported concrete/BCL layout, count/string limits,
visible mutation, failed enumeration or interrupted collection return unknown without repair.
A structurally default-looking object cannot reveal whether a constructor actually ran; no
initialization conclusion is extracted from matching field values or absent keys.

### Independent regression and mutation evidence

Original36 cases and all three failures are rebuilt from the frozen files on every normal run;
manifest hashes must match. Corrected53 cases retain the original substantive tests and add:
all three derived collection positions (Count/enumerator0), mutation inside enumeration,
ABA with explicit unknown temporal claims, all four false/true initial/coverage combinations,
missing rollback obligations, identity failures, conflicting baseline/count deltas, bounded
immutable record retention, reader-scope collision, root/owner replacement, GC retention,
sparse traversal/string budgets and a source comparer that must never execute. Pre-existing
source enumerators remain valid after repeated reads, detecting accidental transient dictionary
mutation independently of endpoint contents. Fixture driver truth is independent of the ledger.

Seven disposable regressions compile successfully, then fail the corrected fixtures with exit1:

| Mutation | Independent rejection |
|---|---|
| Remove exact concrete-type guard | derived ownership Count/enumerator executes; guard-missing IL rejection |
| Promote endpoint equality to continuity | ABA regression fails; non-literal-unknown getter rejected |
| Trust caller coverage | unknown-coverage assertions fail; claim-promotion IL rejection |
| Trust caller initial-state claim | unknown-initial assertions fail; claim-promotion IL rejection |
| Remove reader-scope equality | distinct equal-numbered aliases correlate; missing scope comparison rejected |
| Remove storage traversal cap | sparse dictionary bypasses public Count budget; missing traversal bound rejected |
| Remove string-size caps | oversized key accepted; missing compiled string bounds rejected |

Each mutant is also compiled as a separate non-fixture library: the independent IL audit must
reject it for its intended reason. The actual corrected diagnostic artifact passes the same
audit. No production source is modified during mutation tests. PowerShell and Python parse the
actual schema2 fixture export, including local validity, sample agreement, two retained sequences,
unknown origin/atomicity/authority/coverage/initial/neutral/rollback and false HistoryComplete.
The JSON is a fixture demonstration, not a cross-reader/process replay or provenance protocol.

### Reassessment: ten questions

| Question | Evidence-backed answer and limit |
|---|---|
| 1 Unsupported implementations rejected without invoking them? | Yes within the fixed managed model: exact-type branch dominates all source interface reads; derived Count/enumerator counters0; original and guard-removal mutant demonstrate the contrast. Live detours remain unverified |
| 2 Useful non-mutating local snapshots? | Yes for supported bounded existing objects in fixtures: independent contents, keys, references, flags, binding values, counts and pre-existing enumerators preserved. BCL managed path inspected. No atomic/native/runtime purity guarantee |
| 3 Unsupported continuity refused? | Yes: unknown getter, ABA regression and killed continuity mutant. Equal retained projections are not continuous history |
| 4 Unauthenticated coverage rejected? | Yes: claims remain textual/unverified, unknown getter, all Boolean combinations and killed promotion mutant. Reader scope is alias provenance only, never writer coverage |
| 5 Independent initial ownership established? | No: populated late capture and apparently empty samples cannot prove start, settling or other owners; initialization stays unknown |
| 6 Complete mutation coverage? | No: MS setter/restore paths known, but unobserved ABA, reflection, external writers, dynamic patches and skipped/inlined hooks are not closed by metadata or numbering |
| 7 Authoritative neutral state? | No: incomplete inventories/owners/effects/bindings/manifest/persistence; existing DEV-007 gate remains blocked |
| 8 Rollback safety? | No: in-memory absence and an unequip endpoint are insufficient; original cleanup/neutral-reload conditions unchanged |
| 9 Requires future runtime evidence? | Original return/argument provenance, current owner/session binding, actual Harmony chain/inlining, Mono native reflection/lifetime/scheduling, real collection layout/input sizes, read-only non-interference and any gameplay/persistence claims |
| 10 Not reasonably resolved by further offline code here? | Actual running identities/state/order, unknown external writes, full source completeness and durable save/cloud effects cannot follow from static files or more fixture assertions. Controlled independent evidence/verification strategy and separate permissions would be necessary |

### Validation, warnings and preservation

The final completion report contains exact commands, exit codes, hashes, complete Git status and
all warnings. Ordinary/A/B/C1/PoC builds, exclusion/activation audits, existing identity/bridge/
contracts/lifecycle/neutral/Python/workflow suites, fresh ownership export, strict knowledge and
whitespace checks were run. No runtime integration, production acceptance or existing C1 source
was changed. The PoC symbol alone cannot include Diagnostics without the separate diagnostic
build mode; ordinary/A/B/C1/installed outputs exclude the PoC. The production bridge's two-gate,
default-Off acceptance remains unchanged; the installed original predates that bridge.

Expected failures are original --falsify exit2, seven fixture mutants and seven IL audit mutants
exit1, and existing invalid C1 build modes. Newly discovered alias and budget cases were first
recorded failing before correction. An early audit incorrectly expected a ReferenceEquals call;
actual compiled beq proved why that assumption was wrong, and the control-flow audit was corrected
before accepting any mutant results. Initial summary log collection looked in the wrong output
root; logs were collected under the current directory and the runner corrected. Prior frozen
failures/primary reports remain retained. Existing neutral fixtures still emit9 CS0649 warnings
per normal/mutant compilation; no warning suppression or accepted-snapshot refresh was used.
No SDK/plugin software was installed. All detailed outputs remain ignored/local.

Full before-bytes are in .dev-state/exp005-correction-20261026/starting-work; frozen original
implementation/harness and manifest are versionable fixtures, not production compilation inputs.
All prior unrelated working-tree bytes remain intact. No game launch/deployment/configuration/
save/item/server action, runtime patch installation, ZIP, staging, commit or push occurred.
