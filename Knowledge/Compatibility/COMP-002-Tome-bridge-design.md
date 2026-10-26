---
id: "COMP-002-Tome-bridge-design"
title: "Experimental MagicPlugin to Magic Supremacy Tome adapter"
type: "compatibility"
status: "partial"
created: "2026-10-25"
last_verified: "2026-10-25"
versions: {"MagicPlugin": "2.2.2", "Magic Supremacy": "3.1.5", "PoV": "5.3.2", "SeneaL API": "1.2.0", "SeneaL UI": "1.2.7", "Valheim": "1.0.20"}
evidence: ["[[INV-003-Magic-book-tome-compatibility]]", "Scripts/tests/Test-TomeContracts.ps1: installed metadata contract checks", "Magic_Supremacy.CustomSlotSystem methods 0x0600044c,456,467,46a-472,47a-481", "MagicPlugin.Patches.PlayerPatch methods 0x0600007f,82,85"]
related: ["[[INV-003-Magic-book-tome-compatibility]]", "[[DEV-005-Tome-diagnostics]]", "[[MOD-blacks7ar-magicplugin]]", "[[MOD-dreanegade-magic-supremacy]]", "[[MOD-seneal-seneal-api]]", "[[MOD-seneal-seneal-ui]]", "[[MOD-treextr-pathofvalheiman]]", "[[DEV-004-Intervention-framework]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/blacks7ar-MagicPlugin/MagicPlugin.dll", "dll:BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll", "dll:BepInEx/plugins/seneaL-SeneaL_API/SeneaL-API/SeneaL-API.dll", "dll:BepInEx/plugins/seneaL-SeneaL_UI/SeneaL-UI/SeneaL-UI.dll", "dll:BepInEx/plugins/treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll", "src:Source/PoVPlus/Plugin.cs", "src:Source/PoVPlus/Compatibility/TomeBridgePolicy.cs", "src:Source/PoVPlus/Compatibility/MagicPluginTomeBridge.cs"]
claims: {}
contradicts: []
supersedes: []
---

# Experimental MagicPlugin to Magic Supremacy Tome adapter

## Scope and evidence boundary

**Initial acceptance bridge implemented offline, disabled by default; never deployed or runtime-tested.** The user prefers Magic Supremacy (MS)
as the sole authoritative Tome equipment system. Retain its existing slot, lifecycle and save
format. MagicPlugin (MP) remains the provider of book prefabs and original magical effects.
The apparent displayed descriptor is `AzuEPI.emulated/$tome_DO`, rendered by SeneaL UI;
its bound runtime instance and actual four-item acceptance results remain unverified because
the earlier probe aborted. [[INV-003-Magic-book-tome-compatibility]] preserves that history.

Classify the desired change as integration/customization, not a demonstrated upstream defect.
Do not enable MP's native Tome slot, add a second descriptor, alter progression recipes or
change third-party configuration. Restrict the candidate family to BMP_BeginnersMagicBook,
BMP_AdvanceMagicBook and BMP_DruidsTome. Earrings, belts, weapons and materials remain excluded.

## Implemented offline acceptance bridge - 2026-10-25

The current user instruction explicitly authorizes offline implementation before renewed runtime
confirmation. Source is experimental and inactive unless the PoVPlus-owned BepInEx setting
`[Compatibility.MagicSupremacy] EnableMagicPluginTomeBridge = true` is present at startup.
Default is false. No installed configuration was created/edited; a future plugin launch may bind
this setting in its own povplus.core.cfg. The setting is startup-only, not a live toggle.

Source: Source/PoVPlus/Compatibility/TomeBridgePolicy.cs and MagicPluginTomeBridge.cs.
Plugin.Awake retains its startup log, binds the Boolean default false and starts the adapter.
Five soft dependencies order MP, MS, SeneaL API/UI and PoV before PoVPlus when present without
hard assembly references. When disabled there is no Harmony construction, dependency inspection,
acceptance/effect change or slot registration. Soft load-order declarations and config binding
are startup changes even with the gameplay feature Off.

When enabled, seven exact SHA-256 pins (five mods, game and HarmonyX) require the inspected
versions in DEV-005. Both target methods must be private static Boolean methods with bodies:

