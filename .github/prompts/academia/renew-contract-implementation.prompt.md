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
name: implement-renew-contract
description: Guide delivery of the RenewContract slice for updating an existing contract end date.
author: John Miller
tags: [implementation, vertical-slice, academia, employment, contract]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed RenewContract slice with command handler, validation, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RenewContract

## Objective

Deliver workflow 2.3 so HR can replace an academic's existing contract end date with a new future date.

## Slice Boundary

- In scope: renew an existing contract, require contracted status first, validate future date, persist the replacement date.
- Non-goals: creating an initial contract, converting to tenure, or listing contracted academics.
- Dependencies: AssignContract.
- Entry points: `src/backend/Features/Employment/Commands/RenewContract/`, `/api/academics/{empNr}/employment/contract/renewal`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                   | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ---------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm renewal semantics and response shape                     | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement renew-contract command, validator, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate contracted-only precondition and future-date rules      | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                                      | Files                                                         | Done When                                      | Verification                           |
| ---- | --------------------------- | ----------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------- | ---------------------------------------------- | -------------------------------------- |
| 1    | `product-manager`           | Confirm whether the new contract date may be earlier than the current future date                           | Plan and workflows                                            | Scope note approved                            | Checklist updated                      |
| 2    | `backend-slice-implementer` | Build renewal command, validator, handler, and endpoint with contracted-only precondition                   | `src/backend/Features/Employment/Commands/RenewContract/**`   | Backend compiles and replacement date persists | `dotnet build` passes                  |
| 3    | `backend-slice-implementer` | Add tests for valid renewal, non-contracted academic rejection, invalid future date, and not-found handling | `tests/backend/Features/Employment/Commands/RenewContract/**` | Tests pass                                     | `dotnet test` passes for renewal scope |
| 4    | `slice-verifier`            | Run manual renewal scenario and capture evidence                                                            | HTTP collection or integration tests                          | Acceptance met                                 | Verification summary saved             |

## Acceptance Criteria

- Given a contracted academic and a valid future date, when HR renews the contract, then the stored contract end date is replaced with the new date.
- Given an academic who is not currently contracted, when the renewal request is submitted, then the system rejects it.
- Given a past or present date, when the request is submitted, then validation fails and the original contract date is preserved.

## Verification Plan

- Automated: backend build plus renewal tests.
- Manual: renew a valid contract, then retry for a tenured or status-free academic and with an invalid date.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with a contracted academic.
2. Call `POST /api/academics/{empNr}/employment/contract/renewal` with a new future date.
   Expected: the contract date changes to the new value.
3. Retry for an academic without contract status.
   Expected: the API rejects the renewal.

Value demonstrated: contract renewals are explicit and only available when a contract already exists.

## Output Artifacts

- Renew-contract command, validator, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only RenewContract.
- [ ] Acceptance criteria cover contracted-only precondition and invalid-date failure.
- [ ] Showcase proves controlled renewal behavior.
