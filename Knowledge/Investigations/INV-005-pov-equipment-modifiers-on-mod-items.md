---
id: "INV-005"
title: "PoV implicits and equipment modifiers on mod items"
type: "investigation"
status: "open"
created: "2026-11-10"
last_verified: "2026-11-15"
versions: {"PathOfValheiman": "6.0.12"}
evidence: ["Decompiled-Vault/PathOfValheiman-6.0.8/PathOfValheiman.decompiled.cs (static read, line numbers below)", "Decompiled-Vault/PathOfValheiman-6.0.12/NOTES.md, section 'Read for PoVPlus.Gear' (static read of the installed version)", "S:/Valheim-Modding/PoVPlus.Gear: AUDIT.md, PLAN.md, audit/ (in-game runs on PoV 6.0.12, 2026-11-15)"]
related: ["[[INDEX]]", "[[INV-004-pov-scaling-integration]]"]
confidence: "mixed"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# PoV implicits and equipment modifiers on mod items

## Question

A user asked for ValheimArmory items to "get PoV implicits when crafted". Can weapons and armour from
the mods in the PoVPlus stack receive PoV's implicits and its other equipment modifiers? Read from the
PoV 6.0.8 decompile only (the profile was on 6.0.6); no mod item data was inspected and nothing was
tested in game.

## Findings

Line numbers are in `Decompiled-Vault/PathOfValheiman-6.0.8/PathOfValheiman.decompiled.cs`.

