# Cross-review A: independent review of Claude's Stage C1 findings

Reviewer: OpenAI Codex. Date: 2026-10-27. **Offline only; no fixes integrated.**

## 1. Immutable scope and overall verdict

| Object | Exact commit / parent |
|---|---|
| Common Stage C1 checkpoint | `0c1f3d1a909e0c40a7bcf36425f8efc6a1c09722` |
| Claude review source | `d84903d8f383d8dac24c635fc9fa675b5e87264b`, direct parent common checkpoint |
| Codex DEV-008 source/current HEAD | `27ba868f7bcc82d6f5bbef0c6b6acf23a6deac6f`, direct parent common checkpoint |

Primary worktree is S:/Valheim-Modding/PoV-Plus, branch
`research/dev-008-supervised-testing`; initially clean. Both commits were accessible through
local Git objects. Nothing was read from Claude's current worktree or scratch state. Claude's
17-file committed package contains the report, seven reproduction/source files, eight result
files and directory .gitattributes. `reproduce.py` materializes these immutable blobs and the
common-baseline PoC/harness/auditor into **this** worktree's ignored `.dev-state/crossreview-a`.
It does not create/switch Git worktrees or alter any branch/index. DEV-008 is reviewed at its
own immutable commit, not assumed to exist in Claude's branch. AGENTS and Knowledge/INDEX read.

**Overall:** Claude's central test-sensitivity and audit-coverage findings survive independent
reproduction. There are real reasons to improve verification infrastructure. They do not
demonstrate a deployed equipment defect, prove that the unchanged reader mutates valid source
state, or reopen C1. Several interpretations and proposed remedies need qualification. In
particular, five additional distinctions challenge the remaining-survivor discussion, monotonic
read stamps do not establish currentness, the cited NPC path has a player guard, and Swap/count
generalizations are too broad. No conclusion here defends an earlier implementation merely
because Codex authored it.

**Stage B acceptance is verified. Stage C1 ownership completeness, equipment lifecycle,
persistence, neutral-state certification, and rollback safety remain unverified.**

## 2. Reproduction environment and safety

Windows 11 Pro host; desktop .NET SDK10.0.100, net48 targeting/runtime, Python3.14.0; Windows
PowerShell5.1. Existing cached PowerShell7.6.5 is executable at
`C:/Users/<owner>/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/powershell/pwsh.exe`.
Its directory is not on the default shell PATH. No software installed or network research needed.

Fixture projects match the declared net48, C#7.3, Release and
`POVPLUS_TOME_PROVENANCE_POC;POVPLUS_PROVENANCE_FIXTURE` options. Audit libraries omit the fixture
symbol and contain only the PoC; no target game/plugin assemblies execute. Dotnet restore uses
an existing local directory as its sole source. Generated executables/libraries, copied source,
logs and metadata stay ignored. The two ordinary-plugin identity builds reference installed
assemblies read-only, with Private=false; they are compiled, never loaded or deployed.

Every supplied script was inspected before use. Instead of running Claude's Bash wrapper in
either live worktree, a new Python runner uses the exact committed generators, source and
fixtures, records every restore/build/run result, and independently checks counts and exits.
Only metadata inspection/auditing loads installed Mono.Cecil as tooling. Installed game/mod
images are read as PE metadata, never executed. Tests use synthetic items and fixture directories.
The lock demonstration uses only fresh synthetic lock paths and child PowerShell processes.
No operational deploy/launch functions, desktop input, save paths or server operations are invoked.

Reproduce from the primary worktree, in order:

```powershell
python -B Reviews/Codex-2026-10-27-CrossReview/reproduce.py
python -B Reviews/Codex-2026-10-27-CrossReview/verify_methodology.py
python -B Reviews/Codex-2026-10-27-CrossReview/independent.py
powershell -NoProfile -File Reviews/Codex-2026-10-27-CrossReview/Audit-Libraries.ps1
powershell -NoProfile -File Reviews/Codex-2026-10-27-CrossReview/Inspect-Installed.ps1
powershell -NoProfile -File Reviews/Codex-2026-10-27-CrossReview/Test-Locks.ps1
python -B Reviews/Codex-2026-10-27-CrossReview/build_identity.py
powershell -NoProfile -File Reviews/Codex-2026-10-27-CrossReview/Inspect-BuildIdentity.ps1
python -B -m unittest discover -s Scripts/tests -v
python -B Scripts/knowledge.py validate --strict
```

