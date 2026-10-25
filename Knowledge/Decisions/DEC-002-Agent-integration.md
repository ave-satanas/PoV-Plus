---
id: "DEC-002-Agent-integration"
title: "Approved AGENTS knowledge integration"
type: "decision"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {"Valheim": "1.0.20", "BepInEx": "5.4.23.5", "Unity": "6000.0.75.2503836", "PoVPlus": "0.1.0"}
evidence: ["[[EVID-001-Local-observations]]"]
related: ["[[KB-001-System]]", "[[DEC-001-Development-baseline]]", "[[INDEX]]"]
confidence: "decision"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Approved AGENTS knowledge integration

## Approved append-only addition

The user approved this proposal on 2026-10-24. The following section was appended to root
AGENTS.md, preserving all existing instructions byte-for-byte:

```markdown
## Persistent engineering knowledge
- Before substantial development or investigation, read Knowledge/INDEX.md and relevant linked records.
- Check prior investigations, decisions, evidence, and applicability to current assembly/configuration versions before repeating research.
- During meaningful work, distinguish verified facts, hypotheses, decisions, and unresolved questions; cite precise evidence.
- After meaningful work, update canonical records and related links, record tests/outcomes and remaining questions, and add a concise session entry when useful.
- Run python Scripts/knowledge.py validate; review drift and revalidation notices. Do not change accepted snapshots merely to silence warnings.
- Treat the development installation as read-only unless explicitly authorized; knowledge maintenance does not authorize deployment, configuration changes, or game launch.
```

Root AGENTS.md now requires future relevant sessions to consult and maintain the knowledge base.
The approval applied to this instruction addition only; deployment, game launch and installation
changes remain separately controlled. Evidence: user message "You have approval", 2026-10-24,
and the appended AGENTS.md section. The original proposal was presented before application.
