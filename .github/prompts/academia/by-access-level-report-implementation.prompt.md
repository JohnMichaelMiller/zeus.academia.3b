---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-04-18-academia-slice-implementation-prompts"
prompt: |
  create a implementation prompt for each slice in the #file:academia-implementation-plan.md
started: "2026-04-18T13:10:00-07:00"
ended: "2026-04-18T13:55:00-07:00"
task_durations:
  - task: "context analysis"
    duration: "00:10:00"
  - task: "prompt authoring"
    duration: "00:28:00"
  - task: "catalog and provenance updates"
    duration: "00:07:00"
total_duration: "00:45:00"
ai_log: "ai-logs/2026/04/18/2026-04-18-academia-slice-implementation-prompts/conversation.md"
source: "johnmillerATcodemag-com"
name: implement-by-access-level-report
description: Guide delivery of the ByAccessLevelReport slice for count and list reporting by derived access level.
author: John Miller
tags: [implementation, vertical-slice, academia, reporting, access-level]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ByAccessLevelReport slice with aggregation query, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ByAccessLevelReport

## Objective

Deliver workflows 7.3, 3.3, and 3.5 so management can view counts and listings grouped by derived AccessLevel.

## Slice Boundary

- In scope: aggregate by derived AccessLevel only; do not store AccessLevel directly.
- Non-goals: editing AccessLevel or rank reference data.
- Dependencies: ChangeRank.
- Entry points: `src/backend/Features/Reports/Queries/ByAccessLevelReport/**`, `/api/reports/academics/by-access-level`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Reporting slices should use dedicated read-optimized queries rather than aggregate rehydration.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                           | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | -------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm grouping contract and display labels             | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement access-level report query, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate derived grouping behavior                       | Implemented slice                       | Verification notes |

## Acceptance Criteria

- Given academics exist across derived AccessLevels, when the report is queried, then the response contains counts and member lists per AccessLevel.
- Given a rank change alters an academic's AccessLevel, when the report is queried afterward, then the academic appears in the new AccessLevel group.
- Given no academics exist for one AccessLevel, when the report is queried, then the output handles the missing group without error.

## Verification Plan

- Automated: backend build plus by-access-level report tests.
- Manual: inspect grouped results before and after changing an academic's rank.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with academics across different ranks.
2. Call `GET /api/reports/academics/by-access-level`.
   Expected: groups are reported by `INT`, `NAT`, and `LOC` based on current ranks.
3. Change one academic's rank and rerun the report.
   Expected: the academic moves to the new derived AccessLevel group.

Value demonstrated: access-based planning can rely on derived data instead of duplicated state.

## Output Artifacts

- By-access-level report query, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ByAccessLevelReport.
- [ ] Acceptance criteria cover derived grouping updates.
- [ ] Showcase proves the value of reporting on computed state.
