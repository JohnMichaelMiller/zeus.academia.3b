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
name: implement-academia-ep-3-7-change-rank
description: Implement the ChangeRank command slice with automatic access-level recalculation
author: John Miller
tags: [academia, implementation, academics, ranks, command]
context: "Zeus Academia Phase 3 rank maintenance implementation"
expected_output: "A slice-scoped implementation plan for ChangeRank"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ChangeRank

## Slice Summary and Business Value

- Slice: ChangeRank
- Business outcome: update an academic's rank while recalculating derived access level immediately.
- Out of scope: analytical reports that consume the changed rank later.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- Shared Kernel rank and access-level code
- ManageRanks and RegisterAcademic slice files
- .github/instructions/fluentvalidation-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: RegisterAcademic, ManageRanks
- Blocking risks: access-level derivation must remain centralized in the aggregate or shared domain code. If academics still persist raw rank codes before full rank-reference FK wiring exists, this slice must preserve the explicit read-path failure contract rather than treating corrupt stored values as ordinary business validation.
- Existing patterns to reuse: valid-rank lookup, aggregate mutation, and event publication when needed for downstream reports.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                                                 | Inputs                                              | Outputs                   | Escalate when                                                              |
| -------------------- | -------------------------------------------------------------------------------- | --------------------------------------------------- | ------------------------- | -------------------------------------------------------------------------- |
| slice-coordinator    | confirm event needs and route placement                                          | execution plan, report dependencies, current routes | approved command contract | report dependencies require a broader event model than currently available |
| backend-domain       | implement command, validator, handler, endpoint, and any rank-change event       | Shared Kernel rules, ManageRanks data               | rank-change code path     | handler would need to assign access level directly rather than deriving it |
| testing-verification | verify valid rank change, invalid rank rejection, and access-level recalculation | implemented slice                                   | tests and evidence        | profile or report seed data still shows stale access levels                |

## Ordered Implementation Steps

1. Confirm route, identifier, and event expectations.
   Targets: src/features/Academics/ChangeRank/ or equivalent and any domain-event registration.
   Owner: slice-coordinator.
   Validation before next step: accepted rank source, downstream notification behavior, and the current read-path policy for invalid persisted rank values are explicit.
2. Implement change-rank behavior.
   Targets: command, validator, handler, endpoint, mappings, and optional RankChanged event.
   Owner: backend-domain.
   Validation before next step: only valid rank codes are accepted and access level is recalculated from the aggregate rule.
3. Verify read-model consistency.
   Targets: tests for valid change, invalid rank, missing academic, and profile/read-model assertions.
   Owner: testing-verification.
   Validation before next step: changed rank and derived access level are immediately visible.

## Verification and Acceptance Criteria

- Only valid rank codes from ManageRanks can be applied.
- Successful rank changes update the academic's effective access level automatically.
- Missing academics or invalid rank codes fail cleanly.
- If corrupted persisted rank values are encountered on read after a change, the failure is treated as a persistence or data-corruption signal rather than normal command validation.
- Automated tests prove the P, SL, and L mapping still yields INT, NAT, and LOC after changes.

## Human Showcase Steps

1. Starting state: a registered academic exists and rank data is available.
   Action: submit a rank-change request to another valid rank.
   Expected result: the command succeeds and a follow-up read shows the updated rank and derived access level.
   Value demonstrated: access permissions tied to rank stay consistent without manual recalculation.
2. Starting state: the same academic exists.
   Action: submit a request with an invalid rank code.
   Expected result: validation fails and the current rank remains unchanged.
   Value demonstrated: the system protects the canonical rank catalog.

## Completion Checklist

- [ ] Valid-rank lookup is enforced.
- [ ] AccessLevel remains derived, not manually assigned.
- [ ] Missing-record and invalid-rank cases are tested.
- [ ] Any existing read-path contract for invalid persisted rank values remains explicit and verified.
- [ ] Read models reflect the updated rank and access level.
- [ ] Verification evidence exists for the slice's acceptance criteria.
- [ ] Any required event publication is documented or implemented.
