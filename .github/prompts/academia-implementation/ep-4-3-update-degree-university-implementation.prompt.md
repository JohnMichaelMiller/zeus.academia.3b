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
name: implement-academia-ep-4-3-update-degree-university
description: Implement the UpdateDegreeUniversity command slice
author: John Miller
tags: [academia, implementation, qualifications, command]
context: "Zeus Academia Phase 4 qualification maintenance implementation"
expected_output: "A slice-scoped implementation plan for UpdateDegreeUniversity"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement UpdateDegreeUniversity

## Slice Summary and Business Value

- Slice: UpdateDegreeUniversity
- Business outcome: correct the university attached to an existing academic degree record.
- Out of scope: adding new qualifications or removing them.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- RecordDegreeObtained slice files
- ManageUniversities slice files

## Prerequisites and Dependency Checks

- Required prior slices: RecordDegreeObtained
- Blocking risks: updates must target an existing qualification record and preserve the one-university-per-academic-degree rule.
- Existing patterns to reuse: qualification reference lookups, deterministic missing-record handling, and duplicate protection aligned between code and persistence.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                         | Inputs                                            | Outputs                     | Escalate when                                                           |
| -------------------- | -------------------------------------------------------- | ------------------------------------------------- | --------------------------- | ----------------------------------------------------------------------- |
| slice-coordinator    | confirm qualification identifier strategy and route      | execution plan and current qualification model    | approved command contract   | qualification records are not uniquely addressable in the current model |
| backend-domain       | implement update command, validator, handler, endpoint   | qualification model and university reference data | university-update code path | update semantics would break the academic-degree uniqueness rule        |
| testing-verification | verify existing-record update and missing-record failure | implemented slice                                 | tests and evidence          | updates create new rows instead of modifying the intended record        |

## Ordered Implementation Steps

1. Confirm qualification identity and update route.
   Targets: src/features/Qualifications/UpdateDegreeUniversity/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: the target qualification can be identified unambiguously.
2. Implement update behavior.
   Targets: command, validator, handler, endpoint.
   Owner: backend-domain.
   Validation before next step: only existing qualification records are updated, new university references are valid, and any persisted qualification identifier or code-length rules are enforced before persistence through shared canonical definitions.
3. Verify update behavior.
   Targets: tests for successful update, missing qualification, invalid university, read-model visibility, and proof that the existing committed migration artifact still backs qualification uniqueness or, if this slice changes schema, a new committed migration artifact in the confirmed persistence root.
   Owner: testing-verification.
   Validation before next step: the updated university is visible and no duplicate qualification is created.

## Verification and Acceptance Criteria

- Existing qualification records can be updated to a new valid university.
- Missing qualification targets fail cleanly.
- Invalid university references are rejected.
- Qualification reads reflect the updated university after success.
- Updating a qualification does not create or permit duplicate Academic+Degree state in persistence.
- Qualification identity, code length, and normalization rules are defined once for reuse across domain validation and EF Core mapping.

## Human Showcase Steps

1. Starting state: an academic has a recorded degree qualification.
   Action: submit an update-degree-university request with a different valid university code.
   Expected result: the qualification is updated in place.
   Value demonstrated: administrative corrections can be made without deleting and recreating the qualification.
2. Starting state: choose a nonexistent qualification target.
   Action: submit the same command.
   Expected result: the request fails cleanly.
   Value demonstrated: updates are applied only to real records.

## Completion Checklist

- [ ] Existing-record targeting is explicit.
- [ ] University references are validated.
- [ ] Shared qualification type mutability is intentional and documented in review notes (immutable-by-default unless lifecycle mutation is required).
- [ ] Qualification identifier, length, and normalization rules are enforced before persistence using shared constants or a single canonical definition reused by EF Core mappings.
- [ ] Updated data is visible after success.
- [ ] Missing-record and invalid-reference paths are tested.
- [ ] Constraint-validation tests assert stable signals (exception type, constraint name, or SQL state), not provider-specific full error-message text.
- [ ] Target-provider mappings for qualification keys and related foreign keys are explicit enough to keep generated migrations valid.
- [ ] Verification ties qualification uniqueness to the existing committed migration artifact or a new one when this slice changes schema.
- [ ] The slice stays focused on updates only.
