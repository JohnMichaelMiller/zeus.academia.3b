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
name: implement-list-available-extensions
description: Guide delivery of the ListAvailableExtensions slice for reading provisioned but unassigned extension numbers.
author: John Miller
tags: [implementation, vertical-slice, academia, extension, query]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ListAvailableExtensions slice with projection query, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ListAvailableExtensions

## Objective

Deliver workflow 4.5 so IT can query the pool of provisioned extension numbers that are not currently assigned.

## Slice Boundary

- In scope: read only provisioned and currently unassigned extension numbers.
- Non-goals: provisioning, assigning, or releasing extensions.
- Dependencies: AssignExtension.
- Entry points: `src/backend/Features/Extensions/Queries/ListAvailableExtensions/**`, `/api/extensions/available`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                               | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------ | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm sorting and response contract                        | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement available-extension query, endpoint, and tests     | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate only unassigned provisioned extensions are returned | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                             | Files                                                                  | Done When                                              | Verification                                       |
| ---- | --------------------------- | ------------------------------------------------------------------ | ---------------------------------------------------------------------- | ------------------------------------------------------ | -------------------------------------------------- |
| 1    | `product-manager`           | Confirm sort order and paging expectations                         | Plan and workflows                                                     | Scope note approved                                    | Checklist updated                                  |
| 2    | `backend-slice-implementer` | Build list-available query and endpoint using projection only      | `src/backend/Features/Extensions/Queries/ListAvailableExtensions/**`   | Backend compiles and query returns available pool only | `dotnet build` passes                              |
| 3    | `backend-slice-implementer` | Add tests for mixed assigned/unassigned inventories and empty pool | `tests/backend/Features/Extensions/Queries/ListAvailableExtensions/**` | Tests pass                                             | `dotnet test` passes for available-extension scope |
| 4    | `slice-verifier`            | Run manual available-extension query and capture evidence          | HTTP collection or integration tests                                   | Acceptance met                                         | Verification summary saved                         |

## Acceptance Criteria

- Given a mix of provisioned assigned and unassigned extensions, when available extensions are queried, then only unassigned provisioned extensions are returned.
- Given no available extensions, when the query is submitted, then the system returns an empty result without error.
- Given an extension is released, when the query is run afterward, then the released extension appears in the result.

## Verification Plan

- Automated: backend build plus available-extension query tests.
- Manual: query the pool before and after assigning or releasing an extension.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with several provisioned extensions, some assigned and some free.
2. Call `GET /api/extensions/available`.
   Expected: only unassigned extension numbers are returned.
3. Release one assigned extension and query again.
   Expected: the released extension now appears in the available list.

Value demonstrated: operational users can see immediately which extension numbers are ready for reuse.

## Output Artifacts

- Available-extension query, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ListAvailableExtensions.
- [ ] Acceptance criteria cover mixed inventory and empty results.
- [ ] Showcase proves real operational visibility into the extension pool.
