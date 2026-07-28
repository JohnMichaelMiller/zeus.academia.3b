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
name: implement-academia-ep-6-4-tenured-academics-report
description: Implement the TenuredAcademicsReport slice
author: John Miller
tags: [academia, implementation, reports, employment]
context: "Zeus Academia Phase 6 reporting implementation"
expected_output: "A slice-scoped implementation plan for TenuredAcademicsReport"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement TenuredAcademicsReport

## Slice Summary and Business Value

- Slice: TenuredAcademicsReport
- Business outcome: report only tenured academics with the supporting context needed by stakeholders.
- Out of scope: contracted or expiring-contract reporting.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- GrantTenure and ConvertContractToTenure slice files
- qualification read slices

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.

## Prerequisites and Dependency Checks

- Required prior slices: GrantTenure
- Blocking risks: tenured filtering must remain consistent after conversion and status-clearing flows.
- Existing patterns to reuse: read-optimized employment reporting and qualification summary projection.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                              | Inputs                                  | Outputs                  | Escalate when                                                       |
| -------------------------- | ------------------------------------------------------------- | --------------------------------------- | ------------------------ | ------------------------------------------------------------------- |
| slice-coordinator          | confirm tenured-state semantics and report columns            | execution plan and employment slices    | approved report contract | employment-state semantics are ambiguous after multiple transitions |
| report-projection       | implement tenured report and qualification summary projection | employment state and qualification data | tenured report code path | qualification summaries need a separate read model not yet designed |
| testing-verification | verify filtered output and transition behavior                | implemented slice                       | tests and evidence       | converted academics do not appear correctly                         |

## Ordered Implementation Steps

1. Confirm the tenured report contract.
   Targets: src/features/Reports/TenuredAcademicsReport/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: output includes rank and qualification summary for tenured academics only.
2. Implement the report query.
   Targets: query, handler, DTOs, endpoint.
   Owner: report-projection.
   Validation before next step: only tenured academics are returned.
3. Verify filtered behavior.
   Targets: tests for direct-tenure, contract-conversion, and cleared-status cases.
   Owner: testing-verification.
   Validation before next step: report output matches current employment state.

## Verification and Acceptance Criteria

- Only tenured academics appear in the report.
- Qualification summaries are accurate for each returned academic.
- Academics converted from contract to tenure appear correctly.
- Academics whose employment status is cleared do not appear.

## Human Showcase Steps

1. Starting state: one academic is directly tenured and one is converted from contract to tenure.
   Action: call the tenured-academics report.
   Expected result: both appear with rank and qualification summary.
   Value demonstrated: stakeholders can inspect the tenured workforce reliably.
2. Starting state: clear one academic's employment status.
   Action: call the report again.
   Expected result: the cleared academic no longer appears.
   Value demonstrated: the report follows live employment state, not historical assumptions.

## Completion Checklist

- [ ] Tenured-only filtering is accurate.
- [ ] Qualification summaries are present.
- [ ] Conversion and clear-state transitions are reflected.
- [ ] Tests cover employment transition effects.
- [ ] The report remains focused on tenured academics only.
