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
name: implement-academia-ep-2-1-register-academic
description: Implement the RegisterAcademic slice, the first mandatory delivery gate for the academic lifecycle
author: John Miller
tags: [academia, implementation, academics, registration]
context: "Zeus Academia Phase 2 academic registration implementation"
expected_output: "A slice-scoped implementation plan for RegisterAcademic"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RegisterAcademic

## Slice Summary and Business Value

- Slice: RegisterAcademic
- Business outcome: create an academic with valid identity, rank-derived access level, at least one qualification, and one available extension so every dependent slice has a real source record.
- Out of scope: profile viewing, later employment changes, extension reassignment, and reporting.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/models/workflows/academia-implementation-plan.md
- .github/instructions/project-overview.instructions.md
- .github/instructions/vertical-slice-implementation.instructions.md
- .github/instructions/mediatr-implementation.instructions.md
- .github/instructions/fluentvalidation-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: ManageRanks, ManageDegrees, ManageUniversities, ProvisionExtension
- Blocking risks: this slice is the first hard dependency gate; do not parallelize dependent slices until registration passes integration tests.
- Existing patterns to reuse: command validator beside handler, atomic persistence, rank-derived access-level logic from Shared Kernel, and extension uniqueness backed by database constraints.

## Assigned Agents and Role Boundaries

| Role | Responsibilities | Inputs | Outputs | Escalate when |
| --- | --- | --- | --- | --- |
| Slice coordinator | confirm endpoint route, transaction boundary, and prerequisite readiness | execution plan, current repo tree, reference-data slices | approved implementation sequence and blocker list | any prerequisite slice is incomplete or lacks integration proof |
| Backend/domain agent | implement command, validator, handler, endpoint, mappings, and persistence workflow | Shared Kernel, reference-data contracts, slice conventions | registration code path and response contract | qualification or extension rules require changing Shared Kernel behavior |
| Testing/verification agent | verify valid registration, duplicate empNr, invalid references, and extension conflicts | implemented slice and prerequisite data | integration-first tests and evidence | registration is not atomic or leaves partial data behind |

## Ordered Implementation Steps

1. Confirm prerequisite slices and final slice targets.
   Targets: src/features/Academics/RegisterAcademic/ or current equivalent, prerequisite endpoints/data, and persistence transaction boundary.
   Owner: Slice coordinator.
   Validation before next step: rank, degree, university, and extension reference data are available for test scenarios.
2. Implement the registration contract and validator.
   Targets: RegisterAcademic command, request/response types, validator, and mapping helpers.
   Owner: Backend/domain agent.
   Validation before next step: empNr length, EmpName length, qualification minimum, and extension availability are all validated before persistence.
3. Implement the handler and endpoint atomically.
   Targets: handler, endpoint, persistence mapping, and transaction flow for academic creation, qualification creation, and extension linkage.
   Owner: Backend/domain agent.
   Validation before next step: successful registration persists one academic with derived access level, at least one qualification, and one assigned extension.
4. Add integration-first verification.
   Targets: integration tests, validator tests, and any required fixtures.
   Owner: Testing/verification agent.
   Validation before next step: duplicate empNr, invalid rank, missing qualification, and unavailable extension cases all fail cleanly without partial writes.

## Verification and Acceptance Criteria

- A valid registration request creates one academic record with a 6-character unique empNr and a name no longer than 15 characters.
- Registration rejects payloads that do not include at least one degree and university pair.
- Registration rejects invalid rank, degree, university, or extension references.
- Registration uses an unassigned extension only and persists the rank-derived access level automatically.
- Automated tests cover the happy path plus duplicate empNr, invalid reference data, missing qualification, and extension-conflict failures.

## Human Showcase Steps

1. Starting state: reference-data slices are populated and at least one extension is available.
   Action: submit a valid register-academic request to the chosen academics route, preserving the repo's existing route prefix if one already exists.
   Expected result: the API returns success and the created academic can be retrieved immediately by its identifier or list endpoint.
   Value demonstrated: the system can now create a complete academic record that the rest of the lifecycle depends on.
2. Starting state: one academic already exists with an empNr and assigned extension.
   Action: resubmit the same empNr or attempt to register using the already assigned extension.
   Expected result: validation or conflict responses are returned and no second academic is created.
   Value demonstrated: registration protects key identity and extension invariants before the backlog expands.

## Completion Checklist

- [ ] RegisterAcademic remains the first delivery gate for academic lifecycle work.
- [ ] Validation covers empNr length, name length, qualification minimum, and reference-data existence.
- [ ] Persistence is atomic across academic, qualifications, and extension linkage.
- [ ] Derived access level is persisted or exposed consistently from Rank.
- [ ] Integration tests prove clean failure behavior for invalid and conflicting requests.
- [ ] Dependent slices are blocked until registration verification passes.