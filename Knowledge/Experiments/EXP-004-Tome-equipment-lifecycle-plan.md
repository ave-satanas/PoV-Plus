---
id: "EXP-004-Tome-equipment-lifecycle-plan"
title: "Stage C supervised Tome equipment lifecycle test plan"
type: "experiment"
status: "proposed"
created: "2026-10-25"
last_verified: "2026-10-25"
versions: {"PoVPlus": "0.1.0", "MagicPlugin": "2.2.2", "Magic Supremacy": "3.1.5", "SeneaL API": "1.2.0", "SeneaL UI": "1.2.7", "PoV": "5.3.2", "Valheim": "1.0.20"}
evidence: ["[[INV-003-Magic-book-tome-compatibility]]", "[[COMP-002-Tome-bridge-design]]", ".dev-state/reports/INV-003-stage-b-retry-final-evidence.json", ".dev-state/stage-b-retry/restoration-comparison.json"]
related: ["[[INV-003-Magic-book-tome-compatibility]]", "[[COMP-002-Tome-bridge-design]]", "[[DEV-005-Tome-diagnostics]]", "[[DEV-003-Workflow]]"]
confidence: "unverified"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Stage C supervised Tome equipment lifecycle test plan

## Status and authorization boundary

**Proposed, not executed; this document grants no runtime authorization.** Stage B verified only
canonical-prefab acceptance, both patched MS gates, rendered-cell ownership and negative controls.
It did not equip inventory instances or test effects, exclusivity, persistence or networking.
MS remains the equipment authority; PoVPlus remains a default-Off acceptance extension.

Use only exact `PoVPlus_Test` / `PoVPlus_Lab`. The development installation shares normal local
saves; no launcher flag establishes isolation. The user must select the character/world and
perform every item/equipment action manually. Codex may collect approved read-only observations
and logs; no automated spawning, equipment changes, stat/registry edits or save manipulation.

Obtain explicit approval of separate scopes before each:

| Scope | Additional permission required | Boundary |
|---|---|---|
| C1: local equipment/effects | Deployment/own-config activation, launch/rollback, named test-item acquisition, manual equip/unequip and any Eitr expenditure/food consumption | No persistence or multiplayer claims |
| C2: persistence | Normal game saves, logout/login and clean restart using designated disposable character/world | Retain bridge On until every persisted book is safely unequipped and that state saved |
| C3: multiplayer | A designated disposable host/server, consenting test peer, matching builds/config and ordinary network/save writes | Never use or modify the existing dedicated server implicitly |
| C4: adverse disablement recovery | An explicitly disposable recovery copy plus user-managed verified backups and an exact recovery procedure | Not part of the initial lifecycle trial; never casually remove support while a book is worn |

Game-managed saves can occur even during C1. Approval must acknowledge that normal play writes
the designated disposable saves. Do not promise a no-save-write equipment trial. No direct save
file access, copying, editing or deletion by Codex is implied; user-managed backup preparation
is separate and must not involve personal saves. If recovery provenance is unclear, do not launch.

## Preparation and go/no-go gate

1. Re-read AGENTS, [[DEV-003-Workflow]], [[DEV-005-Tome-diagnostics]] and Stage B evidence.
   Verify commit, clean/reviewed source, exact installed dependency hashes/contracts and tests.
   If versions/patch chains differ, stop and revalidate; Stage B does not generalize to updates.
2. User confirms the designated identities are disposable, unique in the selection UI, and
   recoverable using a user-managed test-only backup. Exact labels alone do not establish unique
   filesystem save identity. Do not enumerate the shared saves directory to resolve ambiguity.
3. Confirm development client stopped and external server untouched. Record DLL/own-config bytes,
   SHA256, third-party fingerprints, baseline session and rollback receipt. Use only Dev.ps1
   Build/Deploy preview/authorized deployment/Launch/Rollback. No live DLL/config writes.
4. Choose the tested bridge source with reviewed instrumentation. The Stage B probe captures
   one prefab snapshot only; it is not a lifecycle monitor. If numeric/API evidence is unavailable
   in UI/logs, prepare separately reviewed observation-only instrumentation and offline tests
   before deployment. Do not invent console commands or invoke equip methods for measurement.
