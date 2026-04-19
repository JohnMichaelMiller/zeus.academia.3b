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
name: implement-by-rank-report
description: Guide delivery of the ByRankReport slice for count and list reporting by rank.
author: John Miller
tags: [implementation, vertical-slice, academia, reporting, rank]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ByRankReport slice with aggregation query, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ByRankReport

## Objective

Deliver workflows 7.2 and 3.4 so management can view counts and listings grouped by rank.

## Slice Boundary

- In scope: aggregate by rank, include list members and derived AccessLevel.
- Non-goals: access-level aggregation handled by a separate report slice.
- Dependencies: RegisterAcademic.
- Entry points: `src/backend/Features/Reports/Queries/ByRankReport/**`, `/api/reports/academics/by-rank`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Reporting slices should use dedicated read-optimized queries rather than aggregate rehydration.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                   | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------ | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm grouping shape and ordering              | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement rank report query, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate counts and member grouping              | Implemented slice                       | Verification notes |

## Acceptance Criteria

- Given academics exist across multiple ranks, when the report is queried, then the response contains counts and member lists per rank.
- Given a rank change occurs, when the report is queried afterward, then the counts and member placement reflect the new rank.
- Given no academics exist for a rank, when the report is queried, then the output handles the missing group without error.

## Verification Plan

- Automated: backend build plus by-rank report tests.
- Manual: inspect grouped results before and after changing an academic rank.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with academics across several ranks.
2. Call `GET /api/reports/academics/by-rank`.
   Expected: each rank includes a count and member list with derived AccessLevel.
3. Change one academic's rank and rerun the report.
   Expected: the academic moves groups and counts update.

Value demonstrated: management can understand staffing distribution by academic rank in one query.

## Output Artifacts

- By-rank report query, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ByRankReport.
- [ ] Acceptance criteria cover grouping and post-change accuracy.
- [ ] Showcase proves reporting value for rank distribution.
