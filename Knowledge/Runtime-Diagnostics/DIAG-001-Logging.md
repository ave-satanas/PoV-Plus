---
id: "DIAG-001-Logging"
title: "Runtime diagnostics and logging authority"
type: "diagnostic"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[INV-001-Startup-message]]", "[[EXP-001-Logging-override]]", "[[CFG-001-Configuration-map]]", "[[COMP-001-Warnings]]"]
confidence: "verified"
watch: ["dll:BepInEx/core/BepInEx.dll", "dll:BepInEx/patchers/sighsorry-LoadTimeProfiler/LoadTimeProfiler.dll", "cfg:BepInEx/config/sighsorry.LoadTimeProfiler.cfg"]
claims: {}
contradicts: []
supersedes: []
---

# Runtime diagnostics and logging authority

## Sources and authority

| Source | Establishes | Does not establish |
|---|---|---|
| BepInEx/LogOutput.log | Messages reaching listeners; load reports/captured failures | Every attempted log or callback |
| config/LoadTimeProfiler/session.log | Effective recorded filter policy, timing interval and combined lifecycle measurements | Every individual Awake instruction |
| Installed IL/source | Declared code and hook targets | All runtime state or patch order |
| BepInEx.cfg | Listener settings at observation | Earlier run settings without session evidence |
| Profiler cfg/session | Per-GUID filtering configuration/effective policy | Absence of every other filter |

## Installed initialization path

BaseUnityPlugin inherits MonoBehaviour. Its constructor checks BepInPlugin, obtains PluginInfo,
creates the plugin-named ManualLogSource and ConfigFile object. Chainloader.Start logs Loading,
loads assembly, calls ManagerObject.AddComponent(pluginType), records instance, catches loading
exceptions and counts failures. Unity handles Awake; the announcement precedes component creation.

## Installed filtering path

Installed patcher record: [[MOD-sighsorry-loadtimeprofiler]]. The other preloader patcher is
[[MOD-sighsorry-fast-assetbundle-loader]].

LoadTimeProfiler 1.3.2 patches ManualLogSource.Log(LogLevel,object) with
LogFiltering.ManualLogPrefix (priority 800). Allows maps logger to GUID and asks LogPolicy.Allows.
PluginOwnership installs a BaseUnityPlugin constructor postfix; Constructed binds logger/GUID.
Manual filtering installs during patcher Initialize, ownership before Chainloader.Start.
Unlisted GUIDs permit Errors/Warnings. Information means Message+Info. Unity/unmapped loggers pass.
Filtering Info is silent; the BepInEx loading announcement need not share the plugin's policy.

## Other interception and patch diagnostics

Artisan_Mastery ShaderLogFilter wraps listeners but blocks selected Unity Log shader warnings
for Custom/ShadowBlob or Custom/WaterMask only. PoVLink.Diagnostics.Start adds a listener;
the inspected method does not remove existing listeners. Neither explains PoVPlus's missing Info.
FastAssetBundleLoader and LoadTimeProfiler are preloader patchers. AccessTools warnings are
API-probe evidence, not an active conflict registry. Runtime patch owner/order analysis requires
an approved runtime diagnostic; none was installed or run here.

[[INV-001-Startup-message]] and [[EXP-001-Logging-override]] own the concrete startup case.
