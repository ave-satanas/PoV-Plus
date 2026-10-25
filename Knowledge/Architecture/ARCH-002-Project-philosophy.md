---
id: "ARCH-002-Project-philosophy"
title: "PoVPlus project identity and philosophy"
type: "architecture"
status: "active"
created: "2026-10-24"
last_verified: "2026-10-24"
versions: {}
evidence: ["User instruction: Formalize Project Philosophy and Engineering Standards, 2026-10-24", "[[DEC-004-Project-governance]]"]
related: ["[[ARCH-001-PoVPlus]]", "[[DEV-004-Intervention-framework]]", "[[DEC-004-Project-governance]]"]
confidence: "decision"
watch: []
claims: {}
contradicts: []
supersedes: []
---

# PoVPlus project identity and philosophy

## Identity

PoVPlus is primarily a personal Valheim mod compatibility, integration, and customization
framework. It shapes the installed ecosystem around the user's gameplay preferences and
design philosophy. The existing startup-only plugin is the implementation baseline; this
broader identity describes the project's direction, not functionality already implemented.
[[ARCH-001-PoVPlus]] owns verified implementation facts.

PoVPlus does not aim to recreate an entire modpack or compete unnecessarily with its
dependencies. Reuse sound implementations and supported APIs. Existing ownership or availability
does not oblige the project to retain unsuitable systems.

## Guiding principles

**Preserve what works. Improve what falls short. Replace what fundamentally fails.**

**Minimum unnecessary duplication, maximum intentional design.**

Duplication needs a purpose. A replacement can intentionally own a responsibility previously
handled elsewhere when that produces a clearer, safer implementation. Avoid maintaining two
uncoordinated systems for the same responsibility. Original functionality is appropriate when
existing solutions cannot meet the intended experience.

## Development priorities

1. Resolve genuine compatibility problems across the installed mod ecosystem.
2. Modify existing features to match the user's gameplay preferences and design philosophy.
3. Integrate otherwise disconnected systems into a cohesive gameplay experience.
4. Correct defective, inconsistent, redundant, or poorly implemented mechanics.
5. Introduce original functionality when existing solutions are insufficient.

These priorities guide planning. They do not require proving an upstream bug before pursuing
an explicitly chosen preference or integration. Describe the reason honestly: an intentional
upstream design can function correctly while being unsuitable for this project.

## Reuse, preservation, and deliberate change

Preserve established working behavior, dependency contracts, and intentional mod-author
compatibility adaptations. Identify the purpose of an adaptation before changing adjacent
functionality; retain its necessary interoperability when integrating or replacing a system.
Do not silently remove author overrides or infer a defect from a warning or overlapping setting.
If a proposal cannot retain an adaptation, document that boundary and obtain a specific decision
before implementation changes it.

Path of Valheim progression and balance remain the baseline for assessing effects. The user's
intentional design choices may change them with a recorded rationale and impact review.
Preservation is a design constraint to evaluate, not an instruction to freeze every upstream mechanic.

## Choosing an intervention

Configure, integrate, patch, extend, replace, and remove are available options. Choose according
to correctness, gameplay quality, maintainability, compatibility, performance, and implementation
complexity; [[DEV-004-Intervention-framework]] defines the review process and problem categories.
Replacement is explicitly permitted when justified. Several fragile patches are not a required
precondition for selecting a clean replacement that is safer and simpler.

This policy grants design latitude, not permission to change an installation. Existing
[[DEC-003-Workflow-safety]] authorization boundaries and evidence standards remain in effect.
