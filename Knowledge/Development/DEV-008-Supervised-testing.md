---
id: "DEV-008-Supervised-testing"
title: "Supervised autonomous Valheim testing architecture"
type: "development"
status: "proposed"
created: "2026-10-27"
last_verified: "2026-10-27"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Windows": "11 Pro 25H2 build 26200.9711"}
evidence: ["[[EVID-002-Persistence-and-control]]", "[[EXP-006-Testing-policy-fixtures]]", "User DEV-008 authorization: offline investigation only"]
related: ["[[INDEX]]", "[[DEV-003-Workflow]]", "[[ARCH-003-Tome-transition-evidence]]", "[[DEV-007-C1-ownership-audit]]", "[[COMP-002-Tome-bridge-design]]", "[[EXP-004-Tome-equipment-lifecycle-plan]]"]
confidence: "mixed"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Supervised autonomous testing: architecture proposal

## Decision boundary

**Level 0 only. No environment is certified isolated or ready for gameplay.** This proposal
addresses a missing testing capability, not a proven upstream defect. The clean starting
checkpoint was main/0c1f3d1a909e0c40a7bcf36425f8efc6a1c09722. Research is on
research/dev-008-supervised-testing; no commit/push. Claude's worktree and findings were not
consulted. [[EVID-002-Persistence-and-control]] owns installed-code/source evidence;
[[EXP-006-Testing-policy-fixtures]] owns executed tests and their limits.

Stage B acceptance is verified. Stage C1 ownership completeness, equipment lifecycle,
persistence, neutral-state certification, and rollback safety remain unverified.

Do not relax DEV-007's neutral-state requirements for the existing shared-profile client.
Disposable whole-environment recovery is a separate proposed safety strategy. It may eventually
permit a specifically approved experiment despite unknown internal ownership, without proving
ownership, functional correctness, neutral state, or safe production DLL rollback.

## Isolation assessment and recommendation

Names PoVPlus_Test / PoVPlus_Lab identify intended fixtures, not an isolation boundary. The
existing Dev.ps1 launcher has a fixed development-root allowlist and shared-save warning. Do not
point it at another installation or bypass its safeguards. A future environment adapter requires
reviewed changes to those safeguards and regression tests before deployment authorization.

| Candidate | Coverage / remaining risk | Decision |
|---|---|---|
| Copy installation, same Windows/Steam identity | Separates normal install-relative BepInEx files; leaves profile, preferences, cloud and arbitrary mod paths shared | Reject for autonomous gameplay |
| Add -savedir | Installed ordinary client parser does not handle it; cloud and direct Unity/mod paths independently bypass override | Reject as an isolation guarantee |
| Rewrite USERPROFILE/APPDATA variables | Does not establish Windows known-folder, HKCU, platform-provider or native Steam identity separation | Reject as security boundary |
| Separate non-admin Windows account and copied client | Can separate profile/HKCU; requires explicit effective ACL denial for protected/shared volumes and independent Steam identity/cloud policy; hardcoded/network mod paths remain | Conditional lower-cost alternative, not approved or verified |
| Dedicated disposable VM with full private disk/profile/client | OS boundary can exclude host saves, host installs and shared Steam cache; includes registry/mod/native writes inside guest; GPU, entitlement, control transport and launch feasibility unknown | Preferred isolation candidate for a separately approved feasibility review |
| Windows Sandbox | Disposable OS with configurable network/shares; guest lifetime, Steam provisioning, graphics and reproducibility complicate reusable baselines | Possible demonstration target, not selected Valheim platform |

Prefer a VM with no host filesystem mappings, shared clipboard/drives or personal credentials;
no dedicated-server routes; outbound network denied during experiments; inputs transported to
the guest desktop rather than globally injected on a busy host. Import licensed installation
material later through a narrowly scoped read-only transfer; export evidence through a separate
bounded channel. Host output must never be a guest-writable general directory. A VM snapshot
alone is not an independent backup. No VM, account, ACL, firewall, Steam or feature was created
or changed. Hypervisor presence does not prove a suitable Valheim-capable VM exists.

The owner must choose the OS/Steam/graphics tradeoff before provisioning. A native account may
be sufficient for accidental-I/O containment if all protected resources are actually denied;
it provides a weaker containment argument than an unmapped guest. Do not claim Steam offline
mode alone prevents reconnection or local synchronization. Same Steam account in a different
Windows account can still address the same remote saves. Lack of observed writes is not access
denial. Permission tests must use synthetic protected sentinels, never normal saves.

