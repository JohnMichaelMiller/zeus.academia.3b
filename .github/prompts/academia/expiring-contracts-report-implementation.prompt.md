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
name: implement-expiring-contracts-report
description: Guide delivery of the ExpiringContractsReport slice for identifying contracts near expiration.
author: John Miller
tags: [implementation, vertical-slice, academia, reporting, contract]
context: "Zeus Academia backend-first slice delivery with vertical-slice boundaries and explicit agent handoffs"
expected_output: "A completed ExpiringContractsReport slice with threshold-based query, endpoint, tests, verification evidence, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ExpiringContractsReport

## Objective

Deliver workflows 7.6 and 2.6 so HR can identify contracts expiring within a configurable threshold.

## Slice Boundary

- In scope: filter contracted academics whose end date is within a configurable window, defaulting to 90 days.
- Non-goals: full contracted list reporting handled separately.
- Dependencies: AssignContract.
- Entry points: `src/backend/Features/Reports/Queries/ExpiringContractsReport/**`, `/api/reports/academics/contracts/expiring`.

## Required Context

- Review the plan, workflow catalogue, ORM file, and backend instruction files listed in `manage-ranks-implementation.prompt.md`.
- Reporting slices should use dedicated read-optimized queries rather than aggregate rehydration.
- Repo status: no `src/` or `tests/` scaffold exists yet.
- Execution support: use `.github/agents/backend-slice-implementer.agent.md` for implementation and `.github/agents/slice-verifier.agent.md` for verification, or run manually.

## Agent Plan

| Role                   | Agent                       | Responsibility                                              | Inputs                                  | Outputs            |
| ---------------------- | --------------------------- | ----------------------------------------------------------- | --------------------------------------- | ------------------ |
| Scope and acceptance   | `product-manager`           | Confirm threshold parameter contract and default window     | Plan and workflows                      | Approved scope     |
| Backend implementation | `backend-slice-implementer` | Implement threshold-based report query, endpoint, and tests | Approved scope and backend instructions | Code and tests     |
| Verification           | `slice-verifier`            | Validate default and custom-threshold behavior              | Implemented slice                       | Verification notes |

## Acceptance Criteria

- Given contracted academics with end dates inside and outside the default window, when the report is queried without a parameter, then only contracts expiring within 90 days are returned.
- Given a custom threshold is supplied, when the report is queried, then the filter uses that threshold instead of the default.
- Given no contracts fall within the threshold, when the report is queried, then the system returns an empty result without error.

## Verification Plan

- Automated: backend build plus expiring-contracts report tests.
- Manual: run the report with the default threshold and with a custom threshold.
- Evidence: API responses and test output.

## Showcase Steps

1. Start the API with contracted academics expiring at different future dates.
2. Call `GET /api/reports/academics/contracts/expiring`.
   Expected: only contracts expiring within 90 days are returned.
3. Call `GET /api/reports/academics/contracts/expiring?days=30`.
   Expected: the result narrows to the shorter threshold.

Value demonstrated: HR can identify renewal risk early enough to act before contracts lapse.

## Output Artifacts

- Expiring-contracts report query, endpoint, tests, and verification notes.

## Validation Checklist

- [ ] The prompt targets only ExpiringContractsReport.
- [ ] Acceptance criteria cover default and custom thresholds.
- [ ] Showcase proves practical contract-risk visibility.
