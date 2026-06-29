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
name: implement-academia-ep-4-2-convert-contract-to-tenure
description: Implement the ConvertContractToTenure command slice
author: John Miller
tags: [academia, implementation, employment, command]
context: "Zeus Academia Phase 4 contract lifecycle implementation"
expected_output: "A slice-scoped implementation plan for ConvertContractToTenure"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ConvertContractToTenure

## Slice Summary and Business Value

- Slice: ConvertContractToTenure
- Business outcome: promote a contracted academic to tenured state in one safe transition.
- Out of scope: direct tenure grant for academics with no contract.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- AssignContract and GrantTenure slice files
- Shared Kernel employment guard code

## Prerequisites and Dependency Checks

- Required prior slices: AssignContract
- Blocking risks: the conversion must only succeed from contracted state and must clear the contract date.
- Existing patterns to reuse: aggregate guard methods, persistence-backed employment invariants, employment read-model assertions, and transition tests.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                 | Inputs                                          | Outputs                   | Escalate when                                                       |
| -------------------- | ------------------------------------------------ | ----------------------------------------------- | ------------------------- | ------------------------------------------------------------------- |
| slice-coordinator    | confirm transition semantics and route           | execution plan and employment slices            | approved command contract | current model cannot distinguish conversion from direct tenure      |
| backend-domain       | implement conversion command, handler, endpoint  | Shared Kernel rules and prior employment slices | conversion code path      | conversion needs additional domain events not yet modeled           |
| testing-verification | verify contracted-only and post-conversion state | implemented slice                               | tests and evidence        | contract data survives the conversion or tenure state is incomplete |

## Ordered Implementation Steps

1. Confirm contracted-only conversion semantics.
   Targets: src/features/Employment/ConvertContractToTenure/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: handler behavior for non-contracted academics is explicit.
2. Implement conversion behavior.
   Targets: command, handler, endpoint, mappings.
   Owner: backend-domain.
   Validation before next step: success clears ContractEndDate and sets tenured state only, transition semantics are enforced before persistence, and any persistence-backed employment fields use explicit target-provider-compatible mappings.
3. Verify transition behavior.
   Targets: tests for valid conversion, invalid starting state, and follow-up reads, plus proof that the existing committed migration artifact still backs the XOR invariant or, if this slice changes schema, a new committed migration artifact in the confirmed persistence root.
   Owner: testing-verification.
   Validation before next step: XOR employment rule is preserved after the transition.

## Verification and Acceptance Criteria

- Conversion succeeds only from contracted state.
- Successful conversion clears the contract end date.
- Successful conversion leaves the academic tenured.
- Read models reflect tenured state only after success.
- The persisted employment state remains valid under the Shared Kernel XOR invariant after the transition.
- Persistence for employment-state fields remains target-provider-compatible and does not rely on implicit decimal, bool, or date defaults.

## Human Showcase Steps

1. Starting state: a contracted academic exists.
   Action: submit the convert-contract-to-tenure command.
   Expected result: the academic becomes tenured and contract date data is cleared.
   Value demonstrated: the system supports a clean employment promotion path.
2. Starting state: a non-contracted academic exists.
   Action: submit the same command.
   Expected result: the request fails cleanly.
   Value demonstrated: invalid employment transitions are blocked.

## Completion Checklist

- [ ] Contracted-only precondition is enforced.
- [ ] Employment state-transition logic checks XOR preconditions before committing field mutation.
- [ ] Contract date is cleared on success.
- [ ] Tenured state is visible after conversion.
- [ ] Failure paths are tested.
- [ ] Constraint-validation tests assert stable signals (exception type, constraint name, or SQL state), not provider-specific full error-message text.
- [ ] Domain transition rules and EF Core mappings for employment state stay aligned and explicit across layers.
- [ ] Target-provider mappings for employment-state storage are explicit enough to keep generated migrations valid.
- [ ] Verification ties the employment XOR invariant to the existing committed migration artifact or a new one when this slice changes schema.
- [ ] XOR employment rule remains intact.
