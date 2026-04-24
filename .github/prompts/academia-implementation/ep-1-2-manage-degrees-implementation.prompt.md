---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "616990b5-0c5d-4735-a876-23fd1ebb4ff6"
prompt: |
  Create an implementation prompt for each slice in the #file:academia-execution-plan.md
started: "2026-04-20T20:40:00Z"
ended: "2026-04-20T21:40:00Z"
task_durations:
  - task: "analyze slice dependencies"
    duration: "00:15:00"
  - task: "draft slice implementation prompt"
    duration: "00:35:00"
  - task: "traceability and review"
    duration: "00:10:00"
total_duration: "01:00:00"
ai_log: "ai-logs/2026/04/20/616990b5-0c5d-4735-a876-23fd1ebb4ff6/conversation.md"
source: ".github/models/workflows/academia-execution-plan.md"
name: implement-academia-ep-1-2-manage-degrees
description: Implement the ManageDegrees reference-data slice for canonical degree codes
author: John Miller
tags: [academia, implementation, reference-data, degrees]
context: "Zeus Academia Phase 1 reference-data implementation"
expected_output: "A slice-scoped implementation plan for ManageDegrees"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ManageDegrees

## Slice Summary and Business Value

- Slice: ManageDegrees
- Business outcome: provide the canonical degree catalog required by registration and qualification workflows.
- Out of scope: qualification assignment to academics, university relationships, and reporting.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/models/workflows/academia-implementation-plan.md
- .github/instructions/vertical-slice-implementation.instructions.md
- .github/instructions/mediatr-implementation.instructions.md
- .github/instructions/fluentvalidation-implementation.instructions.md
- .github/instructions/aspnetcore-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: Shared Kernel
- Blocking risks: degree codes may already appear in fixtures or examples; normalize them instead of creating duplicate catalogs.
- Existing patterns to reuse: reference-data command/query shape, unique code validation backed by database constraints, and deterministic list contracts.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                               | Inputs                                       | Outputs                                      | Escalate when                                                    |
| -------------------- | -------------------------------------------------------------- | -------------------------------------------- | -------------------------------------------- | ---------------------------------------------------------------- |
| slice-coordinator    | confirm degree storage, seed expectations, and route placement | execution plan, current repo tree            | approved targets and blocker notes           | current codebase already contains an incompatible degree catalog |
| backend-domain       | implement add and list degree behavior                         | Shared Kernel degree type, slice conventions | commands, queries, handlers, DTOs, endpoints | degree code rules are unclear or clash with seeded data          |
| testing-verification | verify uniqueness and list behavior                            | implemented slice                            | tests and evidence                           | persistence allows duplicates or queries return unstable values  |

## Ordered Implementation Steps

1. Confirm the canonical degree-data location and route shape.
   Targets: src/features/ReferenceData/ManageDegrees/ or current equivalent, persistence root, migration path, and any seed scripts.
   Owner: slice-coordinator.
   Validation before next step: one degree catalog source is identified and slice targets are approved.
2. Implement add-degree behavior.
   Targets: AddDegree command, validator, handler, response, endpoint, and any mappings.
   Owner: backend-domain.
   Validation before next step: duplicate degree codes are rejected and valid codes persist successfully.
3. Implement list-degree behavior.
   Targets: ListDegrees query, handler, response contract, and endpoint.
   Owner: backend-domain.
   Validation before next step: query returns stable degree records suitable for registration lookups.
4. Verify the slice end to end.
   Targets: validator tests, handler tests, integration tests for add/list flows, and the committed migration artifact in the confirmed persistence root when this slice introduces or changes duplicate-code protection.
   Owner: testing-verification.
   Validation before next step: tests cover valid add, duplicate rejection, and list-query results.

## Verification and Acceptance Criteria

- Adding a new degree code persists one canonical reference-data record.
- Adding a duplicate degree code fails without creating a second record.
- Degree-code uniqueness is protected in both application behavior and persistence.
- If this slice introduces or changes the uniqueness schema, a committed migration artifact exists in the confirmed persistence root.
- Listing degrees returns stable records that downstream registration and qualification slices can resolve.
- Validation and persistence rules agree on what constitutes a valid degree payload.
- Automated tests cover the success path, duplicate path, and list-query behavior.

## Human Showcase Steps

1. Starting state: Shared Kernel exists and degree reference data is not yet populated.
   Action: add baseline degree codes through the approved endpoint or seed/admin workflow.
   Expected result: the degree catalog becomes queryable with one record per code.
   Value demonstrated: qualification-related slices can reference canonical degrees instead of free-form text.
2. Starting state: degree data exists.
   Action: call the list-degrees endpoint.
   Expected result: the system returns stable degree codes that can feed registration and qualification UI or API workflows.
   Value demonstrated: later slices can validate degree references against controlled data.

## Completion Checklist

- [ ] ManageDegrees stays limited to degree reference-data behavior.
- [ ] Degree code uniqueness is enforced.
- [ ] Required migration files are present when this slice introduces or changes reference-data uniqueness schema.
- [ ] List behavior is stable and reusable by dependent slices.
- [ ] Tests cover add and list success and failure cases.
- [ ] Any seed or bootstrap path is documented.
- [ ] No later qualification behavior is folded into this slice.
