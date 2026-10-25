---
id: "DEP-001-Environment"
title: "Accepted installation snapshot"
type: "dependency"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[MOD-000-Inventory]]", "[[DEV-001-Build]]", "[[DEV-002-Maintenance]]"]
confidence: "verified"
watch: ["dll:BepInEx/core/BepInEx.dll", "dll:BepInEx/core/0Harmony.dll", "dll:valheim_Data/Managed/assembly_valheim.dll"]
claims: {"installed.bepinex.assembly": "5.4.23.5", "installed.valheim.log-version": "1.0.20"}
contradicts: []
supersedes: []
---

# Accepted installation snapshot

Focused follow-up, 2026-10-25: [[INV-003-Magic-book-tome-compatibility]] records current
fingerprints for MagicPlugin, PoV, SeneaL API/UI and Magic Supremacy, verified equipment API
signatures and their differing Tome classifiers. This supplements the baseline without
refreshing its accepted snapshot or implying that all runtime behavior has been revalidated.

Offline follow-up, 2026-10-25: [[DEV-005-Tome-diagnostics]] and
[[COMP-002-Tome-bridge-design]] document the repaired opt-in tooling and proposed MS-authoritative
integration. Installed mod/game contracts were checked against the recorded fingerprints.
Repository-source fingerprints are separately reviewed for the new conditional compilation and
observation tooling after offline tests; installed assembly/configuration fingerprints and the
original snapshot capture timestamp remain historical and are not refreshed to hide runtime drift.

## Accepted metadata baseline

[environment-snapshot.json](environment-snapshot.json) stores 245 assembly fingerprints/reference
lists (12 core, 90 plugin, 2 patcher, 141 game-managed DLLs), 87 manifests and 99 cfg fingerprints.
Select JSON entries by path. No binary bytes, configuration values, saves or full logs are copied.
Snapshot timestamp is UTC; note dates use America/Los_Angeles.

Runtime log: Valheim 1.0.20; Unity 6000.0.75.2503836; Windows Bits64; CLR 4.0.30319.42000.
This is Unity Mono, distinct from the installed desktop .NET SDK/runtime.
BepInEx assembly 5.4.23.5, pack 5.4.2351; HarmonyX 0Harmony assembly 2.9.0.0.
0Harmony20 2.0.0.0 coexists; presence alone is not a conflict. Mono.Cecil 0.10.4.0;
MonoMod.RuntimeDetour/Utils 22.1.29.1. Jotunn 2.30.2 declares net462;
PathOfValheiman 5.3.2 and PoVLink 0.3.0 declare net48.

## Available game APIs

Previous Cecil inspection located Player, Character, Inventory, ItemDrop, ObjectDB, ZNet and
ZNetScene in valheim_Data/Managed/assembly_valheim.dll. Assembly-CSharp alone is insufficient.
UnityEngine.CoreModule supplies MonoBehaviour. Verify actual signatures before patching.
Add only needed Unity modules; do not indiscriminately reference game framework DLLs.

[[MOD-000-Inventory]] separates package requirements, BepInDependency attributes and CLR
references. Different version numbers across those layers do not automatically indicate defects.
Harmony attributes are declarations, not the active runtime patch registry/order.
See [[DEV-002-Maintenance]] for snapshot drift and revalidation.
