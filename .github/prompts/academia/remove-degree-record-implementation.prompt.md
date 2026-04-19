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
name: implement-remove-degree-record
description: Guide delivery of the RemoveDegreeRecord slice for removing a qualification while preserving the at-least-one rule.
author: John Miller
tags: [implementation, vertical-slice, academia, qualification]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed RemoveDegreeRecord slice with command handler, domain-rule enforcement, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RemoveDegreeRecord

## Objective

Deliver workflow 5.3 so HR can remove a qualification record while ensuring the academic retains at least one qualification.

## Slice Boundary

- In scope: remove one academic-degree qualification, reject removal if it would leave zero qualifications.
- Non-goals: deregistration or historical archival beyond the slice's delete behavior.
- Dependencies: RecordDegreeObtained.
- Entry points: `src/backend/Features/Qualifications/Commands/RemoveDegreeRecord/**`, `/api/academics/{empNr}/qualifications/{degreeCode}`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                    | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ----------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm remove behavior and retained-history assumptions          | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement remove-degree command, endpoint, guard logic, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate at-least-one-degree rule and delete behavior             | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                               | Files                                                                  | Done When                                     | Verification                                        |
| ---- | --------------------------- | ------------------------------------------------------------------------------------ | ---------------------------------------------------------------------- | --------------------------------------------- | --------------------------------------------------- |
| 1    | `product-manager`           | Confirm whether the endpoint should return remaining qualification count             | Plan and workflows                                                     | Scope note approved                           | Checklist updated                                   |
| 2    | `backend-slice-implementer` | Build remove-degree command, handler, endpoint, and domain guard                     | `src/backend/Features/Qualifications/Commands/RemoveDegreeRecord/**`   | Backend compiles and guarded removal persists | `dotnet build` passes                               |
| 3    | `backend-slice-implementer` | Add tests for successful removal, last-degree rejection, and missing-record handling | `tests/backend/Features/Qualifications/Commands/RemoveDegreeRecord/**` | Tests pass                                    | `dotnet test` passes for qualification-remove scope |
| 4    | `slice-verifier`            | Run manual removal scenario and capture evidence                                     | HTTP collection or integration tests                                   | Acceptance met                                | Verification summary saved                          |

## Acceptance Criteria

- Given an academic with more than one qualification, when HR removes one degree record, then the selected qualification is removed.
- Given an academic with exactly one qualification, when HR attempts to remove it, then the system rejects the request.
- Given a missing qualification record, when the request is submitted, then the system returns not found or equivalent failure.

## Verification Plan

- Automated: backend build plus remove-degree tests.
- Manual: remove a non-final degree and retry removing the final remaining degree.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with an academic that has at least two qualifications.
2. Call `DELETE /api/academics/{empNr}/qualifications/{degreeCode}` for one qualification.
   Expected: the selected qualification is removed.
3. Repeat until one qualification remains, then retry.
   Expected: the last removal is rejected.

Value demonstrated: qualification cleanup is possible without violating the rule that every academic must retain at least one qualification.

## Output Artifacts

- Remove-degree command, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only RemoveDegreeRecord.
- [ ] Acceptance criteria cover the at-least-one-degree guard.
- [ ] Showcase proves safe removal behavior.
