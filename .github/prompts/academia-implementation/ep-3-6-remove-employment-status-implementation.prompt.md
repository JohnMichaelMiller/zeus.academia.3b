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
name: implement-academia-ep-3-6-remove-employment-status
description: Implement the RemoveEmploymentStatus command slice
author: John Miller
tags: [academia, implementation, employment, command]
context: "Zeus Academia Phase 3 employment-state implementation"
expected_output: "A slice-scoped implementation plan for RemoveEmploymentStatus"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RemoveEmploymentStatus

## Slice Summary and Business Value

- Slice: RemoveEmploymentStatus
- Business outcome: clear current employment state cleanly when an academic should be neither tenured nor contracted.
- Out of scope: assigning new tenure or contract state.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel employment guard code
- GrantTenure and AssignContract slice files when available

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic
- Blocking risks: clearing state must not leave stale contract dates or contradictory read-model values.
- Existing patterns to reuse: aggregate mutation methods and follow-up read-model verification.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                               | Inputs                                          | Outputs                | Escalate when                                                |
| -------------------------- | -------------------------------------------------------------- | ----------------------------------------------- | ---------------------- | ------------------------------------------------------------ |
| Slice coordinator          | confirm identifier, route, and expected cleared-state response | execution plan and current routes               | approved command scope | existing routes imply a different employment lifecycle model |
| Backend/domain agent       | implement clear-state command, handler, endpoint               | academic aggregate and current employment rules | removal code path      | clearing state would violate an unstated persistence rule    |
| Testing/verification agent | verify tenure-clear and contract-clear scenarios               | implemented slice                               | tests and evidence     | either path leaves stale persisted employment data           |

## Ordered Implementation Steps

1. Confirm the cleared-state contract.
   Targets: src/features/Employment/RemoveEmploymentStatus/ or equivalent.
   Owner: Slice coordinator.
   Validation before next step: handler behavior is defined for both tenured and contracted academics.
2. Implement clear-employment behavior.
   Targets: command, handler, endpoint, and mappings.
   Owner: Backend/domain agent.
   Validation before next step: the command clears IsTenured and ContractEndDate safely.
3. Verify both transition paths.
   Targets: tests for starting from tenured, starting from contracted, and reading back cleared state.
   Owner: Testing/verification agent.
   Validation before next step: read models show no residual employment state.

## Verification and Acceptance Criteria

- Clearing employment status succeeds from either tenured or contracted state.
- The command leaves both IsTenured and ContractEndDate cleared.
- Missing academics fail cleanly.
- Read models reflect the cleared employment state after success.

## Human Showcase Steps

1. Starting state: a tenured academic exists.
   Action: submit the remove-employment-status command.
   Expected result: the academic remains registered but no longer shows tenured or contracted state.
   Value demonstrated: the system can reset employment state without removing the academic.
2. Starting state: a contracted academic exists.
   Action: run the same command and re-read the profile.
   Expected result: contract data is cleared and the profile shows no employment status.
   Value demonstrated: the workflow supports administrative corrections consistently.

## Completion Checklist

- [ ] Both starting states are covered.
- [ ] Stale contract data is removed.
- [ ] Read models reflect cleared state.
- [ ] Missing-record handling is tested.
- [ ] The slice does not assign any new employment state.
