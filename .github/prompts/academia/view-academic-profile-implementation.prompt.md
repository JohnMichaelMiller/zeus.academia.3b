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
name: implement-view-academic-profile
description: Guide delivery of the ViewAcademicProfile slice for reading a single academic profile.
author: John Miller
tags: [implementation, vertical-slice, academia, academic, query]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ViewAcademicProfile slice with query handler, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ViewAcademicProfile

## Objective

Deliver workflow 1.2 so an authorized user can retrieve a complete academic profile.

## Slice Boundary

- In scope: read one academic by identifier, project rank, derived AccessLevel, extension, qualifications, and employment status.
- Non-goals: updates, search, or aggregated reporting.
- Dependencies: RegisterAcademic.
- Entry points: `src/backend/Features/Academics/Queries/ViewAcademicProfile/`, `/api/academics/{empNr}`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                       | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ---------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Lock profile response shape and audience assumptions | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement projection query, endpoint, and tests      | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate returned fields and not-found behavior      | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                           | Files                                                             | Done When                               | Verification                           |
| ---- | --------------------------- | -------------------------------------------------------------------------------- | ----------------------------------------------------------------- | --------------------------------------- | -------------------------------------- |
| 1    | `product-manager`           | Confirm response contract and whether qualifications are ordered in the response | Plan and workflows                                                | Scope note approved                     | Checklist updated                      |
| 2    | `backend-slice-implementer` | Build read-only profile query and endpoint using direct projection               | `src/backend/Features/Academics/Queries/ViewAcademicProfile/**`   | Query compiles and returns full profile | `dotnet build` passes                  |
| 3    | `backend-slice-implementer` | Add tests for found and not-found scenarios                                      | `tests/backend/Features/Academics/Queries/ViewAcademicProfile/**` | Tests pass                              | `dotnet test` passes for profile scope |
| 4    | `slice-verifier`            | Run manual read scenario and record evidence                                     | HTTP collection or integration tests                              | Acceptance met                          | Verification summary saved             |

## Acceptance Criteria

- Given an existing academic, when an authorized user requests the profile, then the response includes `empNr`, name, rank, derived AccessLevel, extension, qualifications, and current employment status.
- Given a non-existent academic identifier, when the profile is requested, then the system returns not found.
- Given an academic with contract status, when the profile is requested, then the response includes the contract end date and not the tenured flag.

## Verification Plan

- Automated: backend build plus profile query tests.
- Manual: retrieve an existing academic and a non-existent academic.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with at least one registered academic.
2. Call `GET /api/academics/{empNr}` for an existing academic.
   Expected: the full profile is returned with derived AccessLevel.
3. Call the same endpoint for a missing `empNr`.
   Expected: the API returns not found.

Value demonstrated: the system can surface a complete read model for one academic without loading unrelated slices.

## Output Artifacts

- Profile query, response DTO, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ViewAcademicProfile.
- [ ] Acceptance criteria cover full read projection and not-found behavior.
- [ ] Showcase proves the profile read model value.
