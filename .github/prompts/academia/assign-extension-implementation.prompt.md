---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-04-18-academia-slice-implementation-prompts"
prompt: |
  create a implementation prompt for each slice in the #file:academia-implementation-plan.md
started: "2026-04-18T13:10:00-07:00"
ended: "2026-04-18T13:55:00-07:00"
task_durations:
  - task: "context analysis"
    duration: "00:10:00"
  - task: "prompt authoring"
    duration: "00:28:00"
  - task: "catalog and provenance updates"
    duration: "00:07:00"
total_duration: "00:45:00"
ai_log: "ai-logs/2026/04/18/2026-04-18-academia-slice-implementation-prompts/conversation.md"
source: "johnmillerATcodemag-com"
name: implement-assign-extension
description: Guide delivery of the AssignExtension slice for assigning an available extension to an academic.
author: John Miller
tags: [implementation, vertical-slice, academia, extension]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed AssignExtension slice with command handler, uniqueness enforcement, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement AssignExtension

## Objective

Deliver workflows 4.1 and 4.4 so an available extension can be assigned to an academic and later read as part of the academic's state.

## Slice Boundary

- In scope: assign a provisioned unassigned extension to an academic, enforce 1:1 uniqueness, expose current extension state through the slice's response.
- Non-goals: reassigning or releasing extensions.
- Dependencies: RegisterAcademic and ProvisionExtension.
- Entry points: `src/backend/Features/Extensions/Commands/AssignExtension/**`, `/api/academics/{empNr}/extension`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Ensure a database-level uniqueness constraint is part of the implementation plan in addition to handler guards.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                            | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm assignment semantics and readback expectation                     | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement assign-extension command, endpoint, uniqueness guard, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate 1:1 uniqueness and readback behavior                             | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                                                                               | Files                                                           | Done When                                       | Verification                                    |
| ---- | --------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------- | ----------------------------------------------- | ----------------------------------------------- |
| 1    | `product-manager`           | Confirm whether assigning a new extension to an academic who already has one is rejected in this slice                                               | Plan and workflows                                              | Scope note approved                             | Checklist updated                               |
| 2    | `backend-slice-implementer` | Build assign-extension command, validator, handler, endpoint, and unique persistence rules                                                           | `src/backend/Features/Extensions/Commands/AssignExtension/**`   | Backend compiles and assignment persists safely | `dotnet build` passes                           |
| 3    | `backend-slice-implementer` | Add tests for successful assignment, already-assigned extension rejection, missing extension rejection, and academic-already-has-extension rejection | `tests/backend/Features/Extensions/Commands/AssignExtension/**` | Tests pass                                      | `dotnet test` passes for assign-extension scope |
| 4    | `slice-verifier`            | Run manual assignment scenarios and collect evidence                                                                                                 | HTTP collection or integration tests                            | Acceptance met                                  | Verification summary saved                      |

## Acceptance Criteria

- Given a provisioned unassigned extension and an academic without one, when the extension is assigned, then the academic uses that extension.
- Given an extension already assigned to another academic, when the request is submitted, then the system rejects it.
- Given an academic who already has an extension, when a new assignment is attempted in this slice, then the system rejects it.
- Given the assignment succeeds, when the academic profile is read, then the assigned extension number is returned.

## Verification Plan

- Automated: backend build plus assign-extension tests.
- Manual: assign a valid extension, retry with an in-use extension, and verify the academic profile.
- Evidence: API responses, test output, and migration evidence for unique constraints.

## Showcase Steps

1. Start the API with one academic lacking an extension and one available provisioned extension.
2. Call `POST /api/academics/{empNr}/extension` with the extension number.
   Expected: the assignment succeeds.
3. Call the academic profile endpoint.
   Expected: the profile shows the assigned extension.
4. Retry assigning the same extension elsewhere.
   Expected: the API rejects the request.

Value demonstrated: the system enforces the 1:1 extension rule while making the academic reachable by extension immediately.

## Output Artifacts

- Assign-extension command, endpoint, tests, uniqueness persistence changes, and verification notes.

## Validation Checklist

- [ ] The prompt targets only AssignExtension.
- [ ] Acceptance criteria cover extension and academic uniqueness.
- [ ] Showcase proves both assignment and readback value.