5. Temporarily enable only PoVPlus's startup setting with exact original byte backup; MP native
   Tome remains Off. Verify both actual postfixes, MS authority and one visible Tome cell.
   After launch initialization, pause for manual character/world/inventory confirmation.
6. Stop on identity mismatch, missing backup, partial patches, unknown mod/version, new duplicate
   slots, or initialization exception. Ask user for normal exit; do not repair/redeploy mid-session.

## Items, baseline and evidence sheet

Acquire one ordinary, unmodified inventory instance of each approved prefab:
`BMP_BeginnersMagicBook`, `BMP_AdvanceMagicBook`, `BMP_DruidsTome`, and native control
`ArmorMushroomcallerTome_DO`. Use an already-owned test item or user-approved normal crafting
on the disposable character; record recipe/resource/progression effects. If unavailable, stop
that case. Spawning, duplication, item-ID editing or creative commands require separate specific
permission and a reviewed acquisition method; this plan does not supply guessed commands.
Do not retrieve equipment from personal characters/worlds. Use the same quality/durability for
repeat observations, record item count/location and distinguish real inventory data from prefabs.

Before testing, the user records inventory layout/counts, empty Tome state, other worn equipment,
relevant Belt/native utility occupancy, item quality, active effects, food with remaining duration,
health/Eitr maxima and current Eitr. Hold food, other equipment, rested/shelter/wet/cold states,
passive bonuses and movement constant. Reserve free inventory cells for every removal/swap;
full-bag, death/grave, disconnect-loss and combat tests are deferred. Do not destroy items as cleanup.

For each action record UTC/session ID, case, prefab/instance label, pre/post slot and equipped
flags, item count/location, effects, max/current Eitr, regeneration samples, user observation and
log lines/screenshots. Prefer user screenshots and audited runtime reads; no account/network IDs
or save contents in Git. Local evidence belongs under ignored `.dev-state/reports/`.

## C1: equipment, effects and exclusivity

| Case / manual action | Expected invariant or hypothesis | Evidence / failure criteria |
|---|---|---|
| Native control equip, unequip, then repeat | MS ordinary behavior retained; one Tome owner and one item, effects apply/remove once | UI/actual equipped state/counts and effects agree; no duplicate, stuck flag, lost item or exception |
| Each MP book equip, wait, unequip; repeat individually | Real inventory metadata passes guards; MS lifecycle owns equip/removal; original item/SE retained | Record visible and actual state. Acceptance alone is insufficient; failed real-instance eligibility stops the case without broadening policy |
| Native-to-book, book-to-native, book-to-book swaps | Previous item safely returns to inventory; old SE removed, new SE applied once | Compare all item counts/locations and SE entries after every swap; ghost effects or duplicate ownership stop the test |
| Tome versus actual relevant Belt/utility slots in both orders | Preserve installed author safeguards; one item cannot be simultaneously claimed by two owners | Record current declared supported combinations first. Legal distinct equipment is not automatically a conflict; Stage B's unregistered belt string is not this test |
| Unequip all test Tomes | Empty slot, original item restored, original baseline stats/effects recover | Stop on missing item, persistent SE, changed quality/type or residual bonus; no automated correction |

Review actual Belt/utility support before attempting a user drag. Do not force items into invalid
cells. If the same book is accepted by competing routes, stop and preserve ownership evidence
before attempting another action. Test only observed, supported UI routes; MP native remains Off.

Maximum Eitr: derive the expected delta from the installed MP bonus settings, original SE and
applicable overrides; record the exact config/source reference before scoring. Do not hardcode
new formulas or assume 0.15/0.25 shared regeneration modifiers are flat Eitr/sec. Measure empty,
native, each book and empty-again under identical food/passive conditions. Require one appropriate
bonus, no stacking across swaps/repeats, and recovery after removal. If the authoritative expected
delta cannot be established, mark the numeric result unverified rather than invent a pass value.

Regeneration: first record the static concern: MP's hook filters through its disabled native
classifier, so the acceptance bridge may leave its book regeneration contribution absent.
For each condition use the same separately approved, safe Eitr expenditure method and recovery
range, wait out verified regen delay, and measure timed Eitr samples for at least three comparable
trials. Avoid damage, combat, food refresh or weather changes. If no safe expenditure/measurement
method is approved, defer regeneration. Compare observed rate with independently derived expected
mod behavior; absent/duplicate contributions are findings, not permission to patch formulas.
Status-effect presence alone is not proof of its effective Eitr bonus or regeneration rate.

