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
name: implement-provision-extension
description: Guide delivery of the ProvisionExtension slice for provisioning and deprovisioning available extensions.
author: John Miller
tags: [implementation, vertical-slice, academia, extension, reference-data]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ProvisionExtension slice with endpoints, validation, persistence, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ProvisionExtension

## Objective

Deliver workflows 4.6 and 4.7 so administrators can provision new extension numbers and deprovision unused ones.

## Slice Boundary

- In scope: provision extension, deprovision extension, numeric extension validation, assigned-extension guard.
- Non-goals: assigning or reassigning extensions to academics.
- Dependencies: Shared Kernel only.
- Entry points: `src/backend/Features/Extensions/Commands/ProvisionExtension/`, `/api/extensions/provisioned`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                             | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | -------------------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm extension lifecycle scope and admin-only behavior                  | Plan, workflows, ORM rules              | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement provision/deprovision commands, endpoint, persistence, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate numeric format and assigned guard behavior                        | Implemented slice                       | Verification notes |

## Implementation Steps

| Step | Owner                       | Action                                                                                                                       | Files                                  | Done When           | Verification                                          |
| ---- | --------------------------- | ---------------------------------------------------------------------------------------------------------------------------- | -------------------------------------- | ------------------- | ----------------------------------------------------- |
| 1    | `product-manager`           | Confirm extNr format, uniqueness expectations, and admin authorization assumptions                                           | Plan and ORM rules                     | Scope note approved | Checklist updated                                     |
| 2    | `backend-slice-implementer` | Implement provision and deprovision commands with numeric validation and assignment guard                                    | `src/backend/Features/Extensions/**`   | Backend compiles    | `dotnet build` passes                                 |
| 3    | `backend-slice-implementer` | Add tests for successful provision, successful deprovision, duplicate extNr, invalid extNr, and assigned-extension rejection | `tests/backend/Features/Extensions/**` | Tests pass          | `dotnet test` passes for extension provisioning scope |
| 4    | `slice-verifier`            | Run API showcase and collect evidence                                                                                        | HTTP collection or integration tests   | Acceptance met      | Verification summary saved                            |

## Acceptance Criteria

- Given a numeric extension number that is not yet provisioned, when an administrator provisions it, then the extension is stored as available.
- Given an already provisioned extension number, when the request is submitted again, then the system rejects it with a conflict result.
- Given a non-numeric or invalid extension number, when the request is submitted, then validation fails and nothing is stored.
- Given an extension currently assigned to an academic, when an administrator attempts to deprovision it, then the system rejects the request and keeps the record.

## Verification Plan

- Automated: backend build plus provision/deprovision tests.
- Manual: provision an extension, deprovision an unused extension, then attempt to deprovision an assigned one.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API.
2. Call `POST /api/extensions/provisioned` with a new numeric extension number.
   Expected: the extension is created and marked available.
3. Call `DELETE /api/extensions/provisioned/{extNr}` for an unused extension.
   Expected: the extension is removed.
4. Attempt the same delete for an extension already assigned to an academic.
   Expected: the API rejects the request.

Value demonstrated: the available extension pool can be managed safely without breaking the 1:1 academic-extension rule.

## Output Artifacts

- Extension provisioning commands, validators, endpoints, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ProvisionExtension.
- [ ] Execution support is stated explicitly.
- [ ] Acceptance criteria cover duplicate, invalid, and assigned-extension failures.
- [ ] Showcase proves safe lifecycle management of extension inventory.
