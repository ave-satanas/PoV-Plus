---
id: "EVID-002-Persistence-and-control"
title: "Installed persistence routing and desktop-control evidence"
type: "reference"
status: "partial"
created: "2026-10-27"
last_verified: "2026-10-27"
versions: {"Valheim": "1.0.20", "Unity": "6000.0.75.2503836", "BepInEx": "5.4.23.5"}
evidence: ["Scripts/tests/Export-TestingArchitectureEvidence.ps1", "Scripts/tests/verify_testing_evidence.py", ".dev-state/dev-008/persistence-il.json", "[[DEP-001-Environment]]"]
related: ["[[DEV-008-Supervised-testing]]", "[[EXP-006-Testing-policy-fixtures]]", "[[DEV-003-Workflow]]", "[[INV-003-Magic-book-tome-compatibility]]"]
confidence: "mixed"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# DEV-008 primary evidence and scope

## Environment observed 2026-10-27

Repository clean on main at 0c1f3d1a909e0c40a7bcf36425f8efc6a1c09722; origin fetch/push
https://github.com/ave-satanas/PoV-Plus.git; core.autocrlf=true and existing scoped .gitattributes.
Research branch research/dev-008-supervised-testing. No remote fetch required for this offline
design; origin equality was last verified at the preceding checkpoint. No Claude worktree read.

Win32_OperatingSystem reports Microsoft Windows 11 Pro 10.0.26200; registry DisplayVersion25H2,
UBR9711. Registry ProductName still says Windows10 Pro; do not confuse that legacy string with
the OS caption/build. HypervisorPresent=True does not establish Hyper-V management availability,
guest graphics or a configured VM. No Valheim client process was found before research.

dotnet10.0.100; Python3.14.0; default shell Windows PowerShell5.1; existing PowerShell7 is at
C:/Users/<owner>/.cache/codex-runtimes/codex-primary-runtime/dependencies/native/powershell/pwsh.exe
(not initially on PATH). No software installed. Procmon was not found on PATH, not proven absent
elsewhere. No Steam user data, account identity, cloud settings or actual saves were inspected.
Steam.exe was running from G:/STEAM/steam.exe, FileVersion11.12.30.07. Only process/executable
metadata was read; its UI and session were left alone. No game client was running at final check.

valheim.exe and UnityPlayer.dll FileVersion6000.0.75.2503836; this is a Unity file version,
not the game semantic version. Valheim1.0.20 is prior verified log/accepted metadata evidence,
not a newly executed Version.GetVersionString call. Doorstop config enables BepInEx/core/
BepInEx.Preloader.dll; redirect_output_log=false, debug_enabled=false; steam_appid.txt contains
892970. Direct launcher currently uses exact development working directory and window flags,
not Steam URI. No launch argument/environment was changed.

## Reproducible installed IL

Root S:/Valheim-Modding/PoVPlus-Dev. Exporter reads metadata with installed Mono.Cecil, never
executes target assemblies, checks every source SHA before/after, and stores full selected IL
and direct I/O callsites locally ignored. Inspection covers 95 images: two game images, two
Splatform images, BepInEx and90 plugin DLLs. This is not all native/managed dependencies or
dynamic Harmony behavior. The independent Python verifier checks5 pinned images,10 method
contracts, parser/caller boundaries and2 direct-mod-path contracts; offsets/tokens are image-local.

| Image | SHA-256 |
|---|---|
| valheim_Data/Managed/assembly_valheim.dll | 94998885B103D6E5F5480123EAEE0A093E82AF2DDEEEE37536C15BF555F24F3A |
| valheim_Data/Managed/assembly_utils.dll | 771D59CBF3EF704007267E8EAF89AAC231C172B052BD93F9C273A18381BD6D59 |
| valheim_Data/Managed/Splatform.dll | AB1753F05F5FAE8C8614B729DE852C3282C5F174EB6E672F76480579B1F41DCE |
| valheim_Data/Managed/Splatform.Steam.dll | 1DF9DF6D3368D5413D0027916A186B39785FBF422AC249EC7CD9207DAB8FB5E6 |
| BepInEx/core/BepInEx.dll | 36BDB9AF2B8F2EDA4C911D2A49C2A05A4F643637AD724B876D292A9A5FCD8AE6 |

