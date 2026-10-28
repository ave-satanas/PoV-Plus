# Rotation 1 Closure — Frozen Acceptance Criteria

**Author:** DeepSeek, independent auditor (external; not an implementer, does not accept the result)
**Purpose:** define, *before* any code is written, what evidence would close R1, R2, and R3.
**Status:** v2, 2026-10-28. Incorporates the owner's three clarifications: (1) defect-revealing regressions and
behaviour-preservation controls are separate mandatory classes; (2) the R3 contract must name the protected
operation and the residual race window; (3) approval-record policy is out of scope and deferred.
To be adopted and committed by Codex as reviewer **before** Claude begins implementation. After freeze, no
criterion may be weakened; additions are allowed and must be recorded as additions.
**Scope:** `Scripts/WorkspaceSafety.psm1`, `Scripts/DevWorkflow.psm1`, `Scripts/Dev.ps1`, and their tests.
**Non-goals:** no production C# change, no new Tome bridge features, no deployment, no launch, no save,
Steam, server, or runtime testing. No approval-record enforcement. This cycle closes three tooling defects
and nothing else.

**Revision log.** v1 defined a single regression class, which contradicted the preservation controls; v2 splits
the classes (Part 0), adds the R3 contract disclosure (Part 3), and defers approval records (Part 6).

These criteria exist so that three things cannot happen: the fix passes only the new tests while breaking
first-use behaviour; the reviewer accepts a fix because it looks plausible; or the owner is told the mod is
safer at runtime than it is.

---

## Part 0 — Process invariants

| ID | Criterion |
|---|---|
| AC-P1 | These criteria are committed before implementation inspection, by the reviewer, at a named commit. |
| AC-P2 | The implementation is one immutable checkpoint. The reviewer reviews `git show <checkpoint>:<path>`, not a working tree. |
| AC-P3 | For each finding, the implementer first commits a ≤5-line **design note** stating the chosen semantics and the rejected alternative. Reviewers may not be asked to infer intent. |
| AC-P4 | Every test carries a **class**, and both classes are mandatory. **D — defect-revealing:** must be shown failing on `633ddb2941942fd0136bc892a37895d358f4fb87` with its exact assertion, and passing on the revision. **C — preservation control:** must pass on **both** the old checkpoint and the revision; it exists to prove the repair did not over-correct. A C test is not a weaker D test; requiring it to fail on the old code would make it meaningless. |
| AC-P5 | The reviewer re-executes with its **own** fixtures. My probe is a cross-check, not the acceptance harness; reusing it is not independent verification. |
| AC-P6 | Determinism: no criterion may be satisfied only by a timing-dependent race. Fault injection must be explicit and repeatable. |
| AC-P7 | Both Windows PowerShell 5.1 and `pwsh` 7 are exercised, or 7 is explicitly declared untested again. |

---

## Part 1 — R1: interrupted lease-record publication

**Defect (confirmed by my own reproduction):** a zero-length lease record is classified as "no previous
record" and allows acquisition without `-AcknowledgeUncleanLease`. Intact `held` records are correctly refused.

| ID | Criterion |
|---|---|
| AC-R1-1 | Prior-record state must distinguish at least: **absent** (first use), **released**, **held/unclean**, **unreadable-or-empty**. An existing empty record is `unreadable-or-empty`, never `absent`. |
| AC-R1-2 | First-ever acquisition must be distinguished structurally — e.g. exclusive create — not by content length. |
| AC-R1-3 | An interrupted publication must leave either the previous complete record or an explicitly blocking state. No failure path may degrade a record to `absent`. |
| AC-R1-4 | `unreadable-or-empty` refuses without a token; the refusal names the state and prints a token. The correct token permits exactly one acquisition and is recorded (`AcknowledgedPrevious`). A wrong token refuses. |
| AC-R1-5 | The token must be stable across repeated attempts (derived from durable content or a generation marker), not from ephemeral data such as timestamps. |
| AC-R1-6 | The OS lock semantics are unchanged: one exclusive handle, released in `finally`, record is evidence and never ownership. |

**Required regression tests.** Class **D** must fail on `633ddb2`; class **C** must pass on both.
Both are mandatory; a missing control is a rejected checkpoint, not a partial pass.

| Test | Class | Case | Expected |
|---|---|---|---|
| T-R1-a | **D** | Pre-existing zero-length record, no acknowledgement | Refuse; message identifies unknown/unreadable state; token offered |
| T-R1-b | **D** | Same, with correct token | Acquire once; `AcknowledgedPrevious` recorded |
| T-R1-c | **D** | Same, with wrong token | Refuse |
| T-R1-d | **D** | Fault-injected write failure **after** truncation (injectable writer or stream proxy) | Next acquisition refuses without acknowledgement; previous evidence not silently erased |
| T-R1-e | **C** | **First-ever** acquisition on a fresh lease root | Acquire; no acknowledgement required |
| T-R1-f | **C** | Normal acquire → release → acquire | Acquire; no acknowledgement required |
| T-R1-g | **C** | Two concurrent acquisitions after an unclean record, both with correct token | Exactly one acquires |

T-R1-e and T-R1-f are the **anti-over-correction controls**. A fix that simply refuses every existing empty
file, or that breaks first use, fails acceptance even though it passes T-R1-a.

---

## Part 2 — R2: hardlink aliasing in path containment

**Defect (reproduced):** a hardlink inside `.dev-state` is accepted by `Resolve-WorkspaceOutput`, and writing
through it alters bytes in a different directory. Junctions are correctly refused.