The PS5.1 failure test executes a byte-exact Git extraction of baseline
`Scripts/tests/Test-TomeProvenance.ps1` in the private mirror, not its primary/default location.
It derives all output paths from that mirror. The full Bash wrapper was inspected, not executed;
equivalence of these fixture/build inputs is checked, not claimed for every Bash implementation.

## 3. Finding-by-finding disposition

Verdicts concern Claude's full definitions. "Verified" does not imply a runtime failure.

| ID | Claude's claim | Verdict | Independent evidence | Limits and recommended disposition |
|---|---|---|---|---|
| F1 | 55/77 guard edits survive53 fixtures;36 additional cases discriminate;19 argued redundant/equivalent | **Partially Verified** | Exact77 unique declared edits verified;22 killed/55 survive; corrected36-case suite passes;36 survivors each fail exactly their labeled case. Five more distinctions reproduced below | Counts and masking diagnosis verified. These are77 edit groups, some compound, not77 independent atomic predicates or a coverage percentage. Do not label the remaining19 equivalent. Add reason-sensitive tests/positive controls and discriminators, or freeze the experiment |
| F2 | Latent source-writing mutants pass fixtures and IL audit; audit is a denylist | **Verified** (audit gap) | Both committed payloads compile, pass53 and audit. Two independently reachable count47 payloads also pass53/audit but fail direct state-preservation checks | Exact int.MaxValue examples are not well-formed bounded-map demonstrations; original array payload cannot clear on host layout even after forced trigger. No unchanged-source mutation demonstrated. Limit audit wording; review exact calls, receiver provenance and behavior |
| F3 | Stale/reused snapshots can yield endpoint labels without read order/freshness | **Partially Verified** | A1/A2 reproduce. Independent three-distinct-reads-in-order case still gives EquipEndpointAgreement after actual removal | Read stamps can reject duplicate/reordered reads, not currentness, missing writes or full lifecycle coverage. Keep temporal authority Unknown; require separately reviewed capture/operation association before stronger labels |
| F4 | Foreign slot delta during an open transaction is absorbed while between-record delta rejects | **Partially Verified** | A3 count-changing addition reproduces. Independent same-count foreign replacement BETWEEN records is also absorbed | Projection records count, not every key/value. No complete-delta claim is supportable. Define projection scope; retain key/reference deltas only if a later consumer truly needs them |
| F5 | EndpointsAgree and LocalReadValid always have identical values | **Verified** | Snapshot constructor assigns both from valid; no independent setter | Validity currently includes endpoint-copy agreement. This is dependent reporting, not a second corroborating channel or a safety failure. Clarify wording or simplify under later approval |
| F6 | Exact Dictionary type does not fix comparer semantics; case-variant Tome can be reported absent | **Verified**, fixture scope | A12 reproduces exact OrdinalIgnoreCase map with Tome, LocalReadValid=true, HasTome=false | Current pinned MS constructor uses parameterless default comparer. Reader deliberately avoids executing source comparer. Do not "fix" by calling arbitrary comparer/TryGetValue. State ordinal-copy scope or reject unsupported comparer metadata safely |
| F7 | First owner wins capture; NPC return may poison target witness | **Partially Verified** | A6 invalidates witness after different owner. Installed unequip prefix0600047c calls IsValidPlayer before GetRuntimeData; IsValidPlayer06000452 rejects non-Player | The cited NPC path is not evidence of natural NPC capture. Public GetEquippedItem/SetEquippedItem/IsItemEquippedInCustomSlot permit broader Humanoid arguments, and player guard is not a local-player guard. Preserve exact-target filtering requirement; NPC frequency/execution unverified |
| F8 | Swap, GUID projection, unwired reader, first-failed-read scope and null baseline semantics | **Partially Verified** | A4/A5/A7/A8/A10 reproduce. Occupied tracked-item Swap Begin also succeeds in independent test | Distinct-item replacement is unsupported, but "Swap only reachable from empty" is false generally. Other behaviors are explicit limits/contract choices, not production bugs. See detailed breakdown |
| F9 | Repository-local locks do not serialize the same installation across worktrees; no session lease | **Verified** | Source State=repo/.dev-state, FileShare.None paths; separate process acquires B lock while A held, same A path blocked; release permits reacquire although lock file remains | No real deployment race attempted. Whole-session/resource lease and explicit operation scopes needed before shared-client automation. File existence/stale age is not ownership. Do not globally interfere with protected independent servers |
| F10 | Hardcoded primary paths cause cross-tree output or artifact reuse | **Verified** by source inspection | Export-TomeOwnershipAudit defaults Output/DiagnosticDll to primary; Test-TomeOwnershipAudit and Test-TomeLifecycleArtifact default to primary evidence/artifact; DEV005/006/EXP005 OutputPath examples absolute | No cross-tree command executed. Fixed game-reference paths and historical evidence paths can be legitimate; writable output and implicit verifier inputs must be worktree-relative/explicit. Audit all affected defaults under a separate fix |
| F11 | Provenance runner depends on pwsh and PS5.1 stderr handling aborts expected mutants | **Partially Verified** | Exact baseline runner exits1 at NativeCommandError from unsafe-collection stderr under PS5.1. Child audit invokes bare pwsh. Cached7.6.5 demonstrably exists | Not evidence of a different physical host or global PowerShell7 absence. Default PATH differs; current DEV008 records cache location. Declare/resolve host, make5.1 handling robust if supported, and check exact kill reasons |
| F12 | Identical-source worktree builds differ due absolute PDB path; normalize or compare IL | **Verified**, remedy qualified | Two private identical-source roots produce different DLL hashes/CodeView paths but identical method-IL hash. Native-path PathMap gives byte-identical DLLs | Forward-slash mapping did not apply on this host; CI=true alone retained differing relative subroots. Do not assume the two suggested options are interchangeable. Keep exact hash identity for deployment of one approved artifact |
| F13 | Bridge can stop acceptance mid-session while a book might remain owned | **Partially Verified** (static risk strengthened) | Eligible dynamically reads MP toggle; nonzero calls Stop. Postfix exceptions also Stop. MP config helper registers toggle as synchronized; Awake installs SettingChanged handler; BepInEx setter raises change event | Own PoVPlus enabled setting is captured at startup, so changing that setting alone is not a proven live disable path. No equipped runtime state observed. Design explicit adverse-state response/test in a genuinely isolated environment, not a live fix |
| F14 | DEV008 Boolean evidence and check/use paths must not become authoritative executor safeguards | **Verified as an acknowledged limitation**, no present executor defect | Committed model/docstrings and EXP006 expressly disclaim attestation/race safety;56 current Python tests pass, including false-proof and focus-race examples | Claude read earlier WIP, so its source was not immutable at review time; current commit independently checked. No live executor exists. Derive future facts through trusted adapters/independent verifier and OS containment; copying these gates would be unsafe |

