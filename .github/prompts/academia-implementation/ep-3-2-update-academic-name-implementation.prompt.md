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
name: implement-academia-ep-3-2-update-academic-name
description: Implement the UpdateAcademicName command slice
author: John Miller
tags: [academia, implementation, academics, command]
context: "Zeus Academia Phase 3 academic maintenance implementation"
expected_output: "A slice-scoped implementation plan for UpdateAcademicName"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement UpdateAcademicName

## Slice Summary and Business Value

- Slice: UpdateAcademicName
- Business outcome: allow academic names to be corrected while preserving identity and other state.
- Out of scope: rank, employment, qualification, or extension changes.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/instructions/vertical-slice-implementation.instructions.md
- .github/instructions/fluentvalidation-implementation.instructions.md
- RegisterAcademic and ViewAcademicProfile slice files

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic
- Blocking risks: the name-length rule must stay aligned with registration.
- Existing patterns to reuse: command validator, simple aggregate mutation, and profile/list query visibility checks.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                       | Inputs                                | Outputs                | Escalate when                                                            |
| -------------------- | ------------------------------------------------------ | ------------------------------------- | ---------------------- | ------------------------------------------------------------------------ |
| slice-coordinator    | confirm route, identifier, and update flow             | execution plan and current tree       | approved command scope | current repo uses a different canonical identifier than the plan assumes |
| backend-domain       | implement rename command, validator, handler, endpoint | registration model and existing rules | rename code path       | renaming requires cross-slice side effects not called for in the plan    |
| testing-verification | verify length rule, persistence, and query visibility  | implemented slice                     | tests and evidence     | updated name is not visible through read models after save               |

## Ordered Implementation Steps

1. Confirm command route and identifier conventions.
   Targets: src/features/Academics/UpdateAcademicName/ or equivalent and current academic routing.
   Owner: slice-coordinator.
   Validation before next step: command target and id strategy are explicit.
2. Implement rename behavior.
   Targets: command, validator, handler, endpoint, and mappings.
   Owner: backend-domain.
   Validation before next step: names longer than 15 characters are rejected and valid updates persist.
3. Verify downstream visibility.
   Targets: integration tests for valid update, invalid name, and follow-up read-model assertions.
   Owner: testing-verification.
   Validation before next step: profile or list queries reflect the updated name.

## Verification and Acceptance Criteria

- Valid name updates persist successfully for an existing academic.
- Names longer than 15 characters are rejected.
- Missing academics return the repo-standard not-found behavior.
- Updated names appear in the relevant read models after persistence.

## Human Showcase Steps

1. Starting state: one academic exists.
   Action: submit a valid rename request.
   Expected result: the command succeeds and a subsequent profile or list query shows the new name.
   Value demonstrated: academic records can be corrected without re-registration.
2. Starting state: one academic exists.
   Action: submit a rename request with a name over 15 characters.
   Expected result: validation fails and the persisted name remains unchanged.
   Value demonstrated: the slice preserves the shared naming rule consistently.

## Completion Checklist

- [ ] Name-length validation matches registration.
- [ ] Rename behavior does not alter unrelated academic fields.
- [ ] Read models reflect the persisted update.
- [ ] Success and failure paths are tested.
- [ ] Verification evidence exists for the slice's acceptance criteria.
- [ ] Missing-record handling is verified.
