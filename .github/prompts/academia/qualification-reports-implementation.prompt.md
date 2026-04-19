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
name: implement-qualification-reports
description: Guide delivery of the QualificationReports slice for grouped qualification reporting by degree and by university.
author: John Miller
tags: [implementation, vertical-slice, academia, reporting, qualification]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed QualificationReports slice with grouped projection queries, endpoints, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement QualificationReports

## Objective

Deliver workflows 7.7 and 7.8 so management can view qualification groupings by degree and by university.

## Slice Boundary

- In scope: grouped report by degree with academic and university detail, plus grouped summary by university with counts.
- Non-goals: direct qualification list queries already covered by ListQualifications.
- Dependencies: RecordDegreeObtained.
- Entry points: `src/backend/Features/Reports/Queries/QualificationReports/**`, `/api/reports/qualifications`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Reporting slices should use dedicated read-optimized queries rather than aggregate rehydration.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                                                       | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------------------------------ | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm grouped report shapes and endpoints                                          | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement degree-grouped and university-grouped report queries, endpoints, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate grouping counts and member detail                                           | Implemented slice                       | Verification notes |

## Acceptance Criteria

- Given qualification data exists, when the report is queried by degree, then qualifications are grouped by degree with academic and university detail.
- Given qualification data exists, when the report is queried by university, then qualifications are grouped by university with counts.
- Given qualification data changes, when the report is queried afterward, then the groups and counts reflect the latest data.

## Verification Plan

- Automated: backend build plus qualification-report tests.
- Manual: inspect degree-grouped and university-grouped outputs after adding or updating qualifications.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with academics holding multiple qualifications across universities.
2. Call the degree-grouped qualification report endpoint.
   Expected: records are grouped by degree with academic and university detail.
3. Call the university-grouped summary endpoint.
   Expected: counts per university are returned.

Value demonstrated: management can inspect the institution's qualification mix from both credential and university perspectives.

## Output Artifacts

- Qualification-report queries, endpoints, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only QualificationReports.
- [ ] Acceptance criteria cover both grouped report modes.
- [ ] Showcase proves analytical value beyond simple list queries.