## 4. F1 sensitivity, residual19 and methodology

The immutable mutation table has77 distinct names and each before-text matches exactly once.
Every generated file equals source.replace(before,after), with no incidental edit. LF and CRLF
generation both emit77. Some entries remove a compound guard (e.g. enum ranges or owner+tracked
type checks); call them edit groups rather than interpreting22/77 as an objective safety coverage
metric. All156 baseline reproduction builds succeeded. No compiler/restore error was scored as
a kill. Original36 tests, falsify exit2, corrected53 tests and adversarial11 FINDING outputs
match the saved package. The two sweep result sets match all77 and55 saved verdicts/messages.

All36 D cases pass corrected source; each of the36 distinguished survivors fails one labeled D
case. D07's thrown null dereference is a legitimate violation of fail-closed/no-throw behavior,
not a compiler failure. Constructor cases require ArgumentException, not arbitrary exceptions.
The additional cases frequently still assert generic Incomplete; their mutation contrast makes
these particular distinctions meaningful, but importing them does not establish comprehensive
branch/reason coverage. The seven original regression mutants also die at the exact reported
messages, including the cross-labelled caller-coverage/initial messages.

### CR-01: five more observable distinctions among the remaining19

Independent.cs uses additional independent oracles, never game objects:

| Previously surviving edit | Counterexample / result |
|---|---|
| read-entry-type | Dictionary<string,Item> contains DerivedItem in a non-Tome entry, tracked Item ordinary. Corrected source refuses; mutant accepts LocalReadValid |
| read-ownitem-type | Tracked DerivedItem inherits the audited fields. FieldInfo.GetValue accepts assignable subclasses; it need not throw. Corrected exact-type gate refuses; mutant accepts |
| read-same-refeq | Different derived source values replace one another between copies. Mutant dispatches a foreign virtual Equals before later invalid-entry rejection; corrected source does not invoke it. Unknown at the end does not erase the foreign call |
| borrow-dead | Weak target explicitly cleared to model missing root. Both Borrow calls return null, but only corrected witness latches Invalid. This is an observable contract difference, even with no current consumer |
| read-roottype | Synthetic unsealed Data with DerivedData: inherited FieldInfo read succeeds without gate. Generic equivalence argument fails; however installed HumanoidCustomSlotData is sealed, so this case is excluded by that pinned root contract |

