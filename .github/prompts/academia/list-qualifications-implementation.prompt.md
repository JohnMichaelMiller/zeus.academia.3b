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
name: implement-list-qualifications
description: Guide delivery of the ListQualifications slice for reading qualification data by academic, degree, or university.
author: John Miller
tags: [implementation, vertical-slice, academia, qualification, query]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ListQualifications slice with projection queries, endpoints, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ListQualifications

## Objective

Deliver workflows 5.4, 5.5, and 5.6 so users can query qualification data by academic, degree code, or university code.

## Slice Boundary

- In scope: three read models for qualifications by academic, by degree, and by university.
- Non-goals: grouped summary reporting across the whole institution.
- Dependencies: RecordDegreeObtained.
- Entry points: `src/backend/Features/Qualifications/Queries/ListQualifications/**`, `/api/qualifications`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                        | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ----------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm query shapes and filter contracts             | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement qualification queries, endpoints, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate each query surface and empty-result behavior | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                | Files                                                                 | Done When                                           | Verification                                       |
| ---- | --------------------------- | ------------------------------------------------------------------------------------- | --------------------------------------------------------------------- | --------------------------------------------------- | -------------------------------------------------- |
| 1    | `product-manager`           | Confirm whether one endpoint with mode filters or three dedicated routes is preferred | Plan and workflows                                                    | Scope note approved                                 | Checklist updated                                  |
| 2    | `backend-slice-implementer` | Build read-only qualification projection queries and endpoint contract                | `src/backend/Features/Qualifications/Queries/ListQualifications/**`   | Backend compiles and query paths return projections | `dotnet build` passes                              |
| 3    | `backend-slice-implementer` | Add tests for by-academic, by-degree, by-university, and empty-result cases           | `tests/backend/Features/Qualifications/Queries/ListQualifications/**` | Tests pass                                          | `dotnet test` passes for qualification-query scope |
| 4    | `slice-verifier`            | Run manual query scenarios and collect evidence                                       | HTTP collection or integration tests                                  | Acceptance met                                      | Verification summary saved                         |

## Acceptance Criteria

- Given an academic with qualifications, when qualifications are queried by academic, then all degree-university pairs for that academic are returned.
- Given a degree code, when qualifications are queried by degree, then all matching academics and their universities are returned.
- Given a university code, when qualifications are queried by university, then all matching academics and degrees are returned.
- Given no matches, when any qualification query is submitted, then the system returns an empty result without error.

## Verification Plan

- Automated: backend build plus qualification-query tests.
- Manual: run the three query modes and an empty-result scenario.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with academics holding several qualifications.
2. Query qualifications for one academic.
   Expected: that academic's full qualification list is returned.
3. Query by degree code and then by university code.
   Expected: each query returns only the matching rows.

Value demonstrated: qualification data can be browsed from the main user perspectives without custom reports.

## Output Artifacts

- Qualification queries, endpoints, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ListQualifications.
- [ ] Acceptance criteria cover all three query perspectives.
- [ ] Showcase demonstrates useful read access beyond single-record profiles.
