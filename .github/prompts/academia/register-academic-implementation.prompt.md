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
name: implement-register-academic
description: Guide delivery of the RegisterAcademic slice for onboarding a new academic.
author: John Miller
tags: [implementation, vertical-slice, academia, academic, onboarding]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed RegisterAcademic slice with aggregate creation, endpoint, validation, persistence, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RegisterAcademic

## Objective

Deliver workflow 1.1 so HR can register a new academic with valid rank, extension, and at least one qualification.

## Slice Boundary

- In scope: create academic aggregate, validate `empNr`, `EmpName`, rank, extension assignment, qualification payload, and optional employment status.
- Non-goals: profile updates, search, contract renewal, rank changes, and reporting.
- Dependencies: ManageRanks, ManageDegrees, ManageUniversities, and ProvisionExtension.
- Entry points: `src/backend/Features/Academics/Commands/RegisterAcademic/`, `/api/academics`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Confirm Shared Kernel contains `Academic`, `Rank`, `Degree`, `University`, `Extension`, `AccessLevel`, `Result<T>`, and shared exceptions before implementation begins.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                               | Inputs                                  | Outputs                               |
| ---------------------- | --------------------------- | ---------------------------------------------------------------------------- | --------------------------------------- | ------------------------------------- |
| Scope and acceptance   | `product-manager`           | Lock scope, request payload, and acceptance criteria for onboarding          | Plan, workflows, ORM rules              | Approved scope and checklist          |
| Backend implementation | `backend-slice-implementer` | Implement command, validator, handler, persistence, endpoint, and tests      | Approved scope and backend instructions | Code changes and tests                |
| Verification           | `slice-verifier`            | Validate happy path, duplicate and invalid-path behavior, and demo readiness | Implemented slice                       | Verification notes and residual risks |

## Implementation Steps

| Step | Owner                       | Action                                                                                                                                      | Files                                                           | Done When                                          | Verification                                |
| ---- | --------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------------------- | -------------------------------------------------- | ------------------------------------------- |
| 1    | `product-manager`           | Confirm onboarding payload shape, auth assumptions, and whether employment status can be omitted                                            | Plan, workflows, ORM rules                                      | Scope note approved                                | Checklist updated                           |
| 2    | `backend-slice-implementer` | Implement registration command, validator, handler, aggregate guards, and endpoint                                                          | `src/backend/Features/Academics/Commands/RegisterAcademic/**`   | Backend compiles and persists new academic records | `dotnet build` passes                       |
| 3    | `backend-slice-implementer` | Add tests for valid registration, duplicate `empNr`, missing qualification, invalid extension, invalid rank, and exclusive employment rules | `tests/backend/Features/Academics/Commands/RegisterAcademic/**` | Tests cover success and failure paths              | `dotnet test` passes for registration scope |
| 4    | `slice-verifier`            | Run onboarding showcase and collect evidence                                                                                                | HTTP collection or integration tests                            | Acceptance met                                     | Verification summary saved                  |

## Acceptance Criteria

- Given a unique 6-character `empNr`, valid `EmpName`, valid rank, available extension, and at least one degree-university pair, when HR submits the registration request, then the system creates the academic and returns the new record.
- Given a duplicate `empNr`, when the request is submitted, then the system rejects it with a conflict result and preserves existing data.
- Given a missing qualification list or an invalid degree-university pair, when the request is submitted, then validation fails and nothing is stored.
- Given an extension that is already assigned or not provisioned, when the request is submitted, then the system rejects the request.
- Given both tenured and contracted status in the same payload, when the request is submitted, then the exclusive employment rule is enforced and creation fails.

## Verification Plan

- Automated: backend build and registration slice tests.
- Manual: register a valid academic, retry with duplicate `empNr`, retry with an assigned extension, and retry with no qualifications.
- Evidence: API results, persisted data check, and test output.

## Showcase Steps

1. Start the API with rank, degree, university, and extension reference data seeded.
2. Call `POST /api/academics` with valid onboarding data.
   Expected: the response returns the academic identifier, rank, derived AccessLevel, extension, and qualifications.
3. Retry with the same `empNr`.
   Expected: the API returns a conflict and no duplicate academic is created.
4. Retry with an already assigned extension or no qualifications.
   Expected: validation or business-rule failure is returned.

Value demonstrated: the platform can onboard a valid academic while enforcing the core domain invariants that all downstream slices depend on.

## Output Artifacts

- Registration command/handler/validator/response, endpoint wiring, persistence changes, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only RegisterAcademic.
- [ ] Dependencies on reference-data slices are explicit.
- [ ] Acceptance criteria cover happy path plus domain-rule failures.
- [ ] Showcase proves onboarding value and protected invariants.
