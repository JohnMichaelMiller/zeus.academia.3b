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
name: implement-update-academic-name
description: Guide delivery of the UpdateAcademicName slice for changing an academic's name.
author: John Miller
tags: [implementation, vertical-slice, academia, academic, command]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed UpdateAcademicName slice with command handler, validation, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement UpdateAcademicName

## Objective

Deliver workflow 1.3 so HR administrators can update an academic's display name.

## Slice Boundary

- In scope: update `EmpName`, validate length, preserve all other academic state.
- Non-goals: uniqueness checks, rank updates, employment updates, or qualification changes.
- Dependencies: RegisterAcademic.
- Entry points: `src/backend/Features/Academics/Commands/UpdateAcademicName/`, `/api/academics/{empNr}/name`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                      | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm update behavior and response shape                          | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement name update command, validator, endpoint, and tests       | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate update success, validation failure, and not-found behavior | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                       | Files                                                             | Done When                                 | Verification                          |
| ---- | --------------------------- | -------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- | ----------------------------------------- | ------------------------------------- |
| 1    | `product-manager`           | Confirm whether the endpoint returns the updated profile fragment or a command response only | Plan and workflows                                                | Scope note approved                       | Checklist updated                     |
| 2    | `backend-slice-implementer` | Build update-name command, validator, handler, and endpoint                                  | `src/backend/Features/Academics/Commands/UpdateAcademicName/**`   | Backend compiles and name changes persist | `dotnet build` passes                 |
| 3    | `backend-slice-implementer` | Add tests for successful rename, too-long name, and not-found academic                       | `tests/backend/Features/Academics/Commands/UpdateAcademicName/**` | Tests pass                                | `dotnet test` passes for rename scope |
| 4    | `slice-verifier`            | Execute manual rename scenario and record evidence                                           | HTTP collection or integration tests                              | Acceptance met                            | Verification summary saved            |

## Acceptance Criteria

- Given an existing academic and a name of at most 15 characters, when HR submits the update, then the academic name is changed and persisted.
- Given a name longer than 15 characters, when the request is submitted, then validation fails and the original name is preserved.
- Given a non-existent academic identifier, when the request is submitted, then the system returns not found.

## Verification Plan

- Automated: backend build plus update-name tests.
- Manual: rename an academic, then retry with an invalid name and a missing academic identifier.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with at least one registered academic.
2. Call `PUT /api/academics/{empNr}/name` with a valid new name.
   Expected: the request succeeds and a follow-up profile read shows the new name.
3. Retry with a name longer than 15 characters.
   Expected: validation fails and the stored name does not change.

Value demonstrated: a targeted command can update academic identity data without touching unrelated state.

## Output Artifacts

- Update-name command, validator, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only UpdateAcademicName.
- [ ] Acceptance criteria cover valid update, validation failure, and not-found behavior.
- [ ] Showcase proves isolated state change behavior.
