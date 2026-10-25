---
id: "EVID-001-Local-observations"
title: "Evidence ledger and reproducible locators"
type: "reference"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {}
evidence: ["Local evidence locators below; collected 2026-10-24 America/Los_Angeles"]
related: ["[[INV-001-Startup-message]]", "[[COMP-001-Warnings]]", "[[DEP-001-Environment]]", "[[REF-001-External-docs]]"]
confidence: "verified"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Evidence ledger and reproducible locators

## Collection boundary

2026-10-24 America/Los_Angeles (UTC date may be 2026-10-25).
Installation: S:\Valheim-Modding\PoVPlus-Dev; repository: S:\Valheim-Modding\PoV-Plus.
Retained metadata, fingerprints, short diagnostic excerpts and locators only. No personal log lines,
credentials, saves, game assets or DLL bytes copied. External logs are mutable; cite session IDs.

## E1 — current startup

BepInEx/LogOutput.log observed length 68106; LastWriteTimeUtc 2026-10-25 01:51:26.
Lines 1-6: BepInEx 5.4.23.5, pack 5.4.2351, Unity 6000.0.75.2503836, CLR 4.0.30319.42000,
Windows Bits64. Line 15 Valheim 1.0.20. Line 18 identifies profiler session 18:48:04.
183-184: loading PoVPlus then `[Info   :   PoVPlus] PoVPlus 0.1.0 loaded.`.
291: `Chainloader startup complete (88 loaded, 0 skipped, 0 failed)`.
Warning locators: [[COMP-001-Warnings]]. No Error/Exception matches in sampled session.

## E2 — profiler pair

BepInEx/config/LoadTimeProfiler/2026-10-24_18-34-47.log:11-14: default Errors/Warnings,
mods=[], Manual=True, ownership False at start/True at end. Line 115: PoVPlus combined
construction/Awake/OnEnable timing 0.000s rounded, not instruction-level proof.
2026-10-24_18-48-04.log:11-14: povplus.core=Errors, Warnings, Information at start/end;
ownership enabled by end. Start To Lobby completed, 1 min 26.869s.

## E3 — previous source/IL verification

Prior Release/deployed SHA-256:
`667C5D5BB6175D99690E9DA18DF67066612260773B62ECBD93A3F40BDDC673B7`.
MVID 0f502f4f-f0bf-44d5-811b-05165ea25f05; net48;
BepInPlugin povplus.core / PoVPlus / 0.1.0. Awake loads exact string and calls LogInfo.
Current deployed hash is in [snapshot](../Dependencies/environment-snapshot.json).
Prior installed Cecil analysis located Chainloader.Start AddComponent, BaseUnityPlugin ctor,
LogFiltering.ManualLogPrefix/Allows/PluginOwnership.Constructed and LogPolicy. Those prior verified
findings are reused; method-body fingerprints were not archived separately.

## Reproducible commands

```powershell
Get-FileHash .\Source\PoVPlus\bin\Release\net48\PoVPlus.dll
Select-String -LiteralPath 'S:\Valheim-Modding\PoVPlus-Dev\BepInEx\LogOutput.log' -Pattern 'PoVPlus|Chainloader startup complete'
.\Scripts\Get-KnowledgeSnapshot.ps1 -OutputPath "$PWD\Source\PoVPlus\obj\environment-current.json"
python Scripts/knowledge.py validate --compare-snapshot Source/PoVPlus/obj/environment-current.json
```

Outside reads may require approval. Do not use a mutable log filename alone as historical identity.
