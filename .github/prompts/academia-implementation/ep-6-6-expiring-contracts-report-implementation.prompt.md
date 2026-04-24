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
name: implement-academia-ep-6-6-expiring-contracts-report
description: Implement the ExpiringContractsReport slice
author: John Miller
tags: [academia, implementation, reports, contracts]
context: "Zeus Academia Phase 6 reporting implementation"
expected_output: "A slice-scoped implementation plan for ExpiringContractsReport"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ExpiringContractsReport

## Slice Summary and Business Value

- Slice: ExpiringContractsReport
- Business outcome: identify contracts expiring within a configurable date window, defaulting to 90 days.
- Out of scope: the broader contracted roster outside the chosen threshold.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- AssignContract and RenewContract slice files
- ContractedAcademicsReport files when available

## Prerequisites and Dependency Checks

- Required prior slices: AssignContract
- Blocking risks: date-window boundaries must be explicit and tested, especially around today and threshold edges.
- Existing patterns to reuse: report filtering by date and configurable query parameters.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                      | Inputs                                               | Outputs                             | Escalate when                                                     |
| -------------------- | ----------------------------------------------------- | ---------------------------------------------------- | ----------------------------------- | ----------------------------------------------------------------- |
| slice-coordinator    | confirm default threshold and date-boundary semantics | execution plan and current date-handling conventions | approved report contract            | current date handling is inconsistent across employment slices    |
| report-projection    | implement threshold-based report query                | contract end-date data and filter rules              | expiring-contracts report code path | date logic requires timezone semantics beyond current conventions |
| testing-verification | verify default and custom windows plus boundary dates | implemented slice                                    | tests and evidence                  | near-boundary contracts appear inconsistently                     |

## Ordered Implementation Steps

1. Confirm threshold defaults and boundary rules.
   Targets: src/features/Reports/ExpiringContractsReport/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: default threshold is 90 days and custom threshold behavior is defined.
2. Implement the report query.
   Targets: query, handler, DTOs, endpoint.
   Owner: report-projection.
   Validation before next step: report includes contracts expiring within the configured window only.
3. Verify threshold behavior.
   Targets: tests for default threshold, custom threshold, and boundary-date cases.
   Owner: testing-verification.
   Validation before next step: date-window behavior is deterministic.

## Verification and Acceptance Criteria

- The default report window is 90 days unless another threshold is supplied.
- Contracts inside the threshold appear; contracts outside it do not.
- Boundary dates are handled consistently.
- Automated tests cover default, custom, and boundary scenarios.

## Human Showcase Steps

1. Starting state: multiple contracted academics have varied end dates.
   Action: call the expiring-contracts report with no custom threshold.
   Expected result: only contracts expiring within 90 days are returned.
   Value demonstrated: administrators can identify near-term contract risk quickly.
2. Starting state: the same seeded data.
   Action: call the report with a shorter or longer threshold.
   Expected result: the result set changes consistently with the supplied window.
   Value demonstrated: the report supports planning at different horizons.

## Completion Checklist

- [ ] Default threshold is 90 days.
- [ ] Custom thresholds are supported.
- [ ] Boundary-date handling is tested.
- [ ] Results match current contract data.
- [ ] Verification evidence exists for the slice's acceptance criteria.
- [ ] The report stays focused on expiring-window analysis.
