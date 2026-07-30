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
name: implement-academia-ep-6-8-access-level-distribution-report
description: Implement the AccessLevelDistributionReport slice
author: John Miller
tags: [academia, implementation, reports, access-level]
context: "Zeus Academia Phase 6 reporting implementation"
expected_output: "A slice-scoped implementation plan for AccessLevelDistributionReport"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement AccessLevelDistributionReport

## Slice Summary and Business Value

- Slice: AccessLevelDistributionReport
- Business outcome: provide a compact analytical distribution of active academics by access level.
- Out of scope: per-rank grouped listings or qualification analytics.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel access-level derivation code
- ChangeRank and DeregisterAcademic slice files

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.
- .github/instructions/xunit-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: ChangeRank
- Blocking risks: distribution totals must reflect only current active academics and current derived access levels.
- Existing patterns to reuse: grouped report DTOs and read-optimized access-level projections.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                                 | Inputs                                      | Outputs                       | Escalate when                                                   |
| -------------------------- | ---------------------------------------------------------------- | ------------------------------------------- | ----------------------------- | --------------------------------------------------------------- |
| slice-coordinator          | confirm whether the report is counts-only or counts-plus-details | execution plan and reporting conventions    | approved report contract      | active-academic semantics remain ambiguous after deregistration |
| report-projection       | implement grouped distribution query and DTOs                    | current academic state and derivation rules | distribution report code path | active-state filtering requires a broader lifecycle redesign    |
| testing-verification | verify totals, grouping, and post-change updates                 | implemented slice                           | tests and evidence            | totals do not match current active academics                    |

## Ordered Implementation Steps

1. Confirm distribution contract and active-state semantics.
   Targets: src/features/Reports/AccessLevelDistributionReport/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: report output and active-record filtering are explicit.
2. Implement the report query.
   Targets: query, handler, DTOs, endpoint.
   Owner: report-projection.
   Validation before next step: totals are grouped accurately by INT, NAT, and LOC.
3. Verify grouped totals.
   Targets: tests for base counts, post-rank-change updates, and post-deregistration totals.
   Owner: testing-verification.
   Validation before next step: report totals match the current active academic population.

## Verification and Acceptance Criteria

### Review-Prevention Guardrails

- Dependency compatibility is validated for coupled tooling packages when touched (for example xUnit core and runner major versions align).
- Result-style failure factories guard non-null failure payloads in both generic and non-generic wrappers when touched.
- Value-object parse/create APIs reject lossy coercion unless explicitly required and covered by tests.
- Integration tests that provision external resources include deterministic best-effort cleanup in `finally` blocks.
- The report returns correct totals per access level.
- Totals update after rank changes.
- Deregistered academics no longer contribute to active distribution totals.
- Automated tests verify grouped totals against seeded data.

## Human Showcase Steps

1. Starting state: active academics exist across multiple access levels.
   Action: call the access-level distribution report.
   Expected result: the output shows correct totals for INT, NAT, and LOC.
   Value demonstrated: stakeholders can assess access-level balance at a glance.
2. Starting state: change one academic's rank and deregister another.
   Action: call the report again.
   Expected result: totals update to reflect current active data only.
   Value demonstrated: the distribution stays accurate as operational state changes.

## Completion Checklist

- [ ] Review-prevention guardrails were evaluated and marked N/A where not applicable.
- [ ] If test packages changed, compatibility is verified (for example xUnit core and runner major versions align).
- [ ] If value-object parsing or creation changed, lossy coercion is rejected unless explicitly required and tested.
- [ ] If integration tests create external resources, teardown is enforced with best-effort `finally` cleanup.
- [ ] Grouped totals are accurate.
- [ ] Rank changes update the distribution.
- [ ] Deregistration effects are reflected.
- [ ] Tests cover grouped totals.
- [ ] The slice remains a compact analytical report.
