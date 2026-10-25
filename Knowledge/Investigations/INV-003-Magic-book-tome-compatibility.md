---
id: "INV-003-Magic-book-tome-compatibility"
title: "MagicPlugin books and the two Tome equipment routes"
type: "investigation"
status: "open"
created: "2026-10-25"
last_verified: "2026-10-25"
versions: {"MagicPlugin": "2.2.2", "PathOfValheiman": "5.3.2", "SeneaL API": "1.2.0", "SeneaL UI": "1.2.7", "Magic_Supremacy": "3.1.5", "Valheim": "1.0.20"}
evidence: ["Installed MagicPlugin.Functions.MagicSlot.IsTomeItem, MethodDef 0x06000109, IL_0000-000d", "Installed PassiveTree.Gear.State.UtilitySlotFor, MethodDef 0x060025ab", "BepInEx/config/blacks7ar.MagicPlugin.cfg:28-38", "BepInEx/LogOutput.log:126-129,164-167", "https://valheiman.com/api", "https://valheiman.com/patches/5.1.1/?lang=en"]
related: ["[[MOD-blacks7ar-magicplugin]]", "[[MOD-treextr-pathofvalheiman]]", "[[MOD-dreanegade-magic-supremacy]]", "[[MOD-seneal-seneal-api]]", "[[MOD-seneal-seneal-ui]]", "[[DEP-001-Environment]]", "[[CFG-001-Configuration-map]]", "[[DIAG-001-Logging]]", "[[DEV-003-Workflow]]", "[[DEV-004-Intervention-framework]]", "[[ARCH-002-Project-philosophy]]", "[[INV-002-Compatibility-triage]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/blacks7ar-MagicPlugin/MagicPlugin.dll", "dll:BepInEx/plugins/treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll", "dll:BepInEx/plugins/seneaL-SeneaL_API/SeneaL-API/SeneaL-API.dll", "dll:BepInEx/plugins/seneaL-SeneaL_UI/SeneaL-UI/SeneaL-UI.dll", "dll:BepInEx/plugins/Dreanegade-Magic_Supremacy/Magic_Supremacy.dll", "cfg:BepInEx/config/blacks7ar.MagicPlugin.cfg", "cfg:BepInEx/config/seneaL.valheim.ui.cfg", "cfg:BepInEx/config/treextr.pathofvalheiman.cfg"]
claims: {"inv003.magicplugin.disk-tome-toggle.20261025": "Off"}
contradicts: []
supersedes: []
---

# MagicPlugin books and the two Tome equipment routes

## Finding and limits

**Current experiment state:** cleanup is complete. The probe aborted at the exact identity
guard before taking a snapshot; runtime slot acceptance remains unverified. After the user's
normal exit, the original non-diagnostic DLL and production source were restored. Valheim is
stopped. See the cleanup section at the end for hashes, validation and configuration drift.
Earlier pending/preflight descriptions below are historical checkpoints.

## Paused checkpoint and next-session work

The current design preference is **Magic Supremacy as the authoritative Tome equipment system**.
No bridge exists and the visible slot has not been runtime-confirmed. Read the final cleanup
and failed-probe sections before using earlier configuration recommendations or pending handoffs.
The original deployed DLL is restored; ordinary builds contain no temporary instrumentation.

Outstanding work, deferred until development resumes:

1. Diagnose and repair the character/world identity guard while preserving the designated
   character/world allowlist and avoiding unapproved save access.
2. Repeat the controlled runtime Tome investigation with explicit deployment/launch authorization
   and manual user selection of PoVPlus_Test / POVPlus_Lab.
3. Confirm the bound visible Tome descriptor, equipment authority, actual item metadata and
   acceptance results for all three MagicPlugin books and the Magic Supremacy control.
4. Design the MagicPlugin-to-Magic-Supremacy compatibility bridge after obtaining that evidence;
   address lifecycle, effects/regen, duplicate equipment, persistence and version compatibility.
5. Review the observed seneaL.valheim.ui.cfg and sighsorry.Clan.cfg content changes without
   assuming their cause or restoring guessed settings. The baseline retained hashes, not contents.
6. Keep the separate Epic Loot shardstone variant error documented; investigate independently
   if warranted, without assuming it is related to Tome compatibility.

Committed checkpoint scope is investigation knowledge and the artifact safety test. Temporary
probe source archives, logs, receipts, backups, cache fingerprints and reports remain ignored
under .dev-state/; they are local evidence, not prerequisites for the ordinary Release build or
its default exclusion test. Repeating the historical probe requires reviewed new instrumentation;
the optional test -Enabled mode checks a supplied diagnostic artifact, it does not create one.

## Earlier findings and investigation history

**Runtime authorization preflight outcome:** the later preflight section records why the
requested visible-cell proof needs a loaded local player. Under the user's explicit stop
condition, no launch or temporary probe deployment occurred; no new runtime acceptance results
are claimed. The game remains stopped and the normal PoVPlus DLL remains installed.

**Follow-up:** the ownership analysis appended below narrows the likely visible slot to
SeneaL's bridged Magic Supremacy descriptor `AzuEPI.emulated/$tome_DO`. It refines the
initial toggle-first candidate: identify the live descriptor before enabling any extra slot.
The initial observations below remain historical static findings, not runtime results.

The user reports that the Beginner's Magic Book cannot enter the panel's Tome slot.
Its installed origin is **MagicPlugin 2.2.2**, GUID `blacks7ar.MagicPlugin`, publisher/
assembly author metadata `blacks7ar`; prefab **`BMP_BeginnersMagicBook`**. Embedded English
localization spells its name **Beginners Magic Book**. Its registration installs an
equipment status effect: this is magical equipment, not merely a spell-learning consumable.

**Verified sufficient blocker for PoV's native Tome cell:** MagicPlugin's disk setting
`[2- General] Enable Tome Slot = Off` causes its public `IsTomeItem` predicate to return false
before examining an item. PoV calls that predicate. No new per-item registration is needed
to explain this rejection. Both other registered MagicPlugin books share this blocker.

**Important competing route:** installed SeneaL UI also integrates Magic Supremacy's own
Tome system, which accepts its registered prefabs rather than arbitrary magical books.
The existing log confirms that Tome slot registration succeeded. A visible label alone
does not establish which route the user selected. Enabling MagicPlugin's toggle is a
candidate configuration remedy for the native cell, not a demonstrated remedy for that
other slot. Runtime ownership, effective synced configuration and instantiated item data
remain unmeasured. This investigation therefore remains open.

Category under [[DEV-004-Intervention-framework]]: **unresolved compatibility hypothesis
with verified configuration/acceptance mechanisms**. Wanting all suitable books in one
slot is also an integration/design requirement; rejection by an intentional whitelist is
not, by itself, an upstream technical defect.

## Evidence scope and reproducibility

All installation paths below are relative to `S:\Valheim-Modding\PoVPlus-Dev`.
Read-only Mono.Cecil inspection examined method bodies and attributes without executing
plugin code. Tokens below are MethodDef tokens; IL offsets apply only to these fingerprints.

| Assembly path under BepInEx/plugins | Package / assembly version | SHA-256 |
|---|---|---|
| blacks7ar-MagicPlugin/MagicPlugin.dll | 2.2.2 / 2.2.2.0 | `6504C2FF9CEAAE7B317820E41EF84D8C28D33FEDBF32B7D0F4FDDFD680BDA795` |
| treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll | 5.3.2 / 1.0.0.0 | `83BD50F23D9C301FE469B5FBE2C430FD35C928D7A2CCFD99AF5A1040558C9D72` |
| seneaL-SeneaL_API/SeneaL-API/SeneaL-API.dll | 1.2.0 / 1.0.0.0 | `2EF9EB8DD3AB7DC72E7F3A908EFF843C4C7FAD19570CB46C3565BC36841E3574` |
| seneaL-SeneaL_UI/SeneaL-UI/SeneaL-UI.dll | 1.2.7 / 0.1.0.0 | `53B0F675BFFD0BAC5FCCA711AF04AA1EF00BB5BECD543D702EBAA146E5C487CD` |
| Dreanegade-Magic_Supremacy/Magic_Supremacy.dll | 3.1.5 / 3.1.5.0 | `8BAFE7DAEFB9B6AF057FC3B10534764ED9CCEBA430B526A014B2DD12CFABA787` |

