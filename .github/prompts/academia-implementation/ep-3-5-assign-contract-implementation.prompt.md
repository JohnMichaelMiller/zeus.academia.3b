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
name: implement-academia-ep-3-5-assign-contract
description: Implement the AssignContract employment-state command slice
author: John Miller
tags: [academia, implementation, employment, command]
context: "Zeus Academia Phase 3 employment-state implementation"
expected_output: "A slice-scoped implementation plan for AssignContract"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement AssignContract

## Slice Summary and Business Value

- Slice: AssignContract
- Business outcome: place an academic into contracted state with a future end date while preserving XOR employment rules.
- Out of scope: contract renewals and conversion to tenure.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel employment guard code
- RegisterAcademic and ViewAcademicProfile slice files
- .github/instructions/fluentvalidation-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic
- Blocking risks: the future-date rule must be enforced before persistence and stay aligned with RenewContract.
- Existing patterns to reuse: aggregate mutation, command validation, and profile/read-model verification.

## Assigned Agents and Role Boundaries

| Role | Responsibilities | Inputs | Outputs | Escalate when |
| --- | --- | --- | --- | --- |
| Slice coordinator | confirm route and date semantics | execution plan and current API conventions | approved command contract | local time handling is ambiguous for future-date validation |
| Backend/domain agent | implement command, validator, handler, endpoint | Shared Kernel rules and academic model | contract-assignment code path | contract date semantics conflict with current domain primitives |
| Testing/verification agent | verify future-date enforcement and XOR behavior | implemented slice | tests and evidence | contracted state is persisted with invalid or current dates |

## Ordered Implementation Steps

1. Confirm the contract-date semantics.
   Targets: src/features/Employment/AssignContract/ or equivalent and shared date handling.
   Owner: Slice coordinator.
   Validation before next step: the definition of future date is explicit and testable.
2. Implement assign-contract behavior.
   Targets: command, validator, handler, endpoint, and mappings.
   Owner: Backend/domain agent.
   Validation before next step: valid future dates persist and tenured state is cleared.
3. Verify command behavior.
   Targets: tests for valid assignment, current/past date rejection, and profile assertions.
   Owner: Testing/verification agent.
   Validation before next step: the command path preserves XOR employment rules.

## Verification and Acceptance Criteria

- Assigning a contract with a future end date succeeds.
- Current or past end dates are rejected.
- Contract assignment clears tenured state when necessary.
- Profile or list reads reflect the contracted state after success.

## Human Showcase Steps

1. Starting state: a registered academic exists without conflicting employment state.
   Action: submit the assign-contract command with a future end date.
   Expected result: the command succeeds and the academic becomes contracted.
   Value demonstrated: fixed-term employment can be tracked explicitly.
2. Starting state: a registered academic exists.
   Action: submit the command with a past or current date.
   Expected result: validation fails and no contract end date is stored.
   Value demonstrated: the slice protects the business rule that contracts must be future-dated.

## Completion Checklist

- [ ] Future-date validation is explicit and tested.
- [ ] Tenured state is cleared when contract state is applied.
- [ ] Read models reflect the new employment state.
- [ ] Success and failure paths are covered.
- [ ] XOR employment rule remains intact.