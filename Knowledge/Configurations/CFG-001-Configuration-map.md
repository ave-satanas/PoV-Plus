---
id: "CFG-001-Configuration-map"
title: "Configuration ownership and overrides"
type: "configuration"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[DIAG-001-Logging]]", "[[MOD-000-Inventory]]", "[[INV-001-Startup-message]]"]
confidence: "verified"
watch: ["cfg:BepInEx/config/BepInEx.cfg", "cfg:BepInEx/config/sighsorry.LoadTimeProfiler.cfg"]
claims: {}
contradicts: []
supersedes: []
---

# Configuration ownership and overrides

## Inventory and coverage

99 cfg files are fingerprinted without copying values. [[MOD-000-Inventory]] links cfg candidates
whose filename matches a GUID case-insensitively. This is a heuristic, not ownership proof.
Custom YAML/JSON, embedded overrides and progression/balance settings remain unreviewed.
No archives or raw config copies are imported. Preserve author overrides.

## Verified diagnostic settings

BepInEx/config/BepInEx.cfg [Logging.Disk] and [Logging.Console]: Enabled=true; LogLevels includes
Fatal, Error, Warning, Message, Info. Listener allowance does not override source filtering.

BepInEx/config/sighsorry.LoadTimeProfiler.cfg currently contains:

```ini
[General]
ProfilingEnabled = true
LocalizationCacheEnabled = true
ConfigWriteCoalescingEnabled = true
ConfigAutoReloadEnabled = false
TimeoutProtectionSeconds = 120
[Logging.Mods]
povplus.core = Errors, Warnings, Information
```

This task read these settings; it did not change them. See [[DIAG-001-Logging]] and
[[INV-001-Startup-message]]. Overlap alone never establishes configuration conflict.
