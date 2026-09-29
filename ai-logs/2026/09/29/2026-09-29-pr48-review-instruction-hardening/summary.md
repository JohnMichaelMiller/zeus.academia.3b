---
ai_generated: true
model: "anthropic/claude-opus-5.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-09-29-pr48-review-instruction-hardening"
prompt: |
  Review PR 48 review comments and improve instruction files and prompts; go ahead
started: "2026-09-29T14:45:00Z"
ended: "2026-09-29T15:40:00Z"
task_durations:
  - task: "triage, script, and instruction updates"
    duration: "00:55:00"
total_duration: "00:55:00"
ai_log: "ai-logs/2026/09/29/2026-09-29-pr48-review-instruction-hardening/conversation.md"
source: "johnmillerATcodemag-com"
---

# Chat Summary: PR 48 Review-Driven Instruction Hardening

**Chat ID**: 2026-09-29-pr48-review-instruction-hardening
**Date**: 2026-09-29
**Operator**: johnmillerATcodemag-com
**Model**: anthropic/claude-opus-5.5@unknown
**Duration**: 00:55:00

## Objective

Reduce Copilot code-review findings by fixing the guidance and tooling that produced PR 48's six findings.

## Finding → Fix

| PR 48 finding                                  | Root cause                                         | Fix                                                                                                     |
| ---------------------------------------------- | -------------------------------------------------- | ------------------------------------------------------------------------------------------------------- |
| `-AllChangedFeatures` unsupported              | Guidance cited a nonexistent script parameter      | Implemented it; `doc-refs` now fails on any cited nonexistent `verify-slice.ps1` parameter              |
| Snapshot lacks `ValueGeneratedNever` (false +) | Rule demanded textual annotation parity            | `has-pending-model-changes` is the sole authority; EF checks run by default                             |
| `CreateTable` on tables marked deployed        | No deployed-schema rule                            | Deployed-schema gate in vertical-slice §1; Contract Sheet table 5 records schema state                  |
| Unique-index race escapes as 500               | Claim rule treated zero rows as the only lost race | One translation boundary for claim + save; `concurrency` check fails `ExecuteUpdateAsync` outside `try` |
| Null tuple member → `NullReferenceException`   | Null guards covered parameters only                | Element/tuple-member guard rule + null-element test requirement                                         |
| No route test for non-positive `extNr`         | Route tests keyed to status codes                  | Per-field route validation rule; `route-tests` checks an `errors` key per validated field               |

## Key Decisions

- **Mechanical first**: every rule the script can check lives in the script; the check table in `ai-dev-process.instructions.md` must match the implementation exactly.
- **Illustrative paths exempt**: `.github/instructions/`, `.github/prompts/create-*`, and `ai-logs/` are excluded from path-existence checks because they cite example paths by design.
- **Base ref**: the script prefers `refs/remotes/<BaseRef>` because a local branch named `origin/main` exists in this clone.

## Artifacts Produced

| Artifact                                                             | Type        | Purpose                                   |
| -------------------------------------------------------------------- | ----------- | ----------------------------------------- |
| `eng/verify-slice.ps1`                                               | script      | All-changed-features gate with new checks |
| `.github/instructions/csharp-implementation.instructions.md`         | instruction | Snapshot authority; element null guards   |
| `.github/instructions/cqrs-mediatr-efcore.instructions.md`           | instruction | Claim translation boundary                |
| `.github/instructions/vertical-slice-implementation.instructions.md` | instruction | Deployed-schema gate                      |
| `.github/instructions/xunit-implementation.instructions.md`          | instruction | Per-field route validation tests          |
| `.github/instructions/implementation-prompt*.instructions.md`        | instruction | Contract Sheet columns; prompt coverage   |
| `.github/instructions/ai-dev-process.instructions.md`                | instruction | Accurate check table                      |

## Next Steps

- Fix the Part-Twelve failures now reported by `pwsh eng/verify-slice.ps1 -AllChangedFeatures`.
- Consider deleting the stray local branch `origin/main` (user decision).

---

**Summary Version**: 1.0.0
**Created**: 2026-09-29T15:40:00Z
**Format**: Markdown
