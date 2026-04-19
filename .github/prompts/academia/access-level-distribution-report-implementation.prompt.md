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
name: implement-access-level-distribution-report
description: Guide delivery of the AccessLevelDistributionReport slice for aggregate counts by derived access level.
author: John Miller
tags: [implementation, vertical-slice, academia, reporting, access-level]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed AccessLevelDistributionReport slice with aggregation query, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement AccessLevelDistributionReport

## Objective

Deliver workflow 7.9 so management can see the count of academics in each derived AccessLevel bucket.

## Slice Boundary

- In scope: aggregate count per `INT`, `NAT`, and `LOC` based on derived AccessLevel.
- Non-goals: per-academic member listings handled by ByAccessLevelReport.
- Dependencies: ChangeRank.
- Entry points: `src/backend/Features/Reports/Queries/AccessLevelDistributionReport/**`, `/api/reports/academics/access-level-distribution`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Reporting slices should use dedicated read-optimized queries rather than aggregate rehydration.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                 | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | -------------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm output contract for aggregated counts only             | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement access-level distribution query, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate count accuracy after rank changes                     | Implemented slice                       | Verification notes |

## Acceptance Criteria

- Given academics exist, when the report is queried, then the response returns counts for `INT`, `NAT`, and `LOC` based on derived AccessLevel.
- Given rank changes occur, when the report is queried afterward, then the counts reflect the new derived AccessLevel distribution.
- Given no academics exist, when the report is queried, then counts return as zero or the agreed empty representation.

## Verification Plan

- Automated: backend build plus access-level distribution tests.
- Manual: capture counts, change rank assignments, and rerun the report.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with academics across multiple ranks.
2. Call `GET /api/reports/academics/access-level-distribution`.
   Expected: counts are returned for each AccessLevel bucket.
3. Change one academic's rank and rerun the report.
   Expected: the counts update automatically through the derived mapping.

Value demonstrated: capacity planning can rely on a compact derived distribution report without duplicating AccessLevel state.

## Output Artifacts

- Access-level distribution query, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only AccessLevelDistributionReport.
- [ ] Acceptance criteria cover derived count accuracy.
- [ ] Showcase proves the planning value of the aggregate report.