- Magic_Supremacy.Magic_SupremacyPlugin.IsTomeItem(ItemDrop.ItemData), token 0x06000501.
- Magic_Supremacy.CustomSlotSystem.IsItemValidForSlot(ItemDrop.ItemData, string), 0x0600044c.

Postfix argument bindings use __0 and __1, not guessed parameter names. The internal postfix
requires slotId exactly `tome`. Both preserve every original true result. For an original false,
the extension requires an exact allowlisted ID, nonnull ItemData/shared data, the actual canonical
ObjectDB prefab reference, the canonical shared-data reference, and the original per-book SE name:

| Prefab | Required original equip status effect |
|---|---|
| BMP_BeginnersMagicBook | bmp_beginners_magicbook |
| BMP_AdvanceMagicBook | bmp_advance_magicbook |
| BMP_DruidsTome | bmp_druidstome |

It also requires MS's existing shared `tome` registry containing native control
ArmorMushroomcallerTome_DO and MP's native Tome setting Off. The registry is read only through
AppDomain.GetData; no getter that initializes it, slot creation or registry mutation is used.
The canonical/shared/SE guards may intentionally reject valid customized instances if an upstream
mod clones/replaces that data; record the failed condition in future investigation rather than
broadening eligibility blindly. Numeric ItemType is not rewritten or invented as a new rule;
actual effective types and any upstream lifecycle assumptions still need runtime confirmation.

Both targets and all dependencies are validated before patching. Existing Harmony owners on
either target are refused for review. Dedicated owner is povplus.core.magicplugin-tomes. The
activation flag stays false during both installs and patch verification; only a complete pair
activates. Failure logs a warning, attempts owner-only cleanup of both targets, and leaves any
residual postfixes passive even if unpatch fails. Native results continue unchanged. A candidate
runtime read failure or MP native-slot activation disables both extensions until restart and
logs a warning. This safety failure may affect later upstream validation of already equipped
books, so it is not a guarantee of seamless in-game recovery. No live reconfiguration is supported.
Other mods may patch later; startup owner checks are not proof of final Harmony order.

At most six first-extension Info lines (two gates x three books) identify observed predicate
extensions without per-frame spam. They are not evidence of successful equipping or effects.
Per-plugin logging filters can hide them; use the session-specific diagnostic sources.

No equipment/lifecycle/effects, regeneration formula, SE application, item serialization, type
mutation, multiplayer messages or second registry are implemented. MS owns all of those. MP's
maximum-Eitr status path is retained; its disabled-classifier regeneration limitation remains
pending. The acceptance bridge is not a complete gameplay solution until tests below pass.

## Offline verification and next supervised test

Run Scripts/tests/Test-TomeBridge.ps1 (production adapter/policy compiled against explicit test
doubles, no game assemblies executed), Test-TomeBridgeArtifact.ps1 (compiled ordinary artifact),
Test-TomeContracts.ps1 (installed read-only metadata), plus the existing diagnostic, workflow,
Python and strict knowledge checks. These do not execute real Harmony patching in the game.
Mocks cover the installation transaction and error paths, not the actual native detour engine.

The next task is **INV-003 Stage A - Controlled Baseline Runtime Verification**, with the
bridge disabled throughout; [[DEV-005-Tome-diagnostics]] owns the plan. Stage A must verify the repaired identity guard and actual
active Tome descriptor using DEV-005, plus native control and BMP metadata. The diagnostic
conservatively skips methods with active Harmony owners, including this bridge; do not weaken
that guard or treat a skipped callback as false. A separately approved bridge-On trial can use
bounded bridge gate logs from real UI/lifecycle calls plus a reviewed observer if more predicate
evidence is needed. No automatic gameplay inputs are authorized by this plan.

Only in separately authorized **Stage B**, manually test native MS Tomes and all three books, swaps/unequip, max-Eitr and regeneration
exactly once, Belt/Tome exclusivity, full inventory, logout/reload and multiplayer peers if approved.
Read-only lifecycle evidence must be distinguished from actual persistence/effect tests. Before
turning Off/removing/updating the feature, manually unequip eligible books under the approved test
scope and exit normally; do not rely on hot-disable/unpatch to preserve bindings. After stopped
process verification, restore the original DLL using the exact recorded workflow receipt and
verify its hash. Do not edit saves or guess a migration. Nothing in this plan was executed here.

