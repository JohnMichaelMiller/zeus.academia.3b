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
name: implement-academia-ep-6-7-qualification-reports
description: Implement the grouped QualificationReports slice
author: John Miller
tags: [academia, implementation, reports, qualifications]
context: "Zeus Academia Phase 6 reporting implementation"
expected_output: "A slice-scoped implementation plan for QualificationReports"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement QualificationReports

## Slice Summary and Business Value

- Slice: QualificationReports
- Business outcome: provide grouped qualification reporting by degree and by university with counts and listings.
- Out of scope: raw operational qualification listing already covered by ListQualifications.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- RecordDegreeObtained, UpdateDegreeUniversity, RemoveDegreeRecord, and ListQualifications slice files

## Prerequisites and Dependency Checks

- Required prior slices: RecordDegreeObtained
- Blocking risks: grouped counts must stay in sync after add, update, and remove qualification flows.
- Existing patterns to reuse: grouped report contracts and read-optimized projections.

## Assigned Agents and Role Boundaries

| Role                       | Responsibilities                                                 | Inputs                                             | Outputs                        | Escalate when                                                                  |
| -------------------------- | ---------------------------------------------------------------- | -------------------------------------------------- | ------------------------------ | ------------------------------------------------------------------------------ |
| Slice coordinator          | confirm grouped report shapes and route strategy                 | execution plan and qualification query conventions | approved report contract       | degree-grouped and university-grouped outputs need different projection models |
| Backend/domain agent       | implement grouped qualification reports                          | qualification state and grouping rules             | qualification report code path | grouped reporting exposes unresolved duplication in qualification storage      |
| Testing/verification agent | verify counts and grouped listings after qualification mutations | implemented slice                                  | tests and evidence             | counts drift after add, update, or remove operations                           |

## Ordered Implementation Steps

1. Confirm grouped report contracts.
   Targets: src/features/Reports/QualificationReports/ or equivalent.
   Owner: Slice coordinator.
   Validation before next step: both degree-grouped and university-grouped outputs are explicit.
2. Implement grouped report queries.
   Targets: queries, handlers, DTOs, endpoints.
   Owner: Backend/domain agent.
   Validation before next step: counts and member listings align with current qualification state.
3. Verify grouped accuracy.
   Targets: tests for base counts plus add, update, and remove qualification effects.
   Owner: Testing/verification agent.
   Validation before next step: both grouped outputs stay synchronized with source slices.

## Verification and Acceptance Criteria

- Degree-grouped qualification output is accurate.
- University-grouped qualification output is accurate.
- Counts remain correct after qualification add, update, and remove flows.
- Automated tests verify grouped counts and listings.

## Human Showcase Steps

1. Starting state: qualification data exists across multiple academics.
   Action: call the grouped-by-degree and grouped-by-university reports.
   Expected result: both views return correct counts and member lists.
   Value demonstrated: stakeholders can analyze qualification distribution from two useful dimensions.
2. Starting state: modify one qualification and remove another in seeded data.
   Action: call both reports again.
   Expected result: grouped counts and lists update accordingly.
   Value demonstrated: reporting remains aligned with maintenance slices.

## Completion Checklist

- [ ] Degree and university groupings are both implemented.
- [ ] Grouped counts are accurate.
- [ ] Mutation effects are reflected.
- [ ] Tests cover grouped counts and listings.
- [ ] The slice remains analytical rather than operational listing.
