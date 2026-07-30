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
name: implement-academia-ep-6-3-by-access-level-report
description: Implement the ByAccessLevelReport slice
author: John Miller
tags: [academia, implementation, reports, access-level]
context: "Zeus Academia Phase 6 reporting implementation"
expected_output: "A slice-scoped implementation plan for ByAccessLevelReport"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ByAccessLevelReport

## Slice Summary and Business Value

- Slice: ByAccessLevelReport
- Business outcome: report academic counts and listings by derived access level.
- Out of scope: raw rank management.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel rank-to-access mapping code
- ChangeRank slice files

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.
- .github/instructions/xunit-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: ChangeRank
- Blocking risks: access-level grouping must consume the derived value, not a duplicated manual field.
- Existing patterns to reuse: grouped report DTOs and read-optimized queries.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                    | Inputs                                      | Outputs                       | Escalate when                                                                 |
| -------------------------- | --------------------------------------------------- | ------------------------------------------- | ----------------------------- | ----------------------------------------------------------------------------- |
| slice-coordinator          | confirm output shape and route                      | execution plan and report conventions       | approved report contract      | the current model does not surface derived access level cleanly for reporting |
| report-projection       | implement grouped access-level report               | current academic state and derivation rules | access-level report code path | the report would need direct writes to access level state                     |
| testing-verification | verify INT, NAT, LOC counts and post-change updates | implemented slice                           | tests and evidence            | grouped results diverge from rank-derived expectations                        |

## Ordered Implementation Steps

1. Confirm grouping contract and route.
   Targets: src/features/Reports/ByAccessLevelReport/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: output groups around INT, NAT, and LOC only.
2. Implement the report query.
   Targets: query, handler, DTOs, endpoint.
   Owner: report-projection.
   Validation before next step: grouped results are derived from current rank state.
3. Verify grouped behavior.
   Targets: tests for base counts and updates after rank changes.
   Owner: testing-verification.
   Validation before next step: P, SL, and L consistently map to INT, NAT, and LOC in report output.

## Verification and Acceptance Criteria

### Review-Prevention Guardrails

- Dependency compatibility is validated for coupled tooling packages when touched (for example xUnit core and runner major versions align).
- Result-style failure factories guard non-null failure payloads in both generic and non-generic wrappers when touched.
- Value-object parse/create APIs reject lossy coercion unless explicitly required and covered by tests.
- Integration tests that provision external resources include deterministic best-effort cleanup in `finally` blocks.
- The report groups academics by INT, NAT, and LOC accurately.
- Group membership changes when rank changes alter derived access level.
- No direct access-level mutation path is introduced.
- Automated tests verify grouping and mapping behavior.

## Human Showcase Steps

1. Starting state: academics exist with different ranks.
   Action: call the by-access-level report.
   Expected result: counts and listings appear under INT, NAT, and LOC.
   Value demonstrated: stakeholders can analyze access-level distribution directly.
2. Starting state: change one academic from one rank band to another.
   Action: call the report again.
   Expected result: the academic moves to the correct derived access-level group.
   Value demonstrated: reporting respects the derived-domain rule rather than stale stored values.

## Completion Checklist

- [ ] Review-prevention guardrails were evaluated and marked N/A where not applicable.
- [ ] If test packages changed, compatibility is verified (for example xUnit core and runner major versions align).
- [ ] If value-object parsing or creation changed, lossy coercion is rejected unless explicitly required and tested.
- [ ] If integration tests create external resources, teardown is enforced with best-effort `finally` cleanup.
- [ ] Grouping uses derived access level only.
- [ ] INT, NAT, and LOC mappings are verified.
- [ ] Rank changes update grouped output.
- [ ] Tests cover grouping accuracy.
- [ ] No manual access-level assignment path is introduced.
