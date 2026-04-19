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
name: implement-convert-contract-to-tenure
description: Guide delivery of the ConvertContractToTenure slice for converting a contracted academic to tenure.
author: John Miller
tags: [implementation, vertical-slice, academia, employment, tenure]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ConvertContractToTenure slice with command handler, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ConvertContractToTenure

## Objective

Deliver workflow 2.4 so HR can convert a contracted academic to tenure in one explicit command.

## Slice Boundary

- In scope: require contracted status first, clear contract date, set tenured flag, persist the change.
- Non-goals: direct tenure grant for non-contracted academics, contract renewal, or reporting.
- Dependencies: AssignContract.
- Entry points: `src/backend/Features/Employment/Commands/ConvertContractToTenure/**`, `/api/academics/{empNr}/employment/contract/tenure`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                            | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | --------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm conversion semantics and response expectations    | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement conversion command, endpoint, and tests         | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate contracted-only precondition and resulting state | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                    | Files                                                                   | Done When                                | Verification                              |
| ---- | --------------------------- | ----------------------------------------------------------------------------------------- | ----------------------------------------------------------------------- | ---------------------------------------- | ----------------------------------------- |
| 1    | `product-manager`           | Confirm whether conversion should emit an audit event now or later                        | Plan and workflows                                                      | Scope note approved                      | Checklist updated                         |
| 2    | `backend-slice-implementer` | Build conversion command, handler, and endpoint using aggregate guard methods             | `src/backend/Features/Employment/Commands/ConvertContractToTenure/**`   | Backend compiles and conversion persists | `dotnet build` passes                     |
| 3    | `backend-slice-implementer` | Add tests for valid conversion, non-contracted academic rejection, and not-found handling | `tests/backend/Features/Employment/Commands/ConvertContractToTenure/**` | Tests pass                               | `dotnet test` passes for conversion scope |
| 4    | `slice-verifier`            | Run manual conversion scenario and collect evidence                                       | HTTP collection or integration tests                                    | Acceptance met                           | Verification summary saved                |

## Acceptance Criteria

- Given a contracted academic, when HR converts the contract to tenure, then the contract date is cleared and tenured status is set.
- Given an academic who is not contracted, when the conversion request is submitted, then the system rejects it.
- Given the conversion succeeds, when the academic profile is read, then only tenured status is present.

## Verification Plan

- Automated: backend build plus conversion tests.
- Manual: convert a contracted academic, then retry for a non-contracted academic.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with one contracted academic.
2. Call `POST /api/academics/{empNr}/employment/contract/tenure`.
   Expected: the academic becomes tenured and the contract date disappears.
3. Read the academic profile.
   Expected: the profile shows tenured status only.

Value demonstrated: the employment lifecycle supports a direct, rules-safe promotion from contract to tenure.

## Output Artifacts

- Convert-contract-to-tenure command, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ConvertContractToTenure.
- [ ] Acceptance criteria cover contracted-only conversion and resulting state.
- [ ] Showcase proves the lifecycle transition.
