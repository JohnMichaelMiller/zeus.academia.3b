---
ai_generated: true
model: "anthropic/claude-opus-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-09-28-pr46-review-instruction-hardening"
prompt: |
  Review PR 46 review comments and improve instruction files and prompts; go ahead
started: "2026-09-28T22:25:00Z"
ended: "2026-09-28T22:55:00Z"
task_durations:
  - task: "chat summary"
    duration: "00:30:00"
total_duration: "00:30:00"
ai_log: "ai-logs/2026/09/28/2026-09-28-pr46-review-instruction-hardening/conversation.md"
source: "johnmillerATcodemag-com"
---

# Chat Summary: Harden instructions and prompts from PR 46 review findings

**Chat ID**: 2026-09-28-pr46-review-instruction-hardening
**Date**: 2026-09-28
**Operator**: johnmillerATcodemag-com
**Model**: anthropic/claude-opus-4.5@unknown
**Duration**: 00:30:00

## Objective

Reduce Copilot code-review findings on AI-generated slices by fixing the instruction files, agents, and prompts that produced PR 46.

## Findings → Root Causes

| PR 46 finding                                   | Root cause                                                         |
| ----------------------------------------------- | ------------------------------------------------------------------ |
| Host missing ProjectReference                   | No build/reference gate                                            |
| Degree codes format-checked only                | No "format vs. existence" rule; prompt named no degree contract    |
| SharedKernel column rename without migration    | No rule for Shared Kernel configuration edits                      |
| LocalDB in appsettings.json                     | Guard rule covered code, not configuration precedence              |
| Extension input omitted; empNr ≤10 instead of 6 | Prompt lacked a precise field contract                             |
| All failures → 400; no 409                      | No error-to-status mapping rule                                    |
| Validator/route tests missing                   | Rules existed; route tests covered validation only; no enforcement |
| Handler uses SharedKernelDbContext              | "may own" wording was permissive                                   |
| ≥1 qualification only in validator              | Invariant-ownership rule limited to numeric/date/enum              |
| Try\* placeholder, InMemory-only tests          | Rules existed; no enforcement                                      |
| Placeholder provenance timestamps               | No consistency rule                                                |

## Work Completed

1. **Rules** (`.github/instructions/`): vertical-slice (must own feature DbContext, mapping-only via `ExcludeFromMigrations`, Shared Kernel config edits need migrations, host compile and reference-existence gates), csharp (all invariants in aggregate, exact-length identifiers, format ≠ existence), fluentvalidation (`.Length(n)`, existence, validator test gate), aspnetcore (error→status mapping, appsettings safety, host references), mediatr (no SharedKernelDbContext in handlers), xunit (route test per declared status, backfill, no EnsureCreated for migration-owned contexts), ai-dev-process (canonical rule owners table, mechanical gate), ai-assisted-output (timestamp consistency).
2. **Prompt standards**: Contract Sheet section (fields, references, invariants, error→status, persistence, host composition, test matrix) and traceability gate in implementation-prompt and implementation-prompt-generation instructions.
3. **Agents/skill**: backend-slice-implementer, backend-domain, data-persistence, testing-verification, slice-verifier, code-review, and the code-review skill now require `eng/verify-slice.ps1` and the Contract Sheet/traceability checks.
4. **Script**: `eng/verify-slice.ps1` — dry-run on RegisterAcademic flagged host reference, LocalDB, SharedKernelDbContext use, missing validator tests, missing route tests, and provenance; on ManageUniversities flagged the Try\* placeholder; on ProvisionExtension flagged InMemory-only tests and missing route tests.
5. **Prompt**: ep-2-1 regenerated with a full Contract Sheet grounded in the ORM model and repository contracts.

## Key Decisions

- **Mapping-only feature context**: RegisterAcademic gets `RegisterAcademicDbContext` with `ExcludeFromMigrations()` so persistence is feature-local and atomic while migration ownership stays with SharedKernel/ProvisionExtension per the ownership matrix.
- **Enforce mechanically over adding bullets**: recurring findings become script checks; checklists already contained most rules.
- **New `GetDegreeByCodeQuery`**: ManageDegrees lacked a resolve contract; the prompt adds one as a bounded prerequisite increment.

## Artifacts Produced

| Artifact                                                                                    | Type         | Purpose                                       |
| ------------------------------------------------------------------------------------------- | ------------ | --------------------------------------------- |
| `eng/verify-slice.ps1`                                                                      | script       | Mechanical slice gate                         |
| `.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md` | prompt       | Contract Sheet-driven RegisterAcademic prompt |
| `.github/instructions/*.instructions.md` (10 files)                                         | instructions | Tightened rules                               |
| `.github/agents/*.agent.md` (6 files), `.github/skills/code-review/SKILL.md`                | agents/skill | Gate wiring                                   |

## Next Steps

- Re-run RegisterAcademic with the regenerated prompt and fix PR 46.
- Fix PR 46 carry-overs (route-test backfill, `TryResolveName` nullability).
- Fix provenance timestamp inconsistencies the script reports in older artifacts.
- Run `verify-slice.ps1` in CI for changed features.

## Compliance Status

✅ Conversation and summary logs created
✅ README updated
⚠️ Chat start time is approximate
⚠️ Edited instruction files keep their original provenance front matter; this log records the edits

---

**Summary Version**: 1.0.0
**Created**: 2026-09-28T22:55:00Z
**Format**: Markdown
