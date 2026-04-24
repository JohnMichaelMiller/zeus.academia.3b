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
name: implement-academia-ep-3-1-view-academic-profile
description: Implement the ViewAcademicProfile query slice
author: John Miller
tags: [academia, implementation, academics, query]
context: "Zeus Academia Phase 3 profile query implementation"
expected_output: "A slice-scoped implementation plan for ViewAcademicProfile"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ViewAcademicProfile

## Slice Summary and Business Value

- Slice: ViewAcademicProfile
- Business outcome: return a complete academic profile with rank, derived access level, qualifications, extension, and employment state.
- Out of scope: mutating any academic state.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/instructions/vertical-slice-implementation.instructions.md
- .github/instructions/mediatr-implementation.instructions.md
- .github/instructions/aspnetcore-implementation.instructions.md
- RegisterAcademic slice files and tests

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic
- Blocking risks: response shape can drift if derived access level or qualification loading is recomputed inconsistently.
- Existing patterns to reuse: query slice folder, read-only projection, not-found handling, and stable response DTOs.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                 | Inputs                                       | Outputs                        | Escalate when                                                                                 |
| -------------------- | ------------------------------------------------ | -------------------------------------------- | ------------------------------ | --------------------------------------------------------------------------------------------- |
| slice-coordinator    | confirm route and projection shape               | execution plan, current repo tree            | approved query scope           | current academic data model cannot return all required fields without revisiting registration |
| backend-domain       | implement query, handler, response, and endpoint | registration data model, Shared Kernel types | profile query code path        | qualifications or extension data need a new shared projection strategy                        |
| testing-verification | verify happy path and not-found behavior         | implemented slice                            | integration tests and evidence | returned profile omits derived or joined data required by the plan                            |

## Ordered Implementation Steps

1. Confirm the profile response contract and route.
   Targets: src/features/Academics/ViewAcademicProfile/ or equivalent, existing academic route grouping.
   Owner: slice-coordinator.
   Validation before next step: response includes empNr, name, rank, access level, extension, qualifications, and employment state.
2. Implement the query projection.
   Targets: query, handler, response DTO, endpoint, and mapping helpers.
   Owner: backend-domain.
   Validation before next step: one academic record can be projected without command-side mutation logic.
3. Verify query behavior.
   Targets: integration tests for found and not-found cases.
   Owner: testing-verification.
   Validation before next step: tests prove the profile returns the required data and fails cleanly when absent.

## Verification and Acceptance Criteria

- The profile query returns empNr, name, rank, derived access level, qualifications, extension, and current employment state.
- Not-found requests return the repo-standard missing-resource behavior.
- AccessLevel in the response matches the current rank-derived rule.
- Automated tests cover the happy path and not-found path.

## Human Showcase Steps

1. Starting state: one academic exists from RegisterAcademic.
   Action: call the profile endpoint for that academic.
   Expected result: the response contains the full academic profile with derived and related data.
   Value demonstrated: users can inspect a complete academic record without reconstructing it from multiple sources.
2. Starting state: choose a missing academic identifier.
   Action: call the same endpoint with the missing id.
   Expected result: a clean not-found response is returned.
   Value demonstrated: consumers can handle missing records predictably.

## Completion Checklist

- [ ] Query scope is read-only.
- [ ] Response shape includes all required fields.
- [ ] Derived access level is consistent with rank.
- [ ] Not-found behavior is verified.
- [ ] Verification evidence exists for happy and failure paths.