Installed ItemDrop.ItemData is **not sealed** (game type token020003a8) and declares no Equals
override. That answers Claude Q3: a derived-value/derived-tracked fixture is meaningful to the
reader's rejection contract, though no installed mod is shown creating such a subclass. The
read-same-refeq case uses a virtual override on a deliberately derived synthetic item; the correct
source's reference comparison prevents the call. These are missing regression protections,
not newly discovered executions in the current reader.

### Disposition of every remaining-survivor argument

| Edit(s) | Assessment |
|---|---|
| add-alias-pos; add-scope-null | Conditionally redundant for actual reader-generated valid snapshots: tracked alias positive and scope non-null. Not a theorem for forged internal Snapshot objects |
| ret-own-has; ret-own-alias | Entailed by remaining ownership/alias and SampledValuesAgree checks for valid consistent records |
| ret-rem-has; ret-rem-flag | HasTome and Flag are coupled by SampledValuesAgree; remaining check rejects occupied/remnant state in that domain |
| same-hastome; same-owner; same-valid | Conditionally entailed by consistent reader samples, tracked identity and remaining flag/alias checks. Contradictory preconditions require their own controls |
| read-roottype | Generic argument disproved, but pinned MS root is sealed; retain as explicit defense/contract guard |
| read-ownitem-type; read-entry-type; read-same-refeq | Non-equivalent; independent counterexamples above, sealed fixture assumptions insufficient |
| match-hasguid | With genuine copied metadata, a missing key also fails guid-is-string; conditional redundancy |
| copy-precount | Valid sequential BCL layout gives0<=Count<=storage. Malformed free-count states and work-before-rejection are outside that implication; no universal equivalence claim |
| copy-postcount | Do not certify equivalence. Enumerator version checks do not prove no concurrent change after final enumeration/before later reads; fixture hook coverage is not all schedules |
| copy-cap64 | Storage<=64 and stable exact BCL enumeration bound this loop for ordinary sequential inputs; conditional redundancy, not arbitrary runtime/concurrency proof |
| borrow-dead | Observable Invalid-latch distinction reproduced; "same return" is narrower than behavioral equivalence |
| storage-decl | Pinned BCL storage field belongs to exact closed generic type; conditional layout redundancy, useful fail-closed contract for other runtimes |

Thus the55 initial survivors include **41 experimentally distinguished edits** under the combined
fixtures. One of the five new cases is generic-root-only as noted. The other14 are conditional
redundancy arguments or unresolved scheduling/domain questions, not proven equivalent mutants.
Minimum useful follow-up: positive controls that reach each intended guard, assert a scoped
failure reason (new stable reason IDs if separately approved), and preserve a labeled
mutation-to-assertion matrix. Do not widen the PoC into a full equipment monitor to improve a score.

## 5. F2 audit attacks and non-interference boundary

Both exact committed escape sources build and pass all53 fixtures and the audit. But the trigger
details matter:

- Copy calls StorageCount before Count; on a well-formed stable dictionary, Count<=storage<=64.
  Thus count==int.MaxValue cannot occur in that supported state. By deliberately corrupting the
  synthetic BCL freeCount so public Count becomes int.MaxValue, I made the **original** Add
  payload execute: backing used-count increases1->2 before rejection. This is a malformed-input
  witness, not natural game-state evidence.
