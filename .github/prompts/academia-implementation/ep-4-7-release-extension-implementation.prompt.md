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
name: implement-academia-ep-4-7-release-extension
description: Implement the ReleaseExtension command slice
author: John Miller
tags: [academia, implementation, extensions, command]
context: "Zeus Academia Phase 4 extension lifecycle implementation"
expected_output: "A slice-scoped implementation plan for ReleaseExtension"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ReleaseExtension

## Slice Summary and Business Value

- Slice: ReleaseExtension
- Business outcome: return an assigned extension to the available pool without deleting the academic.
- Out of scope: reassignment or academic deregistration.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- AssignExtension slice files
- Shared Kernel extension rules

## Prerequisites and Dependency Checks

- Required prior slices: AssignExtension
- Blocking risks: release must update both academic state and extension availability consistently.
- Existing patterns to reuse: extension uniqueness checks, transactional updates, and follow-up availability verification.

## Assigned Agents and Role Boundaries

| Role | Responsibilities | Inputs | Outputs | Escalate when |
| --- | --- | --- | --- | --- |
| Slice coordinator | confirm route and released-state semantics | execution plan and extension slices | approved command contract | current model does not expose a clear released state |
| Backend/domain agent | implement release command, handler, endpoint | extension assignment model | release code path | release behavior would conflict with deregistration cleanup |
| Testing/verification agent | verify state cleanup and returned availability | implemented slice | tests and evidence | released extensions do not re-enter the available pool |

## Ordered Implementation Steps

1. Confirm release semantics.
   Targets: src/features/Extensions/ReleaseExtension/ or equivalent.
   Owner: Slice coordinator.
   Validation before next step: expected academic state after release is explicit.
2. Implement release behavior.
   Targets: command, handler, endpoint.
   Owner: Backend/domain agent.
   Validation before next step: academic no longer references the extension and the extension becomes available.
3. Verify release behavior.
   Targets: tests for valid release, no-current-extension cases, and follow-up availability checks.
   Owner: Testing/verification agent.
   Validation before next step: availability reads reflect the released extension.

## Verification and Acceptance Criteria

- Releasing a current extension succeeds and clears the academic-side reference.
- Released extensions become available for later assignment.
- Invalid release requests fail cleanly.
- Automated tests verify both academic and extension-pool state after success.

## Human Showcase Steps

1. Starting state: an academic currently holds an extension.
   Action: submit the release-extension command.
   Expected result: the academic no longer has an assigned extension and the extension returns to the pool.
   Value demonstrated: administrators can free extension inventory without removing the academic.
2. Starting state: the extension has been released.
   Action: query available extensions.
   Expected result: the released extension is visible as available.
   Value demonstrated: inventory state remains coherent across slices.

## Completion Checklist

- [ ] Academic state is cleared on release.
- [ ] Released extensions return to the available pool.
- [ ] Invalid release behavior is tested.
- [ ] Follow-up availability checks are verified.
- [ ] The slice remains distinct from reassignment and deregistration.