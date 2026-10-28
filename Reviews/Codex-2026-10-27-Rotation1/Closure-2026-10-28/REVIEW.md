# Rotation 1 R1–R3 closure: independent implementation review

2026-10-28 · Codex · **Recommendation: ACCEPT WITH DOCUMENTED LIMITATIONS.**

What this change does: it distinguishes an empty lease record from first use, refuses existing
hardlinked write targets in the covered tools, and pins the identified installation directory
through each protected transaction or process-start boundary. The frozen behavior requirements
reproduce independently. No blocking implementation defect was found in this bounded review.
The project owner retains final acceptance authority.

## 1. Checkpoints and scope

| Role | Exact Git commit |
|---|---|
| Original | `633ddb2941942fd0136bc892a37895d358f4fb87` |
| Frozen criteria | `b301ceebf0a0592d8ea405e77d641f4f1b91124b` |
| Design | `4657706f0419f06bff59b4538697f7da1d711c64` |
| R2 addendum | `28d0d9312e1e04c3726005d82ba6a35dfe734bc9` |
| Revision | `d1a1761016a69c84603cea0ada66eb441aa905c7` |

Verified direct ancestry: original → design → addendum → revision. Claude's branch was
claude/review-c1, HEAD was the revision, and its worktree was clean. Reviewed exact Git objects,
including IMPLEMENTATION.md, all design notes and all ten committed result files; no mutable
implementation was used. Our starting branch was research/dev-008-supervised-testing at the
frozen-criteria commit. Five earlier unfinished review files were preserved, not amended.

The original-to-revision diff has **27 files**: six operational/test scripts, five Knowledge
files, .gitattributes, five design/report files and ten text evidence files. Changed scripts:
WorkspaceSafety.psm1, DevWorkflow.psm1, Get-KnowledgeSnapshot.ps1, Test-DevWorkflow.ps1,
Test-Rotation1Closure.ps1 and Test-TomeProvenanceArtifact.ps1. **No Source/ or Dev.ps1 change.**
Structured snapshot comparison changes only repository_sources and rotation1_closure_review;
installed assembly/configuration snapshot data is unchanged. That is a Git comparison, not a
new live-installation fingerprint certification.

Criteria were read from b301cee's committed object. SHA-256 remains
`A2397865E2A025587ADA8BFCEC810687E55F5EE91F9AF7F1415031C5241DAE9A`.

## 2. Frozen matrix: independent results

The same reviewer-owned Independent-Closure.ps1 ran against both sets of exact source blobs
in separate processes. Metadata reads use JSON plugin fixtures; external process observations
are empty controlled inputs; no executable named valheim.exe is started. Failure assertions
are retained verbatim in EVIDENCE.json and raw local logs.

All four columns below were **executed**, not inferred from Claude's results.

| Frozen test | Class | Old PS5 | New PS5 | Old PS7 | New PS7 | Assertion under test |
|---|---|---|---|---|---|---|
| T-R1-a | D | FAIL | PASS | FAIL | PASS | Existing empty record refuses with unreadable/unknown state and token |
| T-R1-b | D | FAIL | PASS | FAIL | PASS | Stable correct token is recorded and cannot be reused |
| T-R1-c | D | FAIL | PASS | FAIL | PASS | Wrong token refuses |
| T-R1-d | D | FAIL | PASS | FAIL | PASS | Failed publication preserves blocking held evidence |
| T-R1-e | C | PASS | PASS | PASS | PASS | First-ever acquisition needs no acknowledgement |
| T-R1-f | C | PASS | PASS | PASS | PASS | Normal release/reacquisition succeeds |
| T-R1-g | C | PASS | PASS | PASS | PASS | Correct-token contender cannot acquire while first owner holds |
| T-R2-a | D | FAIL | PASS | FAIL | PASS | Hardlinked evidence refuses; independent canary unchanged |
| T-R2-b | C | PASS | PASS | PASS | PASS | Ordinary single-link overwrite succeeds |
| T-R2-c | C | PASS | PASS | PASS | PASS | Existing junction refuses; no redirected output |
| T-R2-d | D | FAIL | PASS | FAIL | PASS | Hardlinked lease refuses; protected bytes unchanged |
| T-R2-e | C | PASS | PASS | PASS | PASS | Fresh output and parent creation succeed |
| T-R3-a | D | FAIL | PASS | FAIL | PASS | Stale context refuses before target/repository writes; cleanup releases pin |
| T-R3-b | C | PASS | PASS | PASS | PASS | Fresh post-reinstall context deploys exact synthetic source bytes |
| T-R3-c | C | PASS | PASS | PASS | PASS | Unchanged installation deploys; independent record inspection confirms release |