- The original Clear payload hardcodes `_entries`. Host net48 uses `entries`, so even a forced
  int.MaxValue used-count yields NullReferenceException and no clear. Installed Mono metadata
  does have `_entries`, but its assemblies were never executed. Source availability of that
  field is not runtime proof of payload execution. No well-formed map can reach this trigger.
- Independently replace triggers with count/used==47; for Clear use host-compatible backing
  field selection. Both variants pass53 and the original compiled audit. Independent direct
  map-count/backing-value oracles show actual source writes. Corrected controls preserve state.

### CR-02: stronger reachable regression escape, not a present production vulnerability

The two count47 variants establish the assurance gap without enormous allocations, runtime
detours, target assemblies or impossible valid-map counts. The audit really does allow these
extra operations. Its foreign-stfld check is narrower than "no foreign mutation"; calling Add
or clearing a referenced array does not require a foreign stfld in the observer's own IL.

Exact call allowlists would catch these new calls. They are better change detectors, not a
general purity proof: allowed calls' receiver provenance, arguments, control flow, reflection
contract and runtime substitutions still matter. Literal64/128/4096 presence does not alone
prove a bound dominates all relevant paths. Keep structural guard audits, independent behavioral
state-preservation oracles and manually reviewed source/contract boundaries. No claim here says
the unchanged production/PoC source contains the injected writes.

## 6. Temporal/projection findings and owner capture

### CR-03: strictly ordered, distinct reads can still be stale

Independent sequence: read empty baseline; read empty Begin; equip; read occupied Return;
remove; submit recorded Begin/Return. Three real reads are distinct and ordered1<2<3, yet
the current map is empty when ledger reports EquipEndpointAgreement. Reader-issued stamps would
pass this schedule. Claude's proposed remedy closes reuse and some order ambiguity, not this
freshness/currentness counterexample. No new caller-attested freshness bit is justified.

The ledger compares supplied historical projection values. That can remain useful if labels
say exactly that. Operation association, observer timing and current-state evidence require
additional provenance that the unwired model does not have. No stamp can recover missed ABA,
all-writer coverage or authority merely by being monotonic. All temporal/neutral claims stay Unknown.

### CR-04: broader projection and Swap qualifications

A count-changing foreign addition during Return is absorbed, as A3 shows. A same-count foreign
value replacement **between** records is absorbed too: only Tome alias/flag/binding/count are
retained. Neither case proves the entire source map unchanged. The independent fixture sees
EquipAttempted after replacing a utility value; driver truth, not ledger reconstruction, is the
oracle. Scope documentation should not imply complete-map corroboration.

F8 breakdown: A4 proves a distinct-item swap cannot Begin with the new tracked item while old
item owns Tome. It does not prove every occupied Swap is impossible: tracking the already-equipped
item yields SwapAttempted and then NoChangeInRecordedProjection. A5 deliberately loses raw GUID
differences when the equality projection remains true; A7 is the documented unwired reader;
A8 shows fail-closed first-attempt scope binding (a poisoned reader needs a new instance);
A10 allows null-before-any-event to be replaced by a valid baseline, still with initial authority
Unknown. These require specified semantics, not automatic production corrections. A11 correctly
rejects stale equipped flag after removal and is retained as a negative control.

### CR-05: first-owner hazard is real, cited NPC route is guarded

Pinned MS prefix0600047c calls IsValidPlayer at IL000f before GetRuntimeData at001e.
IsValidPlayer06000452 performs isinst Player and requires inventory; ordinary NPCs fail.
The general witness has no target policy and can be invalidated by any differently supplied
owner, which A6 proves. Public GetEquippedItem/SetEquippedItem/IsItemEquippedInCustomSlot do not
impose that player guard, and the prefix does not require the **local** authorized player.
Therefore reject Claude's specific prefix-to-NPC inference, not the need for exact-target
filtering before capture. No runtime frequency/ordering of non-target returns was established.

## 7. Operational and reproducibility findings

F9 source evidence: New-DevContext builds State from Repository. Invoke-DevTransition holds
State/workflow.lock during backup/replace; Start-DevGame releases its same-path handle in finally
after Start-Process returns. Get-DevRuntime filters executable paths under its installation;
a different known client path is ignored. Receipt repository matching is fail-closed, but not a
shared recovery registry. The synthetic two-process test confirms exact-path exclusion and
different-path independence. Persistent lock-file presence after handle release means neither
"file exists" nor an age threshold is a valid lease protocol.