| Installed member/token | IL evidence and defensible conclusion |
|---|---|
| Game FejdStartup.Awake060015e1 | IL0007 calls ParseArguments060015ee; that parser handles console/join/etc., no savedir. No static call to ParseServerArguments found in95 inspected images |
| Game ParseServerArguments060015f2 | IL00f9 savedir string,010f Utils.SetSaveDataPath; later calls GetCreateWorld and ZNet.SetServer. The option exists in a server path; no evidence it is processed by ordinary client launch |
| Utils GetSaveDataPath0600030d | IL0000 cloud-enabled check,0008 IsAutoOrCloud,000f returns empty provider-relative root before0015 override;0022 fallback persistantDataPath. Override cannot redirect cloud provider |
| Utils SetSaveDataPath0600030e / .cctor06000386 | Setter writes m_saveDataOverride; initializer0006 reads Application.persistentDataPath. Never invoked during research |
| Game SaveSystem.GetWorldsSaveRootPath060018a4 / GetCharacterFolderPath060018a5 | Delegate to Utils, append worlds versus worlds_local and characters versus characters_local according to source |
| Game PlayerProfile.GetPath060017e8 | Uses m_fileSource and m_filename through SaveSystem.GetCharacterPath; name alone cannot determine persistence backend |
| Game World.GetSavePaths06001977 | m_chunkedSave branch returns a world directory; otherwise DB and metadata paths. A db/fwl-only backup assumption is invalid for all current formats |
| Utils FileHelpers.GetFiles060000d4 | Enabled cloud route mounts depot and queries IFileAccess; local route uses System.IO. Different backend, not merely another folder |
| Splatform.Steam SteamCloud.get_IsEnabled06000013 | Requires account and app cloud-enabled APIs; actual enabled state unknown/not queried |
| SteamCloud.WriteFile06000020 | IL0025 FileWriteStreamOpen,0088 WriteChunk,00a7 Close; mounted cloud API persistence exists independently of local file restore |
| Game SteamManager.Awake060014e9 | IL00b0 RestartAppIfNecessary then00e6 SteamAPI.Init. Direct execution is not evidence of Steam independence or exact child executable |
| BepInEx.Paths.SetExecutablePath060000aa | Derives BepInEx root (or supplied override), config/plugins/patchers/core/cache; not Utils save override. Runtime overrides still require observation |
| Utils PlatformPrefs.SetString060001a1 / Save060001a7 | Preference provider or PlayerPrefs fallback; Save may launch BackgroundWorker. Method return alone is not durable preference save acknowledgment |

**Classification:** the old reported failure of client -savedir isolation is supported by
installed parser topology. This does not prove every possible launch path ignores it: reflection,
uninspected native entrypoints, mods or future versions could differ. Do not invoke server parsing
to force the option; it has gameplay/network effects. No save getter was invoked for research.

## Known mod persistence outside a save-only snapshot

Export contains exact image hashes, tokens and full bodies. Examples directly inspected:

| Installed mod/member | Path derivation / scope |
|---|---|
| PathOfValheiman PassiveTree.BuildLibrary.get_FilePath0600037c | Application.persistentDataPath + PathOfValheiman/builds.json |
| PoVLink Session.get_DataDir06000096 | Same Unity root + PathOfValheiman |
| EpicLoot Common.ConfigUIFirstRun.get_RecordPath0600004a | Unity root + ModQuickConfig/FirstRun.cfg, with cached result |
| ReferToCompendium Plugin.Awake06000081 IL0059-008a | Unity root + ReferToCompendium/layout-shared.json passed to LayoutStore |
| Clan ClanPlugin.get_DataDirectory06000653 | Utils.GetSaveDataPath(source2) + Clan; source-specific, distinct from direct Unity path |

