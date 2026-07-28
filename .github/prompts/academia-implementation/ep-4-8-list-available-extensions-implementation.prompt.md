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
name: implement-academia-ep-4-8-list-available-extensions
description: Implement the ListAvailableExtensions query slice
author: John Miller
tags: [academia, implementation, extensions, query]
context: "Zeus Academia Phase 4 extension query implementation"
expected_output: "A slice-scoped implementation plan for ListAvailableExtensions"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ListAvailableExtensions

## Slice Summary and Business Value

- Slice: ListAvailableExtensions
- Business outcome: expose the pool of provisioned extensions that are not currently assigned.
- Out of scope: assignment or release commands.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- ProvisionExtension, AssignExtension, ReassignExtension, and ReleaseExtension slice files

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.

## Prerequisites and Dependency Checks

- Required prior slices: AssignExtension
- Blocking risks: availability must reflect the latest assignment lifecycle transitions.
- Existing patterns to reuse: read-only extension projections and deterministic empty-result behavior.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                       | Inputs                                        | Outputs                      | Escalate when                                                      |
| -------------------------- | ------------------------------------------------------ | --------------------------------------------- | ---------------------------- | ------------------------------------------------------------------ |
| slice-coordinator          | confirm route and response shape                       | execution plan and existing extension queries | approved query contract      | current model cannot distinguish provisioned from assigned clearly |
| backend-domain       | implement available-extension query, handler, endpoint | extension pool and assignment model           | availability query code path | availability requires a separate projection store                  |
| testing-verification | verify pool accuracy after assign and release flows    | implemented slice                             | tests and evidence           | query results lag behind current assignment state                  |

## Ordered Implementation Steps

1. Confirm availability semantics and route.
   Targets: src/features/Extensions/ListAvailableExtensions/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: only provisioned, unassigned extensions are considered available.
2. Implement the query.
   Targets: query, handler, response DTOs, endpoint.
   Owner: backend-domain.
   Validation before next step: results exclude assigned extensions and include released ones.
3. Verify availability behavior.
   Targets: tests for initial availability, post-assignment exclusion, and post-release inclusion.
   Owner: testing-verification.
   Validation before next step: query results track lifecycle transitions accurately.

## Verification and Acceptance Criteria

- The query returns only provisioned, currently unassigned extensions.
- Assigned extensions are excluded.
- Released extensions reappear as available.
- Empty-result cases return a clean empty response.

## Human Showcase Steps

1. Starting state: several extensions are provisioned and some are assigned.
   Action: call the available-extensions query.
   Expected result: only the free extensions are returned.
   Value demonstrated: users can see live inventory for assignment workflows.
2. Starting state: release an assigned extension.
   Action: call the query again.
   Expected result: the released extension now appears.
   Value demonstrated: the inventory view stays synchronized with lifecycle commands.

## Completion Checklist

- [ ] Availability is defined as provisioned and unassigned.
- [ ] Assigned and released transitions are reflected.
- [ ] Empty-result behavior is verified.
- [ ] Tests cover state changes across lifecycle commands.
- [ ] The slice remains query-only.
