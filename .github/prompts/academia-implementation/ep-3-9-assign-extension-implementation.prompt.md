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
name: implement-academia-ep-3-9-assign-extension
description: Implement the AssignExtension slice with 1:1 uniqueness enforcement
author: John Miller
tags: [academia, implementation, extensions, command, query]
context: "Zeus Academia Phase 3 extension assignment implementation"
expected_output: "A slice-scoped implementation plan for AssignExtension"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement AssignExtension

## Slice Summary and Business Value

- Slice: AssignExtension
- Business outcome: assign a provisioned extension to an existing academic while preserving 1:1 uniqueness.
- Out of scope: reassignment, release, and availability reporting beyond the current-assignment view.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel extension rules and persistence constraints
- ProvisionExtension and RegisterAcademic slice files

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic, ProvisionExtension
- Blocking risks: concurrency-sensitive uniqueness must be protected in both handler logic and database constraints.
- Existing patterns to reuse: extension inventory lookups, command validation, and integration-first uniqueness testing.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                                                | Inputs                                      | Outputs                             | Escalate when                                                                    |
| -------------------------- | ------------------------------------------------------------------------------- | ------------------------------------------- | ----------------------------------- | -------------------------------------------------------------------------------- |
| slice-coordinator          | confirm route shape and current-assignment query scope                          | execution plan and existing extension model | approved command and query contract | current persistence model cannot express 1:1 uniqueness cleanly                  |
| backend-domain       | implement assignment command, current-assignment query, handlers, and endpoints | extension pool model and academic model     | extension-assignment code path      | safe uniqueness requires a schema change beyond the planned constraint           |
| testing-verification | verify success, duplicate use, and concurrency-sensitive cases                  | implemented slice                           | tests and evidence                  | handler-level checks pass but DB uniqueness still allows conflicting assignments |

## Ordered Implementation Steps

1. Confirm the assignment model and current-assignment response.
   Targets: src/features/Extensions/AssignExtension/ or equivalent, persistence constraints, and route shape.
   Owner: slice-coordinator.
   Validation before next step: the 1:1 academic-extension rule is explicit in both code and schema.
2. Implement assignment behavior.
   Targets: command, validator, handler, endpoint, and any assignment query DTOs.
   Owner: backend-domain.
   Validation before next step: only provisioned, unassigned extensions can be linked to an academic.
3. Implement current-assignment query behavior if the slice keeps that read concern locally.
   Targets: query, handler, response DTO, and endpoint.
   Owner: backend-domain.
   Validation before next step: consumers can inspect the academic's current extension after assignment.
4. Verify uniqueness behavior.
   Targets: integration tests for valid assignment, already-assigned extension, conflicting academic state, and DB constraint alignment.
   Owner: testing-verification.
   Validation before next step: no extension can be assigned to more than one academic.

## Verification and Acceptance Criteria

- A provisioned, unassigned extension can be assigned to an existing academic.
- Attempting to assign an unprovisioned or already assigned extension fails cleanly.
- The slice preserves 1:1 uniqueness between academic and extension in both handler logic and persistence.
- A follow-up query or profile read shows the current assignment after success.

## Human Showcase Steps

1. Starting state: one academic exists and one provisioned extension is free.
   Action: submit the assign-extension command.
   Expected result: the extension is linked to the academic successfully.
   Value demonstrated: extension inventory can be applied to live academic records safely.
2. Starting state: that extension is already assigned.
   Action: attempt to assign it to a second academic.
   Expected result: the command fails and the original assignment is preserved.
   Value demonstrated: the 1:1 extension rule holds under realistic misuse.

## Completion Checklist

- [ ] Provisioned-state and unassigned-state checks are enforced.
- [ ] 1:1 uniqueness is protected in code and persistence.
- [ ] Current-assignment visibility is available after success.
- [ ] Conflict and concurrency-sensitive cases are tested.
- [ ] The slice does not absorb reassignment or release behavior.