Future coordination must identify the canonical disposable environment/resource, including
volume/final-path identity, profile/cloud boundary and desktop session where appropriate; serialize
deploy/rollback/preparation, launch, whole active session, evidence sealing and restoration across
processes/worktrees; record owner/PID/start/boot/authorization; refuse aliases, unknown holders,
check/use races and stale recovery journals. An abrupt process exit releases a file handle but
does not prove client stop or recovery completion. Leases need reconciliation/quarantine, not
automatic stale-file deletion. No crash recovery or actual deployment race was tested here.

Claude's blanket "detect any valheim_server.exe" recommendation is broader than a resource
identity policy. Do not access/stop protected independent servers merely because of a process
name. Shared-profile ordinary clients matter to current unsafe topology; a truly isolated guest
could have a different scope. Decide the protected resource set explicitly.

F10 concrete unsafe defaults are the Output/DiagnosticDll/Metadata/Dll paths listed in its table.
The documented OutputPath commands can write to the primary tree when executed elsewhere. They
were **not** executed. A fixed installed-reference path or a dated historical evidence citation
is not the same defect. All review runs pass explicit private artifacts or derive their root
from this review location. A future repair should remove implicit cross-tree evidence selection,
not erase historical paths or silently retarget the configured game installation.

F11: exact PS5.1 runner reproduction aborts at NativeCommandError from expected mutant stderr
before normal exit checking. It is a tooling compatibility issue, not a failure of the53 corrected
fixtures. Cached PowerShell7.6.5 exists on this same machine; Claude's limited search did not find
it. Baseline nested bare-pwsh lookup is PATH-dependent even if one launches an outer shell by
absolute path. Record/resolve the actual host and assert the intended failure text; do not count
every exit1 as an intended kill. The main immutable reproduction independently checks those messages.

### CR-06: build normalization must be measured, not assumed

Eight final clean rebuilds compare identical baseline production source in two private roots:

| Mode | DLL identity across roots | Observation |
|---|---|---|
| Ordinary Release | different | Absolute CodeView/PDB path and MVID differ |
| PathMap with forward-slash Windows prefix | different | Prefix did not map the actual backslash CodeView path on this host |
| PathMap with native Windows prefix covering entire private root | **identical** | Both DLL SHA2560209ac561ceebaa4453e970d6a5d688e0eb1af96731220757a35e060f4412063; CodeView /_/Source/PoVPlus/obj/Release/net48/PoVPlus.pdb |
| ContinuousIntegrationBuild=true alone | different | Mapped paths retained identity/a versus identity/b under common repository root |

Every module's method-body IL fingerprint is
226DFE6A0EDD1CB2F16C7540F4EA7B464B195DE7AE0FCF9D377E26F889AB2654.
This supports F12's byte/logic distinction; it is not a universal semantic equivalence proof or
a replacement for exact deployed-artifact hashes. Six earlier exploratory builds preceded the
final eight -t:Rebuild runs. No project or workflow setting was changed. Proposed PathMap and
CI properties are not interchangeable recipes; verify output under the chosen layout/toolchain.

F13 static evidence goes beyond speculation about mutable config. MP Awake06000019 at00f1
passes synchronized=true when registering Enable Tome Slot; config helper06000018 registers
ServerSync entry and stores that flag. At0120 Awake adds SettingChanged callback06000020,
which calls AddCustomSlot06000111. BepInEx ConfigEntry<T>.set_Value060001fd writes the value and
raises OnSettingChanged. This establishes live-change machinery, not an observed incoming sync.
The bridge reads that same Value per eligible false-result extension and calls activation.Stop
on nonzero; exceptions also Stop until restart. Changing PoVPlus's own startup-only setting is
different and is not shown to hot-disable it.

