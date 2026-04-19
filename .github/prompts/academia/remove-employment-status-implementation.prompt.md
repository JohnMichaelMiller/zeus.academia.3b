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
name: implement-remove-employment-status
description: Guide delivery of the RemoveEmploymentStatus slice for clearing tenure and contract state.
author: John Miller
tags: [implementation, vertical-slice, academia, employment]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed RemoveEmploymentStatus slice with command handler, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RemoveEmploymentStatus

## Objective

Deliver workflow 2.5 so HR can clear an academic's current employment status.

## Slice Boundary

- In scope: clear `IsTenured` and `ContractEndDate`, persist the empty employment state.
- Non-goals: assigning a new status or deleting the academic.
- Dependencies: RegisterAcademic.
- Entry points: `src/backend/Features/Employment/Commands/RemoveEmploymentStatus/**`, `/api/academics/{empNr}/employment`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                           | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | -------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm clear-status semantics and response expectations | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement remove-status command, endpoint, and tests     | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate both status fields are cleared                  | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                           | Files                                                                  | Done When                                            | Verification                                  |
| ---- | --------------------------- | -------------------------------------------------------------------------------- | ---------------------------------------------------------------------- | ---------------------------------------------------- | --------------------------------------------- |
| 1    | `product-manager`           | Confirm idempotency expectations for academics already without employment status | Plan and workflows                                                     | Scope note approved                                  | Checklist updated                             |
| 2    | `backend-slice-implementer` | Build clear-status command, handler, and endpoint                                | `src/backend/Features/Employment/Commands/RemoveEmploymentStatus/**`   | Backend compiles and empty employment state persists | `dotnet build` passes                         |
| 3    | `backend-slice-implementer` | Add tests for clearing tenure, clearing contract, and not-found handling         | `tests/backend/Features/Employment/Commands/RemoveEmploymentStatus/**` | Tests pass                                           | `dotnet test` passes for status-removal scope |
| 4    | `slice-verifier`            | Run manual status-clear scenario and collect evidence                            | HTTP collection or integration tests                                   | Acceptance met                                       | Verification summary saved                    |

## Acceptance Criteria

- Given a tenured academic, when HR clears employment status, then the academic is no longer tenured.
- Given a contracted academic, when HR clears employment status, then the contract end date is removed.
- Given an academic already without employment status, when the request is submitted, then the result is stable and no invalid state is introduced.
- Given a non-existent academic, when the request is submitted, then the system returns not found.

## Verification Plan

- Automated: backend build plus remove-status tests.
- Manual: clear status for tenured and contracted academics and verify follow-up profile reads.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with one tenured academic and one contracted academic.
2. Call `DELETE /api/academics/{empNr}/employment` for each.
   Expected: both academics end with neither tenure nor contract date.
3. Read the profiles.
   Expected: employment status is empty in both profiles.

Value demonstrated: HR can deliberately reset employment state without breaking the underlying academic record.

## Output Artifacts

- Remove-employment-status command, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only RemoveEmploymentStatus.
- [ ] Acceptance criteria cover clearing both tenure and contract states.
- [ ] Showcase proves the neutral employment-state outcome.
