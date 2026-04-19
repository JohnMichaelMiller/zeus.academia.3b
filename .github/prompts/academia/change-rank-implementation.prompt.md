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
name: implement-change-rank
description: Guide delivery of the ChangeRank slice for updating academic rank and derived access level.
author: John Miller
tags: [implementation, vertical-slice, academia, rank, access-level]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ChangeRank slice with command handler, event emission, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ChangeRank

## Objective

Deliver workflow 3.2 so HR can change an academic's rank and automatically recalculate AccessLevel.

## Slice Boundary

- In scope: validate new rank, replace the current rank, derive new AccessLevel, and raise `RankChangedEvent`.
- Non-goals: rank reference-data management or reporting.
- Dependencies: RegisterAcademic and ManageRanks.
- Entry points: `src/backend/Features/Employment/Commands/ChangeRank/**`, `/api/academics/{empNr}/rank`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Confirm Shared Kernel exposes AccessLevel derivation and domain event plumbing.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                     | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------------ | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm rank-change semantics and event expectation                | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement change-rank command, endpoint, event emission, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate new rank, derived AccessLevel, and event behavior         | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                   | Files                                                      | Done When                                 | Verification                               |
| ---- | --------------------------- | ---------------------------------------------------------------------------------------- | ---------------------------------------------------------- | ----------------------------------------- | ------------------------------------------ |
| 1    | `product-manager`           | Confirm event payload needs and authorization assumptions                                | Plan and workflows                                         | Scope note approved                       | Checklist updated                          |
| 2    | `backend-slice-implementer` | Build change-rank command, validator, handler, endpoint, and `RankChangedEvent` dispatch | `src/backend/Features/Employment/Commands/ChangeRank/**`   | Backend compiles and rank changes persist | `dotnet build` passes                      |
| 3    | `backend-slice-implementer` | Add tests for valid rank change, invalid rank rejection, and AccessLevel recalculation   | `tests/backend/Features/Employment/Commands/ChangeRank/**` | Tests pass                                | `dotnet test` passes for rank-change scope |
| 4    | `slice-verifier`            | Run manual rank-change scenario and record evidence                                      | HTTP collection or integration tests                       | Acceptance met                            | Verification summary saved                 |

## Acceptance Criteria

- Given an academic and a valid rank code, when HR changes the rank, then the academic's rank is replaced and AccessLevel is recalculated automatically.
- Given an invalid rank code, when the request is submitted, then validation fails and the original rank remains unchanged.
- Given a successful rank change, when downstream read models are queried, then they expose the new derived AccessLevel.
- Given a successful rank change, when domain events are inspected, then a `RankChangedEvent` is raised.

## Verification Plan

- Automated: backend build plus change-rank tests.
- Manual: change rank, read the profile, and inspect emitted event behavior if event capture exists.
- Evidence: API responses, test output, and event evidence.

## Showcase Steps

1. Start the API with a registered academic.
2. Call `PUT /api/academics/{empNr}/rank` with a valid new rank.
   Expected: the academic rank changes and the derived AccessLevel updates automatically.
3. Read the profile or report projection.
   Expected: the updated AccessLevel is visible immediately.

Value demonstrated: rank changes remain a single explicit action while AccessLevel stays purely derived.

## Output Artifacts

- Change-rank command, validator, event wiring, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ChangeRank.
- [ ] Acceptance criteria cover derived AccessLevel and event emission.
- [ ] Showcase proves the rank-to-access relationship.
