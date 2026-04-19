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
name: implement-deregister-academic
description: Guide delivery of the DeregisterAcademic slice for off-boarding an academic while retaining qualification history.
author: John Miller
tags: [implementation, vertical-slice, academia, academic, offboarding]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed DeregisterAcademic slice with command handler, event emission, release behavior, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement DeregisterAcademic

## Objective

Deliver workflow 1.5 so HR can deregister an academic, release the extension, clear employment status, and retain qualification history.

## Slice Boundary

- In scope: deregister the academic, release the assigned extension, clear employment status, publish `AcademicDeregisteredEvent`, and preserve historical qualification records.
- Non-goals: hard deleting qualification history or deprovisioning extensions.
- Dependencies: RegisterAcademic and ReleaseExtension.
- Entry points: `src/backend/Features/Academics/Commands/DeregisterAcademic/**`, `/api/academics/{empNr}`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Confirm Shared Kernel exposes domain event plumbing and release-extension behavior is available or duplicated safely inside this slice.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                                | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ----------------------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm off-boarding semantics and data-retention expectations                | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement deregistration command, release behavior, event emission, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate extension release, retained history, and event behavior              | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                                      | Files                                                             | Done When                                          | Verification                                  |
| ---- | --------------------------- | ----------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- | -------------------------------------------------- | --------------------------------------------- |
| 1    | `product-manager`           | Confirm soft-delete vs archive semantics and retained-read behavior                                         | Plan and workflows                                                | Scope note approved                                | Checklist updated                             |
| 2    | `backend-slice-implementer` | Build deregistration command, handler, endpoint, extension release behavior, and event emission             | `src/backend/Features/Academics/Commands/DeregisterAcademic/**`   | Backend compiles and deregistration state persists | `dotnet build` passes                         |
| 3    | `backend-slice-implementer` | Add tests for successful deregistration, extension release, retained qualifications, and not-found handling | `tests/backend/Features/Academics/Commands/DeregisterAcademic/**` | Tests pass                                         | `dotnet test` passes for deregistration scope |
| 4    | `slice-verifier`            | Run manual off-boarding scenario and collect evidence                                                       | HTTP collection or integration tests                              | Acceptance met                                     | Verification summary saved                    |

## Acceptance Criteria

- Given an existing academic, when HR deregisters that academic, then the academic is marked deregistered or archived according to the chosen model and the operation succeeds.
- Given a deregistered academic with an assigned extension, when the operation completes, then the extension is available for reuse.
- Given a deregistered academic with qualifications, when qualification history is queried afterward, then historical degree records remain available.
- Given a successful deregistration, when domain events are observed, then an `AcademicDeregisteredEvent` is published.

## Verification Plan

- Automated: backend build plus deregistration tests.
- Manual: deregister an academic, confirm the extension becomes available, and confirm qualifications remain queryable.
- Evidence: API responses, test output, and event evidence.

## Showcase Steps

1. Start the API with a registered academic that has an assigned extension and qualifications.
2. Call `DELETE /api/academics/{empNr}`.
   Expected: the academic is deregistered, the extension is released, and the command reports success.
3. Call the available-extensions query and a qualification-history query.
   Expected: the extension is reusable and the academic's qualification history remains intact.

Value demonstrated: off-boarding preserves history while releasing operational resources back to the system.

## Output Artifacts

- Deregistration command, event wiring, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only DeregisterAcademic.
- [ ] Acceptance criteria cover extension release, retained history, and event emission.
- [ ] Showcase proves business-safe off-boarding behavior.
