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
name: implement-tenured-academics-report
description: Guide delivery of the TenuredAcademicsReport slice for reporting on tenured academics.
author: John Miller
tags: [implementation, vertical-slice, academia, reporting, tenure]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed TenuredAcademicsReport slice with projection query, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement TenuredAcademicsReport

## Objective

Deliver workflows 7.4 and 2.7 so HR can view all tenured academics with rank and qualifications.

## Slice Boundary

- In scope: filter only tenured academics and project rank plus qualifications.
- Non-goals: contracted and expiring-contract reporting.
- Dependencies: GrantTenure.
- Entry points: `src/backend/Features/Reports/Queries/TenuredAcademicsReport/**`, `/api/reports/academics/tenured`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Reporting slices should use dedicated read-optimized queries rather than aggregate rehydration.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                               | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ------------------------------------------------------------ | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm report columns and sort order                        | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement tenured report query, endpoint, and tests          | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate tenured-only filtering and qualification projection | Implemented slice                       | Verification notes |

## Acceptance Criteria

- Given tenured academics exist, when the report is queried, then only tenured academics are returned with rank and qualification data.
- Given a contracted academic is converted to tenure, when the report is queried afterward, then that academic appears in the result.
- Given no tenured academics exist, when the report is queried, then the system returns an empty result without error.

## Verification Plan

- Automated: backend build plus tenured-report tests.
- Manual: run the report before and after granting tenure.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with a mix of tenured and non-tenured academics.
2. Call `GET /api/reports/academics/tenured`.
   Expected: only tenured academics are listed with rank and qualifications.
3. Grant tenure to another academic and rerun the report.
   Expected: the newly tenured academic appears.

Value demonstrated: HR can monitor permanent academic appointments with the context needed for administrative review.

## Output Artifacts

- Tenured report query, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only TenuredAcademicsReport.
- [ ] Acceptance criteria cover tenured-only filtering and qualification visibility.
- [ ] Showcase proves report responsiveness to employment changes.
