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
name: implement-academia-ep-3-8-record-degree-obtained
description: Implement the RecordDegreeObtained qualification command slice
author: John Miller
tags: [academia, implementation, qualifications, command]
context: "Zeus Academia Phase 3 qualification implementation"
expected_output: "A slice-scoped implementation plan for RecordDegreeObtained"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RecordDegreeObtained

## Slice Summary and Business Value

- Slice: RecordDegreeObtained
- Business outcome: let an existing academic gain an additional qualification tied to one degree and one university.
- Out of scope: changing or removing an existing qualification.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel AcademicQualification rules
- RegisterAcademic, ManageDegrees, and ManageUniversities slice files

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic, ManageDegrees, ManageUniversities
- Blocking risks: duplicate Academic+Degree pairs must be rejected consistently in both code and persistence where practical.
- Existing patterns to reuse: qualification persistence shape established by registration and reference-data lookup flows.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                               | Inputs                                        | Outputs                     | Escalate when                                                                |
| -------------------------- | -------------------------------------------------------------- | --------------------------------------------- | --------------------------- | ---------------------------------------------------------------------------- |
| slice-coordinator          | confirm qualification storage and identifier strategy          | execution plan and current persistence model  | approved slice targets      | registration stored qualifications in a way this slice cannot extend cleanly |
| backend-domain       | implement command, validator, handler, endpoint                | qualification rules and reference-data slices | qualification-add code path | duplicate-detection logic requires a broader data redesign                   |
| testing-verification | verify happy path, invalid references, and duplicate rejection | implemented slice                             | tests and evidence          | duplicate degree records slip through under realistic data                   |

## Ordered Implementation Steps

1. Confirm qualification storage shape and route.
   Targets: src/features/Qualifications/RecordDegreeObtained/ or equivalent and current qualification model.
   Owner: slice-coordinator.
   Validation before next step: academic-degree-university persistence strategy is explicit.
2. Implement qualification-add behavior.
   Targets: command, validator, handler, endpoint, mappings.
   Owner: backend-domain.
   Validation before next step: valid references persist a new qualification and duplicate Academic+Degree pairs are blocked.
3. Verify qualification behavior.
   Targets: tests for happy path, invalid degree or university, duplicate degree pair, and follow-up read checks.
   Owner: testing-verification.
   Validation before next step: the slice leaves the qualification set consistent and queryable.

## Verification and Acceptance Criteria

### Review-Prevention Guardrails

- Dependency compatibility is validated for coupled tooling packages when touched (for example xUnit core and runner major versions align).
- Result-style failure factories guard non-null failure payloads in both generic and non-generic wrappers when touched.
- Value-object parse/create APIs reject lossy coercion unless explicitly required and covered by tests.
- Integration tests that provision external resources include deterministic best-effort cleanup in `finally` blocks.
- A valid new qualification is added for an existing academic.
- Invalid degree or university references are rejected.
- A duplicate Academic+Degree pair is rejected even if the university differs.
- Qualification queries or profile reads show the new qualification after success.

## Human Showcase Steps

1. Starting state: a registered academic exists and degree/university reference data is available.
   Action: submit a valid record-degree-obtained request.
   Expected result: the qualification is added to the academic record.
   Value demonstrated: academic credentials can evolve over time without re-registration.
2. Starting state: the academic already has that degree recorded.
   Action: submit the same degree again.
   Expected result: the request fails and no duplicate qualification is stored.
   Value demonstrated: the system preserves the one-university-per-academic-degree rule.

## Completion Checklist

- [ ] Review-prevention guardrails were evaluated and marked N/A where not applicable.
- [ ] If test packages changed, compatibility is verified (for example xUnit core and runner major versions align).
- [ ] If value-object parsing or creation changed, lossy coercion is rejected unless explicitly required and tested.
- [ ] If integration tests create external resources, teardown is enforced with best-effort `finally` cleanup.
- [ ] Reference-data lookups are enforced.
- [ ] Duplicate Academic+Degree pairs are blocked.
- [ ] Qualification visibility is verified after success.
- [ ] Failure paths are tested.
- [ ] The slice stays focused on adding, not editing or removing, qualifications.