Per host: **old D 0/7, C 8/8; revision D 7/7, C 8/8**. Old process exit7 is expected;
revision exit0. Total:60 independent observations, not60 passing tests. Old failures are the
specific assertions above, not compilation/import failures or incidental early exits.

T-R1-g uses a deterministic overlap: first holder remains active while an independently launched
same-host-version PowerShell contender attempts the same token. No simultaneous-start claim or
sleep-based race oracle. T-R1-d invokes the actual writer through a faulting stream seam that
forwards Position/SetLength/Flush, but throws before writing bytes. The old writer empties the
file; the revision leaves the previous complete held frame. Both classes stay mandatory.

## 3. Criteria trace

| Criteria | Evidence / classes | Assessment |
|---|---|---|
| AC-P1/P2 | Frozen Git object/hash; immutable revision exports and manifests | Verified |
| AC-P3 | Design commit precedes implementation; five numbered contract lines per note, plus heading/provenance; separate R2 addendum | Semantics and rejected alternatives explicit; count refers to five contract statements, not nine total physical lines including metadata |
| AC-P4/P5 | Fifteen reviewer-owned D/C tests above; author's suite reported separately | Verified |
| AC-P6 | Explicit writer fault; controlled link/root substitutions; held-owner/contender schedule | Verified for executed cases |
| AC-P7 | Windows PowerShell5.1.26100.9711 and PowerShell7.6.5, both originals/revisions | Verified; no closure-suite PS7 gap |
| AC-R1-1 | T-R1-a/f (D/C), A1, author X-R1-3/5 | Empty/held/released/absent distinguished; parser agreement exercised |
| AC-R1-2 | T-R1-e (C), source CreateNew/Open loop, X-R1-6 | Structural first use; no content-length inference |
| AC-R1-3 | T-R1-d (D), A1, X-R1-1/2 | Zero-byte failure and selected torn/trailing/hash-damaged states block |
| AC-R1-4 | T-R1-a/b/c (D), X-R1-4 | Refusal/token/acknowledgement/consumption verified |
| AC-R1-5 | T-R1-b (D), source Get-LeaseAcknowledgementToken | Repeated unchanged refusals preserve token and bytes |
| AC-R1-6 | T-R1-f/g (C), workflow contention/crash/exception cases | OS handle remains ownership; release is not recovery |
| AC-R2-1 | T-R2-a (D), X-R2-1..5, A4 | Covered in-place targets refuse aliases; atomic replacement detaches old name |
| AC-R2-2 | T-R2-b/c/e (C) | Positive overwrite/create controls and junction refusal preserved |
| AC-R2-3 | T-R2-d (D), source held-handle link inspection | Lease aliases refused before in-place record writes |
| AC-R2-4 | T-R2-a/d (D), inspected refusal text | Reason names multi-link target and expressly disclaims storage isolation |
| AC-R2-5 | R2 note/addendum, DEV-009, source comments | Check-then-use limitation explicit |
| AC-R3-1 | R3 note, T-R3-a/c (D/C) | Stale context refuses; never silently upgraded |
| AC-R3-2 | T-R3-a (D), A2/A3, X-R3-1/2 | Pin identity checked after acquisition, before protected work; pin covers receipt/process-start seams |
| AC-R3-3 | T-R3-b (C) | Fresh context succeeds after root recreation |
| AC-R3-4 | T-R3-a (D), X-R3-2 | Replacement message differs from contention |
| AC-R3-5/6 | R3 note5, DEV-009, A2/A3 | Root-only pin and remaining child-path/use/release windows explicitly bounded |
| AC-E1/E2/E6 | This matrix + EVIDENCE.json + raw logs | Seven exact old D failures, eight old/new C passes, seven new D passes per host |
| AC-E3/E4/E5 | Limitations/scope below | No equipment/runtime certification; no production C# or live operation |

For AC-R3-2, “before write” means protected repository/installation work: acquiring/releasing
the lease itself necessarily writes its audit record before/after pin validation. Stale-context
refusal leaves that record released/no-op and creates no target/backup/staging/receipt changes.
Bare Enter-ResourceLease is still generic: pinning is enforced by Enter-DevLease in the protected
workflow, not retrofitted onto every caller of the generic primitive.

## 4. Independent adversarial checks and the thirteen additional tests

Additional-Review.ps1 ran **four independent tests on each host,8/8**:

