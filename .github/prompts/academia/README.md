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
---

# Academia Slice Implementation Prompts

Backend-first implementation prompts for every slice listed in `.github/models/workflows/academia-implementation-plan.md`.

## Common Constraints

- Current repository state: no `src/` or `tests/` scaffold exists yet.
- Custom agents available: `.github/agents/product-manager.agent.md`, `.github/agents/backend-slice-implementer.agent.md`, `.github/agents/slice-verifier.agent.md`.
- Recommended rollout order: see `execution-plan.md` in this folder.

## Execution Support

- `execution-plan.md`

## Reference Data

- `manage-ranks-implementation.prompt.md`
- `manage-degrees-implementation.prompt.md`
- `manage-universities-implementation.prompt.md`
- `provision-extension-implementation.prompt.md`

## Core Academic

- `register-academic-implementation.prompt.md`
- `view-academic-profile-implementation.prompt.md`
- `update-academic-name-implementation.prompt.md`
- `search-list-academics-implementation.prompt.md`
- `deregister-academic-implementation.prompt.md`

## Employment And Rank

- `grant-tenure-implementation.prompt.md`
- `assign-contract-implementation.prompt.md`
- `renew-contract-implementation.prompt.md`
- `convert-contract-to-tenure-implementation.prompt.md`
- `remove-employment-status-implementation.prompt.md`
- `change-rank-implementation.prompt.md`

## Qualifications

- `record-degree-obtained-implementation.prompt.md`
- `update-degree-university-implementation.prompt.md`
- `remove-degree-record-implementation.prompt.md`
- `list-qualifications-implementation.prompt.md`

## Extensions

- `assign-extension-implementation.prompt.md`
- `reassign-extension-implementation.prompt.md`
- `release-extension-implementation.prompt.md`
- `list-available-extensions-implementation.prompt.md`

## Reports

- `academic-directory-implementation.prompt.md`
- `by-rank-report-implementation.prompt.md`
- `by-access-level-report-implementation.prompt.md`
- `tenured-academics-report-implementation.prompt.md`
- `contracted-academics-report-implementation.prompt.md`
- `expiring-contracts-report-implementation.prompt.md`
- `qualification-reports-implementation.prompt.md`
- `access-level-distribution-report-implementation.prompt.md`
