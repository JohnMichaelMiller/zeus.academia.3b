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
name: implement-academia-ep-6-5-contracted-academics-report
description: Implement the ContractedAcademicsReport slice
author: John Miller
tags: [academia, implementation, reports, employment]
context: "Zeus Academia Phase 6 reporting implementation"
expected_output: "A slice-scoped implementation plan for ContractedAcademicsReport"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ContractedAcademicsReport

## Slice Summary and Business Value

- Slice: ContractedAcademicsReport
- Business outcome: list currently contracted academics ordered by upcoming contract end date.
- Out of scope: expiring-window analytics.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- AssignContract and RenewContract slice files
- any shared reporting query conventions

## Prerequisites and Dependency Checks

- Required prior slices: AssignContract
- Blocking risks: the report must exclude tenured or cleared-state academics and sort consistently by end date.
- Existing patterns to reuse: read-optimized reporting and deterministic sort contracts.

## Assigned Agents and Role Boundaries

| Role | Responsibilities | Inputs | Outputs | Escalate when |
| --- | --- | --- | --- | --- |
| Slice coordinator | confirm contracted-state semantics and sort order | execution plan and employment slices | approved report contract | employment data does not expose current contract state cleanly |
| Backend/domain agent | implement report query and DTOs | contract state and end-date data | contracted report code path | the report needs separate projection storage for performance |
| Testing/verification agent | verify filtering and ascending sort order | implemented slice | tests and evidence | results are unsorted or include non-contracted academics |

## Ordered Implementation Steps

1. Confirm filter and sort contract.
   Targets: src/features/Reports/ContractedAcademicsReport/ or equivalent.
   Owner: Slice coordinator.
   Validation before next step: only current contracts are included and sort order is ascending by end date.
2. Implement the report query.
   Targets: query, handler, DTOs, endpoint.
   Owner: Backend/domain agent.
   Validation before next step: results reflect current contract state and sort correctly.
3. Verify report behavior.
   Targets: tests for filtering, sort order, and updated dates after renewal.
   Owner: Testing/verification agent.
   Validation before next step: report accuracy is stable on representative data.

## Verification and Acceptance Criteria

- Only currently contracted academics appear.
- Results are sorted by contract end date ascending.
- Renewed contracts show the updated end date correctly.
- Non-contracted academics do not appear.

## Human Showcase Steps

1. Starting state: multiple academics have contract end dates.
   Action: call the contracted-academics report.
   Expected result: only contracted academics appear in ascending end-date order.
   Value demonstrated: administrators can see the active contract roster in priority order.
2. Starting state: renew one contract to a later date.
   Action: call the report again.
   Expected result: that academic moves to the correct sorted position.
   Value demonstrated: the report reacts immediately to lifecycle changes.

## Completion Checklist

- [ ] Contracted-only filtering is accurate.
- [ ] Sort order is deterministic and ascending.
- [ ] Renewal effects are reflected.
- [ ] Tests cover filtering and ordering.
- [ ] The report stays distinct from expiring-window logic.