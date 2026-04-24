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
name: implement-academia-ep-5-1-deregister-academic
description: Implement the DeregisterAcademic command slice
author: John Miller
tags: [academia, implementation, academics, command, lifecycle]
context: "Zeus Academia Phase 5 lifecycle completion implementation"
expected_output: "A slice-scoped implementation plan for DeregisterAcademic"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement DeregisterAcademic

## Slice Summary and Business Value

- Slice: DeregisterAcademic
- Business outcome: close an academic's active lifecycle, release any assigned extension, and emit downstream event data while retaining required history.
- Out of scope: report implementation beyond the event and persisted state needed for reports to consume later.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- RegisterAcademic and ReleaseExtension slice files
- Shared Kernel domain-event contracts

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic, ReleaseExtension
- Blocking risks: deregistration must not orphan assigned extensions or destroy required qualification history.
- Existing patterns to reuse: lifecycle command handling, extension cleanup, and domain event dispatch.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                                     | Inputs                                               | Outputs                   | Escalate when                                                              |
| -------------------- | -------------------------------------------------------------------- | ---------------------------------------------------- | ------------------------- | -------------------------------------------------------------------------- |
| slice-coordinator    | confirm deregistration semantics, retention rules, and event shape   | execution plan and current lifecycle model           | approved command contract | retention rules are ambiguous or conflict with persistence behavior        |
| backend-domain       | implement command, handler, endpoint, cleanup, and event publication | academic model, extension lifecycle, event contracts | deregistration code path  | cleanup or retention requires a larger archival design                     |
| testing-verification | verify extension release, history retention, and event emission      | implemented slice                                    | tests and evidence        | deregistration leaves assigned extensions behind or loses required history |

## Ordered Implementation Steps

1. Confirm deregistration semantics and retention expectations.
   Targets: src/features/Academics/DeregisterAcademic/ or equivalent and domain-event wiring.
   Owner: slice-coordinator.
   Validation before next step: expected post-deregistration state and required event payload are explicit.
2. Implement deregistration behavior.
   Targets: command, handler, endpoint, cleanup logic, event publication.
   Owner: backend-domain.
   Validation before next step: assigned extensions are released and qualification history is retained according to the plan.
3. Verify lifecycle completion behavior.
   Targets: tests for success, missing academic, released extension state, and event emission.
   Owner: testing-verification.
   Validation before next step: report-producing consumers have the data they need.

## Verification and Acceptance Criteria

- Deregistration succeeds only for an existing academic.
- Assigned extensions are released as part of the flow.
- Required qualification history is retained.
- The command emits the required deregistration event for downstream consumers.

## Human Showcase Steps

1. Starting state: an academic exists with an assigned extension and qualification history.
   Action: submit the deregister-academic command.
   Expected result: the academic is removed or marked inactive per design, the extension is released, and the event is emitted.
   Value demonstrated: the academic lifecycle can be closed without corrupting dependent data.
2. Starting state: query the extension pool and retained academic history according to the implemented design.
   Action: inspect both after deregistration.
   Expected result: the extension is available again and required qualification history remains accessible.
   Value demonstrated: offboarding preserves inventory and auditability.

## Completion Checklist

- [ ] Existing-academic precondition is enforced.
- [ ] Assigned extensions are released.
- [ ] Retention behavior is explicit and tested.
- [ ] Event publication is verified.
- [ ] Verification evidence exists for the slice's acceptance criteria.
- [ ] The slice is ready for reporting work to build on stable lifecycle data.