## Complete experimental state, not just saves

The baseline scope must include the whole private test client/mod/config tree, complete test
profile (including LocalLow, Local, Roaming, temporary/cache data), HKCU preferences, all local
character/world metadata/chunks/backups, any private Steam app state, and any permitted external
mod storage. Record OS/Steam/game/mod/code/config versions, identity, ACLs, volume identities,
reparse/hardlink rules, network/share policy, and environment variables. Include machine-level
changes if allowed; prefer denying them and rebuilding the guest when uncertain. Credentials
and personal data never enter a baseline or evidence bundle.

An observed write outside that state envelope invalidates it. Native I/O, dynamically computed
paths, reflection, child processes, external sync and remote service writes are not disproven
by the installed IL scan. A full private disk restore covers unknown local paths within that
disk; it cannot retract cloud writes, network calls or host-share writes. Those exits must be
prevented before first launch. Save-only backups are insufficient even for current known mods.

## Independent baseline and recovery contract

**Preconditions:** exact disposable environment ID/volume and identity; no mapping to protected
roots; no reparse/hardlink aliases; no client or other state writers; separately authorized scope;
no pending recovery journal; adequate disk space; verified network/cloud exclusion. Unknown
process identity or writer status blocks. Do not take a baseline while the game or sync is live.

1. After later approved provisioning, create fresh disposable state rather than import existing
   personal/test saves. Owner creation is a separate authorized operation. Shut down cleanly.
2. Freeze a baseline inventory with relative paths, sizes and SHA-256, empty directories,
   attributes/ACLs/reparse rules, registry/profile and OS version. Hash the manifest independently;
   keep an offline/read-only master outside game credentials and a second independently verified
   recovery copy. A hash stored beside writable content is not authenticity or immutability.
3. Independently restore a new generation, compare exact file set and hashes, identities and
   policy, and rehearse interruption recovery before allowing any experiment. Runtime save
   reloadability still needs a separate approved test; byte equality alone is insufficient.
4. Each run uses a new generation or private child disk. The master is never launched, mounted
   writable by the game, or incrementally overwritten. Bind approval/scenario/artifacts to that
   exact generation and baseline digest. Invalidate approval after any baseline/version change.
5. Journal prepared/running/evidence-sealed/recovering/verified states outside the guest. One
   active environment lease, scoped by environment ID, must serialize all run/deploy/recovery
   paths across worktrees. Current repo-local workflow.lock is insufficient across separate
   controllers. Stale lock means quarantine; validate holder PID/start/boot identity and obtain
   a recovery decision instead of age-based lock stealing.
6. Stop input on any fault. Preserve bounded logs, observations, action journal and screenshots
   before cleanup. Request normal exit; verify exact process and writers stopped. Do not
   auto-kill under existing guidance. Crash/forced termination policy needs explicit approval;
   a timeout with a live process is blocked recovery, not permission to overwrite its files.
7. Discard the working generation and restore into a new destination. Never recursively clean
   a user-supplied path or restore individual files in place. Reject unexpected volume, UNC,
   device/relative/ADS paths, reparse points, aliases or protected ancestor/descendant overlap.
   A future native executor needs Windows handle-based final-path/file-ID checks and controlled
   ACLs throughout operations; string canonicalization has a check/use race.
8. Verify the complete restored generation before selecting it. Single-filesystem staging and
   atomic pointer publication can avoid exposing partial generations; multiple profile/registry/
   disk roots require an outer stopped-environment transaction. A committed journal AND fresh
   independent verification are required. On restart, ambiguous/pending journal or lease always
   blocks; partial files remain quarantined evidence. No presumed success after Windows reboot.

**Postconditions:** independent manifest verifier agrees on full scope; stopped environment;
original master still matches; no external sync/host writes; recovery journal committed;
evidence sealed separately. Any missing postcondition blocks the next experiment. A successful
return code, UI logout, restoration of one DLL, or equipment flag is insufficient. Whole-guest
discard recovery can protect a disposable experiment without calling Magic Supremacy unequip.
It does not certify that unequipping works or that disabling a bridge on a played save is safe.

## Desktop-control architecture