| ID | Criterion |
|---|---|
| AC-R2-1 | Before any in-place write to an **existing** target, the implementation rejects a target whose link count exceeds one, or whose file identity does not match the expected fresh object. |
| AC-R2-2 | First-time creation and ordinary single-link overwrite must still succeed. |
| AC-R2-3 | The lease file specifically must be a uniquely owned object in the lease root; if it is a link to foreign storage, refuse rather than write through it. |
| AC-R2-4 | The refusal message states the reason (multi-link target) and does not claim storage isolation. |
| AC-R2-5 | Documentation states that link-count checks remain check-then-use and do not stop a concurrent actor. |

**Required regression tests:**

| Test | Class | Case | Expected |
|---|---|---|---|
| T-R2-a | **D** | Hardlink in `.dev-state` → external canary, `-Overwrite` | Refuse; canary bytes unchanged |
| T-R2-b | **C** | Same path, ordinary file, `-Overwrite` | Succeed |
| T-R2-c | **C** | Junction at the same location | Refuse (must remain) |
| T-R2-d | **D** | Lease file hardlinked to protected storage | Refuse; protected bytes unchanged |
| T-R2-e | **C** | File created fresh by the tool | Succeed (anti-over-correction) |

---

## Part 3 — R3: cached installation identity

**Defect (source-level, not yet executed):** `New-DevContext` caches the identity; if the installation directory
is replaced at the same path, a stale context and a fresh context can each hold a lease for the same path string.

| ID | Criterion |
|---|---|
| AC-R3-1 | The chosen contract is declared in the AC-P3 design note. My expectation: a **stale context must be detected and refused**, not silently upgraded. |
| AC-R3-2 | Immediately after acquisition and before any write, the implementation re-resolves the installation identity through the stored path and compares it with the cached identity; mismatch releases and refuses. |
| AC-R3-3 | A legitimate reinstall (directory replaced, context restarted) succeeds through a fresh context; it must not require operator improvisation. |
| AC-R3-4 | The refusal message distinguishes "installation replaced or moved" from "another controller holds the lease". |
| AC-R3-5 | No claim is made that this closes check-then-use; the residual is documented. |
| AC-R3-6 | The design note must state, explicitly: (a) which protected operation(s) the revalidation actually fences; (b) the exact window that remains between revalidation and each later use of the path; (c) that a second identity lookup does **not** guarantee exclusive control over a path another process can replace. Acceptance of this criterion is acceptance of a bounded detection, not of exclusivity. |

**Required regression tests:**

| Test | Class | Case | Expected |
|---|---|---|---|
| T-R3-a | **D** | Capture identity → rename away → recreate at same path → proceed with stale context | Refuse before write |
| T-R3-b | **C** | Fresh context against the recreated directory | Acquire |
| T-R3-c | **C** | Unchanged directory, normal acquire | Unchanged behaviour (anti-over-correction) |

**Likely pitfall:** re-resolving inside the lease and then using the *old* path string for the transaction still
leaves the race. If the implementer takes that shortcut, the criterion is not met even though T-R3-a passes.

---

## Part 4 — Evidence the reviewer must produce

| ID | Requirement |
|---|---|
| AC-E1 | A table mapping each criterion above to a test name, its class, and an observed result on both checkpoints. |
| AC-E2 | **D** tests: the old-code failure log with the exact failing assertion, plus the passing result on the revision. **C** tests: the passing result on the old checkpoint **and** on the revision, demonstrating no over-correction. |
| AC-E3 | Disclosure of anything not executed, and of any criterion only partially met. |
| AC-E4 | Explicit statement that R1/R2/R3 closure does **not** establish runtime behaviour of the Tome bridge. |
| AC-E5 | Total scope check: no `Source/` change; no deployment, launch, or save access. |
| AC-E6 | Class totals reported: expected 7 defect-revealing and 8 preservation tests. Missing controls are a rejection, not a partial pass. |

---

## Part 5 — What this cycle still will not prove

Stated now so that closing it is not later mistaken for a safety certificate:

1. The mod's behaviour in Valheim — Harmony ordering, equipment lifecycle, persistence, multiplayer — remains
   unverified.
2. Isolation from Steam, manual edits, and non-cooperating controllers is unchanged: none exists.
3. Cross-user and cross-account exclusion is unchanged: none exists.
4. Whole-session mutual exclusion after launch is unchanged: the lease still ends when the process starts.
5. Hardlink checks remain check-then-use.
6. Save-state recovery does not exist; the backup covers one DLL only.

---

## Part 6 — Deferred: approval-record policy (not a closure condition)

**Status: out of scope for Rotation 1. Tracked separately. No enforcement in this cycle.**

The owner's instruction is accepted: this document must not smuggle a new governance requirement into a
tooling repair. Nothing in this part is a condition of accepting the R1–R3 checkpoint, and no code for it
should be written now.

Retained for later decision as a standalone proposal:

- One append-only record per consequential operation, e.g. `Knowledge/Approvals/APR-<yyyy-mm-dd>-<commit8>.md`.
- Fields: commit SHA, operation, **verbatim owner words**, UTC time, and `Transcribed by:` naming the agent.
- The agent may transcribe; it may not author, paraphrase, or originate the approval. No record may predate the
  message it quotes.
- Enforcement (deployment/launch refusing without a matching record) would be decided with that proposal, not here.

The residual weakness is unchanged and should be stated when it is considered: a transcription is still
testimony. Its mitigation is that it is quoted, attributed, and timestamped rather than summarized by the party
being approved.

---

## Part 7 — What would change my verdict

If this cycle produces a checkpoint that fails the old cases, passes the new ones, and is independently
accepted, I would raise my confidence in the framework's engineering value. What would **not** change it:
another round of review documents without a code change, or a claim that closing R1–R3 makes live use safe.
