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
name: implement-academia-ep-3-3-search-list-academics
description: Implement the SearchListAcademics query slice with filtering and pagination
author: John Miller
tags: [academia, implementation, academics, search, query]
context: "Zeus Academia Phase 3 academic search implementation"
expected_output: "A slice-scoped implementation plan for SearchListAcademics"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement SearchListAcademics

## Slice Summary and Business Value

- Slice: SearchListAcademics
- Business outcome: allow users to find academics by key filters without jumping straight to reports.
- Out of scope: analytical grouping or aggregate reporting.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/instructions/vertical-slice-implementation.instructions.md
- RegisterAcademic and ViewAcademicProfile slice files
- any existing pagination or filter contract in shared API code

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic
- Blocking risks: filters can become inconsistent with profile and report fields if contracts diverge.
- Existing patterns to reuse: read-only query projections, paginated endpoints, and deterministic sort behavior.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                      | Inputs                                       | Outputs                        | Escalate when                                                     |
| -------------------------- | ----------------------------------------------------- | -------------------------------------------- | ------------------------------ | ----------------------------------------------------------------- |
| slice-coordinator          | confirm filter contract and default sort              | execution plan and current query conventions | approved query surface         | existing API pagination rules conflict with the needed filter set |
| backend-domain       | implement filtered query, DTOs, endpoint, and mapping | academic persistence model and filter rules  | search/list code path          | one or more filters need data not yet exposed by registration     |
| testing-verification | verify filters, pagination, and sorting               | implemented slice                            | integration tests and evidence | query output is nondeterministic across repeated runs             |

## Ordered Implementation Steps

1. Confirm the filter contract.
   Targets: src/features/Academics/SearchListAcademics/ or equivalent, shared paging types, endpoint contract.
   Owner: slice-coordinator.
   Validation before next step: filters include name, rank, access level, employment status, degree, and university.
2. Implement the query and endpoint.
   Targets: query, handler, response DTOs, endpoint, and mappings.
   Owner: backend-domain.
   Validation before next step: query is read-optimized and supports stable sorting and pagination.
3. Verify filtering behavior.
   Targets: integration tests for each filter family plus pagination and sort cases.
   Owner: testing-verification.
   Validation before next step: seeded data returns predictable subsets and ordering.

## Verification and Acceptance Criteria

- The list query supports filters for name, rank, access level, employment status, degree, and university.
- Pagination and sorting are deterministic across repeated calls.
- Empty-result cases return a valid empty page rather than an error.
- Automated tests cover multiple filter combinations plus paging and sorting behavior.

## Human Showcase Steps

1. Starting state: several academics exist with varied rank, qualification, and employment data.
   Action: query the list endpoint with different filter combinations.
   Expected result: each call returns only the matching academics in a stable order.
   Value demonstrated: users can find operational records without building a report first.
2. Starting state: a filter combination with no matches.
   Action: call the same endpoint with the empty-result filter set.
   Expected result: the API returns an empty paged result cleanly.
   Value demonstrated: client code can handle no-match scenarios predictably.

## Completion Checklist

- [ ] Filter contract matches the execution plan.
- [ ] Pagination and sorting are deterministic.
- [ ] Empty-result behavior is clean.
- [ ] Tests cover filter, sort, and pagination behavior.
- [ ] Verification evidence exists for the slice's acceptance criteria.
- [ ] The slice remains operational search, not reporting.