## Verified installed contracts

| Layer | Installed mechanism | Design implication |
|---|---|---|
| Descriptor | MS Awake registers `$tome_DO` through embedded Azu AddSlot; SeneaL emulates it under owner AzuEPI.emulated | UI and equipment authority are different responsibilities |
| Descriptor eligibility | private static bool Magic_SupremacyPlugin.IsTomeItem(ItemData), token 0x06000501: seven native prefab names | MP books do not belong to this explicit native list |
| Lifecycle eligibility | private static bool CustomSlotSystem.IsItemValidForSlot(ItemData, string), token 0x0600044c: shared registry membership | A second acceptance gate must agree with the descriptor |
| Definition closure | SyncDefinitionsFromSharedRegistry closure 0x06000489 calls IsItemValidForSlot with captured slotId | A scoped predicate extension can cover matching and restoration without changing registry data |
| Equip | Humanoid prefix/postfix 0x0600047a/b temporarily use ItemType 727, restore original type, then EquipSlotInternal | Preserve this upstream lifecycle device; never permanently reclassify MP prefabs as 727 |
| State | EquipSlotInternal 0x0600046f removes old item, sets equipped flag, obtains item GUID, saves binding, applies original SE and refreshes | Calling public SetEquippedItem alone bypasses required work |
| Persistence | PlayerSaveKey is DO_Equipped_ + slotId; item custom key DO_CustomSlotGuid | Reuse MS's existing tome binding and item identity, not a parallel PoVPlus save format |
| Restore | RestoreOrValidateCustomSlots 0x06000471 -> ValidateSingleSlot 472 -> FindInventoryItemByGuid 467 -> definition.IsValid + GUID comparison | Restored books must pass lifecycle eligibility, not only the visible callback |
| Restore entry points | OnSpawned postfix 481 and OnInventoryChanged postfix 480 (the latter skips player.m_isLoading) | Acceptance must be ready before normal restoration; do not invoke mutating restore helpers in a diagnostic |
| Status effects | ApplyStatusEffect 46a uses item's m_equipStatusEffect; RemoveStatusEffect 46b removes by effect hash | Preserve original book SE objects and upstream refresh/removal behavior |
| Visuals | SetCustomVisualState 46c writes prefab/variant hashes to ZDO on network owner; keys DO_CustomSlotVisual_ / DO_CustomSlotVisualVariant_ + slotId | Do not add RPCs or write remote state; prefab attachment compatibility remains a runtime test |
| MP max Eitr | PlayerPatch.GetTotalFoodValue_Postfix 7f / SetEitr 85 tests book status hashes and configured bonuses, without Tome classifier gate | Original SE presence should retain max Eitr; do not synthesize a second bonus |
| MP regen | PlayerPatch.GetEquipmentEitrRegenModifier_Postfix 82 filters Inventory.GetEquippedItems through MP.IsTomeItem | With MP toggle Off, this book contribution is omitted by that hook |
| Native PoV | Utility routing requires Utility (18), then calls MP classifier for utility.tome | Do not redirect books into a competing native cell merely to satisfy this predicate |
| Belt safeguard | SeneaL Slots.Accepts checks CustomStore.Claimed, preventing mod-claimed items also occupying the belt | Preserve this adaptation and inspect MS-owned/native UI routes in regression tests |

These are installed-IL facts at the fingerprints in INV-003. They do not prove active Harmony
order, item types, effects, restoration or synchronization in a running world. The reviewed MS
equipment class has no public registration method; a scan of public registration-shaped MS
methods found no suitable extension. Embedded Azu AddSlot is slot creation, not a family-extension
API. Public setters are not automatically supported lifecycle APIs. An upstream supported hook
would be preferable if the author provides one later; none is assumed to exist now.

## Historical candidate design at b461543 (before offline implementation)

