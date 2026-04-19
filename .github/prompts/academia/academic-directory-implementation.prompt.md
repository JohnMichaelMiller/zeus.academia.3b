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
name: implement-academic-directory
description: Guide delivery of the AcademicDirectory report slice for the full academic listing.
author: John Miller
tags: [implementation, vertical-slice, academia, reporting, directory]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed AcademicDirectory slice with projection query, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement AcademicDirectory

## Objective

Deliver workflow 7.1 so staff can retrieve a full directory listing of academics.

## Slice Boundary

- In scope: list name, rank, derived AccessLevel, extension, and employment status for all academics.
- Non-goals: aggregation counts or advanced search filters handled by SearchListAcademics.
- Dependencies: RegisterAcademic.
- Entry points: `src/backend/Features/Reports/Queries/AcademicDirectory/**`, `/api/reports/academics/directory`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Reporting slices should use dedicated read-optimized queries rather than aggregate rehydration.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                            | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | --------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm directory columns and ordering                    | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement directory projection query, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate completeness and sorting of the directory        | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                               | Files                                                         | Done When                                                          | Verification                             |
| ---- | --------------------------- | -------------------------------------------------------------------- | ------------------------------------------------------------- | ------------------------------------------------------------------ | ---------------------------------------- |
| 1    | `product-manager`           | Confirm default sort and whether deregistered academics are excluded | Plan and workflows                                            | Scope note approved                                                | Checklist updated                        |
| 2    | `backend-slice-implementer` | Build read-only academic-directory query and endpoint                | `src/backend/Features/Reports/Queries/AcademicDirectory/**`   | Backend compiles and directory projection returns required columns | `dotnet build` passes                    |
| 3    | `backend-slice-implementer` | Add tests for full listing, ordering, and empty directory behavior   | `tests/backend/Features/Reports/Queries/AcademicDirectory/**` | Tests pass                                                         | `dotnet test` passes for directory scope |
| 4    | `slice-verifier`            | Run manual directory scenario and capture evidence                   | HTTP collection or integration tests                          | Acceptance met                                                     | Verification summary saved               |

## Acceptance Criteria

- Given academics exist, when staff requests the directory, then the response lists name, rank, AccessLevel, extension, and employment status for each academic.
- Given no academics exist, when the directory is requested, then the system returns an empty result without error.
- Given rank or employment changes occur, when the directory is requested afterward, then the listing reflects the latest derived state.

## Verification Plan

- Automated: backend build plus directory tests.
- Manual: load the directory before and after changing academic state.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with several academics in different states.
2. Call `GET /api/reports/academics/directory`.
   Expected: a complete listing is returned with rank, AccessLevel, extension, and employment columns.
3. Change one academic's rank or employment status and call the report again.
   Expected: the directory reflects the change.

Value demonstrated: staff get a single authoritative academic listing without combining multiple slice endpoints manually.

## Output Artifacts

- Academic-directory query, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only AcademicDirectory.
- [ ] Acceptance criteria cover complete listing and updated derived state.
- [ ] Showcase proves directory value for staff users.
