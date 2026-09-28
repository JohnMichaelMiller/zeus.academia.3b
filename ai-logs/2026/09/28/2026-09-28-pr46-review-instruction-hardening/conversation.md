---
ai_generated: true
model: "anthropic/claude-opus-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-09-28-pr46-review-instruction-hardening"
prompt: |
  I want to improve the instruction files and prompts to generate better code that won't be flagged by the
  Copilot code review. review the review comments on pull request 46. propose improvements to the
  instruction files and prompts that will improve the quality of the generated code.
started: "2026-09-28T22:25:00Z"
ended: "2026-09-28T22:55:00Z"
task_durations:
  - task: "review PR 46 comments and map to gaps"
    duration: "00:08:00"
  - task: "tighten instruction files and agents"
    duration: "00:08:00"
  - task: "author eng/verify-slice.ps1 and dry-run"
    duration: "00:06:00"
  - task: "regenerate ep-2-1 prompt"
    duration: "00:06:00"
  - task: "provenance and README"
    duration: "00:02:00"
total_duration: "00:30:00"
ai_log: "ai-logs/2026/09/28/2026-09-28-pr46-review-instruction-hardening/conversation.md"
source: "johnmillerATcodemag-com"
---

# AI Conversation Log

- Chat ID: 2026-09-28-pr46-review-instruction-hardening
- Operator: johnmillerATcodemag-com
- Model: anthropic/claude-opus-4.5@unknown (GitHub Copilot agent mode)
- Started: 2026-09-28T22:25:00Z (approximate)
- Ended: 2026-09-28T22:55:00Z
- Total Duration: 00:30:00

## Context

- Inputs: 17 Copilot review comments on PR 46 (`gh api repos/JohnMichaelMiller/zeus.academia.3b/pulls/46/comments`), `.github/instructions/*`, `.github/agents/*`, `.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md`, ORM model, migration ownership matrix.
- Targets: instruction files, agents, code-review skill, ep-2-1 prompt, new `eng/verify-slice.ps1`.
- Constraints/Policies: `.github/instructions/ai-assisted-output.instructions.md`, `.github/instructions/ai-dev-process.instructions.md`.

## Exchanges

### Exchange 1

[2026-09-28T22:25:00Z] johnmillerATcodemag-com

```text
I want to improve the instruction files and prompts to generate better code that won't be flagged by the
Copilot code review. review the review comments on pull request 46. propose improvements to the
instruction files and prompts that will improve the quality of the generated code.
```

[2026-09-28T22:33:00Z] anthropic/claude-opus-4.5@unknown

```text
Most findings violate rules that already exist; root causes are (1) prose slice prompts without a precise
contract, (2) permissive or missing rules (DbContext "may own", format vs. existence, invariants only in
validators, LocalDB in appsettings.json, host ProjectReference, route tests only for validation), and
(3) no mechanical enforcement. Proposed: rule fixes per file, a Contract Sheet section and traceability gate
for prompts, eng/verify-slice.ps1, and a canonical-owner table to stop wording drift.
```

### Exchange 2

[2026-09-28T22:34:00Z] johnmillerATcodemag-com

```text
go ahead
```

[2026-09-28T22:55:00Z] anthropic/claude-opus-4.5@unknown

```text
Applied rule changes to vertical-slice, csharp, fluentvalidation, aspnetcore, mediatr, xunit, ai-dev-process,
ai-assisted-output, implementation-prompt(-generation); updated six agents and the code-review skill; created
eng/verify-slice.ps1 (dry-run detects 7 of the PR 46 findings on RegisterAcademic plus the Try*/route-test
findings on ManageUniversities and ProvisionExtension); regenerated the ep-2-1 prompt with a Contract Sheet.
```

## Work Burst Closure

**Artifacts Produced**:

- `eng/verify-slice.ps1` - mechanical pre-handoff slice checks
- `.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md` - regenerated with Contract Sheet
- Instruction, agent, and skill updates listed in summary.md

**Next Steps**:

- [ ] Re-run RegisterAcademic with the regenerated prompt on PR 46's branch
- [ ] Address PR 46 carry-overs listed in the prompt's Out of scope section
- [ ] Add `verify-slice.ps1` to CI per changed feature

**Duration Summary**:

- review and mapping: 00:08:00
- instruction and agent edits: 00:08:00
- script: 00:06:00
- prompt regeneration: 00:06:00
- provenance: 00:02:00
- Total: 00:30:00