1. One centralized family policy: exact prefab IDs from the verified three-item set, dependency
   availability, original shared metadata/equip SE, and confirmed supported item classification.
   Do not use broad suffix matching or rewrite names/types to impersonate MS items.
2. Prefer an upstream eligibility registration hook. If none exists, evaluate two narrow Harmony
   postfixes: descriptor IsTomeItem and lifecycle IsItemValidForSlot. Only change a false result
   for eligible MP books; the latter must additionally require slotId == "tome". Preserve every
   original true result and every other slot. No shared-registry writes or new slot registration.
3. Keep MS Humanoid hooks, GUID persistence, refresh, removal and network visuals authoritative.
   Confirm SeneaL's cached classification sees the extension from startup; avoid patching UI layout
   or forcing UI synchronization. Apply eligibility consistently before normal item restoration.
4. If runtime measurements confirm missing regen, add one narrowly scoped regen contribution
   for the actual MS-equipped eligible book, using its original m_eitrRegenModifier. Verify that
   vanilla equipment, PoV native cells and other mod postfixes have not already counted it.
   Query the single MS seat rather than scanning every inventory each frame. Do not globally
   force MP.IsTomeItem true: that also enables MP's own Azu-gated equipment path and risks a
   second owner. Maximum Eitr stays in MP's original status-based mechanism.
5. Use a new PoVPlus-owned feature flag, default disabled, with dependency/signature/version guards
   and clear logging. That flag did not exist at the design checkpoint; it now exists as described below. Keep foreign assemblies behind soft dependency
   boundaries and validate targets before installing any hooks. Fail closed on unknown versions;
   do not broaden the allowlist to compensate for API drift.

This is a candidate two-gate adapter plus a conditional effects correction, not a promise that
two patches alone solve the complete problem. Runtime evidence could justify a different hook.
No direct call to private EquipSlotInternal or mutation of private dictionaries is proposed.

## Disablement, persistence and update risks

MS restoration rejects an item that no longer passes definition.IsValid, clears the binding and
removes corresponding runtime effects. Thus hot-disable/unpatch while an MP book is worn is
unsafe as an assumed seamless operation. Plan deliberate user unequip before disabling/removing
the adapter, or a separately reviewed migration. Do not delete items, edit saves, or run broad
automatic repair. Verify that normal inventory copies remain intact on every rejected/disabled path.
Test mod absence/version mismatch on isolated fixtures or a separately authorized environment.

Both peers need consistent eligible-prefab availability and compatible equipment semantics for
visuals. Whether PoVPlus is needed client-only or on all peers is unresolved; do not assert it
from ZDO ownership alone. Test host/client reconnects, original MS Tomes, MP effects and attachment
variants. MP Eitr changes and DataForge effect overrides should retain their original ownership;
using original SE objects is necessary but not proof of correct effective results.

## Intervention tradeoffs

| Option | Correctness, maintenance and gameplay consequences |
|---|---|
| Enable MP setting | Supported for its own system but requests a distinct Tome slot; fails the chosen single-owner goal |
| Existing public MS family API | Best update resilience if available; no suitable installed contract established |
| Two scoped acceptance extensions + measured regen handling | Reuses lifecycle, no extra slot/save format, constant-size policy; private-target/version and patch-order risks require guards/tests |
| Mutate shared AppDomain registry | Hidden cross-mod state; still does not extend the separate descriptor whitelist; harder to audit and undo |
| UI-only extension / public SetEquippedItem call | Inconsistent eligibility and bypassed effects/persistence; reject as incomplete |
| Replace MS equipment system | Permitted by philosophy but adds avoidable migration/network responsibility without present evidence of need |

## Required gates before runtime approval and further effects work

Use [[DEV-005-Tome-diagnostics]] for the next supervised observation. Confirm the bound cell,
both predicate layers, actual item types/components and absence of duplicate seats first.
Source implementation was authorized in the subsequent offline task. Obtain separate
runtime/gameplay-test authorization for: equip/unequip and swaps,
max Eitr and regen exactly once, upstream effect overrides, full bag/belt duplication, death/grave
recovery, logout/reload, feature removal, dependency drift and multiplayer visuals. A predicate
passing or an offline contract test is not evidence that any of these gameplay behaviors passed.
