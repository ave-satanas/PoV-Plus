---
id: "DEV-004-Intervention-framework"
title: "Problem classification and intervention review"
type: "development"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {}
evidence: ["User instruction: Formalize Project Philosophy and Engineering Standards, 2026-10-24", "[[ARCH-002-Project-philosophy]]", "[[DEC-004-Project-governance]]"]
related: ["[[ARCH-002-Project-philosophy]]", "[[DEC-004-Project-governance]]", "[[KB-001-System]]", "[[INV-002-Compatibility-triage]]", "[[DEV-003-Workflow]]"]
confidence: "decision"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Problem classification and intervention review

## Classify the reason for work

Investigations identify a primary category and any relevant secondary categories. State the
confidence and evidence for each. An observed warning can be verified while its proposed
gameplay consequence remains a hypothesis. A user preference establishes a design goal without
establishing a technical defect.

| Category | Required distinction or evidence |
|---|---|
| Verified technical defect | Demonstrated violation of intended behavior, an API contract, or an invariant; cite source, installed code, or reproducible results. |
| Verified compatibility conflict | Demonstrated harmful interaction between identified components/versions, with a causal link to the failed behavior. |
| Design disagreement or personal preference | Current behavior is understood; record the user's desired alternative and design rationale without calling an intentional upstream choice a bug. |
| Missing integration | Identify the existing systems, the desired interaction, and evidence of the absent connection; do not imply either system is defective. |
| Missing feature | Identify the unmet capability and why available functionality is insufficient. |
| Unnecessary or redundant system | Identify duplicated responsibility or lack of value for the intended experience, including effects of disabling it. Similar features alone do not establish redundancy. |
| Unresolved hypothesis requiring further evidence | State the unproven explanation, evidence gaps, and smallest useful verification step. |

Overlapping configuration settings, Harmony patches, or features do not establish a conflict.
An optional compatibility probe or an intentional fallback can explain an unavailable integration.
Required behavior, actual consequences, and the adaptation's intent must be examined before
classifying it as a verified conflict. Existing warnings in [[COMP-001-Warnings]] retain their
recorded uncertainty; this policy does not reclassify or resolve them.

## Select an approach

| Approach | Use |
|---|---|
| Configure | Use existing supported settings when they achieve the desired behavior. Verify keys and effective precedence. |
| Integrate | Connect existing systems that should interact; define responsibility and the interface between them. |
| Patch | Correct a specific defect or undesirable behavior while retaining the surrounding implementation. |
| Extend | Add missing functionality while retaining a sound implementation. |
| Replace | Substitute an unsuitable implementation with a better-designed alternative; define the responsibility transferred and how duplicate behavior is prevented. |
| Remove | Disable or eliminate unnecessary, redundant, or conflicting functionality; establish who, if anyone, supplies its remaining responsibilities. |

These options are not a mandatory sequence or an inflexible hierarchy. Compare plausible
alternatives directly. No requirement exists to attempt configuration or accumulate patches
before proposing replacement. Conversely, replacement needs a concrete benefit and transition
plan; availability of that option does not establish that a particular system should be replaced.
Removal can address a deliberate design preference or redundancy without proving a technical bug.

## Decision criteria

| Criterion | Questions for the decision |
|---|---|
| Correctness | Does the option meet the stated behavior and preserve required invariants? What evidence supports that? |
| Gameplay quality | Does it improve the user's intended experience, progression, balance, and system cohesion? |
| Maintainability | Is responsibility clear? How sensitive is it to upstream versions and future fixes? |
| Compatibility | Which dependencies, author adaptations, patches, configurations, and integrations must still work? |
| Performance | What cost changes, and what measurement is needed if that cost matters? |
| Implementation complexity | What implementation, migration, testing, and ongoing support work does it introduce? |

Record material tradeoffs and uncertainty rather than inventing scores or a universal weighting.
Favor a clean replacement over a fragile patch stack when evidence supports lower risk and
complexity. A supported setting can be the best solution when it fully meets the goal.

## Lightweight review record

Use the existing investigation/decision schemas in [[KB-001-System]]. One concise record is
enough for a small change; link canonical evidence rather than duplicating it. Include:

1. **Problem and goal:** category, observed/intended behavior, user preference where applicable,
   affected versions, and explicit exclusions.
2. **Evidence:** installed code/config/log locators, author adaptations and their purpose,
   relevant prior decisions, hypotheses, and unresolved questions.
3. **Choice:** selected approach, plausible alternatives, reasons and material tradeoffs using
   the criteria above. No need to enumerate options that do not apply.
4. **Boundaries:** ownership, what behavior remains or changes, integrations to preserve,
   potential progression/balance or data-format effects, and any migration/rollback needs.
5. **Verification:** appropriate success/regression checks and the smallest safe experiment.
   Distinguish a build/startup check from proof of gameplay correctness.

Before replacement/removal, identify callers, other patches and configuration dependencies;
plan how the old behavior is disabled, required adaptations retained, and prior state recovered.
Do not design overlapping old/new owners accidentally. A proposal affecting persisted data
must describe the risk; documentation work does not authorize touching saves or worlds.

Prefer a standalone PoVPlus implementation over modifying third-party assemblies. This is a
delivery/ownership preference, not a requirement to use the Patch approach. Verify installed
references and signatures, preserve originals, document modifications, and check licenses.
Implementation or a decision record does not authorize deployment, removal of installed mods,
configuration edits, rollback, or launch; follow [[DEV-003-Workflow]] and obtain scoped approval.
