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
name: implement-academia-ep-4-6-reassign-extension
description: Implement the ReassignExtension command slice
author: John Miller
tags: [academia, implementation, extensions, command]
context: "Zeus Academia Phase 4 extension lifecycle implementation"
expected_output: "A slice-scoped implementation plan for ReassignExtension"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ReassignExtension

## Slice Summary and Business Value

- Slice: ReassignExtension
- Business outcome: move an academic from one extension to another atomically while preserving uniqueness.
- Out of scope: initial assignment and pure release workflows.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- AssignExtension and ProvisionExtension slice files
- Shared Kernel extension uniqueness rules

- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.

## Prerequisites and Dependency Checks

- Required prior slices: AssignExtension
- Blocking risks: reassignment must not leave two academics with the same extension or leave the source extension in an inconsistent state.
- Existing patterns to reuse: extension lookups, transactional writes, and integration tests that verify rollback on failure.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                 | Inputs                                          | Outputs                   | Escalate when                                                         |
| -------------------------- | ------------------------------------------------ | ----------------------------------------------- | ------------------------- | --------------------------------------------------------------------- |
| slice-coordinator          | confirm transaction boundary and route           | execution plan and existing extension flows     | approved command contract | persistence cannot guarantee atomic transition with the current model |
| backend-domain       | implement reassign command, handler, endpoint    | extension assignment model and uniqueness rules | reassignment code path    | atomicity requires broader infrastructure changes                     |
| testing-verification | verify valid reassignments and rollback behavior | implemented slice                               | tests and evidence        | partial updates survive after a failed reassignment                   |

## Ordered Implementation Steps

1. Confirm reassignment semantics and transaction boundary.
   Targets: src/features/Extensions/ReassignExtension/ or equivalent and persistence transaction handling.
   Owner: slice-coordinator.
   Validation before next step: source release and target assignment happen in one atomic flow.
2. Implement reassignment behavior.
   Targets: command, handler, endpoint.
   Owner: backend-domain.
   Validation before next step: source extension is released only when the target assignment can succeed.
3. Verify atomic behavior.
   Targets: tests for valid reassign, invalid target, and rollback on failure.
   Owner: testing-verification.
   Validation before next step: 1:1 uniqueness remains intact after success and failure.

## Verification and Acceptance Criteria

### Review-Prevention Guardrails

- Dependency compatibility is validated for coupled tooling packages when touched (for example xUnit core and runner major versions align).
- Result-style failure factories guard non-null failure payloads in both generic and non-generic wrappers when touched.
- Value-object parse/create APIs reject lossy coercion unless explicitly required and covered by tests.
- Integration tests that provision external resources include deterministic best-effort cleanup in `finally` blocks.
- Reassigning to a valid free extension succeeds.
- Reassigning to an unavailable target extension fails cleanly.
- Failed reassignment attempts leave the original assignment intact.
- Automated tests prove atomicity and preserved uniqueness.

## Human Showcase Steps

1. Starting state: an academic has one assigned extension and another extension is free.
   Action: submit the reassign-extension command.
   Expected result: the academic now holds the new extension and the old one is released.
   Value demonstrated: extension changes can be handled without manual multi-step cleanup.
2. Starting state: the target extension is already assigned elsewhere.
   Action: submit the same command.
   Expected result: the command fails and the original assignment remains unchanged.
   Value demonstrated: the system protects assignment integrity under contention.

## Completion Checklist

- [ ] Review-prevention guardrails were evaluated and marked N/A where not applicable.
- [ ] If test packages changed, compatibility is verified (for example xUnit core and runner major versions align).
- [ ] If value-object parsing or creation changed, lossy coercion is rejected unless explicitly required and tested.
- [ ] If integration tests create external resources, teardown is enforced with best-effort `finally` cleanup.
- [ ] Reassignment is atomic.
- [ ] Original assignments survive failed attempts.
- [ ] Uniqueness remains intact.
- [ ] Rollback behavior is tested.
- [ ] The slice stays distinct from initial assignment and pure release.
