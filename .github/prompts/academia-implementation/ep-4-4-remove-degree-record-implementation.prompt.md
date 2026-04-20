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
name: implement-academia-ep-4-4-remove-degree-record
description: Implement the RemoveDegreeRecord command slice
author: John Miller
tags: [academia, implementation, qualifications, command]
context: "Zeus Academia Phase 4 qualification maintenance implementation"
expected_output: "A slice-scoped implementation plan for RemoveDegreeRecord"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RemoveDegreeRecord

## Slice Summary and Business Value

- Slice: RemoveDegreeRecord
- Business outcome: remove an existing qualification while preserving the rule that every academic retains at least one qualification.
- Out of scope: deregistration or bulk qualification cleanup.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel qualification rules
- RecordDegreeObtained slice files

## Prerequisites and Dependency Checks

- Required prior slices: RecordDegreeObtained
- Blocking risks: removing the last qualification would violate a core academic rule.
- Existing patterns to reuse: qualification identity lookup and invariant-preserving aggregate methods.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                        | Inputs                                        | Outputs                   | Escalate when                                                                          |
| -------------------------- | ------------------------------------------------------- | --------------------------------------------- | ------------------------- | -------------------------------------------------------------------------------------- |
| slice-coordinator          | confirm removal route and qualification target strategy | execution plan and qualification model        | approved command contract | qualification identity is ambiguous                                                    |
| backend-domain       | implement remove command, handler, endpoint             | qualification rules and current storage model | removal code path         | the minimum-one-qualification rule cannot be enforced cleanly in the current aggregate |
| testing-verification | verify safe removal and last-record rejection           | implemented slice                             | tests and evidence        | the command can remove the final qualification                                         |

## Ordered Implementation Steps

1. Confirm qualification targeting and removal semantics.
   Targets: src/features/Qualifications/RemoveDegreeRecord/ or equivalent.
   Owner: slice-coordinator.
   Validation before next step: success, missing-record, and last-record behaviors are explicit.
2. Implement removal behavior.
   Targets: command, handler, endpoint.
   Owner: backend-domain.
   Validation before next step: an academic with multiple qualifications can remove one while the last remaining qualification is protected.
3. Verify invariant preservation.
   Targets: tests for valid removal, missing qualification, and last-qualification rejection.
   Owner: testing-verification.
   Validation before next step: qualification reads remain consistent after valid removal.

## Verification and Acceptance Criteria

- Removing a non-final qualification succeeds.
- Attempting to remove the last remaining qualification fails.
- Missing qualification targets fail cleanly.
- Qualification queries remain consistent after successful removal.

## Human Showcase Steps

1. Starting state: an academic has at least two qualifications.
   Action: remove one qualification.
   Expected result: the command succeeds and one qualification remains.
   Value demonstrated: the system supports cleanup without violating academic completeness rules.
2. Starting state: an academic has exactly one qualification.
   Action: attempt to remove it.
   Expected result: the request fails cleanly.
   Value demonstrated: the slice protects the minimum qualification invariant.

## Completion Checklist

- [ ] Last-qualification protection is enforced.
- [ ] Valid removals keep the remaining set consistent.
- [ ] Missing-record behavior is tested.
- [ ] Qualification reads are verified after success.
- [ ] The slice does not overlap with deregistration behavior.
