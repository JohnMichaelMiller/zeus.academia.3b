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
name: implement-release-extension
description: Guide delivery of the ReleaseExtension slice for returning an extension to the available pool.
author: John Miller
tags: [implementation, vertical-slice, academia, extension]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ReleaseExtension slice with command handler, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ReleaseExtension

## Objective

Deliver workflow 4.3 so an assigned extension can be released back into the available pool.

## Slice Boundary

- In scope: remove the extension assignment from an academic and mark the extension available.
- Non-goals: deregistration logic beyond release behavior or extension deprovisioning.
- Dependencies: AssignExtension.
- Entry points: `src/backend/Features/Extensions/Commands/ReleaseExtension/**`, `/api/academics/{empNr}/extension`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                           | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | -------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm release semantics and readback expectations      | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement release-extension command, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate released extensions become available again      | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                               | Files                                                            | Done When                             | Verification                                     |
| ---- | --------------------------- | ------------------------------------------------------------------------------------ | ---------------------------------------------------------------- | ------------------------------------- | ------------------------------------------------ |
| 1    | `product-manager`           | Confirm idempotency expectations when an academic has no extension                   | Plan and workflows                                               | Scope note approved                   | Checklist updated                                |
| 2    | `backend-slice-implementer` | Build release-extension command, handler, and endpoint                               | `src/backend/Features/Extensions/Commands/ReleaseExtension/**`   | Backend compiles and release persists | `dotnet build` passes                            |
| 3    | `backend-slice-implementer` | Add tests for successful release, no-assignment handling, and follow-up availability | `tests/backend/Features/Extensions/Commands/ReleaseExtension/**` | Tests pass                            | `dotnet test` passes for release-extension scope |
| 4    | `slice-verifier`            | Run manual release scenario and capture evidence                                     | HTTP collection or integration tests                             | Acceptance met                        | Verification summary saved                       |

## Acceptance Criteria

- Given an academic with an assigned extension, when the extension is released, then the academic no longer holds the extension.
- Given a released extension, when available extensions are queried afterward, then the released extension appears in the available pool.
- Given an academic without an assigned extension, when release is requested, then the result is stable and no invalid state is introduced.

## Verification Plan

- Automated: backend build plus release-extension tests.
- Manual: release an assigned extension and verify the available-extension query afterward.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with an academic that has an assigned extension.
2. Call `DELETE /api/academics/{empNr}/extension`.
   Expected: the assignment is removed.
3. Query available extensions.
   Expected: the released extension appears in the available pool.

Value demonstrated: operational resources can be returned cleanly for reuse without deleting the extension itself.

## Output Artifacts

- Release-extension command, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ReleaseExtension.
- [ ] Acceptance criteria cover return-to-pool behavior.
- [ ] Showcase proves the released extension is reusable.
