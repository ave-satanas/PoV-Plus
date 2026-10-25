---
id: "COMP-001-Warnings"
title: "Compatibility warning register"
type: "compatibility"
status: "partial"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[MOD-000-Inventory]]", "[[INV-002-Compatibility-triage]]", "[[DIAG-001-Logging]]"]
confidence: "mixed"
watch: ["dll:BepInEx/plugins/RandyKnapp-EpicLoot/EpicLoot.dll", "dll:BepInEx/plugins/treextr-PathOfValheiman/PathOfValheiman/PathOfValheiman.dll"]
claims: {}
contradicts: []
supersedes: []
---

# Compatibility warning register

## Observed warnings, not automatic defects

Affected mod records: [[MOD-randyknapp-epicloot]], [[MOD-treextr-pathofvalheiman]],
[[MOD-sighsorry-usefultankards]], [[MOD-sighsorry-trolling-fishing]],
[[MOD-sighsorry-finedining]], [[MOD-sighsorry-secondaryattacks]].

Current BepInEx LogOutput.log, 18:48:04 session. Line locators will drift on subsequent runs.
Versions/GUIDs/dependencies belong to [[MOD-000-Inventory]].

| Concern | Evidence | Interpretation / next step |
|---|---|---|
| EpicLoot input | 62-63, 272-273: UsefulTankards/TrollingFishing cannot find EpicLoot.Config.ELConfig.SocketOverlayModifier | Explicit fallback leaves input to EpicLoot; inspect actual members before correction |
| FineDining food cards | 239: PassiveTree.UI.ItemTooltip.owners missing | Integration unavailable; log says existing food gameplay unchanged |
| SecondaryAttacks | 256-257: EpicLoot 0.15.1 Attack_FireProjectileBurst_Prefix; PoV 5.3.2 ProjectileMods.burstFor missing | Compatibility branch unavailable; gameplay impact untested |
| Crossbows category | 201: PoV warns category has two names and mastery duplicated | Verify effective tree config and runtime behavior before editing |
| Optional probes | 187-209: BetterRiding, QuickStackStore, ConfigurationManager, ItemDrawers | Missing optional types do not prove installed mods conflict |
| Further API probes | 944-955: BetterArchery, ZInput, EpicLoot Adventure, PassiveTree Equipment/Module | Ownership and impact untraced |
| Assets/audio/shader | 46, 805, 808, 910, 935 | Separate asset/graphics concerns; no established startup linkage |

No Error/Exception matches in the sampled session. This does not certify gameplay or all other
logs. [[INV-002-Compatibility-triage]] owns research; intentional author fallbacks remain intact.
