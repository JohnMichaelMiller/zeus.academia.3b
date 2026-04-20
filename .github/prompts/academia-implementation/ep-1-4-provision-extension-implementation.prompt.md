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
name: implement-academia-ep-1-4-provision-extension
description: Implement the ProvisionExtension slice for extension pool provisioning and deprovisioning
author: John Miller
tags: [academia, implementation, extensions, reference-data]
context: "Zeus Academia Phase 1 extension provisioning implementation"
expected_output: "A slice-scoped implementation plan for ProvisionExtension"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ProvisionExtension

## Slice Summary and Business Value

- Slice: ProvisionExtension
- Business outcome: create and manage the available extension pool that registration and extension workflows consume.
- Out of scope: assigning extensions to academics, reassignments, releases, and reporting queries.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/models/workflows/academia-implementation-plan.md
- .github/instructions/vertical-slice-implementation.instructions.md
- .github/instructions/mediatr-implementation.instructions.md
- .github/instructions/fluentvalidation-implementation.instructions.md
- .github/instructions/cqrs-mediatr-efcore.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: Shared Kernel
- Blocking risks: extension uniqueness must be preserved for later assignment slices, so do not treat this as disposable seed data.
- Existing patterns to reuse: command-first slice structure, validator beside command, persistence uniqueness, and guard methods preventing invalid deprovisioning.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                        | Inputs                                           | Outputs                                        | Escalate when                                                     |
| -------------------------- | ------------------------------------------------------- | ------------------------------------------------ | ---------------------------------------------- | ----------------------------------------------------------------- |
| slice-coordinator          | confirm extension storage model and route placement     | execution plan, repo tree, persistence root      | approved artifact targets and dependency notes | extension state is already modeled elsewhere in a conflicting way |
| backend-domain       | implement provision and deprovision command behavior    | Shared Kernel Extension model, slice conventions | commands, validators, handlers, endpoints      | extNr formatting or identity semantics are unclear                |
| testing-verification | verify numeric format, uniqueness, and assignment guard | implemented slice and business rules             | tests and evidence                             | deprovision logic cannot reliably detect assigned extensions      |

## Ordered Implementation Steps

1. Confirm the extension-pool model and persistence root.
   Targets: src/features/Extensions/ProvisionExtension/ or current equivalent, entity configuration, and migration path.
   Owner: slice-coordinator.
   Validation before next step: extNr representation and provisioned-state storage are explicit.
2. Implement provision-extension behavior.
   Targets: provision command, validator, handler, response, endpoint, and mappings.
   Owner: backend-domain.
   Validation before next step: only valid numeric extensions are accepted and duplicates are rejected.
3. Implement deprovision-extension behavior.
   Targets: deprovision command, validator if needed, handler, response, and endpoint.
   Owner: backend-domain.
   Validation before next step: assigned extensions cannot be deprovisioned and unassigned ones can.
4. Verify command behavior end to end.
   Targets: validator tests, handler tests, integration tests for provision, duplicate rejection, and deprovision guards.
   Owner: testing-verification.
   Validation before next step: extension pool behavior is reliable enough for registration to depend on it.

## Verification and Acceptance Criteria

- Provisioning accepts only valid numeric extension values and persists a unique extension record.
- Provisioning the same extension twice fails without creating duplicates.
- Deprovisioning an unassigned extension succeeds and removes it from the available pool.
- Deprovisioning an assigned extension fails and preserves the existing assignment state.
- Automated tests cover valid provision, duplicate provision, valid deprovision, and assigned-extension rejection.

## Human Showcase Steps

1. Starting state: Shared Kernel exists and no extension pool has been created yet.
   Action: provision a set of extensions using the approved endpoint or admin path.
   Expected result: the extension pool becomes available for registration and later assignment slices.
   Value demonstrated: dependent slices can allocate real extension inventory instead of fake placeholders.
2. Starting state: at least one extension exists.
   Action: attempt to deprovision an unassigned extension, then attempt to deprovision one that is marked assigned in a test scenario.
   Expected result: the free extension is removed, while the assigned extension is protected.
   Value demonstrated: inventory management preserves the later 1:1 extension rule.

## Completion Checklist

- [ ] ProvisionExtension remains limited to extension-pool lifecycle behavior.
- [ ] Numeric extension validation is enforced.
- [ ] Extension uniqueness is preserved.
- [ ] Assigned extensions are protected from deprovisioning.
- [ ] Tests cover success and failure paths.
- [ ] The slice is safe for RegisterAcademic to consume next.