MS FindInventoryItemByGuid06000467 invokes definition.IsValid at0038; ValidateSingleSlot06000472
clears runtime/binding/effect state when no eligible matching saved item is found (005a-0097).
This supports a conditional downstream hazard if eligibility disappears while a book is owned.
No code here proves that it actually occurs in the installed session or whether effects/items
are lost. Proposed later test: independently recovered disposable occupied baseline, authorized
in-memory/config-change or binding-fault condition, then normal validation/equipment behavior,
item conservation/effects and post-restart checks. Enabling MP's slot or editing live config is
**not authorized now**. Do not resolve competing owners by blindly keeping acceptance active,
or resolve uncertainty by rolling back/removing support while ownership is unknown.

## 8. Review methodology, failure accounting and independent challenges

Saved adversarial, discriminating, original-falsify and both sweep output sets match the independent
run after newline normalization. The committed code and saved conclusions are reproducible for
the inspected host/compiler. There is no basis to call these outputs fabricated. Reproduction
does not turn author-authored tests into independent truth: Independent.cs adds direct driver
checks, subtype/virtual-call tests, ordered-stale schedule, same-count projection, reachable
source writes, and a malformed original-trigger check.

### CR-07: supplied wrapper's failure accounting is not exhaustive

The wrapper checks the adversarial FINDING count but not that invocation's exit code. Its survivor
phase checks36 exit1 results and labeled failures, but does not separately require all55 runs to
have exit0/1; a build failure among the assumed equivalent group could escape that count scheme.
Thus its "every expected count and exit" wording is too strong. My runner requires each restore/
compile success and all run exits in0/1 before counting. **No such build failure occurred in this
reproduction**; this is a harness robustness proposal, not evidence that the saved22/55/36 results
are wrong. Some tests use DuringRead/EnumerationStep fixture seams absent from diagnostic libraries;
they falsify the managed model, not real Mono concurrency, inlining, patch order or runtime timing.

Own investigation failures/limits preserved:

- Initial reachable-Clear test used a dictionary lookup after intentionally corrupting its
  backing entry; that lookup hung and the synthetic process hit its180sec timeout. Only the
  owned fixture subprocess was terminated by subprocess timeout. No game process involved.
  Replaced the oracle with direct backing-entry inspection. Both corrected-source controls
  pass and injected-source version fails for the intended field change.
- An initial library audit started before the last independent library existed and failed
  as missing artifact. After sequential completion, all12 audit outcomes match expected results.
- Initial PS5.1 extraction used shell line-ending transport; reran with byte-exact git-show output.
- Initial PathMap tests showed no normalization; native separators and forced rebuilds tested
  the alternative rather than dismissing the remedy prematurely. No convenient compiler failure
  was scored as a killed mutant.

## 9. Executed validation and preserved artifacts

| Work | Result |
|---|---|
| Immutable input / mutation verification | All source edits exact,77 on LF and CRLF; five saved output sets match |
| Main reproduction |156 successful fixture/library builds; frozen36 pass, three falsifications reproduced (exit2), corrected53 pass, adversarial11 FINDING/one safe negative control, corrected36 D cases pass; sweep22/77 and36/55 distinctions |
| Seven regression tests | Seven intended exit1 messages, not compiler errors; seven expected independent IL rejections |
| Independent suite, final |23 fixture/library builds; ten corrected controls pass; five additional sweep edits distinguished; two reachable mutation regressions pass53/audit but fail state-preservation; exact original Add malformed-input trigger executes and Clear host trigger remains non-writing |
| Library audits |12 expected outcomes: corrected PASS, seven original regressions rejected, two Claude escapes PASS, two reachable independent escapes PASS |
| Ordinary build identity |Eight final clean builds plus six exploratory builds; method IL equal across tested layouts, native PathMap yields exact byte equality |
| Lock primitive | Same-path child denied, different worktree lock acquired concurrently, released path reacquired; synthetic only |
| PowerShell compatibility | Exact PS5.1 runner exits1 at expected-stderr handling; cached7.6.5 exists; no software installed |
| Existing Python tests |56 pass, including committed DEV00828; no GUI/game operations |
| Knowledge |127 records,0errors/0warnings; review deliberately does not amend committed canonical records |
| Whitespace/scope/integrity | Final checks recorded in results/summary.json and ignored scratch; no existing tracked content changed |

