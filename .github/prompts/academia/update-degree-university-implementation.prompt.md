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
name: implement-update-degree-university
description: Guide delivery of the UpdateDegreeUniversity slice for correcting the university on an existing qualification.
author: John Miller
tags: [implementation, vertical-slice, academia, qualification]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed UpdateDegreeUniversity slice with command handler, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement UpdateDegreeUniversity

## Objective

Deliver workflow 5.2 so HR can correct the university recorded for an academic-degree pair.

## Slice Boundary

- In scope: locate an existing academic-degree qualification and replace the university code.
- Non-goals: adding new degree pairs or removing qualifications.
- Dependencies: RecordDegreeObtained.
- Entry points: `src/backend/Features/Qualifications/Commands/UpdateDegreeUniversity/**`, `/api/academics/{empNr}/qualifications/{degreeCode}/university`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm correction semantics and response expectations        | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement update-university command, endpoint, and tests      | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate existing-record requirement and persisted correction | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                            | Files                                                                      | Done When                                        | Verification                                            |
| ---- | --------------------------- | ------------------------------------------------------------------------------------------------- | -------------------------------------------------------------------------- | ------------------------------------------------ | ------------------------------------------------------- |
| 1    | `product-manager`           | Confirm whether change history is out of scope for MVP                                            | Plan and workflows                                                         | Scope note approved                              | Checklist updated                                       |
| 2    | `backend-slice-implementer` | Build update-degree-university command, handler, validator, and endpoint                          | `src/backend/Features/Qualifications/Commands/UpdateDegreeUniversity/**`   | Backend compiles and updated university persists | `dotnet build` passes                                   |
| 3    | `backend-slice-implementer` | Add tests for successful correction, missing qualification rejection, and invalid university code | `tests/backend/Features/Qualifications/Commands/UpdateDegreeUniversity/**` | Tests pass                                       | `dotnet test` passes for qualification-correction scope |
| 4    | `slice-verifier`            | Run manual correction scenario and capture evidence                                               | HTTP collection or integration tests                                       | Acceptance met                                   | Verification summary saved                              |

## Acceptance Criteria

- Given an existing academic-degree qualification and a valid university code, when HR updates the university, then the qualification record reflects the new university.
- Given a missing academic-degree qualification, when the request is submitted, then the system rejects it.
- Given an invalid university code, when the request is submitted, then validation fails and the original record remains unchanged.

## Verification Plan

- Automated: backend build plus update-university tests.
- Manual: update an existing qualification and retry with a missing academic-degree pair.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with an academic that already has a recorded degree.
2. Call `PUT /api/academics/{empNr}/qualifications/{degreeCode}/university` with a different valid university code.
   Expected: the qualification now shows the corrected university.
3. Retry for a degree the academic does not hold.
   Expected: the API rejects the request.

Value demonstrated: qualification data can be corrected without recreating or removing the qualification record.

## Output Artifacts

- Update-degree-university command, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only UpdateDegreeUniversity.
- [ ] Acceptance criteria cover existing-record requirement and invalid-reference failure.
- [ ] Showcase proves safe correction behavior.