## C2: persistence, only after C1 passes

With bridge/dependencies unchanged and enabled, test one item at a time: each book and native
control. User records equipped state/item counts, performs normal logout and reselects only the
same authorized character/world; compare restore and effects once. Separately repeat after a
normal client exit and restart with the same enabled build/config. Pause for each manual load.
MS uses `DO_Equipped_tome` binding and `DO_CustomSlotGuid` item metadata statically; observe only
through reviewed runtime APIs, never edit or read save files directly. Check no lost/duplicate
items, changed identity, stale SE, duplicate bonus or restored mismatch after UI reopens.
If restore fails, stop new cases and retain the enabled installation for reviewed recovery.
Before concluding C2, manually unequip every bridged book, confirm neutral state, perform a
normal save/logout and re-enter under the enabled bridge to confirm empty state persists.

## Disablement hazard, recovery and rollback

**Do not disable, unpatch or roll back while any bridged book remains equipped or its saved
binding is uncertain.** Native MS gates reject those books without the bridge; cleanup can
invalidate bindings/effects or strand equipment. A stopped process alone does not make this safe.

Normal success: user unequips all bridged books with bridge On, verifies physical item counts,
empty Tome and effect removal; if C2 was authorized, save/reload proves neutral persistence.
User exits normally and confirms. Independently verify stopped; restore original DLL through the
recorded Dev.ps1 receipt and own config exact bytes. Verify both hashes, diagnostic absence,
bridge Off and unrelated fingerprints. Preserve natural config drift without automatic reversal.

If unequip fails, an item disappears, or user already exited with a book worn: preserve logs,
receipts and the known enabled build/config. Do not disable or perform a blind rollback. Stop
and request a specific recovery decision. A supervised relaunch of the same enabled version to
manually unequip, or user-managed restore of the designated test-only backup, needs explicit
authorization. Do not access other saves, invent repair commands or auto-redeploy code.

C4, if wanted later, first reproduces adverse disablement in an explicitly disposable recovery
copy whose tested backup/restore path the user approves. Specify exact equipment/exit/config/
restart conditions and ownership checks before proceeding. Keep the enabled binary/config backup
available. Do not run this destructive-risk branch as part of the first Stage C session or claim
that successful unequip-before-rollback proves recovery from disabling an equipped book.

## C3: multiplayer gate and matrix (deferred separate environment)

No current dedicated server or loader is part of this plan. Approval must designate a separate
throwaway host/server/world and consenting test client, including any additional character.
Reverify Valheim/BepInEx/MP/MS/API/UI/PoV versions, item availability, mod manifests, config-sync
ownership and required server-side dependencies. The current bridge pins client/UI contracts;
do not assume it can or should be installed unchanged on a headless server. Source inspection
must identify server authority and actual required placement before any server deployment.

Use matching tested PoVPlus builds and compatible settings on every participating client that
validates equipment. Document server requirements rather than assuming acceptance is synchronized.
Start with agreed bridge-On clients, native control, then each book: owner equip/unequip,
remote visual/state observation, effect authority, ordinary logout/reconnect and host persistence.
Record each endpoint version/config/session, authoritative owner and item counts; verify no
remote stat stacking, duplicated items or inconsistent attachments. Missing-bridge/version-mismatch
negative tests remain separately authorized and disposable; never join an existing live server
with an intentionally incompatible character. Local C1/C2 success cannot mark C3 passed.

## Completion, failure reporting and next decision

Score each case Pass / Fail / Blocked / Not run with evidence and pinned versions. Stop at first
unexpected equipment loss/duplication, ownership conflict, SE/stat persistence, unknown patch or
new attributable exception; no runtime code repair or repeated deployment. Preserve the known
EpicLoot shardstone error and transient Warfare configuration observation independently.
Update [[INV-003-Magic-book-tome-compatibility]] and [[COMP-002-Tome-bridge-design]], recording
which lifecycle gaps remain. Only the exact passed cases gain verified status. Cleanup must
report whether rollback was safely completed or deliberately blocked by an equipped-item hazard.
No ZIP/upstream contact or Stage C action is authorized by this planning document.
