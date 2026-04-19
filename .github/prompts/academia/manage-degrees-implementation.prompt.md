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
name: implement-manage-degrees
description: Guide delivery of the ManageDegrees slice for viewing and adding degree reference data.
author: John Miller
tags: [implementation, vertical-slice, academia, reference-data, degree]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ManageDegrees slice with endpoints, validation, persistence, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ManageDegrees

## Objective

Deliver workflows 6.3 and 6.4 so administrators can list degree codes and add new unique degrees.

## Slice Boundary

- In scope: degree catalogue query, add degree command, uniqueness enforcement.
- Non-goals: academic qualification history and reporting.
- Dependencies: Shared Kernel only.
- Entry points: `src/backend/Features/ReferenceData/Degrees/`, `/api/reference-data/degrees`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                              | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ----------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm degree-code scope and acceptance criteria           | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement degree endpoints, handlers, validators, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate unique degree behavior and demo readiness          | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                   | Files                                             | Done When           | Verification                          |
| ---- | --------------------------- | ------------------------------------------------------------------------ | ------------------------------------------------- | ------------------- | ------------------------------------- |
| 1    | `product-manager`           | Confirm allowed degree-code format and whether descriptions are in scope | Plan and ORM rules                                | Scope note approved | Checklist updated                     |
| 2    | `backend-slice-implementer` | Build list/add degree slice with persistence and validation              | `src/backend/Features/ReferenceData/Degrees/**`   | Backend compiles    | `dotnet build` passes                 |
| 3    | `backend-slice-implementer` | Add tests for list, create, duplicate rejection, and invalid payloads    | `tests/backend/Features/ReferenceData/Degrees/**` | Tests pass          | `dotnet test` passes for degree scope |
| 4    | `slice-verifier`            | Execute API showcase and record results                                  | HTTP collection or integration tests              | Acceptance met      | Verification summary saved            |

## Acceptance Criteria

- Given degree seed data exists, when an authorized user lists degrees, then the response returns the current degree codes.
- Given a unique degree code, when an administrator adds it, then the degree is stored and returned.
- Given a duplicate degree code, when the request is submitted, then the system rejects it with a conflict result.
- Given an invalid or empty code, when the request is submitted, then validation fails and no degree is created.

## Verification Plan

- Automated: backend build plus degree slice tests.
- Manual: list degrees, create a valid degree, retry with duplicate and invalid data.
- Evidence: API results and test output.

## Showcase Steps

1. Start the API.
2. Call `GET /api/reference-data/degrees`.
   Expected: the current degree codes are returned.
3. Call `POST /api/reference-data/degrees` with a unique degree code.
   Expected: the new degree is created and visible in the list.
4. Retry with the same degree code.
   Expected: the API rejects the duplicate.

Value demonstrated: degree reference data is managed centrally and safely before qualification slices depend on it.

## Output Artifacts

- Degree slice code, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ManageDegrees.
- [ ] Execution support is stated explicitly.
- [ ] Acceptance criteria cover success and duplicate failure.
- [ ] Showcase demonstrates reusable reference-data value.
