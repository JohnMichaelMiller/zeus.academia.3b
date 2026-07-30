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
name: implement-academia-ep-4-1-renew-contract
description: Implement the RenewContract command slice
author: John Miller
tags: [academia, implementation, employment, command]
context: "Zeus Academia Phase 4 contract lifecycle implementation"
expected_output: "A slice-scoped implementation plan for RenewContract"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RenewContract

## Slice Summary and Business Value

- Slice: RenewContract
- Business outcome: extend an existing contract with a new future end date.
- Out of scope: initial contract assignment or conversion to tenure.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- AssignContract slice files
- Shared Kernel employment rules

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.
- .github/instructions/xunit-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: AssignContract
- Blocking risks: renewals must fail for academics who are not currently contracted.
- Existing patterns to reuse: future-date validation, aggregate employment guards, and profile verification.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                      | Inputs                                           | Outputs                   | Escalate when                                                                   |
| -------------------------- | ----------------------------------------------------- | ------------------------------------------------ | ------------------------- | ------------------------------------------------------------------------------- |
| slice-coordinator          | confirm route and current contract-state precondition | execution plan and employment slices             | approved command contract | current employment state model cannot distinguish contracted academics reliably |
| backend-domain       | implement renew command, validator, handler, endpoint | AssignContract semantics and Shared Kernel rules | renewal code path         | renewal requires a broader employment redesign                                  |
| testing-verification | verify contracted-only and future-date behavior       | implemented slice                                | tests and evidence        | a renewal succeeds without an existing contract                                 |

## Ordered Implementation Steps

1. Confirm renewal preconditions and route.
   Targets: src/features/Employment/RenewContract/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: current contracted-state detection is explicit.
2. Implement renewal behavior.
   Targets: command, validator, handler, endpoint.
   Owner: backend-domain.
   Validation before next step: only currently contracted academics can receive a new future end date.
3. Verify renewal behavior.
   Targets: tests for valid renewal, missing contract, invalid date, and read-model visibility.
   Owner: testing-verification.
   Validation before next step: contract data updates predictably.

## Verification and Acceptance Criteria

### Review-Prevention Guardrails

- Dependency compatibility is validated for coupled tooling packages when touched (for example xUnit core and runner major versions align).
- Result-style failure factories guard non-null failure payloads in both generic and non-generic wrappers when touched.
- Value-object parse/create APIs reject lossy coercion unless explicitly required and covered by tests.
- Integration tests that provision external resources include deterministic best-effort cleanup in `finally` blocks.
- Renewal succeeds only for currently contracted academics.
- The new contract end date must be in the future.
- Invalid renewals leave prior persisted state unchanged.
- Read models and report seed data reflect the updated end date.

## Human Showcase Steps

1. Starting state: a contracted academic exists.
   Action: submit a renewal with a later future date.
   Expected result: the contract end date is replaced successfully.
   Value demonstrated: contract lifecycle maintenance is supported without reassigning the whole employment state.
2. Starting state: a tenured or unassigned academic exists.
   Action: submit the same renewal command.
   Expected result: the request fails cleanly.
   Value demonstrated: the slice protects contract-only transitions.

## Completion Checklist

- [ ] Review-prevention guardrails were evaluated and marked N/A where not applicable.
- [ ] If test packages changed, compatibility is verified (for example xUnit core and runner major versions align).
- [ ] If value-object parsing or creation changed, lossy coercion is rejected unless explicitly required and tested.
- [ ] If integration tests create external resources, teardown is enforced with best-effort `finally` cleanup.
- [ ] Contracted-only precondition is enforced.
- [ ] Future-date validation is reused consistently.
- [ ] Read models reflect the renewed end date.
- [ ] Failure paths are tested.
- [ ] The slice remains separate from initial assignment and tenure conversion.