| Modifier | How PoV decides | Mod items |
|---|---|---|
| Base implicits | Fixed table keyed by prefab name: `Implicits.bases` (359665), filled by 342 `Implicits.Register("<prefab>", ...)` calls in `BaseImplicits` (182821), plus staves, jewels, rings, belts and PoV's own items. `Implicits.Of` (359815) returns nothing for a prefab that is not in the table. All 342 names are vanilla (and Deep North) prefabs. | **None.** No class-based fallback exists. This is the gap the user reported; it is not a bug in the mods. |
| Rarity and affixes | `Rarity.CanHave` (416015): by item type. Weapons need a skill other than None or Fishing, and damage above 0 unless Blood or Elemental Magic. Armour, shields, capes, tools, utility and trinkets always qualify. Stack size must be 1. | Qualify, including weapons with a custom skill. A weapon with skill None does not. |
| Which affixes fit | `ItemTags.Build` (529933 onward): tags from item type, weapon profile, damage types and attack costs. | Work. A weapon whose skill PoV does not know gets no `kind.*` tag, so weapon-type affixes are not offered to it. |
| Armour, evasion or rune shield | `EquipmentDefences.Kind` (338344): item custom data `pov.defence.base`, then the config rule list "Equipment defence base rules" (prefab patterns), then metadata: eitr regeneration means rune shield, movement penalty of 4% or more means armour, otherwise evasion; helmets and capes copy the chest piece of the same set. | Work, by heuristic. A heavy mod armour with under 4% movement penalty is read as evasion. Fixable per prefab in PoV's config, no code needed. |
| Item level, base biome (affix tiers, Lord's Brand) | `CraftLevel.Level` (525893) and `ItemAscension.BaseBiome` (527969): highest biome among the recipe's materials, from a fixed table of vanilla material names (525848-525854) and four vanilla stations (525828). Unknown materials are followed through their own recipes, five deep; an unknown raw drop counts as Meadows. | **Likely too low** for items crafted mainly from a mod's own drops or at a mod's own station. Not measured. |
| Lord's Brand class | `ItemAscension.ClassOf` (527841): by weapon kind or skill plus hands, armour by slot. | Work for known skills. |

`Implicits.Inherit(prefab, donor)` (359691) and `Implicits.Register` (359708) are public static. PoV
uses `Inherit` for its own items (536129). An adapter can give a mod prefab the implicits of a vanilla
donor with one reflected call per prefab at startup; rolled values are stored on the item under
`pt.implicit`.

## "Items disappear when reforged"

Correction (owner, 2026-11-10): "reforging" here means runic forging at the runic anvil (the forge
that spends an item's potential), not the Rune of Change. An earlier version of this note looked at
the wrong feature.

At the anvil, `Craft.Commit` (518331) works on a clone, copies the result back, then saves the anvil's
inventory to its ZDO (518418-518421). `Craft.Apply` restores the item on any failure (518037-518049).
The one path found that loses an item is the game's own: `ItemDrop.ItemData.Save` writes no prefab for
an item whose `m_dropPrefab` is null, and `Inventory.Load` skips such an entry ("Failed to find item
prefab"). An item a mod handed out without its prefab therefore vanishes the next time the container
holding it is saved and read back, and the anvil does that on every action. `Potential.Eligible`
(521888) also refuses an item with no prefab. **Which mod items, if any, lack a prefab was not
established**; this is a mechanism that fits the report, not a confirmed cause.

## Update 2026-11-15: Gear audited, reworked and moved out of this repository

The sections above were written from the 6.0.8 decompile; the members they name were checked again in the
installed 6.0.12 and are unchanged (same names and signatures). What is below this section describes Gear
0.1.0 and is kept as history.

- **Gear is its own repository now:** `S:\Valheim-Modding\PoVPlus.Gear`, public at
  `github.com/ave-satanas/PoVPlus.Gear` (MIT). `Source/PoVPlus.Gear/` no longer exists here. Its `AUDIT.md`,
  `PLAN.md` and `audit/` folder are the record; this note only summarises.
- **0.1.0 works in game** (PoV 6.0.12, Gale PoVPlus profile): mod armour shows an implicit in PoV's tooltip
  model, and no Gear warning was logged. No control run without Gear was made.
- **Three defects found in 0.1.0** (measured on a world load, 1,373 items matched): PoV's own `PT_` unique items
  were used as donors for 359 items; 546 creature attacks and body pieces (items with no icon) were given
  implicits; on a second world load in one session the mod items counted as vanilla donors.
- **The donor approach itself was the flaw.** A PoV implicit is two choices: the property follows the kind of
  item, the strength follows its tier (274 of PoV's 342 ranges sit on one eight-step ladder). Copying one
  "closest" item ties them together: every mod two-handed sword copied Krom's Mistlands strength.
- **0.2.0 makes the two choices separately** and registers the result with `Implicits.Register`. Scored by
  leave-one-out over the 288 vanilla weapons, shields and armour pieces in PoV's table (exact property and
  exact range): property 72.2%, strength 75.3%. The 0.1.0 logic scores 64.2% and 45.8% on the same test.
  Property is capped near 80.9% for any rule that learns from other items; armour is the weak spot, because
  PoV chose each set's property by hand.
- **State:** 0.2.0 is built and packaged, not installed and not uploaded; the profile has 0.1.0.
- **The owner's rulings, recorded in Gear's PLAN.md and not built:** a magic weapon's property follows its
  element (spirit: curse effect); a dual weapon takes its one-handed kind's property; items of light, the
  holy, support or healing get aura area; game items PoV leaves bare stay bare; Gear keeps reflecting into
  `PassiveTree.*` until PoV's public API (1.9.0 has none) gains a call to register base implicits, which the
  owner will ask PoV's author for.
- **Item level and tier:** still PoV's own reading (`ItemAscension.BaseBiome`). 98 mod items read as Meadows
  because their materials are unknown to PoV. The same cause makes PoV misclassify modded armour as light or
  heavy (`CraftEquipmentRules`, fixed vanilla material lists): 18 of 572 armour recipes get no class. A
  material lookup that follows a mod's material one recipe back is planned in Gear for both.
- **The prefab guard** has never fired in any log seen and is untested.

## Adapter as first built: PoVPlus.Gear 0.1.0 (history)

`Source/PoVPlus.Gear/` at the time (GUID `avesatanas.povplus.gear`, 0.1.0, package `PoVPlus_Gear`; own csproj and
`pack.py`, not in `PoVPlus.sln`). No declared dependencies; in `Start` it disables itself unless
`treextr.pathofvalheiman` >= 6.0.6 is loaded and the members it calls exist. PoV types are found by
simple name plus a required method, so the lookup is the same for 6.0.6 and 6.0.8.

1. **Implicits.** Postfix on `Game.Start`. Every ObjectDB weapon, bow, shield, helmet, chest, legs and
   cape is described from a clone of its prefab data with `m_dropPrefab` set. Items already in
   `Implicits.bases` are donors. Items that are not, do not start with `PT_`, and pass
   `Rarity.CanHave`, each get `Implicits.Inherit(item, donor)`.
   Donor choice: same kind first (armour slot; shield; for weapons PoV's `ItemAscension.ClassOf`, then
   same skill and item type, same skill, same item type, any weapon), then the lowest distance:
   biome tier difference from `ItemAscension.BaseBiome` + 4 × |ln(stat ratio)| (damage, armour or block
   power) + effects (armour: `EquipmentDefences.Kind` differs +3, movement modifier, eitr regeneration;
   shield: parry bonus; weapon: elements differ +1.5, main physical type differs +0.5, eitr cost
   differs +2). Ties by ordinal name. The stat outweighs the tier because of the item-level gap above.
2. **Prefab guard.** Prefix on `ItemDrop.ItemData.Save`: an item with no `m_dropPrefab` gets it back
   from a SharedData-to-prefab map of ObjectDB and a warning names it. Installed separately; a failure
   skips only this part.

Known limits: the donor can change when the mod list changes (the item then rolls the donor's
implicit; the old value stays stored but unused); a vanilla item PoV left without implicits on purpose
would also be given one; no override list; item level on mod gear is not changed.

## Next action

Work continues in the Gear repository (its `NEXT-SESSION-PROMPT.md` and `PLAN.md`). What is still open from
this investigation:

1. Craft one weapon, one shield and one armour piece from mods and read the rolled implicit; run one through
   the runic anvil. (The match list is no longer in the log: Gear writes `BepInEx/PoVPlus.Gear-matches.txt`.)
2. Watch the log for the "had no prefab" warning: it names the mod item behind the anvil losses, if
   that is the cause.
3. Measure item level on a few mod items in game (tooltip level against the biome they belong to).
4. The owner's request to PoV's author for a public call to register base implicits.