| Mechanism | Assessment |
|---|---|
| Existing Computer Use connector | First candidate for separately approved demonstration; skill describes selected-window input and screenshots. Actual availability, identity guarantees, cancellation latency and game acceptance not exercised |
| PowerShell/.NET + UI Automation | Useful for OS dialogs with providers; Unity custom menus cannot be assumed accessible. No live accessibility tree obtained |
| Win32 SendInput + window APIs | Small dependency surface but global input/focus race, UIPI and key-state hazards. Not a strict guarantee against unrelated input |
| Python PyAutoGUI / image matching | Additional dependencies; coordinate/DPI/focus brittleness; no demonstrated advantage over existing connector. Not installed or required |
| Reviewed small C# adapter | Future option for process/start-time/token/window checks and bounded input watchdog if connector contract is inadequate; no resident service proposed |

Recommend screenshot-confirmed single keyboard/click actions plus independently audited runtime
observations, not coordinate macros. Prefer windowed/borderless with fixed physical client
rectangle; record monitor, DPI, client-to-screen transform and resolution. Existing 2560x1440
launch flags are a requested geometry, not observed geometry. Moving/resizing/DPI change or
occlusion/modal dialog invalidates coordinates. Do not assume screenshots are current or visible
just because capture succeeds. Screenshots must be window-scoped to avoid unrelated personal data.

Before EVERY action: unique allowlisted full executable path/hash, PID/start-time/boot/session,
user/integrity identity, HWND owner, expected client rectangle, unlocked interactive desktop,
foreground and fresh recognized state. Bind one observation to one action; refresh and verify
postcondition. PID/title alone is inadequate. Stop on ambiguous process/window, crash, focus loss,
unexpected dialog, stale/blank screenshot, loading timeout, API failure or identity mismatch.
No automatic focus stealing, blind retry, repeated keypress loop or held movement keys initially.

Human intervention cancels the scenario. A visible stop affordance and independent watchdog
must cancel new actions even if the reasoning/controller loop hangs; bound each operation and
total action/time budget. Validate stop latency later with a synthetic target. Paired key/button
release must not send cleanup input into another application; prefer atomic taps with no held
state, report uncertain partial input. Locked/disconnected/secure desktop means stop, not reconnect
automation. Do not bypass UIPI or request elevation simply to make input work.

There is no atomic foreground-check-and-global-input operation in the proposed Win32 approach.
The offline counterexample preserves this limitation. A segregated guest/test desktop plus
supervision reduces consequence; it does not prove raw SendInput is addressed to one HWND.
Whether Valheim receives taps, how menus expose focus, and how the connector behaves remain
Level 1/2 proof obligations. A strict zero-unrelated-input guarantee cannot be claimed today.

## Runner, scenarios and evidence

Use existing PowerShell build/artifact/deployment checks; Python standard-library policy,
scenario validation and report processing; a replaceable desktop adapter. Keep environment
control separate from gameplay and observation. No new orchestration framework, database,
resident service, DLL patch or executable launcher was added. Scripts/testing_policy.py is
an offline necessary-condition model; it has no real adapters and accepts only Level 0 scenarios.

Proposed state machine:

```text
Unprepared -> BaselineVerified -> Authorized -> Prepared -> Launched -> MenuVerified
 -> IdentityVerified -> Experiment -> EvidenceSealed -> Stopped -> Recovering
 -> IndependentlyVerified -> Complete
Any uncertainty -> InputStopped/Quarantined -> owner-reviewed recovery only
```

Build success is an artifact result; input sent is an action result; gameplay postconditions
are an experiment result; recovery is a fourth result. Never combine them into one green flag.
Keep authorization as an explicit owner record scoped to operations, environment, scenario,
versions, budgets, termination policy and expiry. A scenario document is not permission.

Declarative scenarios should name schema/id, baseline digest/environment, minimum permission,
exact character/world plus disposable provenance, dependency/config hashes, target prefab/actual
instance, acquisition permission, native/negative controls, action budget, deadlines, required
pre/postconditions and evidence, failure policy and discard-generation recovery. Reject unknown
fields, scripts/shell fragments, unbounded actions, invalid names, missing baseline/evidence and
unsupported schema. First controller can use an allowlisted finite action vocabulary; do not
embed arbitrary Python/PowerShell or treat an image match as equipment authority.

