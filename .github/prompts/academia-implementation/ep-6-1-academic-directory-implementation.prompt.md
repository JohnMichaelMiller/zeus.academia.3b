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
name: implement-academia-ep-6-1-academic-directory
description: Implement the AcademicDirectory report slice
author: John Miller
tags: [academia, implementation, reports, query]
context: "Zeus Academia Phase 6 reporting implementation"
expected_output: "A slice-scoped implementation plan for AcademicDirectory"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement AcademicDirectory

## Slice Summary and Business Value

- Slice: AcademicDirectory
- Business outcome: provide the baseline report-style directory of active academic records.
- Out of scope: grouped analytics by rank, access level, or qualification.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- SearchListAcademics, ViewAcademicProfile, and DeregisterAcademic slice files
- any shared reporting or pagination contract in the repo

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.
- .github/instructions/xunit-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic, later lifecycle slices that affect active state
- Blocking risks: report logic should use read-optimized projections rather than command-side aggregate loading.
- Existing patterns to reuse: paginated query contracts and stable DTOs.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                                       | Inputs                                           | Outputs                    | Escalate when                                              |
| -------------------------- | ---------------------------------------------------------------------- | ------------------------------------------------ | -------------------------- | ---------------------------------------------------------- |
| slice-coordinator          | confirm report route, active-record semantics, and projection strategy | execution plan and current read patterns         | approved report contract   | reporting requires a projection store not yet present      |
| report-projection       | implement directory query, projection, DTOs, endpoint                  | stable lifecycle data and shared paging patterns | directory report code path | active/inactive semantics are unclear after deregistration |
| testing-verification | verify report completeness and performance on seeded data              | implemented slice                                | tests and evidence         | report output diverges from source-of-truth slice data     |

## Ordered Implementation Steps

1. Confirm directory projection shape and route.
   Targets: src/features/Reports/AcademicDirectory/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: required columns include name, rank, access level, extension, and employment status.
2. Implement the directory report.
   Targets: query, handler, response DTOs, endpoint, and projection logic if needed.
   Owner: report-projection.
   Validation before next step: the report returns stable, read-optimized results.
3. Verify accuracy and performance.
   Targets: integration tests and representative seeded-data checks.
   Owner: testing-verification.
   Validation before next step: report output matches current academic state and performs acceptably.

## Verification and Acceptance Criteria

### Review-Prevention Guardrails

- Dependency compatibility is validated for coupled tooling packages when touched (for example xUnit core and runner major versions align).
- Result-style failure factories guard non-null failure payloads in both generic and non-generic wrappers when touched.
- Value-object parse/create APIs reject lossy coercion unless explicitly required and covered by tests.
- Integration tests that provision external resources include deterministic best-effort cleanup in `finally` blocks.
- The directory lists the required academic fields accurately.
- Report output matches current source data after lifecycle mutations.
- Paging and sorting, if present, are deterministic.
- Seeded-data performance meets the plan's expectations.

## Human Showcase Steps

1. Starting state: multiple academics exist with varied employment and extension states.
   Action: call the academic-directory report.
   Expected result: a complete, readable directory is returned.
   Value demonstrated: stakeholders get a single operational overview of the academic population.
2. Starting state: update one academic and deregister another in controlled test data.
   Action: call the report again.
   Expected result: the directory reflects the changed live state.
   Value demonstrated: reporting stays aligned with operational slices.

## Completion Checklist

- [ ] Review-prevention guardrails were evaluated and marked N/A where not applicable.
- [ ] If test packages changed, compatibility is verified (for example xUnit core and runner major versions align).
- [ ] If value-object parsing or creation changed, lossy coercion is rejected unless explicitly required and tested.
- [ ] If integration tests create external resources, teardown is enforced with best-effort `finally` cleanup.
- [ ] The report uses read-optimized queries.
- [ ] Output fields match the execution plan.
- [ ] Accuracy is verified against source slices.
- [ ] Performance is checked on seeded data.
- [ ] The report remains a directory, not grouped analytics.
