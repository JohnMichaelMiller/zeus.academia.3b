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
name: implement-academia-ep-1-1-manage-ranks
description: Implement the ManageRanks reference-data slice for rank administration and access-level mapping
author: John Miller
tags: [academia, implementation, reference-data, ranks]
context: "Zeus Academia Phase 1 reference-data implementation"
expected_output: "A slice-scoped implementation plan for ManageRanks"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ManageRanks

## Slice Summary and Business Value

- Slice: ManageRanks
- Business outcome: provide the canonical rank reference data required by registration, rank changes, and reporting.
- Out of scope: academic creation, employment-state mutations, and downstream reports.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/models/workflows/academia-implementation-plan.md
- .github/instructions/vertical-slice-implementation.instructions.md
- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.
- .github/instructions/mediatr-implementation.instructions.md
- .github/instructions/fluentvalidation-implementation.instructions.md
- .github/instructions/aspnetcore-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: Shared Kernel
- Blocking risks: rank code representation may already exist in Shared Kernel; do not create a second source of truth.
- Existing patterns to reuse: command plus query slice structure, validator beside command, unique reference-data persistence, and rank-to-access mapping rules.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                              | Inputs                                      | Outputs                                           | Escalate when                                                      |
| -------------------------- | ------------------------------------------------------------- | ------------------------------------------- | ------------------------------------------------- | ------------------------------------------------------------------ |
| slice-coordinator          | confirm whether rank records are seeded, API-managed, or both | execution plan, current data setup          | approved slice boundary and file targets          | repo already stores rank reference data in a conflicting location  |
| backend-domain       | build add and list rank behavior with validation              | shared kernel rank model, slice conventions | commands, queries, handlers, responses, endpoints | code tries to bypass the canonical rank codes P, SL, L             |
| testing-verification | verify uniqueness, allowed codes, and queryability            | implemented slice                           | tests and evidence                                | validator and persistence behavior disagree on allowed rank values |

## Ordered Implementation Steps

1. Confirm how rank data is stored and exposed.
   Targets: src/features/ReferenceData/ManageRanks/ or current equivalent, persistence registration, seed data path.
   Owner: slice-coordinator.
   Validation before next step: one canonical approach is selected for add/list behavior and existing seed data conflicts are resolved.
2. Implement add-rank command behavior.
   Targets: AddRank command, validator, handler, response, endpoint, and mapping helpers within the ManageRanks slice folder.
   Owner: backend-domain.
   Validation before next step: only P, SL, and L are accepted and duplicates are rejected deterministically.
3. Implement rank listing query behavior.
   Targets: ListRanks query, handler, response contract, and endpoint.
   Owner: backend-domain.
   Validation before next step: returned data exposes stable rank codes and their access-level mapping.
4. Add tests and verification evidence.
   Targets: validator tests, handler tests, integration tests for uniqueness and list behavior.
   Owner: testing-verification.
   Validation before next step: add and list flows both pass with valid and invalid inputs.

## Verification and Acceptance Criteria

- Adding a rank accepts only the codes P, SL, and L.
- Attempting to add a duplicate rank code fails without creating a second record.
- Listing ranks returns the canonical codes in a stable form that downstream slices can resolve.
- The slice exposes or documents the mapping from rank to access level so registration and reports do not redefine it.
- Automated tests cover valid add, invalid code, duplicate code, and list-query behavior.

## Human Showcase Steps

1. Starting state: Shared Kernel is present and no conflicting rank seed exists.
   Action: call the add-rank endpoint or run the approved seed/admin path for P, SL, and L.
   Expected result: the canonical ranks are stored once and only once.
   Value demonstrated: every dependent slice can rely on one approved set of rank codes.
2. Starting state: rank records exist.
   Action: call the rank-list query endpoint.
   Expected result: rank data is returned with the expected codes and access-level relationship.
   Value demonstrated: registration and reporting can consume reference data instead of hardcoding it.

## Completion Checklist

- [ ] ManageRanks stays limited to rank reference-data behavior.
- [ ] Rank validation is restricted to P, SL, and L.
- [ ] Duplicate codes are blocked at the application and persistence levels as appropriate.
- [ ] List behavior returns stable rank data for downstream slices.
- [ ] Verification covers add and list success and failure paths.
- [ ] Any chosen seed strategy is documented for later environments.
