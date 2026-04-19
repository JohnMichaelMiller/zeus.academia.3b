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
name: implement-contracted-academics-report
description: Guide delivery of the ContractedAcademicsReport slice for listing academics with contract end dates.
author: John Miller
tags: [implementation, vertical-slice, academia, reporting, contract]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ContractedAcademicsReport slice with projection query, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ContractedAcademicsReport

## Objective

Deliver workflows 7.5 and 2.8 so HR can list contracted academics sorted by contract end date.

## Slice Boundary

- In scope: filter academics with a non-null contract end date and sort ascending.
- Non-goals: expiring threshold filtering handled by a separate report slice.
- Dependencies: AssignContract.
- Entry points: `src/backend/Features/Reports/Queries/ContractedAcademicsReport/**`, `/api/reports/academics/contracted`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Reporting slices should use dedicated read-optimized queries rather than aggregate rehydration.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                         | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------ | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm report columns and sort requirements           | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement contracted report query, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate contracted-only filtering and ascending sort  | Implemented slice                       | Verification notes |

## Acceptance Criteria

- Given contracted academics exist, when the report is queried, then only academics with a contract end date are returned.
- Given multiple contract dates exist, when the report is queried, then the rows are sorted ascending by contract end date.
- Given an academic is converted to tenure or has status cleared, when the report is queried afterward, then that academic disappears from the result.

## Verification Plan

- Automated: backend build plus contracted-report tests.
- Manual: inspect ordering before and after changing employment states.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with several contracted academics and varying end dates.
2. Call `GET /api/reports/academics/contracted`.
   Expected: only contracted academics are returned, sorted by end date ascending.
3. Convert one academic to tenure and rerun the report.
   Expected: that academic no longer appears.

Value demonstrated: HR can prioritize upcoming contract administration work from an ordered list.

## Output Artifacts

- Contracted report query, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ContractedAcademicsReport.
- [ ] Acceptance criteria cover contracted-only filtering and sort order.
- [ ] Showcase proves operational value for HR planning.
