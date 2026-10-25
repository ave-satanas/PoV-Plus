---
id: "DEC-004-Project-governance"
title: "Adopt personal integration and intentional design philosophy"
type: "decision"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {}
evidence: ["User instruction: Formalize Project Philosophy and Engineering Standards, 2026-10-24", "[[DEC-001-Development-baseline]]", "[[DEC-003-Workflow-safety]]"]
related: ["[[ARCH-002-Project-philosophy]]", "[[DEV-004-Intervention-framework]]", "[[DEC-001-Development-baseline]]", "[[DEC-003-Workflow-safety]]"]
confidence: "decision"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# Adopt personal integration and intentional design philosophy

## Decision

Adopt [[ARCH-002-Project-philosophy]] as the canonical project identity, guiding principles,
and priorities. Use [[DEV-004-Intervention-framework]] for issue classification and intervention
decisions. The user's request establishes policy; it is not evidence of a defect in any installed mod.

## Clarifications of existing principles

| Existing wording or context | Interpretation under this decision |
|---|---|
| Generic Valheim development workspace; minimal startup plugin | The project is a personal compatibility/integration/customization framework. Startup-only behavior remains the verified current implementation. |
| Preserve Path of Valheim progression and balance | Preserve the working baseline and assess changes deliberately. Explicit user-directed design changes may alter mechanics with rationale and impact review. |
| Prefer standalone compatibility patches to assembly edits | Keep ownership and distribution safe. This does not rank Patch above Configure, Integrate, Extend, Replace, or Remove. A standalone replacement is permitted. |
| Maintain installed-mod compatibility | Preserve required contracts and intentional author adaptations. Understand consumers and fallback behavior before redesigning adjacent systems. |
| Avoid unnecessary duplication | Reuse sound systems, but allow justified replacement and original capability. Do not preserve unsuitable implementations solely because they exist. |
| Warnings, overlap, missing APIs | Preserve the distinction between observations, verified harmful interactions, preferences and hypotheses. Existing investigations keep their original evidence/status. |

The six approaches are options, not a fixed escalation ladder. Correctness, gameplay quality,
maintainability, compatibility, performance, and implementation complexity determine the choice.
Clean replacement can be safer and simpler than repeated fragile patches. It needs defined
ownership, interoperability, transition, and verification, not a history of failed patch attempts.

## Boundaries retained

Existing evidence rules, original-file preservation, verified API references, licensing checks,
mod-author configurations and compatibility adaptations remain in force. Changes to an adaptation
require a specific documented decision; this governance task changes none of them.
[[DEC-003-Workflow-safety]] continues to govern installation mutations and launch authorization.
No gameplay feature, patch, replacement, removal, configuration or save change is implemented here.

## Reconciliation and history

This clarifies the scope of [[DEC-001-Development-baseline]]; that record remains active for
implementation and safety constraints. It is not superseded wholesale. Existing runtime and
compatibility findings remain factual observations of their recorded versions. The stale
approval-pending title in [[DEC-002-Agent-integration]] is corrected to match its existing active
status and documented approval; its proposal text and historical approval evidence are retained.