JSONL evidence: run/scenario/baseline/artifact IDs, UTC and monotonic elapsed time, action sequence,
PID/start/session/window identity, pre/post screenshot hashes, observation source/phase/confidence,
expected versus observed result, log byte/session boundaries, explicit gaps/drops and recovery
manifest/journal. Hash manifests detect drift, not truth. Preserve raw evidence locally ignored;
export small reviewed redacted excerpts and outcomes to Knowledge. No saves/credentials in Git.
No claim of complete event history from contiguous action numbers. Bound count, byte size,
retention and deadlines; evidence exhaustion stops action rather than silently dropping failures.

Logs: record prelaunch size/hash/time and actual BepInEx startup session, handle rotation/truncation,
associate LoadTimeProfiler files only by that session's reference. Its filtering can hide Info;
missing startup message is not initialization failure. New warning signatures need attribution,
native controls and baseline comparison. Known EpicLoot shardstone error remains separate.

## Authorization progression

| Level | Entry criteria and explicit owner approval | Stops / recovery |
|---|---|---|
| 0 current | Offline research/build/fixtures, no live adapters | Unexpected state preserves evidence; no game actions |
| 1 synthetic desktop | Owner approves exact synthetic window, active session, capture area, tap budget and stop test; no game/save/Steam operations | Any identity/focus uncertainty stops; close synthetic target only under approval, no process killing |
| 2 isolated launch/menu | Owner approves selected isolation implementation, licensed provisioning, newly disposable state, measured boundary/backup recovery, launch/deploy scope and screenshot/navigation budget | No inventory/item manipulation; prohibit personal-profile/cloud paths, unexpected Steam relaunch, unidentified writer or new state surface; stopped full-environment recovery required |
| 3 bounded gameplay | Level 2 isolation and restore/reload rehearsal passed; owner approves scenario, item acquisition, equip/unequip/persistence scope, controls and crash policy | First mismatch/unknown owner/new error stops; preserve, normally exit, discard only disposable generation; no blind bridge disablement |
| 4 supervised loop | Reproducible scenarios and recovery; adversarial stop/identity tests; explicit batch limits, approved source/deploy scope, version pins and revocation rules | No permission expansion; drift or failed/unknown recovery blocks next cycle and requires review |

Silence is never permission. Existing AGENTS no-auto-termination and installation-write rules
remain intact. These levels are requirements for future approvals, not grants in this record.

## Stage C1 roadmap and smallest next milestones

1. Review this proposal against Claude's independent assessment later, without merging now.
   Owner chooses VM feasibility investigation versus the weaker native-account alternative.
2. Level 1: separately approve a synthetic-window observation/tap/stop demonstration. Prove exact
   target binding, human takeover, focus-loss/locked-session stop and DPI changes before games.
3. Separately authorize provisioning and a **no-game** isolation/recovery rehearsal using only
   sentinels. Test protected-resource denial, external-sync exclusion, full-state restoration,
   interrupted publication, crash/reboot quarantine and cross-controller locks.
4. Only after independent isolation/recovery acceptance, request Level 2. Fresh private profile,
   no personal Steam cloud identity; verify actual executable/command line/BepInEx and menu I/O
   with a filesystem/registry trace before allowing character/world access. Trace drops or a
   forbidden attempted access block, not just successful writes. Establish baseline anew after
   explicitly approved fresh character/world creation, normal shutdown, restore and reload.
5. Request a specific Level 3 native-control equip/unequip experiment, then Beginner's book,
   then swaps/exclusivity and representative books. Recreate baseline per case; no automatic
   acquisition/progression bypass. Compare item conservation, visible slot, actual MS map/flags/
   bindings when safely available, Eitr maximum and measured regeneration, SE application/removal,
   failed swap and restart persistence. Unavailable controls remain blocked.
6. Only later deliberate failure/recovery cases and repeated runs; multiplayer/server separate.

Read-only bounded identity/session/patch-owner inventories, screenshots, log attribution, direct
local snapshots and effect measurements remain useful. EXP-005 can falsify contradictions in
sampled values; it cannot establish missing history, authoritative initial state or neutral
rollback. Do not reactivate blocked C1 static reads to obtain convenient observations.

Three outcomes must be scored independently: environment recoverable; gameplay function correct;
ownership understood. Offline work cannot establish Valheim input acceptance, actual persistence
envelope, cloud exclusion in the chosen runtime, GPU/Steam feasibility, or live diagnostic purity.
No first runtime trial is justified by these fixtures alone.
