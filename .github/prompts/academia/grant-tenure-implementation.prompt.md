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
name: implement-grant-tenure
description: Guide delivery of the GrantTenure slice for setting an academic to tenured status.
author: John Miller
tags: [implementation, vertical-slice, academia, employment, tenure]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed GrantTenure slice with command handler, domain-rule enforcement, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement GrantTenure

## Objective

Deliver workflow 2.1 so HR can mark an academic as tenured while enforcing the employment exclusivity rule.

## Slice Boundary

- In scope: set `IsTenured = true`, clear any contract end date, persist the change, and return updated employment state.
- Non-goals: assigning contracts, changing rank, or reporting.
- Dependencies: RegisterAcademic.
- Entry points: `src/backend/Features/Employment/Commands/GrantTenure/`, `/api/academics/{empNr}/employment/tenure`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Confirm Shared Kernel guard methods enforce tenured XOR contracted.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                        | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ----------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm tenure behavior and authorization assumptions | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement grant-tenure command, endpoint, and tests   | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate exclusivity rule and outcome visibility      | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                           | Files                                                       | Done When                            | Verification                          |
| ---- | --------------------------- | ------------------------------------------------------------------------------------------------ | ----------------------------------------------------------- | ------------------------------------ | ------------------------------------- |
| 1    | `product-manager`           | Confirm response shape and whether tenure changes require auditing now or later                  | Plan and workflows                                          | Scope note approved                  | Checklist updated                     |
| 2    | `backend-slice-implementer` | Implement grant-tenure command, aggregate mutation, endpoint, and persistence                    | `src/backend/Features/Employment/Commands/GrantTenure/**`   | Backend compiles and tenure persists | `dotnet build` passes                 |
| 3    | `backend-slice-implementer` | Add tests for granting tenure from no status and from contracted status, plus not-found handling | `tests/backend/Features/Employment/Commands/GrantTenure/**` | Tests pass                           | `dotnet test` passes for tenure scope |
| 4    | `slice-verifier`            | Run manual tenure scenario and record evidence                                                   | HTTP collection or integration tests                        | Acceptance met                       | Verification summary saved            |

## Acceptance Criteria

- Given an existing academic with no employment status, when HR grants tenure, then the academic becomes tenured.
- Given a contracted academic, when HR grants tenure, then the contract date is cleared and the academic becomes tenured.
- Given a non-existent academic, when the request is submitted, then the system returns not found.
- Given the update succeeds, when the profile is read afterward, then the profile shows tenured status and no contract end date.

## Verification Plan

- Automated: backend build plus grant-tenure tests.
- Manual: grant tenure to an unassigned academic, grant tenure to a contracted academic, and verify the profile projection.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with one contracted academic.
2. Call `POST /api/academics/{empNr}/employment/tenure`.
   Expected: the academic becomes tenured and the contract date is cleared.
3. Read the academic profile.
   Expected: the profile shows tenured status only.

Value demonstrated: tenure decisions are applied with the core employment invariant preserved automatically.

## Output Artifacts

- Grant-tenure command, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only GrantTenure.
- [ ] Acceptance criteria cover exclusivity-rule behavior.
- [ ] Showcase proves the contract-to-tenure transition outcome.
