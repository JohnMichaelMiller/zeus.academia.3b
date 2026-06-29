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
name: implement-academia-ep-3-4-grant-tenure
description: Implement the GrantTenure employment-state command slice
author: John Miller
tags: [academia, implementation, employment, command]
context: "Zeus Academia Phase 3 employment-state implementation"
expected_output: "A slice-scoped implementation plan for GrantTenure"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement GrantTenure

## Slice Summary and Business Value

- Slice: GrantTenure
- Business outcome: move an academic into tenured state while preserving the employment XOR rule.
- Out of scope: contract renewals, contract-to-tenure conversion, and reports.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel employment guard code
- RegisterAcademic and ViewAcademicProfile slice files
- .github/instructions/fluentvalidation-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic
- Blocking risks: the command must reuse the aggregate guard rather than re-encode XOR logic in the handler only.
- Existing patterns to reuse: aggregate mutation methods, command validation, persistence-backed employment invariants, and follow-up profile assertions.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                             | Inputs                                  | Outputs                | Escalate when                                                         |
| -------------------- | ------------------------------------------------------------ | --------------------------------------- | ---------------------- | --------------------------------------------------------------------- |
| slice-coordinator    | confirm command route and response contract                  | execution plan, current routes          | approved command scope | employment logic appears in multiple conflicting places               |
| backend-domain       | implement command, handler, endpoint, and any event emission | Shared Kernel guards and academic model | tenure code path       | tenure transition requires changing the Shared Kernel invariant model |
| testing-verification | verify XOR preservation and read-model visibility            | implemented slice                       | tests and evidence     | contract state is not cleared or profile output is inconsistent       |

## Ordered Implementation Steps

1. Confirm the tenure transition contract.
   Targets: src/features/Employment/GrantTenure/ or equivalent and route placement.
   Owner: slice-coordinator.
   Validation before next step: success and missing-record behaviors are explicit.
2. Implement grant-tenure behavior.
   Targets: command, handler, endpoint, mappings, and any event contract.
   Owner: backend-domain.
   Validation before next step: applying tenure clears contract end date and leaves a valid employment state.
3. Verify employment invariants.
   Targets: unit or integration tests for tenured transition, missing academic, and prior contracted state, plus proof that the existing committed migration artifact still backs the XOR invariant or, if this slice changes schema, a new committed migration artifact in the confirmed persistence root.
   Owner: testing-verification.
   Validation before next step: XOR rule is preserved through the command path.

## Verification and Acceptance Criteria

- Granting tenure sets the academic to tenured state.
- Granting tenure clears any existing contract end date.
- Missing academics fail cleanly.
- The persisted employment state remains valid under the Shared Kernel XOR invariant after the transition.
- Automated tests prove the employment XOR rule still holds after the transition.

## Human Showcase Steps

1. Starting state: a registered academic exists, optionally with a contract end date in a controlled scenario.
   Action: submit the grant-tenure command.
   Expected result: the academic becomes tenured and any contract end date is cleared.
   Value demonstrated: the system can move academics into their permanent employment state safely.
2. Starting state: the same academic after the command.
   Action: read the profile.
   Expected result: the profile shows tenured state only.
   Value demonstrated: downstream consumers see a consistent employment picture.

## Completion Checklist

- [ ] Aggregate guard methods are reused.
- [ ] Employment state-transition logic checks XOR preconditions before committing field mutation.
- [ ] Contract state is cleared on tenure.
- [ ] Missing-record handling is verified.
- [ ] Profile visibility is tested.
- [ ] Constraint-validation tests assert stable signals (exception type, constraint name, or SQL state), not provider-specific full error-message text.
- [ ] Verification ties the employment XOR invariant to the existing committed migration artifact or a new one when this slice changes schema.
- [ ] XOR employment rule remains enforced.
