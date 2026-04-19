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
name: implement-assign-contract
description: Guide delivery of the AssignContract slice for assigning a future contract end date.
author: John Miller
tags: [implementation, vertical-slice, academia, employment, contract]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed AssignContract slice with command handler, validation, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement AssignContract

## Objective

Deliver workflow 2.2 so HR can assign a future contract end date to an academic.

## Slice Boundary

- In scope: set a future `ContractEndDate`, clear tenured status, enforce exclusivity and future-date validation.
- Non-goals: contract renewal, conversion to tenure, or reporting.
- Dependencies: RegisterAcademic.
- Entry points: `src/backend/Features/Employment/Commands/AssignContract/`, `/api/academics/{empNr}/employment/contract`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Confirm Shared Kernel guard methods enforce tenured XOR contracted.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                    | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ----------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm contract date semantics and authorization assumptions     | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement assign-contract command, validator, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate future-date rules and exclusivity behavior               | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                      | Files                                                          | Done When                                   | Verification                            |
| ---- | --------------------------- | ------------------------------------------------------------------------------------------- | -------------------------------------------------------------- | ------------------------------------------- | --------------------------------------- |
| 1    | `product-manager`           | Confirm contract-date input format and response payload                                     | Plan and workflows                                             | Scope note approved                         | Checklist updated                       |
| 2    | `backend-slice-implementer` | Implement assign-contract command, validator, aggregate mutation, endpoint, and persistence | `src/backend/Features/Employment/Commands/AssignContract/**`   | Backend compiles and contract date persists | `dotnet build` passes                   |
| 3    | `backend-slice-implementer` | Add tests for valid future date, past date rejection, and converting tenured to contracted  | `tests/backend/Features/Employment/Commands/AssignContract/**` | Tests pass                                  | `dotnet test` passes for contract scope |
| 4    | `slice-verifier`            | Run manual contract scenario and record evidence                                            | HTTP collection or integration tests                           | Acceptance met                              | Verification summary saved              |

## Acceptance Criteria

- Given an existing academic and a future contract end date, when HR assigns the contract, then the contract date is stored and tenured status is cleared.
- Given a past or present date, when the request is submitted, then validation fails and no change is persisted.
- Given a tenured academic, when a contract is assigned, then the academic becomes contracted only.
- Given a non-existent academic, when the request is submitted, then the system returns not found.

## Verification Plan

- Automated: backend build plus assign-contract tests.
- Manual: assign a future contract, retry with an invalid date, and verify the profile response.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with a registered academic.
2. Call `POST /api/academics/{empNr}/employment/contract` with a future date.
   Expected: the academic becomes contracted until that date.
3. Retry with a past date.
   Expected: validation fails and the stored contract date remains unchanged.

Value demonstrated: contract assignments are explicit, validated, and mutually exclusive with tenure.

## Output Artifacts

- Assign-contract command, validator, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only AssignContract.
- [ ] Acceptance criteria cover future-date validation and exclusivity.
- [ ] Showcase proves safe employment-state assignment.
