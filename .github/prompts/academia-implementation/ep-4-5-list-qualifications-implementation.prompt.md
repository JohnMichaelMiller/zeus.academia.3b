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
name: implement-academia-ep-4-5-list-qualifications
description: Implement the ListQualifications query slice
author: John Miller
tags: [academia, implementation, qualifications, query]
context: "Zeus Academia Phase 4 qualification query implementation"
expected_output: "A slice-scoped implementation plan for ListQualifications"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ListQualifications

## Slice Summary and Business Value

- Slice: ListQualifications
- Business outcome: expose qualification data by academic, degree, and university for operational use.
- Out of scope: grouped analytical qualification reports.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- RecordDegreeObtained, UpdateDegreeUniversity, and RemoveDegreeRecord slice files
- any shared pagination/query contract already in use

## Prerequisites and Dependency Checks

- Required prior slices: RecordDegreeObtained
- Blocking risks: three query modes must share a stable contract without drifting into report logic. This query slice reads qualification state but does not own duplicate protection or last-qualification retention invariants.
- Existing patterns to reuse: read-only projections, filter contracts, and deterministic pagination when needed.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                                                  | Inputs                                      | Outputs                       | Escalate when                                                |
| -------------------- | --------------------------------------------------------------------------------- | ------------------------------------------- | ----------------------------- | ------------------------------------------------------------ |
| slice-coordinator    | confirm whether one endpoint or multiple endpoints best match current conventions | execution plan and current API style        | approved query surface        | current API conventions make the planned query modes unclear |
| backend-domain       | implement qualification queries and response DTOs                                 | qualification model and shared paging rules | list-qualifications code path | one query mode needs a different storage/projection strategy |
| testing-verification | verify all query modes and empty-result handling                                  | implemented slice                           | tests and evidence            | query modes return inconsistent shapes                       |

## Ordered Implementation Steps

1. Confirm query modes and response shape.
   Targets: src/features/Qualifications/ListQualifications/ or equivalent and any shared paging types.
   Owner: slice-coordinator.
   Validation before next step: by-academic, by-degree, and by-university behaviors are explicit.
2. Implement qualification queries.
   Targets: queries, handlers, response DTOs, endpoints.
   Owner: backend-domain.
   Validation before next step: all query modes are read-only and return stable contracts.
3. Verify the query modes.
   Targets: tests for all three filters plus empty-result scenarios.
   Owner: testing-verification.
   Validation before next step: returned qualification sets match the seeded data accurately.

## Verification and Acceptance Criteria

- Qualifications can be listed by academic, by degree code, and by university code.
- Empty-result requests return a clean empty result.
- Query contracts are stable across the supported list modes.
- Returned qualification state reflects the command-side slices that add, update, and remove qualifications; this query does not redefine qualification invariants.
- Automated tests cover all three query modes.

## Human Showcase Steps

1. Starting state: seeded qualification data exists across multiple academics.
   Action: call each supported qualification query mode.
   Expected result: each call returns the correct subset of qualifications.
   Value demonstrated: operational users can inspect qualifications from different entry points.
2. Starting state: choose a filter with no matches.
   Action: run the same query mode with that filter.
   Expected result: an empty result is returned cleanly.
   Value demonstrated: clients can handle no-match cases without special failure logic.

## Completion Checklist

- [ ] All planned query modes are implemented.
- [ ] Query contracts are stable.
- [ ] Empty-result behavior is verified.
- [ ] Qualification reads remain observational and do not imply ownership of add/update/remove invariants.
- [ ] Tests cover each query mode.
- [ ] Verification evidence exists for the slice's acceptance criteria.
- [ ] The slice stays operational, not analytical.
