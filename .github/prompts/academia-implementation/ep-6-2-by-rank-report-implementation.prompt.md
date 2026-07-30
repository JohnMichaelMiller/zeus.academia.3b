---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "616990b5-0c5d-4735-a876-23fd1ebb4ff6"
prompt: |
  Create an implementation prompt for each slice in the #file:academia-execution-plan.md
started: "2026-04-20T20:40:00Z"
ended: "2026-04-20T21:40:00Z"
task_durations:
  - task: "analyze slice dependencies"
    duration: "00:15:00"
  - task: "draft slice implementation prompt"
    duration: "00:35:00"
  - task: "traceability and review"
    duration: "00:10:00"
total_duration: "01:00:00"
ai_log: "ai-logs/2026/04/20/616990b5-0c5d-4735-a876-23fd1ebb4ff6/conversation.md"
source: ".github/models/workflows/academia-execution-plan.md"
name: implement-academia-ep-6-2-by-rank-report
description: Implement the ByRankReport slice
author: John Miller
tags: [academia, implementation, reports, rank]
context: "Zeus Academia Phase 6 reporting implementation"
expected_output: "A slice-scoped implementation plan for ByRankReport"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ByRankReport

## Slice Summary and Business Value

- Slice: ByRankReport
- Business outcome: report counts and listings by rank, including derived access level context.
- Out of scope: access-level-only distribution reporting.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- ManageRanks and ChangeRank slice files
- AcademicDirectory report files when available

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic, ChangeRank
- Blocking risks: grouped totals must stay aligned with current rank state after updates.
- Existing patterns to reuse: read-optimized reporting and stable grouped DTOs.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                              | Inputs                                                  | Outputs                  | Escalate when                                                |
| -------------------------- | ------------------------------------------------------------- | ------------------------------------------------------- | ------------------------ | ------------------------------------------------------------ |
| slice-coordinator          | confirm grouping output and route shape                       | execution plan and report conventions                   | approved report contract | rank grouping requires a projection strategy not yet chosen  |
| report-projection       | implement grouped query and response contracts                | current academic rank state and access-level derivation | by-rank report code path | grouped output would duplicate logic already owned elsewhere |
| testing-verification | verify counts, listing accuracy, and reaction to rank changes | implemented slice                                       | tests and evidence       | counts drift after rank changes                              |

## Ordered Implementation Steps

1. Confirm the grouped output contract.
   Targets: src/features/Reports/ByRankReport/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: report includes rank-based groupings and associated access-level context.
2. Implement the report query.
   Targets: query, handler, DTOs, endpoint, and projection support if needed.
   Owner: report-projection.
   Validation before next step: grouped counts and member listings reflect current rank state.
3. Verify grouped behavior.
   Targets: tests for seeded counts, listing correctness, and post-rank-change updates.
   Owner: testing-verification.
   Validation before next step: grouped output remains accurate as source data changes.

## Verification and Acceptance Criteria

### Review-Prevention Guardrails

- Dependency compatibility is validated for coupled tooling packages when touched (for example xUnit core and runner major versions align).
- Result-style failure factories guard non-null failure payloads in both generic and non-generic wrappers when touched.
- Value-object parse/create APIs reject lossy coercion unless explicitly required and covered by tests.
- Integration tests that provision external resources include deterministic best-effort cleanup in `finally` blocks.
- The report groups academics by rank accurately.
- Returned listings reflect current rank values after changes.
- Derived access-level context aligns with the current rank mapping.
- Automated tests verify counts and listing accuracy.

## Human Showcase Steps

1. Starting state: several academics exist across multiple ranks.
   Action: call the by-rank report.
   Expected result: the output shows correct counts and member lists per rank.
   Value demonstrated: leadership can inspect rank composition directly.
2. Starting state: change one academic's rank.
   Action: call the report again.
   Expected result: counts and listings update accordingly.
   Value demonstrated: the report reflects live operational changes.

## Completion Checklist

- [ ] Review-prevention guardrails were evaluated and marked N/A where not applicable.
- [ ] If test packages changed, compatibility is verified (for example xUnit core and runner major versions align).
- [ ] If value-object parsing or creation changed, lossy coercion is rejected unless explicitly required and tested.
- [ ] If integration tests create external resources, teardown is enforced with best-effort `finally` cleanup.
- [ ] Grouping logic is accurate.
- [ ] Derived access-level context is consistent.
- [ ] Rank changes are reflected.
- [ ] Tests cover counts and listings.
- [ ] The report remains distinct from pure access-level analytics.
