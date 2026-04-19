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
name: implement-search-list-academics
description: Guide delivery of the SearchListAcademics slice for filtered and paginated academic listing.
author: John Miller
tags: [implementation, vertical-slice, academia, academic, query, search]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed SearchListAcademics slice with query handler, filtering, pagination, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement SearchListAcademics

## Objective

Deliver workflow 1.4 so authorized users can search and list academics with filters and pagination.

## Slice Boundary

- In scope: filter by name, rank, AccessLevel, employment status, degree, and university; return paginated results.
- Non-goals: single-profile reads, aggregate reporting, or updates.
- Dependencies: RegisterAcademic.
- Entry points: `src/backend/Features/Academics/Queries/SearchListAcademics/`, `/api/academics` with query parameters.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                      | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm filter contract, pagination defaults, and response shape    | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement search query, projection, pagination, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate filter behavior, pagination, and empty-result handling     | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                            | Files                                                             | Done When                                     | Verification                          |
| ---- | --------------------------- | --------------------------------------------------------------------------------- | ----------------------------------------------------------------- | --------------------------------------------- | ------------------------------------- |
| 1    | `product-manager`           | Confirm filter semantics, default sort, and pagination contract                   | Plan and workflows                                                | Scope note approved                           | Checklist updated                     |
| 2    | `backend-slice-implementer` | Build paginated search query with direct projection and filter composition        | `src/backend/Features/Academics/Queries/SearchListAcademics/**`   | Query compiles and supports requested filters | `dotnet build` passes                 |
| 3    | `backend-slice-implementer` | Add tests for each filter family, combined filters, pagination, and empty results | `tests/backend/Features/Academics/Queries/SearchListAcademics/**` | Tests pass                                    | `dotnet test` passes for search scope |
| 4    | `slice-verifier`            | Run manual search scenarios and capture evidence                                  | HTTP collection or integration tests                              | Acceptance met                                | Verification summary saved            |

## Acceptance Criteria

- Given academics exist, when an authorized user searches with no filters, then the system returns a paginated listing.
- Given filter values for name, rank, AccessLevel, employment status, degree, or university, when the query is submitted, then the result set includes only matching academics.
- Given combined filters, when the query is submitted, then all active filters are applied together.
- Given a page beyond the available result set, when the query is submitted, then the system returns an empty page without error.

## Verification Plan

- Automated: backend build plus search slice tests.
- Manual: execute unfiltered search, filtered search, combined-filter search, and out-of-range paging.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with multiple academics covering different ranks, employment states, degrees, and universities.
2. Call `GET /api/academics?page=1&pageSize=10`.
   Expected: a paginated list is returned.
3. Call `GET /api/academics?rank=P&employmentStatus=Contracted`.
   Expected: only matching academics are returned.
4. Call `GET /api/academics?degree=PHD&university=MIT`.
   Expected: only academics matching both filters are returned.

Value demonstrated: users can navigate and discover academic records efficiently without custom reporting endpoints.

## Output Artifacts

- Search query, response contract, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only SearchListAcademics.
- [ ] Acceptance criteria cover filters, composition, and pagination.
- [ ] Showcase demonstrates real search value, not just raw listing.
