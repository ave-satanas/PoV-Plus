---
id: "DEV-002-Maintenance"
title: "Knowledge tools and revalidation"
type: "development"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[KB-001-System]]", "[[DEP-001-Environment]]", "[[INV-002-Compatibility-triage]]", "[[DEC-002-Agent-integration]]"]
confidence: "verified"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Knowledge tools and revalidation

## Routine commands

Repository root; Python 3.9+ and Windows PowerShell 5.1. No pip/NuGet/Obsidian plugins required.

```powershell
python Scripts/knowledge.py validate --strict
python Scripts/knowledge.py search 'SocketOverlayModifier'
python Scripts/knowledge.py new INV-003 example --title 'Focused investigation'
python -m unittest discover -s Scripts/tests -v
```

new writes one unverified template, refuses an existing destination, and needs an inbound link.
search finds topics/GUIDs/evidence across notes. Validation checks required metadata, IDs,
wikilink targets, INDEX reachability, dates, watch keys, structured contradictions, local source
drift and unresolved investigation/experiment queue. Warnings fail only with --strict. Open
research is a queue, not a structural error. File targets are checked; heading anchors/external
URLs need review. Validation never reads the game installation or contacts a network.

## Installation updates

After approval for external reads, collect a candidate, not a replacement for accepted evidence
(ensure ignored Source/PoVPlus/obj exists):

```powershell
.\Scripts\Get-KnowledgeSnapshot.ps1 -OutputPath "$PWD\Source\PoVPlus\obj\environment-current.json"
python Scripts/knowledge.py validate --compare-snapshot Source/PoVPlus/obj/environment-current.json
```

Review added/removed/changed DLL, manifest, cfg and local-source hashes and affected watched records. Follow related links to assess
downstream impact manually. The 90-day stale reminder is not applicability proof. Changes detect
drift, not its cause. Ordinary validation compares watched local source files automatically.

After review, deliberately replace Knowledge/Dependencies/environment-snapshot.json with the
candidate; run `python Scripts/Render-KnowledgeInventory.py`; inspect the diff; reconcile removed/
renamed groups and changed relationships; update evidence and verification metadata manually.
Renderer replaces marker-delimited inventory blocks only, preserves manual prose/frontmatter,
does not certify new observations and never deletes obsolete mod records. Mark removals historical.
Reconcile related/watch frontmatter with new content; removed watch keys are validation errors.
Preserve meaningful old observations in investigations, not large log/snapshot archives.

## Limits

No automatic runtime patch order verification, prose contradiction proof, authorship/license
certification or exhaustive effective-config analysis. Root instructions integration is approved and applied; see
[[DEC-002-Agent-integration]]. Do not silently refresh a snapshot to hide drift notices.