| Test | Evidence |
|---|---|
| A1 | Empty, bad-prefix, truncated, trailing-byte and altered-checksum records block acquisition; Status agrees; refusals preserve bytes |
| A2 | Actual synthetic Deploy/Rollback: root rename refused at all four prepared/completed receipt writes; final bytes restored; receipts identify correct resource; rename works after release |
| A3 | Root rename refused inside final process-start stub and succeeds after return; no actual client launched |
| A4 | Actual journal publisher detaches an existing hardlink on File.Replace; alias retains old bytes; preexisting linked temporary refuses without overwrite |

Claude's test file was inspected assertion-by-assertion, then rerun unchanged on old sources
(D,C) and revised sources (D,C,X). It is **reproducibility evidence, not independent confirmation**.
Final run: old exit7; revision **28/28**, including **13/13 X**, on both hosts.

| Author tests | What their assertions actually establish |
|---|---|
| X-R1-1/2 | Injected partial release and complete-frame-plus-trailing-bytes refuse with expected state/reason |
| X-R1-3/4 | Selected legacy released/held records preserve behavior; no/wrong/superfluous-token messages differ |
| X-R1-5/6 | Selected Status parsing agrees; injected exists→vanished retry and sharing error branches behave as named; not all I/O error codes |
| X-R2-1/2/3 | Report overwrite positive/negative; snapshot/artifact hardlinks refuse before missing installation/Cecil; not all output aliases or races |
| X-R2-4/5 | Existing journal temporary refuses; directory-destination publication failure cleans own temp; normal publication succeeds; deployment detaches old alias |
| X-R3-1/2 | Root move fails during transition, succeeds after; stale launch performs no process-start call and leaves released/no-op evidence |

No assertion establishes power-loss durability, arbitrary-actor confinement, complete state
recovery or game behavior. X-R3-1 samples the transaction before replacement; independent A2
extends observation to prepared/completed receipts and rollback without changing its criterion.

## 5. Commands, results and evidence

Existing tools: Python3.14.0, Git2.55.0.windows.5, .NET SDK10.0.100, Windows11;
PowerShell versions above. Everything ran in our ignored scratch using immutable blob exports.
Exact expanded commands, exit codes, durations and log hashes are in [EVIDENCE.json](EVIDENCE.json).
Portable commands replace machine roots with ${REPO}/${USERPROFILE}; raw logs retain actual commands.

```powershell
python -B Reviews/Codex-2026-10-27-Rotation1/Closure-2026-10-28/run_review.py
python -B Reviews/Codex-2026-10-27-Rotation1/Closure-2026-10-28/run_review.py --author --short-temp
python -B Reviews/Codex-2026-10-27-Rotation1/Closure-2026-10-28/run_checks.py --no-git-query
python -B Reviews/Codex-2026-10-27-Rotation1/Closure-2026-10-28/verify_review.py --validate-snapshot
python -B Reviews/Codex-2026-10-27-Rotation1/Closure-2026-10-28/verify_review.py
```

| Check | Result |
|---|---|
| Test-DevWorkflow | Exit0,30/30; its denied-ACL case reports not reproducible, not access-denial coverage |
| Test-WorkspacePaths | Exit0,10/10 |
| Revision Python unittest | Exit0,28/28 |
| Release Build.ps1 | Exit0,0 warnings/errors after fixture adjustment described below |
| Ordinary artifact audits | Diagnostic, lifecycle, provenance and bridge audits all exit0; default-off bridge, two postfixes,211 audited calls |
| Build output | Only PoVPlus.dll; SHA-256 `a470b4f8e01b5860a641f2e13e1b0df2afcd780f8a4ebcd29e61f7b3a01e3877` |
| Strict knowledge, own workspace | Exit0,127 records,0 errors/warnings |
| Strict knowledge, revision | Exit0,125 records,0 errors/warnings; accepted hashes unchanged,13 watched CRLF files materialized per checkout contract |
| Git whitespace and review syntax/evidence checks | Pass; exact source blobs and earlier artifacts preserved |

Retained ignored evidence roots:

- `run-42e0181751e14cbabbaa888b77861071`: independent matrix, four raw logs and per-run JSON.
- `run-266bfdb6d66c4cc78fbfa272c42b0ac5`: final author-harness reproduction.
- `checks-eab29810948a454bb340932699990961`: independent extras, workflow/path/Python and initial build failure.
- `checks-76e0262830414c3fb05b6ed2f4e6d05d`: adjusted successful build and four artifact audits.

All are under `.dev-state/rotation1/closure-review-20261028/`. verify_review.py validates these
retained runs; a fresh replay produces new UUID paths and should compare results, not identifiers.

Failures were retained, not suppressed:

