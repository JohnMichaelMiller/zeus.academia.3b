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
name: implement-reassign-extension
description: Guide delivery of the ReassignExtension slice for moving an extension assignment safely.
author: John Miller
tags: [implementation, vertical-slice, academia, extension]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ReassignExtension slice with command handler, guard logic, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ReassignExtension

## Objective

Deliver workflow 4.2 so HR or IT can move an extension from one academic context to another without breaking uniqueness.

## Slice Boundary

- In scope: release the source extension assignment first, assign the target extension, and enforce that the target academic does not already hold another extension.
- Non-goals: provisioning or deprovisioning extension inventory.
- Dependencies: AssignExtension.
- Entry points: `src/backend/Features/Extensions/Commands/ReassignExtension/**`, `/api/academics/{empNr}/extension/reassignment`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm reassignment semantics and transactional expectations | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement reassign-extension command, endpoint, and tests     | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate atomicity and uniqueness behavior                    | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                                        | Files                                                             | Done When                                             | Verification                                      |
| ---- | --------------------------- | ------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------- | ----------------------------------------------------- | ------------------------------------------------- |
| 1    | `product-manager`           | Confirm whether reassignment changes the extension number or only the owning academic                         | Plan and workflows                                                | Scope note approved                                   | Checklist updated                                 |
| 2    | `backend-slice-implementer` | Build reassign-extension command, transactional handler, and endpoint                                         | `src/backend/Features/Extensions/Commands/ReassignExtension/**`   | Backend compiles and reassignment persists atomically | `dotnet build` passes                             |
| 3    | `backend-slice-implementer` | Add tests for valid reassignment, target academic already holding an extension, and missing source assignment | `tests/backend/Features/Extensions/Commands/ReassignExtension/**` | Tests pass                                            | `dotnet test` passes for reassign-extension scope |
| 4    | `slice-verifier`            | Run manual reassignment scenario and capture evidence                                                         | HTTP collection or integration tests                              | Acceptance met                                        | Verification summary saved                        |

## Acceptance Criteria

- Given an academic with an assigned extension and a valid reassignment request, when the operation completes, then the old assignment is released and the new assignment is stored.
- Given a target academic who already has an extension, when reassignment is attempted, then the system rejects it and preserves the original state.
- Given a missing source assignment, when the request is submitted, then the system rejects it.

## Verification Plan

- Automated: backend build plus reassign-extension tests.
- Manual: reassign an extension successfully and retry with an invalid target.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with an academic holding an extension and another eligible assignment target.
2. Call `POST /api/academics/{empNr}/extension/reassignment` with the new extension or target mapping as defined by the scope decision.
   Expected: the old assignment is released and the new one is active.
3. Retry when the target already holds an extension.
   Expected: the API rejects the request and leaves the original state unchanged.

Value demonstrated: extension movement can happen safely without transiently violating the one-extension-per-academic rule.

## Output Artifacts

- Reassign-extension command, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ReassignExtension.
- [ ] Acceptance criteria cover atomic reassignment behavior.
- [ ] Showcase proves uniqueness is preserved through the move.
