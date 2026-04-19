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
name: implement-manage-ranks
description: Guide delivery of the ManageRanks slice for listing and adding rank reference data.
author: John Miller
tags: [implementation, vertical-slice, academia, reference-data, rank]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ManageRanks slice with endpoints, validation, persistence, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ManageRanks

## Objective

Deliver workflows 6.1 and 6.2 so administrators can list ranks and add a new unique rank with its AccessLevel mapping.

## Slice Boundary

- In scope: view ranks, add rank, persist unique rank code, store exactly one AccessLevel mapping.
- Non-goals: assigning ranks to academics, changing academic rank, report generation.
- Dependencies: Shared Kernel only.
- Entry points: `src/backend/Features/ReferenceData/Ranks/`, `/api/reference-data/ranks`.

## Required Context

- Review `.github/models/workflows/academia-implementation-plan.md`, `.github/models/workflows/academia-workflows.md`, and `.github/models/orm/academia.txt`.
- Review `.github/instructions/project-overview.instructions.md`, `.github/instructions/vertical-slice-implementation.instructions.md`, `.github/instructions/custom-agents.instructions.md`, `.github/instructions/implementation-prompt-generation.instructions.md`, `.github/instructions/aspnetcore-implementation.instructions.md`, `.github/instructions/csharp-implementation.instructions.md`, `.github/instructions/cqrs-mediatr-efcore.instructions.md`, `.github/instructions/mediatr-implementation.instructions.md`, `.github/instructions/fluentvalidation-implementation.instructions.md`, and `.github/instructions/xunit-implementation.instructions.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                       | Inputs                                  | Outputs                               |
| ---------------------- | --------------------------- | -------------------------------------------------------------------- | --------------------------------------- | ------------------------------------- |
| Scope and acceptance   | `product-manager`           | Confirm rank scope, seed expectations, and acceptance criteria       | Plan, workflows, ORM rules              | Approved scope and checklist          |
| Backend implementation | `backend-slice-implementer` | Implement command/query, validator, persistence, endpoint, and tests | Approved scope and backend instructions | Code changes and passing tests        |
| Verification           | `slice-verifier`            | Validate behavior, duplicate handling, and demo readiness            | Implemented slice and checklist         | Verification notes and residual risks |

Handoff sequence: `product-manager` approves scope, `backend-slice-implementer` delivers the slice, `slice-verifier` signs off on evidence.

## Implementation Steps

| Step | Owner                       | Action                                                                                 | Files                                           | Done When                                             | Verification                        |
| ---- | --------------------------- | -------------------------------------------------------------------------------------- | ----------------------------------------------- | ----------------------------------------------------- | ----------------------------------- |
| 1    | `product-manager`           | Confirm whether ranks stay seed-only as `P`, `SL`, `L` or remain extensible beyond MVP | Plan and workflows                              | Scope note is explicit                                | Acceptance checklist updated        |
| 2    | `backend-slice-implementer` | Implement list/add rank slice with unique rank code and AccessLevel mapping validation | `src/backend/Features/ReferenceData/Ranks/**`   | Endpoint, handler, validator, and persistence compile | `dotnet build` passes               |
| 3    | `backend-slice-implementer` | Add tests for list, create, duplicate rejection, and invalid mapping                   | `tests/backend/Features/ReferenceData/Ranks/**` | Tests cover success and failure paths                 | `dotnet test` passes for rank scope |
| 4    | `slice-verifier`            | Run manual API showcase and capture evidence                                           | API collection or HTTP file                     | Behavior matches acceptance criteria                  | Verification summary saved          |

## Acceptance Criteria

- Given seed data exists, when an authorized user requests ranks, then the response lists each rank code with its derived AccessLevel mapping.
- Given a unique rank code and valid AccessLevel mapping, when an administrator creates a rank, then the rank is stored and returned.
- Given a duplicate rank code, when the create request is submitted, then the system rejects it with a conflict result.
- Given an invalid AccessLevel code, when the create request is submitted, then validation fails and no record is stored.

## Verification Plan

- Automated: build the backend, run rank slice tests, and verify unique constraint coverage.
- Manual: list ranks, add a valid rank, retry with duplicate code, retry with invalid AccessLevel.
- Evidence: request/response samples plus test output.

## Showcase Steps

1. Start the API.
2. Call `GET /api/reference-data/ranks`.
   Expected: rank codes and AccessLevel mappings are returned.
3. Call `POST /api/reference-data/ranks` with a unique code and valid mapping.
   Expected: create succeeds and the new rank appears in the list.
4. Repeat the same `POST`.
   Expected: the API returns a conflict and preserves the original record only.

Value demonstrated: the system owns rank reference data and protects uniqueness before academic slices depend on it.

## Output Artifacts

- Rank endpoints, command/query handlers, validators, persistence changes, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ManageRanks.
- [ ] Execution support is stated explicitly.
- [ ] Acceptance criteria describe observable outcomes.
- [ ] Verification includes duplicate and validation failures.
- [ ] Showcase proves reference-data value without hidden setup.