Code, reproduction commands and compact summary remain in this review directory. Full build
logs, generated source/diffs, DLL/EXE/PDB, metadata and original input copies are in ignored
`.dev-state/crossreview-a/`. inputs.json and mutation-edit-manifest.json trace sources to commits;
results.json, independent-results.json, audit-results.json and identity-audit.json retain per-run
outcomes/hashes. No raw game logs, third-party binaries, private runtime state or saves are copied
into this package. No upstream ZIP generated.

## 10. Consequences and prioritized follow-up (proposals only)

| Category | Decision |
|---|---|
| Before harmless synthetic-window testing | Separate owner authorization, unique harmless target, stop/takeover and focus-race policy. F1/F2/C1 completeness and game-installation lease are not prerequisites to an isolated harmless GUI demonstration that never touches the game. No GUI authority granted here |
| Before isolated Valheim testing | F9 resource/session lease, F10 safe paths/evidence ownership, independently proved persistence/cloud/host containment and complete baseline recovery; approved launch/deploy scope. VM GPU/Steam feasibility remains unknown, and test launch cannot precede containment merely to discover its write set |
| Stage C1 correctness | F13 adverse activation-loss behavior and actual equip/unequip/swap/effects/conservation/reload need controlled evidence. Ownership completeness remains separate from functional gameplay and disposable recovery |
| Test infrastructure | F1 reason-sensitive discriminators; F2 precise audit boundaries; F3 historical-read semantics; F11 reliable host/kill verification; F12 measured artifact provenance |
| Documentation | Qualify EndpointsAgree dependence, unsupported distinct swaps, projected counts, NPC explanation, and existing WIP/host descriptions without rewriting historical evidence |
| Lower-priority hygiene | Wrapper exit checks, readable stable failure IDs and consistent path examples; important to future reproducibility but not a pretext for reopening runtime work |

1. Preserve this independent review for owner comparison; do not merge or reconcile agent worktrees.
2. Before any shared-client executor, separately authorize and review F9/F10 tooling changes.
   Include interrupted-session reconciliation and a defined protected-resource scope, not just a
   differently located lock file. Do not infer safety from both controllers having passed process
   checks before either writes.
3. Choose whether to freeze the inactive EXP005 line or retain it as a narrowly scoped regression
   reference. If retained, adopt discriminators/counterexamples and honest audit/sample labels under
   a separate correction task. No need to build a mutation-monitoring subsystem or reintroduce
   complete-history claims. Read stamps may improve bookkeeping only.
4. For DEV008, keep Boolean model and check/use helper out of actual authority. Decide the
   isolation candidate, independently rehearse containment/recovery with sentinels, then seek
   explicit Level1/Level2 authority as appropriate. Do not count one write trace as complete future
   confinement. Separate process/UI observations are not independent ownership authorities.
5. Before any actual bridge equipment trial, review F13's conditional live-toggle/fault scenario
   and operator response. No hot change, equipment or persistence trial is implied.

No immediate production correction is authorized or implemented. The original and corrected
experiments remain preserved, ordinary bridge behavior unchanged, and the closed C1 observer is
not reactivated. Root AGENTS and the two fixed commits remain intact. This review adds only
Reviews/Codex-2026-10-27-CrossReview and ignored local scratch; no commit, push, merge, deploy,
launch, save operation, Steam Cloud change, desktop action or dedicated-server interaction.

### Final integrity and working-tree state

All343 monitored installation fingerprints (243 DLLs and100 cfgs) match the existing Stage B
baseline. The original installed **pre-bridge** PoVPlus DLL SHA256 is
351E948AFC75D8B5BFBEF67F2F1FBABB5A8A645AA7A5D51E9E804F7D501DF548;
own configuration SHA256 is
3F21FABF4696D7BA4193B130700F3C794BF1C244664DA38E176FB8132A1A4798;
EnableMagicPluginTomeBridge=false. No Valheim client or synthetic PoVPlus fixture process remains.
This is the monitored-file scope, not a claim to have inspected all machine/server/save state.

HEAD remains27ba868f7bcc82d6f5bbef0c6b6acf23a6deac6f on the existing research branch. No staged
changes; no modifications to existing tracked files. Only the12 new review/report/helper/summary
files under Reviews/Codex-2026-10-27-CrossReview are untracked. Generated scratch remains ignored.
The report is not silently integrated into Knowledge/INDEX or either agent's committed research;
that requires a later owner-reviewed preservation/integration step.