The initial direct-call scan found I/O-related members in72/90 plugins (includes configuration
helpers and reads, not72 gameplay writers). That count is discovery, not complete risk coverage.
Do not infer a missing hit means no persistence: custom helpers, serialization into player/world
data, native I/O, hardcoded paths, config sync and network effects remain. MS custom binding/player
inventory serialization is already traced in ARCH-003; BepInEx configs/LoadTimeProfiler output and
caches add installation state. User-specific log paths may contain private data; do not publish
raw traces. Actual files at any of these save/profile paths were NOT read or enumerated.

## External authoritative references, checked 2026-10-27

These are general contracts, not observations of the installed runtime:

- [Iron Gate server guide](https://www.valheimgame.com/support/a-guide-to-dedicated-servers/):
  documents savedir for server world/permission storage; not a client-wide isolation contract.
- [Steam API initialization/restart](https://partner.steamgames.com/doc/api/steam_api#SteamAPI_RestartAppIfNecessary):
  restart can select the Steam-library executable; steam_appid.txt affects that restart behavior.
  Its presence does not isolate cloud/account data.
- [Steam Cloud](https://partner.steamgames.com/doc/features/cloud): cloud can use explicit APIs or
  configured filesystem roots and session synchronization. Remote changes survive local rollback.
- [Unity persistentDataPath](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/Application-persistentDataPath.html):
  Windows usually uses a LocalLow known folder. Environment variable rewriting is not proof of redirection.
- [Unity PlayerPrefs](https://docs.unity3d.com/6000.0/Documentation/ScriptReference/PlayerPrefs.html):
  Windows standalone preferences use HKCU, outside ordinary save-directory backup.
- [BepInEx Paths](https://docs.bepinex.dev/v5.4.16/api/BepInEx.Paths.html): general5.x API;
  installed5.4.23.5 IL above is authoritative for this workspace.
- [SendInput](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-sendinput):
  injects input subject to integrity restrictions; insertion count is not application acknowledgment.
- [SetForegroundWindow](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-setforegroundwindow),
  [GetForegroundWindow](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getforegroundwindow),
  [GetWindowThreadProcessId](https://learn.microsoft.com/en-us/windows/win32/api/winuser/nf-winuser-getwindowthreadprocessid):
  necessary identity/focus checks; no atomic targeting guarantee from this combination.
- [UI Automation providers](https://learn.microsoft.com/en-us/windows/win32/winauto/uiauto-providersoverview):
  custom controls need providers/proxies. Unity menu inspectability remains untested.
- [Process Monitor](https://learn.microsoft.com/en-us/sysinternals/downloads/procmon): potential
  independent file/registry/process trace; no installation or capture performed. A trace needs
  drop/error checks and cannot prove that all future execution paths are confined.
- [Hyper-V checkpoints](https://learn.microsoft.com/en-us/windows-server/virtualization/hyper-v/checkpoints):
  useful state reversion, not a complete independent backup or external-cloud recovery.
- [Windows Sandbox configuration](https://learn.microsoft.com/en-us/windows/security/application-security/application-isolation/windows-sandbox/windows-sandbox-configure-using-wsb-file):
  shares/network are configurable; writable mappings persist host changes and defeat containment.

Computer Use skill and bundled guidance were read for target selection/one-action-refresh design.
No automation API, live accessibility scan, screenshot, desktop focus or input operation was used.
Backend readiness and Valheim keyboard/mouse acceptance require separate authorized demonstrations.

## Reproduction without saves or runtime

```powershell
pwsh -NoProfile -File Scripts/tests/Export-TestingArchitectureEvidence.ps1
python -B Scripts/tests/verify_testing_evidence.py .dev-state/dev-008/persistence-il.json
python -B -m unittest discover -s Scripts/tests -p test_testing_policy.py -v
```

Only exporter metadata reads use the development installation. Fixtures use newly generated
temporary directories. Do not substitute save paths into the fixture helper. No baseline refresh
was made to hide drift;343 installation hashes are checked against the existing Stage B baseline.
