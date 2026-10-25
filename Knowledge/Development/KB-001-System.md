---
id: "KB-001-System"
title: "Record schema and session workflow"
type: "reference"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[DEV-002-Maintenance]]", "[[INDEX]]", "[[DEC-002-Agent-integration]]"]
confidence: "verified"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Record schema and session workflow

## Record contract

Use stable descriptive ID filenames; enduring subjects omit version/date (sessions use dates).
IDs are immutable. Link exact filenames using Obsidian wikilinks, optional display labels.
related/contradicts/supersedes and prose establish relationships; Obsidian backlinks give reverse
navigation without duplicating facts. Link the canonical owner of each finding.

Frontmatter is YAML using **JSON values, one field per line**. No multiline YAML/comments in the
header: the restricted format permits dependency-free validation. Every Markdown record requires:

| Fields | Meaning |
|---|---|
| id, title, type | Unique stable ID, descriptive title, category |
| status | active, partial, open, proposed, resolved, superseded, historical |
| created, last_verified | ISO local dates; verification covers stated scope only |
| versions | Applicable observed versions; empty for version-independent guidance |
| evidence | Nonempty citations with paths/members/session IDs/line locators |
| related | Wikilink array |
| confidence | verified, mixed, hypothesis, unverified, decision |
| watch | Snapshot keys: dll:/cfg:/pkg:/src: plus relative file path (pkg = manifest) |
| claims | Object of canonical scoped claim keys/values for contradiction checking |
| contradicts, supersedes | Wikilink arrays; explain disagreement/replacement in prose |

Partial inventory records verify metadata, not behavior/ownership/compatibility. Explicitly
separate verified facts, external documentation, hypotheses, decisions and unresolved questions.
The new-record command creates an unverified template; last_verified initially dates template
review, not factual verification. Replace pending evidence before marking verified.

## Session lifecycle

Before substantial work: AGENTS, INDEX, relevant records, prior investigations/decisions, then
version/hash applicability. Reuse applicable evidence instead of unnecessary repeat scans.
During work: retain material evidence and assumptions. After meaningful work: update canonical
findings, preserve historical context, record tests/outcomes/questions, add one useful session note,
link it from INDEX or another reachable record, then validate. Trivial operations need no notes.

## Contradictions and change

Explain old/new evidence rather than silently upgrading hypotheses. Use historical/superseded
when a whole record is obsolete; link replacements using supersedes. For unresolved disagreement
use contradicts. Scope structured claim keys carefully. Free-text contradictions need manual review;
validators cannot prove prose semantics. [[DEV-002-Maintenance]] owns commands and refresh procedure.
