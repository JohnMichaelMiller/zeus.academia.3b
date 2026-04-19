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
name: implement-record-degree-obtained
description: Guide delivery of the RecordDegreeObtained slice for adding a qualification to an academic.
author: John Miller
tags: [implementation, vertical-slice, academia, qualification]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed RecordDegreeObtained slice with command handler, duplicate protection, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RecordDegreeObtained

## Objective

Deliver workflow 5.1 so HR can add a new degree-university record to an academic.

## Slice Boundary

- In scope: add a qualification to an existing academic, validate degree and university codes, reject duplicate academic-degree pairs.
- Non-goals: changing a qualification's university, removing qualifications, or grouped qualification reporting.
- Dependencies: RegisterAcademic, ManageDegrees, and ManageUniversities.
- Entry points: `src/backend/Features/Qualifications/Commands/RecordDegreeObtained/**`, `/api/academics/{empNr}/qualifications`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                   | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ---------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm qualification payload and duplicate semantics            | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement add-qualification command, endpoint, and tests         | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate duplicate rejection and persisted qualification history | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                                             | Files                                                                    | Done When                                   | Verification                                     |
| ---- | --------------------------- | ------------------------------------------------------------------------------------------------------------------ | ------------------------------------------------------------------------ | ------------------------------------------- | ------------------------------------------------ |
| 1    | `product-manager`           | Confirm whether response returns the full qualification list or just the newly created row                         | Plan and workflows                                                       | Scope note approved                         | Checklist updated                                |
| 2    | `backend-slice-implementer` | Implement record-degree command, validator, handler, and endpoint                                                  | `src/backend/Features/Qualifications/Commands/RecordDegreeObtained/**`   | Backend compiles and qualification persists | `dotnet build` passes                            |
| 3    | `backend-slice-implementer` | Add tests for successful add, duplicate academic-degree rejection, invalid reference codes, and not-found academic | `tests/backend/Features/Qualifications/Commands/RecordDegreeObtained/**` | Tests pass                                  | `dotnet test` passes for qualification-add scope |
| 4    | `slice-verifier`            | Run manual qualification-add scenario and collect evidence                                                         | HTTP collection or integration tests                                     | Acceptance met                              | Verification summary saved                       |

## Acceptance Criteria

- Given an existing academic and valid degree and university codes, when HR records a new qualification, then the qualification is added.
- Given an existing qualification with the same academic-degree pair, when the request is submitted, then the system rejects it with a conflict result.
- Given an invalid academic, degree, or university reference, when the request is submitted, then the request fails and nothing is stored.

## Verification Plan

- Automated: backend build plus qualification-add tests.
- Manual: add a qualification, retry with duplicate academic-degree, and retry with invalid codes.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with one registered academic and seeded degree and university reference data.
2. Call `POST /api/academics/{empNr}/qualifications` with a valid degree-university pair.
   Expected: the qualification is added.
3. Retry with the same degree for the same academic.
   Expected: the API rejects the duplicate pair.

Value demonstrated: qualification history can grow safely without violating the academic-degree uniqueness rule.

## Output Artifacts

- Record-degree command, validator, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only RecordDegreeObtained.
- [ ] Acceptance criteria cover duplicate-pair rejection.
- [ ] Showcase proves qualification growth without data corruption.