Recheck with `Get-FileHash -Algorithm SHA256 -LiteralPath <absolute-DLL-path>`;
inspect the listed members with a metadata/IL reader. Temporary reader scripts and excerpts
are ignored under `.dev-state/investigations/magic-tome/`; this record retains the findings
and locators independently of those disposable artifacts. No third-party DLL/assets are
added to the repository. Runtime baseline remains [[DEP-001-Environment]].

## Item registration, classification and intended use

`MagicPlugin.Functions.PrefabsSetup.AdvanceBook`, `BeginnersBook`, `DruidsTome`
(tokens `0x06000130`, `131`, `132`) each construct an `ItemManager.Item` from the embedded
`MagicPlugin.assets.magicbundle`, register crafting requirements and obtain its `ItemDrop`.
They create `MagicPlugin.SE.AddEitr`, name it respectively `bmp_advance_magicbook`,
`bmp_beginners_magicbook`, `bmp_druidstome`, and assign `m_shared.m_equipStatusEffect`.
Config change handlers update the bonus. This supports equipment intent independently of
the words book/tome. No spell-learning or consumption action was found in these registrations.

The three methods do **not** assign `m_shared.m_itemType`; that initial value is serialized
in the asset bundle. It was not decoded in this investigation. Likewise actual instantiated
`m_name`, components, custom-data contents and later mutations were not captured. Localization
contains the expected keys `$bmp_beginners_magicbook`, `$bmp_advance_magicbook`, `$bmp_druidstome`;
their use as actual shared names is strongly supported, not an ObjectDB observation.
Do not label their actual type Utility as a verified fact until prefab/runtime data is read.
Game enum metadata and PoV routing verify **Utility = 18** as the required native-slot type.

The local generated `BepInEx/config/DataForge/items.reference.yml:8856-8908` records equip
effects and Eitr regeneration modifiers **0.15 / 0.25 / 0.25** for beginner/advance/druid.
This is a generated observation, not an active override or proof of current runtime values.
MagicPlugin.cfg sections Beginners Magic Book, Advance Magic Book and Druids Tome specify
Eitr bonuses **24 / 48 / 62**. `DataForge/items.yml` and `effects.yml` contain commented
schema/examples, including MagicPlugin effect override support; no active book entries were
found there. These files do not establish an effective book reclassification.

## Tome ownership and exact acceptance paths

### PoV native equipment

The owner is `treextr.pathofvalheiman`, with logic in `PassiveTree.Gear` and the supported
facade `Valheiman.Api.Equipment`. The configured `Equipment system = true` is at
`BepInEx/config/treextr.pathofvalheiman.cfg:1286`.

1. `Equipment.Accepts(string, ItemDrop.ItemData)` (`0x06000090`) delegates to State acceptance.
2. `State.UtilitySlotFor` (`0x060025ab`) rejects null/non-Utility items, then asks
   `MagicSlots.IsTome`, then `IsEarring`, before other utility routing.
3. `MagicSlots.Probe` (`0x0600249a`) finds plugin `blacks7ar.MagicPlugin`, type
   `MagicPlugin.Functions.MagicSlot`, and public `IsTomeItem` / `IsEarringItem` methods.
4. `MagicSlot.IsTomeItem` (`0x06000109`, `IL_0000-000d`) returns false if `_enableTomeSlot`
   is Off. Otherwise it tests shared-name prefix `$bmp_` and suffix `_magicbook` or `tome`.
   This is a configurable name classifier, not a per-prefab whitelist.
5. `HasTome` (`0x0600249b`) checks method availability, not the toggle value. A slot can
   therefore exist while its effective classifier rejects every item.

Installed `Equipment.Describe` (`0x0600009d`) maps enum 58 to **`utility.tome`** and 59 to
`utility.earring`. `State.Accepts` (`0x060025b7`) compares the requested utility slot with
the routing result. Making only the UI accept an item would leave this guard inconsistent.

Installed public signatures verified: `bool Accepts(string, ItemData)`,
`bool IsOpen(Player, string)`, `bool Put(Player, string, ItemData)`,
`void TakeOut(Player, string)`, `ItemData ItemIn(Player, string)`, and `Cells` / `Cell(string)`.
No Tome-classifier registration method was found on this facade. Hosting/storage APIs do
not grant permission to override acceptance. Internal FakeTome/FakeEarring delegates belong
to self-tests, not a supported integration contract.