1. The first author run (`run-5f1b38b856804f7b8d68ee01ba85d23b`) had revision exit1 on both
   hosts: X-R2-3's nested Windows PowerShell script did not load, reporting unsigned-script policy
   refusal from the long fixture path. Repeating unchanged sources with a shorter ignored TEMP
   root made all28 pass; no persistent execution-policy change or test weakening. This establishes
   an environment/path-length-sensitive harness limitation, not the exact Windows internal cause.
2. The first build failed because our path-test `.git` marker is deliberately not a Git checkout.
   A build-only rerun set **process-local EnableSourceControlManagerQueries=false**, supported by
   the installed SDK's SourceLink targets. It disables source-revision metadata lookup only.
   Build.ps1, project code, reference validation and artifact safety checks were unchanged. This is
   an adjusted source-fixture compile, not a claim that a real unmodified checkout was rebuilt here.
3. One discovery command used an unexpanded Windows rg wildcard and failed; corrected with rg's
   `-g` filter. A Git-show/Select-Object read pipeline exited1 from early pipe closure; not a test
   failure. Subsequent exact blob exports avoid that pipeline.
4. The staged byte-identity check initially detected Git normalizing CRLF in the generated JSON.
   Evidence serialization now writes UTF-8 LF bytes; review-local .gitattributes preserves these
   hashed files on checkout. No production attributes, criteria or test outcomes changed.

The build hash differs from Claude's committed build hash. This is not a deployment mismatch:
source directory and source-control metadata differ in this fixture, and the earlier unidentified
build-byte variance remains historical. No determinism claim or hash-based equivalence is made.

## 6. Findings and limits

**Blocking defects: none demonstrated within the frozen, covered R1–R3 contract.**

**Nonblocking documented limits:**

1. Root pinning is bounded. WorkspaceSafety:399/420 and DevWorkflow:55/263/362 bind a handle
   to the expected root until final receipt/process start returns. Descendant files/directories
   remain path-based; after release nothing is pinned. Ancestor-rename behavior, alternate
   filesystems and privileged actors are untested. No machine-wide or whole-session ownership.
2. Hardlink checks are handle-based at WorkspaceSafety:210/219; path-based callers still have a
   check/use window. A link added after inspection is not prevented. Directory-entry replacement
   safely preserves an old alias's bytes; it is distinct from refusing an in-place hardlink write.
3. Framing detects the exercised incomplete data, not forged history or power-loss durability.
   Token acknowledgement remains an operator decision, not independent recovery verification.

**Out of scope, disclosed rather than silently “fixed”:** the six unchanged Tome build harnesses
Test-TomeBridge, Test-TomeLifecycle, Test-TomeNeutral, Test-TomeOffline, Test-TomeStageB and
Test-TomeProvenance still regenerate fixed `.dev-state` project/log/evidence paths without the
new link checks (their WriteAllText/Set-Content call sites were inspected). R2's addendum explicitly
narrows its original broad claim to listed writers. This review does not certify those six harnesses
as alias-safe or create a new infrastructure task. They do not invalidate the covered DLL workflow
or the15 frozen tests; any stronger “all PoVPlus writers” claim remains false.

No criterion was omitted because a test name claimed coverage. AC-P3's five numbered statements
have extra heading/provenance lines; the literal whole-file five-line reading is not established.
All operational R1/R2/R3 criteria above are supported only within their stated fixture limits.
Full workflow regression was rerun under PS5; PS7 coverage here is the complete closure matrix,
author's thirteen X tests and independent extras, not every historical project suite.

## 7. Preservation and handoff

New review-only files: Independent-Closure.ps1, Additional-Review.ps1, run_review.py, run_checks.py,
verify_review.py, EVIDENCE.json, review-local .gitattributes and this REVIEW.md. The previous five unfinished review artifacts
and frozen suite/criteria remain untouched. Generated binaries, logs and scratch are ignored.
Any preservation commit is limited to these eight new files; no previous artifacts are silently
staged. Its SHA is reported externally because it cannot be embedded in its own commit.

No production fix, integration, push, merge, rebase, cherry-pick, main change, Claude-worktree write,
Valheim launch/deployment, protected-installation/configuration write, Gale-profile or save access,
Steam/cloud or server operation occurred. Build/artifact checks read existing installed reference
assemblies only. No claim of a fresh343-file installation scan is made.

**R1–R3 closure does not establish runtime behavior of the Tome bridge. Stage B acceptance is
verified. Stage C1 ownership completeness, equipment lifecycle, persistence, neutral-state
certification and rollback safety remain unverified.**

Final recommendation: **ACCEPT WITH DOCUMENTED LIMITATIONS** as a scoped offline engineering
baseline. Owner acceptance and any integration are separate. Do not start remediation, isolated
environment implementation or runtime work from this review. Stop pending the owner's decision.
