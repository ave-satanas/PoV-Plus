---
id: "INV-001-Startup-message"
title: "Missing PoVPlus startup message"
type: "investigation"
status: "resolved"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[DIAG-001-Logging]]", "[[EXP-001-Logging-override]]", "[[EVID-001-Local-observations]]"]
confidence: "verified"
watch: ["src:Source/PoVPlus/Plugin.cs", "dll:BepInEx/plugins/PoVPlus/PoVPlus.dll", "cfg:BepInEx/config/sighsorry.LoadTimeProfiler.cfg"]
claims: {}
contradicts: []
supersedes: []
---

# Missing PoVPlus startup message

Source applicability review, 2026-10-25: the offline INV-003 repair adds an opt-in diagnostic
entry point, excluded from ordinary builds. The ordinary Awake startup LogInfo call is retained;
build/exclusion tests passed. This does not rerun or change the historical runtime/filtering
findings below. See [[DEV-005-Tome-diagnostics]] for the separate observation tooling.

## Earlier observation

18:34:47 session: BepInEx reported loading povplus.core but its own startup Info line was absent.
This was a filtering hypothesis, not proof of Awake failure. Source/Release/deployed IL showed
private instance parameterless void Awake calling LogInfo with the exact startup string;
attributes were correct, binaries matched. [[EVID-001-Local-observations]] preserves the hash.
Earlier profile had Errors/Warnings, no overrides and ownership enabled by end. Combined
construction/Awake/OnEnable timing is not individual callback proof. No log exceptions matched.

## New evidence and resolution

Current installation already contains povplus.core=Errors, Warnings, Information. Later
18:48:04 profile records it at start/end; LogOutput.log lines 183-184 show loading followed by
`[Info   :   PoVPlus] PoVPlus 0.1.0 loaded.`; line 291 reports 88 loaded, 0 skipped, 0 failed.
This task did not edit the configuration or launch the game. [[EXP-001-Logging-override]]
documents the retrospective comparison; [[DIAG-001-Logging]] owns implementation details.

Startup logging is verified in the later run. Filtering explains the earlier omission with high
confidence; no code correction is indicated. Actor/change time and isolation of every changed
variable are unknown, so this is not a fully controlled experiment. If it recurs, compare effective
policy, exact GUID, hashes and session identity before changing initialization code.