[Official API documentation](https://valheiman.com/api) identifies `Valheiman.Api` as the
stable contract, requires main-thread calls and feature checks, and treats `PassiveTree.*`
as unstable internals. Its cell examples are less complete than installed metadata.
[PoV 5.1.1 release notes](https://valheiman.com/patches/5.1.1/?lang=en) describe MagicPlugin
Tome/earring integration. External documentation corroborates intent; local IL determines
the specific installed behavior.

### SeneaL UI, Azu API redirection and Magic Supremacy

`LogOutput.log:126-129` records MagicPlugin loading and SeneaL API Early redirecting its
embedded Azu `IsLoaded`, `AddSlot`, `RemoveSlot` stubs. Lines 164-167 show the analogous
Magic Supremacy bridge and `[Tome] ... Tome slot added: True`. There is no mapped real AzuEPI
plugin in the accepted inventory; that does **not** make these bridged paths inactive.
`SeneaL.API.StubBridge.Answer` (`0x06000027`) can answer true when SeneaL UI exists.

Magic Supremacy (`Dreanegade.Magic_Supremacy`) registers seven `Armor*callerTome_DO` prefabs.
Its private `Magic_SupremacyPlugin.IsTomeItem` (`0x06000501`) explicitly accepts those seven,
not BMP books. `CustomSlotSystem.IsItemValidForSlot` (`0x0600044c`) checks the shared slot
prefab registry (`DO_CustomSlotSystem_SlotPrefabs`); initial local definitions contain those
seven. Other runtime registry contributions remain unmeasured. Public getters/setters for
custom equipment exist, but the inspected registry helpers are private; public visibility
alone is not a documented third-party registration contract.

SeneaL UI 1.2.7 `Mods.MagicSupremacy.Probe` (`0x06000679`) binds that custom-slot system.
`Tome` (`0x0600067b`) requires Utility and delegates to `IsCustomSlotItem`; `Owns`
(`0x0600067c`) also depends on PoV dual-utility state. `Inv.Slots.SlotRaw` (`0x06000d63`)
routes owned items to seat 13; `Accepts` (`0x06000d33`) consults SlotFor/ownership and
PoV dual-utility rules. It is not simply PoV's `utility.tome` classifier.

When using PoV-hosted cells, SeneaL `Mods.PovGear.Accepts` (`0x06000729`) calls the public
PoV predicate and IsOpen, with an internal fallback. `DrawsSlot` (`0x06000712`) recognizes
owner `treextr.pathofvalheiman` and IDs beginning `pov.`. Thus the panel renderer and
equipment authority can differ. `PovEquipment = false` is present at UI cfg:6920, but no
binding of that literal was found in installed UI code; **do not infer ownership from it**.
`PovOwnSlots = false` (:6858) controls row/food sizing, not Tome classification.
The actual displayed cell ID/owner must be recorded before choosing either fix.

## Related-item compatibility matrix

All BMP rows originate in MagicPlugin **2.2.2**. `B` means the config-gated shared-name
classifier above; `E` means the separate `$bmp_*_earring` classifier (currently On).
`U?` means actual serialized/runtime ItemType remains unverified; Utility is required.
"Rejected (code)" is a conditional static result with the observed disk settings, not
a newly performed drag/drop test. All matrix effects require runtime confirmation.

| Item | Prefab | Origin | Intended function | Current classification/metadata | Tome eligibility | Current compatibility | Failure mechanism | Recommended treatment | Confidence |
|---|---|---|---|---|---|---|---|---|---|
| Beginners Magic Book | BMP_BeginnersMagicBook | MagicPlugin 2.2.2 | Equipment, +24 Eitr | U?; B; equip SE bmp_beginners_magicbook; reference regen .15 | Yes | User-reported rejected; native rejected (code) | B Off; separate MS route also excludes it initially | Configure native route after ownership test | Verified gate; runtime open |
| Advance Magic Book | BMP_AdvanceMagicBook | MagicPlugin 2.2.2 | Equipment, +48 Eitr | U?; B; SE bmp_advance_magicbook; reference regen .25 | Yes | Native rejected (code); runtime untested | Same B Off | Same shared treatment | Verified gate; runtime open |
| Druids Tome | BMP_DruidsTome | MagicPlugin 2.2.2 | Equipment, +62 Eitr | U?; B; SE bmp_druidstome; reference regen .25 | Yes | Native rejected (code); runtime untested | Same B Off | Same shared treatment | Verified gate; runtime open |
| Dvergr Earring | BMP_Dvergr_Earring | MagicPlugin 2.2.2 | Accessory | U?; E, not B | No: earring | Tome excluded by classifier; earring untested | Different equipment family | Exclude from Tome; test earring regression | Strongly supported |
| Eitr Earring | BMP_Eitr_Earring | MagicPlugin 2.2.2 | Accessory, Eitr | U?; E; SE bmp_eitr_earring | No: earring | Same | Different family | Exclude; retain earring effects | Strongly supported |
| Fire Resist Earring | BMP_FireResist_Earring | MagicPlugin 2.2.2 | Resistance accessory | U?; E; SE bmp_fireresist_earring | No: earring | Same | Different family | Exclude | Strongly supported |
| Frost Resist Earring | BMP_FrostResist_Earring | MagicPlugin 2.2.2 | Resistance accessory | U?; E; SE bmp_frostresist_earring | No: earring | Same | Different family | Exclude | Strongly supported |
| Poison Resist Earring | BMP_PoisonResist_Earring | MagicPlugin 2.2.2 | Resistance accessory | U?; E; SE bmp_poisonresist_earring | No: earring | Same | Different family | Exclude | Strongly supported |
| Dvergrs Belt | BMP_DvergrBelt | MagicPlugin 2.2.2 | Belt | Belt registration; actual type unverified | No | Runtime untested | Not a book-name match | Exclude; retain belt routing | Strongly supported |
| Wizards Belt | BMP_EitrBelt | MagicPlugin 2.2.2 | Eitr belt | Same family | No | Runtime untested | Same | Exclude | Strongly supported |
| Elementalist Belt | BMP_ElementalistBelt | MagicPlugin 2.2.2 | Belt | Same family | No | Runtime untested | Same | Exclude | Strongly supported |
| Hogwarts Belt | BMP_HogwartsBelt | MagicPlugin 2.2.2 | Belt | Same family | No | Runtime untested | Same | Exclude | Strongly supported |
| Haldors Belt | BMP_LightBelt | MagicPlugin 2.2.2 | Belt | Same family | No | Runtime untested | Same | Exclude | Strongly supported |
| Necromancers Belt | BMP_NecromancerBelt | MagicPlugin 2.2.2 | Belt | Same family | No | Runtime untested | Same | Exclude | Strongly supported |
| Dvergr/Eitr/Fire Resist/Frost Resist/Poison Resist Rings | Generated Jewelcrafting templates; actual prefab IDs unverified | MagicPlugin 2.2.2 conditional JC integration | Five ring accessories | $jc_ring_* keys; CreateRingFromTemplate Blue/Purple/Red/Cyan/Green | No: ring | No mapped JC plugin; creation unverified | Conditional integration, not Tome classification | Exclude; do not create phantom items | Verified code, installation availability unverified |
| Dvergr/Eitr/Fire Resist/Frost Resist/Poison Resist Necklaces | Generated Jewelcrafting templates; actual prefab IDs unverified | MagicPlugin 2.2.2 conditional JC integration | Five necklaces | $jc_necklace_* keys; CreateNecklaceFromTemplate same five colors | No: necklace | Same | Same | Exclude | Verified code, availability unverified |
| Mushroomcaller Tome (control candidate) | ArmorMushroomcallerTome_DO | Magic Supremacy 3.1.5 | Custom equipment/Inspiration source | Explicit MS whitelist/registry; initial serialized type not decoded | Yes, MS route | MS predicate accepts (code); native MagicPlugin predicate excludes its name; runtime untested | Different ownership/classifier | Preserve as MS-route control | Verified predicate, runtime open |

The other six MS controls are `ArmorDeathcallerTome_DO`, `ArmorFrostcallerTome_DO`,
`ArmorFirecallerTome_DO`, `ArmorStonecallerTome_DO`, `ArmorLightcallerTome_DO`,
`ArmorStormcallerTome_DO`. They are comparators, not items from MagicPlugin.
There is **no runtime-verified known-working Tome control** in the evidence collected here.
PoV's synthetic TomesStep self-test (`0x06004fb8`) exercises fake classifier delegates;
it is useful implementation evidence, not proof that a real prefab works with current settings.

Family enumeration: PrefabsSetup `0x0600012b-138` covers five earrings, three books, six belts;
`JCAdditions.RingsAndNecklaces` `0x060000c6-cf` covers ten conditional accessories.
Embedded `MagicPlugin.translations.English.yml` provides display names. No additional grimoire
or spell-learning book was found in these registrations. Wands/staves/scepters and summoning
totems are weapons/activation systems; armor, food, mead and crafting materials are excluded
from Tome adaptation. This is complete coverage of the inspected book/accessory registrations,
not a claim that every item and component in the asset bundle was decoded.

## Equipment effects, storage and risks

PoV's `UtilitySlots/Equip.Prefix` (`0x06005586`) routes recognized utility items through
`UtilitySlots.Put` (`0x060025f0`), cell movement and refresh. Loading already placed virtual
utility items can clear `m_equipped`; that flag alone is not a reliable equipped-state query.
`UtilitySlots/Effects.Add` (`0x06005589`, on Humanoid.UpdateEquipmentStatusEffects) contributes
their `m_equipStatusEffect` to the status-effect set. `MagicSlots/Regen.Postfix`
(`0x060054e6`) adds accepted, non-pending Tome/earring regen only when `m_equipped` is false.
MagicPlugin's `PlayerPatch.GetEquipmentEitrRegenModifier_Postfix` (`0x06000082`) instead sums
matching items returned by Inventory.GetEquippedItems. The guards show deliberate support
for virtual utility effects and avoiding simple double counting; active patch order still needs testing.

MagicPlugin `PlayerPatch.GetTotalFoodValue_Postfix` (`0x0600007f`) / `SetEitr` (`0x06000085`)
derive bonus Eitr from presence of the named status effects. Hence retaining the original
equip SE should preserve the intended bonuses; changing only ItemType/name or forcing UI
acceptance cannot establish this. No proposed fix should add a second copy of the same Eitr bonus.

MagicPlugin's custom Humanoid equip/unequip/status hooks (`0x0600006a-6c`) and
MagicSlot.AddCustomSlot (`0x06000111`) check Azu.API.IsLoaded. Because of SeneaL redirection,
their behavior must be evaluated with the bridge present. MagicSlot.SetTomeItem (`0x0600010f`)
writes visual state to ZDO key TomeItem for the network owner. Enabling the setting can register
another custom slot, not just change a predicate: duplicate ownership/visuals/effects are test risks.

PoV Put uses either inventory equipment-row positions or a selected external storage provider
through Cells.MoveIn/MoveOut. EquipRows.Remember (`0x0600245f`) and Place (`0x0600245c`)
preserve rowStart/talentRows in Player.m_customData under `valheiman.equipment.v1.*`.
Magic Supremacy separately has saved equipped GUID helpers (`0x06000464-466`), custom equip/
unequip and status-effect methods, and network visual keys. These mechanisms establish
different ownership models; they do not prove round-trip preservation for BMP books routed
through an altered MS registry. Logout/reload, full inventory, missing-mod fallback, multiplayer
visual synchronization and loss prevention remain untested. Do not modify saved equipment
keys or serialize a new ownership model for an acceptance-only workaround.

## Intervention evaluation and recommendation

| Option | Assessment |
|---|---|
| Configure existing MagicPlugin toggle | Smallest native-route candidate: one supported setting, constant-time existing predicate, no new PoVPlus code or per-item list. Requires authorization, effective server-sync check, reason for current Off, and checking SeneaL-created slots/effects. Not a fix for MS's separate whitelist. |
| Integrate through public APIs | Use verified PoV Accepts/IsOpen/ItemIn for diagnostics and Put/TakeOut only in approved tests. No supported PoV Tome registration was found. SeneaL CustomSlots.Add(owner, SlotDef) exists, but registering a new slot is not extending PoV's classifier or MS equipment lifecycle. Request an upstream classifier/adapter hook if needed. |
| Targeted acceptance patch | Fragile if only UI, State or a whitelist is changed: origin equipment fields, regen, effects and persistence may disagree. A version-guarded central classifier bridge is preferable to three prefab-specific Harmony patches only if configuration cannot meet the confirmed routing requirement. |
| Centralized adapter | Appropriate for a verified shared problem across the three books. Define provider/slot ownership, capability checks, one canonical family predicate and native effect responsibility; data-drive only verified family exceptions. Soft dependencies, fail closed with diagnostics on API drift, no recurring inventory-wide scans. Design remains provisional until route/effect tests. |
| Replace equipment integration | Permitted by project philosophy but not justified by present evidence. Adds save migration, networking and lifecycle responsibility where a supported setting may suffice. Revisit if ownership is fundamentally incompatible, rather than stacking fragile patches. |
| Remove/exclude | Keep unrelated magical items out of Tome; preserve MS's existing equipment/Inspiration behavior. Disabling an existing owner requires separate gameplay rationale and safe migration. |

**Recommendation:** first perform a read-only runtime ownership/classification probe, then,
if the desired cell is native `utility.tome`, test the supported toggle On with explicit approval.
Do not implement a patch yet. If the selected cell is the MS Tome, clarify whether the intended
design is separate native slots or one unified Tome slot; investigate a single ownership adapter
before modifying a whitelist. Current Off may be an intentional modpack choice: its author
rationale and server overrides were not established, and are not to be silently overridden.

Classification hypothesis: **supported at the acceptance-policy level**, including a disabled
classifier and a distinct explicit registry. **Not established as an incorrect serialized ItemType,
missing ItemManager registration, or defective beginner item.** Merely classifying all magic
items as Utility/Tome would be incorrect and could bypass intentional integration rules.

## Controlled verification plan — requires separate authorization

1. Use [[DEV-003-Workflow]] Status/Diagnose; verify deployed/build hashes and record new session
   boundaries. Do not redeploy unless a separately reviewed diagnostic probe is required.
   Launch, temporary diagnostics deployment and configuration changes each need authorization.
2. Establish safe disposable character/world testing explicitly. The client may use normal saves;
   do not rely on -savedir or load a personal character merely to test. No save access occurred here.
3. First probe without changing configuration: capture panel renderer, cell owner/ID, PoV feature
   availability, Cells and IsOpen, SeneaL slot descriptors and effective synced toggle. Read ObjectDB
   prefab data for the three books: shared name/type, components, custom data, equip SE and regen.
   Evaluate MagicPlugin.IsTomeItem and Equipment.Accepts("utility.tome", item), plus the actual
   displayed cell predicate. Record false/closed/unknown separately.
4. Use Mushroomcaller Tome as the **MS code-accepted control candidate**, verifying its actual
   positive result first. Use Druids Tome as native-route positive candidate only after approved
   enablement. There is no justified assertion that either currently works in the user's UI.
   Test beginner plus advance/druid, an Eitr Earring and ordinary food/weapon negative controls.
5. If approved, change only the verified setting for an A/B comparison, preserving its original
   value. Check effective server value and custom-slot descriptors again before moving items.
   Stop on duplicate Tome ownership, unexpected slots or failed control.
6. Equip/unequip through the owning lifecycle; compare Eitr maximum, regen and status effects
   to baseline (configured +24/+48/+62, subject to effective overrides). Verify replacement,
   swap, full bag and removal return exactly one item and remove effects exactly once. Recheck
   MS Inspiration and earrings; never grant MS Inspiration to BMP books incidentally.
7. With explicit save-test permission, verify controlled logout/reload and owner restart, including
   item identity/count/quality/custom data and equipped restoration. Plan dependency-absent/update
   tests in fixtures or a separately authorized isolated installation, not by deleting installed mods.
   Test multiplayer only with approved peers: local effects and network visuals are distinct checks.
8. Diagnose the same session's BepInEx and LoadTimeProfiler logs; inspect Harmony owners/order
   if effects disagree. [[DIAG-001-Logging]] explains filtering: absent Info is not failed execution.
   Stop on exceptions, lost items or duplicated effects; return the approved config to its baseline
   only through the authorized test procedure, and record results without rewriting this history.

## Existing diagnostics and unresolved questions

Read-only log observation: LogOutput.log was 68,106 bytes, last-write UTC **2026-10-25 06:39:45**.
Profiler `config/LoadTimeProfiler/2026-10-24_23-37-47.log` identifies session start
**2026-10-24 23:37:47 -07:00**, completed to lobby. Policy default Errors/Warnings, with
Information enabled only for povplus.core. It includes MagicPlugin/SeneaL/MS lifecycle timings.
Neither the load announcements nor combined timings prove a book's equip path ran.
No new session was produced; the executable timestamp in the BepInEx header is not session time.

Remaining evidence: actual clicked cell identity; live slot/provider registrations and patch order;
actual prefab ItemType/name/components; effective server-synced toggle; rationale for Off;
positive control in the same cell; exact effects/persistence/network round trips. Existing unrelated
compatibility warnings retain their earlier status in [[COMP-001-Warnings]].

## Investigation checks

Documentation-only changes; installed files were read, not written. No gameplay source, builds,
deployment, configuration changes, launches, save access, commits or pushes were performed.
Validation on 2026-10-25:

- `python -B Scripts/knowledge.py validate --strict`: 117 Markdown records, zero errors/warnings;
  this investigation correctly remains in the open queue. Validation does not read the live install.
- `python -B -m unittest discover -s Scripts/tests -v`: all 12 tests passed.
- `git diff --check`: passed; the new, untracked record was separately checked for trailing
  whitespace. Reviewed all eight existing-file diffs and this new record: only Knowledge Markdown
  changed; no Source, Scripts, root instructions, README or historical investigation changes.
- Research helper/excerpts remain ignored under `.dev-state/`; no binaries/assets are staged.

These are documentation/schema checks, not runtime equipment tests. Any later runtime findings
must append or explicitly qualify these static results.

## Follow-up: slot identity and restriction analysis, 2026-10-25

### Verified registration identity versus visible-instance evidence

The five inspected DLL fingerprints still match the table above. Additional IL inspection
resolves three different IDs, which must not be treated as synonyms:

| System | Owner / identity | Acceptance | Current interpretation |
|---|---|---|---|
| PoV native cell | `treextr.pathofvalheiman`; public cell `utility.tome`; enum UtilityTome = 58 | Utility type plus MagicPlugin.IsTomeItem | Exists in PoV, but is absent from the inspected SeneaL PoV cell presentation lists |
| Magic Supremacy bridged descriptor | SeneaL API owner `AzuEPI.emulated`; ID/title `$tome_DO`; key `AzuEPI.emulated/$tome_DO`; origin resolved from Magic_Supremacy | Registered callback Magic_SupremacyPlugin.IsTomeItem through the emulation wrapper | Leading identification of the visible linked Tome slot |
| Magic Supremacy equipment state | Internal CustomSlotSystem slot `tome`, shared registry key DO_CustomSlotSystem_SlotPrefabs | Membership in registry's tome prefab set | Equipment authority behind the MS descriptor, not a PoV API cell |
| MagicPlugin's optional bridged descriptor | SeneaL owner `AzuEPI.emulated`; ID/title `Tome` | MagicPlugin.Functions.MagicSlot.IsTomeItem | Requested separately when the MagicPlugin toggle is On; Off requests removal |

Evidence chain for the MS descriptor:

1. `Magic_SupremacyPlugin.Awake`, token `0x060004fc`, `IL_5996-59f6`, calls the
   embedded Azu API with name `$tome_DO`, getter GetEquippedTomeItem and predicate
   IsTomeItem. The existing registration-success message is the already-recorded log:167.
2. `SeneaL.API.StubHost.Emulate`, `0x06000032`, builds a SlotDef using the **untranslated
   input name as ID**, sets Mode Linked (2), Source and Accepts wrappers, and calls
   `CustomSlots.Add("AzuEPI.emulated", def)` at `IL_025f-026b`.
   The wrapper (`0x06000289`) returns Any of the registered acceptance callbacks.
3. SlotDef.Key (`0x0600020b`) concatenates Owner + `/` + Id. Display localization does
   not change the ID or merge `$tome_DO` with `Tome`.
4. SeneaL UI `CustomStore.Resync` (`0x06000a9a`) reads CustomSlots.Snapshot and places
   Linked descriptors into LinkedAll, subject to the real-Azu/native-slot exclusion.
   `InventoryView.LinkClick` (`0x06000b6e`) checks `def.Allows(item)`, inventory membership,
   then CustomSlots.Set(owner, id, item). Allows (`0x06000227`) delegates to Accepts.
5. CustomSlots.Wear (`0x060001f9`) checks Allows again and, absent a descriptor Put callback,
   calls Humanoid.EquipItem; native mod patches then provide equipment behavior. Acceptance
   is therefore separate from successful equipment and effects.

**Why the native PoV cell is not the leading visible-slot explanation:** SeneaL UI
`PovGear.BindApi` (`0x06000703`) builds its fixed ID map without utility.tome or
utility.earring; `PovGear.Read` (`0x06000737`) similarly does not add enum 58/59.
PoV `Compat.SeneaL.AddSlots` (`0x06002f2a`) exports only its `newCells` array. That array
(`0x06002f32`) contains potions, wisp, wishbone and keys, not utility.tome/earring.
Thus the inspected presentation paths do not export the native Tome cell to this UI.
This is narrower than claiming that PoV lacks a Tome implementation.

The installed UI CHANGELOG.md under seneaL-SeneaL_UI, version 1.2.6 section, confirms that
PoV's equipment is drawn automatically when available and that the old PovEquipment switch
was removed. This explains the leftover cfg entry; it is not an active owner switch.
The README.md:257 separately describes its MS Tome/belt presentation and dual-utility fallback.

**Confidence:** descriptor creation, IDs, predicate chain and omitted native mapping are verified
from installed code. Identification of the exact on-screen instance remains **strongly supported**,
not observed: no active UI object, screenshot or hovered-cell descriptor was captured. A native
PoV panel, binding failure/fallback, dynamic registration or a different running configuration
could change presentation. Do not claim the user's exact clicked cell has already been measured.

### Beginner book versus the MS control

| Question | BMP_BeginnersMagicBook | ArmorMushroomcallerTome_DO |
|---|---|---|
| Origin/registration | MagicPlugin 2.2.2; ItemManager asset-bundle item plus AddEitr equip SE | Magic Supremacy 3.1.5; Awake IL_35c9 onward constructs ItemManager item from magic_supremacy assets |
| MS descriptor IsTomeItem, token 0x06000501 | False for this exact drop-prefab name | True for this exact drop-prefab name |
| Initial MS internal tome registry | Absent | Present; CustomSlotSystem .cctor 0x06000473, IL_015f-01c2 |
| MagicPlugin predicate with effective Off | False before item inspection | False before item inspection |
| MagicPlugin predicate with On | Expected name match; actual shared name still needs measurement | Its origin/name is outside the MagicPlugin book family |
| Intended effects | Named equip SE; configured +24 Eitr, reference regen .15 | Its own custom Tome/effect/Inspiration path; not a reason to grant MS-specific bonuses to BMP items |
| Actual instantiated ItemType | Still unmeasured | Still unmeasured; do not confuse the temporary equip sentinel below with a permanent type |

For the bridged MS descriptor the exclusion is an **explicit prefab restriction**, independent
of whether the BMP book is Utility. For SeneaL's native MS/belt presentation, its Tome helper
also checks Utility and its custom registry, so classification and ownership are additional gates.
The direct callback, shared registry and UI routing are separate layers; extending only one
would not produce a complete integration.

There is no evidence here of an incompatible Azu API signature causing this rejection:
SeneaL redirects the installed signature and the existing startup reports successful registration.
That success is not proof of a successful equip. Missing item registration and wrong ItemType
remain unsupported explanations for this **specific descriptor's** deterministic exclusion.

### Is Off an intentional modpack policy?

MagicPlugin cfg:28-32 records upstream default On and installed value Off. This establishes a
nondefault configuration, **not who selected it or why**. A targeted Cecil scan of all 90 plugin
DLLs found the literal Enable Tome Slot / _enableTomeSlot and direct field references only in
MagicPlugin itself. The scan did not execute assemblies. It does not exclude generic config
writers, dynamic reflection, preloader changes or server synchronization.

A text search of installed Markdown/cfg/YAML/text sources found no explanation tying this Off
value to avoiding MS equipment competition. Therefore author intent is **unconfirmed**. Avoiding
extra slots is plausible; it must not be recorded as an established modpack-author decision.
Missing evidence is the pack's configuration provenance/changelog or an author statement.

One relevant author safeguard **is** documented: installed SeneaL UI CHANGELOG.md:60 states
that MagicPlugin tomes could equip twice, as belt and Tome, and that mod-claimed items are now
refused by the belt cell. The corresponding Slots.Accepts/CustomStore.Claimed path exists in
installed code. Preserve this adaptation; do not bypass belt rejection to make the book fit.

### Consequences of enabling MagicPlugin's setting

`MagicSlot.<AddCustomSlot>g__AddSlot|19_2` (`0x06000113`) reads the toggle: On issues
Azu AddSlot with ID `Tome`, Off issues RemoveSlot with that ID. The registration caller and
config change handler exist in MagicPlugin.Awake. In the observed SeneaL bridge, `Tome` and
`$tome_DO` are distinct emulation dictionary keys, so they are not deduplicated automatically.
The embedded GetSlots returns an empty SlotInfo with initialized arrays; it does not map this
request onto the existing MS ID. Enabling also makes the PoV native classifier potentially accept
the book family and enables MagicPlugin's own predicate-dependent equip/regen paths.

Expected consequence: a separately registered MagicPlugin slot alongside MS, subject to actual
UI visibility settings, rather than extension of the existing MS slot. This may be a legitimate
separate-slot design, but is not the requested evidence of a safe single-slot integration.
ServerSync precedence, actual slot count, effect ownership and patch ordering need runtime checks.
The prior recommendation to try On applies only to an explicitly chosen native/separate-slot
design; it is **not the recommended first experiment for the likely visible MS slot**.

### Can one existing slot safely host both families?

Potentially, but safety is not demonstrated and a UI-only predicate override is insufficient:

- The MS descriptor callback and its internal registry both need to agree on the admitted family.
  Its Humanoid equip prefix (`0x0600047a`) finds a registered definition and temporarily substitutes
  ItemType 727; postfix (`0x0600047b`) restores the original type and invokes EquipSlotInternal.
  This sentinel is an intentional lifecycle device, not a prefab classification to copy.
- EquipSlotInternal (`0x0600046f`) manages prior-slot removal, m_equipped, item GUID persistence,
  original item status effect and refresh. Public SetEquippedItem (`0x0600045a`) alone merely
  changes the runtime dictionary; it is not a complete equip API and must not be used as one.
- With MagicPlugin's classifier kept Off, its regen postfix will not count BMP books.
  Vanilla Player.GetEquipmentEitrRegenModifier (game token 0x06000270) sums native equipment
  fields, not arbitrary MS custom-slot items. No direct references to that getter or
  m_eitrRegenModifier were found in the inspected Magic Supremacy assembly. An acceptance-only
  bridge therefore risks losing native book regen. Other installed patches may contribute;
  effective behavior remains unverified, not a proven current gameplay loss.
- Original equip SE presence should preserve MagicPlugin's status-based max-Eitr contribution;
  duplication, removal, refresh and dynamic effect overrides still require measurement. Do not
  apply a second synthetic Eitr bonus or grant MS Inspiration incidentally.

The smallest plausible **single-slot implementation**, after diagnostic confirmation, is one
version-guarded family adapter reusing the existing slot and one authoritative lifecycle, retaining
MagicPlugin Off and registering no new slots. It must coordinate admission, equip/unequip,
original effects, regen and persistence, preserve existing MS items and belt safeguards, and
detach safely if a dependency/API changes. No supported public MS family-registration contract
was established; prefer an upstream extension hook if available before relying on private registry
layout or multiple Harmony patches. This is a provisional design, not an approved implementation.

### Minimal read-only runtime diagnostic design — not implemented

Use a single explicit main-thread snapshot command, plus a second snapshot after opening the
inventory if needed. Log via the existing PoVPlus logger, whose Info policy is already enabled;
do not change logging configuration. No item spawning, dragging, equipping or save edits are needed.
Deployment/launch and any character/world access remain separately authorized operations.

1. Record session UTC/offset, process start, mod versions/DLL hashes, PoV Equipment.On/features,
   UI binding/host state and effective MagicPlugin ConfigEntry value. Do not call setters.
2. Enumerate `SeneaL.API.CustomSlots.Snapshot()` (verified token 0x060001f5 copies/sorts a local
   list, not the live equipment). Log each Tome-like descriptor's Owner, Id, Key, raw/localized
   Title, Origin, Mode, and delegate declaring assembly/type/method for Accepts/Source/Put/Take.
3. Associate the actual visible cell with its descriptor: installed InventoryView._links contains
   LinkCell objects with Def, Slot and Item fields. A version-guarded read of these existing
   objects can log the visible cell hierarchy/location and its Def key without clicking it.
   This is diagnostic reflection into installed UI internals, not a proposed stable gameplay API.
   Do not call Resync/Assign/CustomSlots.Set or invoke source getters solely to fill unknown fields.
   If the cell is native, report its public PoV ID instead; log ambiguous/unmapped explicitly.
4. Read existing ObjectDB prefab ItemData for BMP beginner/advance/druid, MS Mushroomcaller Tome
   and an earring. If ObjectDB is unavailable, return deferred; do not instantiate prefabs or load
   a character automatically. Log prefab ID, shared name, numeric/enum ItemType, equip SE name,
   regen modifier and component type names. Log custom-data key names only; no personal values.
5. For each item, evaluate the already-audited pure MagicPlugin and MS direct predicates,
   the selected descriptor's Allows and public PoV Equipment.Accepts("utility.tome", item).
   Invoke only audited delegates; report unaudited targets as skipped. Catch/report query
   exceptions distinctly from false. IsOpen is separate and requires existing player state.
   Avoid MS GetMatchingSlotDefinition for the probe: it calls EnsureDefinitionsInitialized and
   may populate a registry. Read an already-existing registry membership instead, or defer it.
6. Emit a compact decision table with columns item/type/selected-cell-key/selected-allows/
   MP-classifier/MS-classifier/native-PoV-accepts/open, using null/deferred/skipped rather than
   fabricated values. Include unchanged-config hashes and count of Tome descriptors. Rate-limit
   to explicit snapshots; no per-frame inventory scans or gameplay-return-value patches.

Decisive result: the hovered/rendered descriptor key is AzuEPI.emulated/$tome_DO, its callback
returns false for BMP and true for the MS control, with valid live prefab data. That confirms
the visible-slot restriction. If the UI instead identifies native utility.tome, record that
contradicting presentation observation and test the effective toggle/predicate there; do not
force the evidence into the MS explanation. A valid MS control is still code-derived until
this snapshot measures it. No runtime diagnostic has been built, deployed or run in this task.

### Revised intervention and remaining uncertainty

**First intervention: observation only.** Obtain the one-shot ownership/predicate snapshot with
current configuration intact. Do not enable MagicPlugin's toggle as a diagnostic shortcut.
If the MS route is confirmed and the user wants one shared Tome seat, review a centralized
integration adapter/upstream hook with the lifecycle requirements above. If separate slots are
desired, supported configuration can be evaluated as a different, reversible design choice.

Still unresolved: actual rendered cell identity, effective synced setting, instantiated types,
any additional runtime registry contributors, config-author rationale, complete active patch order
and measured effects/persistence/network behavior. The same three BMP books remain the supported
candidate family; this follow-up does not broaden admission to earrings, belts or materials.

Follow-up validation: strict knowledge validation passed for 117 records with zero errors and
warnings; all 12 Python tests passed; tracked diff and untracked INV-003 whitespace checks passed.
This continuation changed only INV-003 and its INDEX summary; earlier uncommitted knowledge
changes were preserved. Installed MagicPlugin/UI cfg hashes were unchanged across the final
research/documentation checks: `752768E01097C5E67B8D803754D747222D694E73784812F9E472F4B5CEEB9FD4`
and `3D94477BAB6846D19B10C51361BF791249A96EC89C533855E7E1A88C87FF6A89`, respectively.
No installation writes, configuration edits, save access, source changes, builds, deployments,
launches, equipment behavior, commits or pushes occurred. Read-only IL helpers/output are ignored.

## Runtime authorization preflight: stop before player/world access, 2026-10-25

The user authorized a controlled diagnostic but explicitly required stopping if a character or
world must be loaded to establish the missing facts. This section records that stop condition,
not a failed runtime experiment. **No game launch, probe build/deployment or new game session
was performed.** The static ownership/acceptance findings above are not upgraded to runtime facts.

### Review of ScanTomePolicy.ps1

The ignored `.dev-state/investigations/magic-tome/ScanTomePolicy.ps1` is a **static metadata
scanner**, not the proposed in-process runtime probe. It loads the installed Mono.Cecil reader,
opens plugin DLLs using ReadAssembly, recursively walks method IL for the exact config literal/
field references, writes findings to standard output and disposes each AssemblyDefinition.
It does not load plugin types for execution, attach to Valheim, invoke acceptance callbacks,
write configuration, or access saves. Its intended static use is read-only.

Correctness limitations: it cannot observe live registries, effective server-synced values,
ObjectDB ItemData, instantiated UI descriptors or actual callback results. Its per-assembly catch
prints READ ERROR and continues without a failing exit status; a caller must check those errors
before treating scan coverage as complete. Literal/member-reference matching also misses dynamic
keys/reflection/generic config writers. The previous 90-assembly result remains scoped static
evidence. Rerunning it would not answer the requested runtime questions, so it was not rerun here.

### Why the exact visible-cell proof cannot be completed at the menu

Fresh IL inspection of installed SeneaL UI (same SHA-256 as the earlier table) establishes:

1. InventoryGuiPatches.Update (`0x060002f7`, IL_0088-009e) creates InventoryView only after
   HudRoot.Built is available, using an InventoryGui. Construction is not proof that linked cells
   have been populated or are visible for a player.
2. InventoryView.Tick (`0x06000ba1`, **IL_0250-0264**) reads Player.m_localPlayer, returns
   when it is absent, and only otherwise calls Sync(Player).
3. Sync (`0x06000ba3`, IL_07fc-07ff) calls TickCustom(Player, Inventory), which
   (`0x06000b6b`, IL_00d5) calls TickLinked. TickLinked (`0x06000b6c`, IL_0013-002e)
   obtains/creates the LinkCell and assigns its Def from the shown descriptor list.

Thus a registered descriptor can be queried independently of proof that it is the player's
rendered cell. A menu-only snapshot may establish registrations and predicate decisions if the
corresponding objects/prefabs already exist, but it cannot establish the missing normal-player
cell binding through this path without a local player. A menu preview/manually constructed Player
would be a synthetic control, not the actual inventory under investigation; do not create one to
bypass the boundary. Prefab availability at the menu was not measured and is not assumed absent.

The exact rendered-cell/equipment-authority objective therefore needs an authorized disposable
character/world session. In compliance with the stop condition, no partial menu launch or probe
deployment was undertaken. This does not claim that every registry/predicate query requires a world.

### Equipment effects: additional static verification

Fresh MagicPlugin IL still matches SHA-256
`6504C2FF9CEAAE7B317820E41EF84D8C28D33FEDBF32B7D0F4FDDFD680BDA795`.
GetTotalFoodValue_Postfix (`0x0600007f`, IL_0263-02a1) calls SetEitr for the three named
book status effects and their configured bonuses. SetEitr (`0x06000085`, IL_0000-0018)
tests SEMan.HaveStatusEffect and adds the configured amount. These inspected paths do not
consult the Tome toggle or the Tome classifier. **The max-Eitr calculation is status-dependent,
not directly toggle-dependent.** Whether an equipment system applies that status correctly
is a separate, still unmeasured lifecycle question.

GetEquipmentEitrRegenModifier_Postfix (`0x06000082`, IL_000a-0053) filters equipped inventory
items through IsTomeItem. **MagicPlugin's own book regen contribution depends on the classifier,
and hence the effective toggle.** Its normal custom equip/unequip hooks also consult the
classifier. This is why retaining Off and extending another slot's acceptance alone cannot be
assumed to preserve all original effects. No player stats or equipment effects were measured.

### Required test conditions and bounded probe

The earlier one-shot diagnostic design remains applicable; no implementation is needed until
the player/world boundary is authorized. A proposed temporary probe would first check for an
existing local player and inventory view and emit deferred otherwise. It must never select a
character, join a world, construct a player or force UI synchronization on its own.

To complete runtime verification, explicitly authorize:

- A designated **disposable test character and test world**, including creating/using their test
  save files. Existing personal saves remain excluded. Establish the save boundary first; the
  development installation and -savedir do not establish isolation. An isolated Windows profile
  or another verified disposable environment is an option, not something created in this task.
- One temporary diagnostic build/deployment through the standardized PoVPlus workflow, with
  the normal DLL/receipt retained for restoration, one development launch and clean close.
- Opening that character's inventory, with no item spawning, moving, equipping, crafting or
  configuration changes. Read ObjectDB prefab data for BMP_BeginnersMagicBook and
  ArmorMushroomcallerTome_DO; actual inventory possession is not needed for predicate queries.
- Logging the **actual visible LinkCell.Def key** and delegate origins; evaluate its audited
  Allows callback, the direct MP/MS predicates and native PoV acceptance. Return deferred for
  unavailable data, rather than initializing missing registries or substituting synthetic items.
- Removal/disablement of instrumentation and restoration of the verified normal deployment
  after a clean close. Any real equip/effect/persistence test requires a later expanded scope.

The probe must log through the existing povplus.core policy, preserve all third-party files and
the Tome toggle, and fail closed on version/signature drift. Observing another installed process
or entering an arbitrary existing character/world is not a substitute for these test conditions.

### State, evidence and cleanup

`Scripts/Dev.ps1 Status` reports **Stopped** and identical build/deployed PoVPlus SHA-256
`351E948AFC75D8B5BFBEF67F2F1FBABB5A8A645AA7A5D51E9E804F7D501DF548`.
The last available BepInEx log timestamp remains 2026-10-25T06:39:45.5319273Z, belonging to
the earlier run, not this preflight. MagicPlugin cfg SHA-256 remains
`752768E01097C5E67B8D803754D747222D694E73784812F9E472F4B5CEEB9FD4`.

No temporary instrumentation was installed, so none required removal; the ordinary deployed
plugin source still contains only its startup logger. New ignored files contain metadata-reader
output only. Existing third-party DLLs/configurations and saved games were not written or accessed
as save data. Only this record and the INDEX summary are updated for this continuation.

Preflight validation: `python -B Scripts/knowledge.py validate --strict` passed (117 records,
zero errors/warnings); all 12 Python tests passed; Git diff/whitespace checks and the separate
untracked-record whitespace check passed. Source, Scripts, AGENTS.md and README have no Git
diff. Existing uncommitted knowledge work is preserved. No commit or push occurred.

## Authorized disposable-session experiment — awaiting manual interaction

The user designated existing character **PoVPlus_Test** and world **POVPlus_Lab**, authorized
temporary PoVPlus instrumentation/build/deployment, launch, normal restoration and knowledge
updates. Only the user may select/load them or interact with inventory. Magic Supremacy is the
preferred future authoritative Tome system; this is a design constraint, not bridge authorization.

Implementation: Source/PoVPlus/Diagnostics/TomeDiagnostic.cs, compiled only with
EnableTomeDiagnostic=true / POVPLUS_TOME_DIAGNOSTIC. The normal build excludes the diagnostic
type and Update entry point. The probe polls at most once per second, waits for the exact
character/world and open inventory, reads actual active LinkCell.Def instances, and captures
one bounded snapshot. Once inventory is opened, an unready view times out after 120 seconds;
there is no automatic selection, spawning, item movement or equipment/stat change.

The probe verifies the five previously audited mod DLL hashes before introspection, invokes
only audited MP/MS/direct-PoV acceptance methods and the validated emulation wrapper whose
underlying predicates must all be the audited MS IsTomeItem. Unknown descriptor/callback,
dependency drift or exception produces one ABORT and disables the probe. It does not invoke
GetMatchingSlotDefinition, source/put/take delegates, lifecycle mutators or registry initialization.
Prefab ItemData is read directly, never constructed/cloned or equipped. No Harmony patches or
third-party config bindings are installed. Prefix: [PoVPlus.TomeDiagnostic].

Predeployment validation: ordinary and diagnostic Release builds passed with zero warnings/errors;
Scripts/tests/Test-TomeDiagnostic.ps1 checks default exclusion and enabled compiled IL for foreign
field stores/direct mutation calls (353 member calls inspected). Reflection purity remains an
explicit installed-code audit, not something this artifact test proves. All 14 workflow fixture
tests and 12 Python tests passed. Strict knowledge validation has only temporary source-drift
revalidation notices pending final source restoration/review; no baseline is refreshed to hide them.

Original deployed SHA-256:
`351E948AFC75D8B5BFBEF67F2F1FBABB5A8A645AA7A5D51E9E804F7D501DF548`.
Diagnostic SHA-256:
`F5DC08C82060EA64D99FCA76674529249717B9F10380B410C0BB2664509ADCC3`.
Verified completed deployment/rollback receipt:
`.dev-state/backups/20261025T0759339396782Z-0d5f88d380b64b9cb22d69b63b224100/manifest.json`.
Its PoVPlus.dll backup matches the original hash. Preview was reviewed before deployment.
Before/deployed installation snapshots cover 2,927 files (2,906 non-log content hashes);
only the managed DLL changed. Evidence/checkpoint is in `.dev-state/inv003-runtime/`.
No saves were read for the comparison. The initial collector's overly broad .old suffix rule
stopped on a translation YAML backup; it was corrected to reject actual save suffixes without
skipping that configuration backup. A postdeployment shell wrapper misread LASTEXITCODE for a
PowerShell command; independent receipt/hash verification confirmed success without redeploying.

The local execution report is `.dev-state/reports/INV-003-runtime-tome.md`.
No runtime ownership/acceptance result or final restoration is claimed until the user opens the
inventory and later exits normally. Restore through Dev.ps1 Rollback using the exact receipt
above after confirming the process stopped; never replace the DLL while the game is running.

Launch checkpoint: Dev.ps1 started PID 26884 at local 2026-10-25 01:01:01 -07:00. Current
profiler session 2026-10-25_01-01-03 reports Start To Lobby completed (1 min 26.533 s; intro/menu
readiness excluded), BepInEx reports 88 loaded / 0 skipped / 0 failed, and the probe logged ARMED.
No Error lines were present at handoff. The agent is waiting for the user to manually open the
designated test inventory; no elapsed-time assumption or automated gameplay input is permitted.
The diagnostic remains deployed and the game running while this mandatory handoff is pending.

### Runtime collection after user confirmation — identity guard aborted

The user confirmed manually loading PoVPlus_Test into POVPlus_Lab and opening inventory.
Collection UTC **2026-10-25T08:06:29.605570Z** selected the launched session
**2026-10-25_01-01-03**, using its explicit profiler filename reference, not a latest-file guess.
LogOutput.log at collection: 98,924 bytes, SHA-256
`A44B9E3D0BA400E2049218A36D36CA30BE4A32AD811C6536EC93D0D5C8A88A3D`.
Selected evidence with original line numbers is retained in
`.dev-state/inv003-runtime/runtime-evidence.json` and the local Markdown report.

| Observation | Evidence / interpretation |
|---|---|
| Probe initialized | Log:185 ARMED |
| Probe stopped at identity boundary | Log:1430 ABORT System.InvalidOperationException: Unauthorized character/world: probe disabled without collecting their data. |
| No acceptance snapshot executed | BEGIN=0, END=0, RENDERED=0, VISIBLE_ACCEPT=0; one ABORT |
| Development client still running | PID 26884, executable S:\Valheim-Modding\PoVPlus-Dev\valheim.exe |
| User performed required interaction | User confirmation; not independently established by the aborted probe |

The exact comparison rejected at least one nonempty value from Player.GetPlayerName() /
ZNet.GetWorldName(). **It does not establish that the user selected a different character or
world.** The probe deliberately did not log actual mismatching names or even which comparison
failed, so the evidence cannot distinguish casing/spelling, API naming/initialization behavior
or another identity mismatch. No save files are inspected to resolve that uncertainty.

Implementation limitation identified from source: this guard executes **before** the open-
inventory readiness check. It can abort during player initialization, before the confirmed
inventory-open moment. A transient identity is therefore a hypothesis, not a measured cause.
The guard sets _done=true in the exception handler, so reopening inventory cannot rearm it.
No attempts were made to relax the boundary, restart the probe, patch a live process or redeploy.

Consequently the registered/rendered owner, actual item types, four acceptance comparisons and
equipment effects were **not runtime-confirmed**. The earlier MS prefab restriction and proposed
single-authority architecture remain static findings/provisional design. This failed observation
must not be presented as evidence that the book or any acceptance API failed.

One separate current-session Unity Error appears at log:1420, before the diagnostic abort:
`Variant index 1 does not exist` for the Epic Loot Cyan shardstone item; the game reports resetting
to variant 0. Its normalized signature is absent from the captured prelaunch baseline. That
baseline is a different session and does not establish novelty across the whole installation's
history. Causation and gameplay impact remain uninvestigated; it is not evidence of a Tome conflict.
No shard, configuration, third-party assembly or save correction was attempted. The probe had
not reached prefab/predicate inspection when it stopped.

The attempted full in-session installation fingerprint stopped at the game's locked
`BepInEx/cache/harmony_interop_cache.dat`. No lock was bypassed, file replaced or game process
interrupted. Final installation/config comparisons are deferred until the user exits normally;
the prelaunch deployment comparison already verified only the managed DLL changed at that stage.

**Next action:** ask the user to exit normally, verify stopped, then roll back using the recorded
receipt. Do not continue runtime testing on this failed probe. A later reviewed diagnostic revision
could retain the exact allowlist, defer measurement until inventory/player readiness, and log only
which identity check matches (and case-only/unset categories) without logging unauthorized names.
That would distinguish guard/initialization issues without granting broader character/world access.
It requires another explicit manual launch/session handoff; no such revision is implemented here.

Collection-stage checks: all 12 Python tests and all 14 PowerShell workflow fixture tests passed;
the enabled compiled diagnostic safety audit also passed. Strict knowledge validation reports
117 records, zero errors and three source revalidation warnings while temporary activation is
still present (ARCH-001, DEV-001, INV-001); final strict completion remains pending cleanup.
Git diff and separate untracked-record whitespace checks passed. MagicPlugin cfg hash still
matches the baseline, and the deployed DLL remains the diagnostic hash. No commit/push occurred.
Checkpoint stage: awaiting-user-exit-after-identity-abort. The agent has left Valheim running
and will request normal manual exit before any rollback or deployed-file change.

### Completed cleanup after normal manual exit, 2026-10-25

The user confirmed normal exit and authorized cleanup. Dev.ps1 Status independently reported
Stopped before rollback; no valheim/valheim_server process was found. The agent reviewed
`Dev.ps1 Rollback -BackupId 20261025T0759339396782Z-0d5f88d380b64b9cb22d69b63b224100 -WhatIf`,
then used the same command with -Authorized and answered the standard ShouldContinue prompt.
No safeguard was bypassed and no files were replaced while the client was running.

- **Restored deployed DLL:** SHA-256
  `351E948AFC75D8B5BFBEF67F2F1FBABB5A8A645AA7A5D51E9E804F7D501DF548`, exactly the
  original prediagnostic deployment; version 0.1.0.
- **Completed rollback receipt / diagnostic backup:**
  `.dev-state/backups/20261025T0811496603929Z-b12efed6cf6049c59dad83532f265822/manifest.json`.
  The receipt links to the diagnostic deployment and preserves its DLL outside plugin discovery.
- **Source cleanup:** temporary Plugin.cs activation/Update and csproj compile switch removed;
  the untracked diagnostic class was archived with the two temporary source versions as verified
  text under `.dev-state/inv003-runtime/source-archive/`, then removed from Source. Production
  source matches the prediagnostic Git contents. The new artifact safety test remains available.
- **Warnings resolved:** Git's autocrlf checkout initially restored equivalent CRLF source,
  while the accepted fingerprints used LF. After verifying LF-normalized bytes matched the
  accepted hashes, the two files were restored to those exact bytes. The three temporary
  revalidation warnings cleared without updating any snapshot or suppressing validation.
- **Default artifact checks:** fresh ordinary Release build succeeded with zero errors/warnings.
  Both that build and the actual restored installation DLL pass Test-TomeDiagnostic.ps1:
  no diagnostic type and no plugin polling callback. The fresh build hash
  `F78A22C61520BAC707F86C47460DE6DADCE9440112CB92CE74B02CCD610FBF63` differs from the
  preserved original binary; it was not deployed. The installed state is the exact backup,
  not an assumed byte-identical rebuild.

Final installation snapshot `.dev-state/inv003-runtime/restored.json` and comparison
`restoration-comparison.json` cover the same 2,927 files / 2,906 non-log hashes as before.
All plugin/core/game DLLs and other game assets match pretest content, including PoVPlus after
rollback. **Not every third-party configuration stayed byte-identical during the game run:**

| Config path under BepInEx/config | Prelaunch SHA-256 | Post-exit SHA-256 |
|---|---|---|
| seneaL.valheim.ui.cfg | AAD142DDB7B0BE7E84D187E8488444A4B5EC17223777DEB6D3618EC719D8729C | 63F729BFB3C3781823E8056C71159C742F982A06034326A7AA0FD094D3C61E44 |
| sighsorry.Clan.cfg | A7549E4B0411EAF2E338ECA8A1DE554BE1AD3887BA90B19304668EAAF6872560 | 62B3D1A55DB4B8068F92CDF707A7067B9EBB67F7AC96B6CC8AE71D20D8AC4D01 |

These content changes occurred between the baseline and post-exit capture. The agent did not
write/revert these files and the probe contains no configuration writes. Whether runtime
serialization, user interaction or another installed component caused each change is unverified.
The baseline stores fingerprints, not original config contents, so an exact before/after setting
diff cannot be reconstructed from it. Do not restore guessed values or refresh accepted config
fingerprints to hide this drift. **MagicPlugin.cfg is unchanged and Enable Tome Slot remains Off.**

Other changed non-log files are chainloader_typeloader.dat and harmony_interop_cache.dat,
three cache text files under BossRules/DropNSpawn, and EpicLoot/creaturesorter-report.txt.
A new session profiler log exists and one older profiler log is absent. Many files have new
timestamps but identical contents; the comparison records that distinction. No saves were read,
copied, restored or deleted by cleanup; user-driven test saves remain outside this file audit.

Remaining investigation issues: exact identity-guard mismatch, missing rendered-cell/predicate
snapshot, the separately logged Epic Loot shardstone error, and the two configuration changes.
The MS-authoritative bridge remains a provisional design and is not implemented. No new test,
game launch, configuration change, commit or push was performed during cleanup.

Final cleanup validation: ordinary Release build passed; all 14 workflow fixture tests and
12 Python tests passed; built and installed diagnostic-exclusion checks passed. Strict knowledge
validation passed for 117 records with zero errors/warnings. Tracked diff and untracked INV-003
whitespace checks passed; Source has no Git diff. The workflow record now links this verified
real rollback. No accepted snapshots were changed and no runtime test was restarted.
