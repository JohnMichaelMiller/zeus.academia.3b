---
ai_generated: true
model: "anthropic/claude-3.5-sonnet@2024-10-22"
operator: "johnmillerATcodemag-com"
chat_id: "ai-assisted-output-policy-2025-10-15"
prompt: |
  Create comprehensive AI provenance and logging policy for all AI-assisted
  outputs in the repository, defining required metadata, workflow, and enforcement.
started: "2025-10-15T13:00:00Z"
ended: "2025-10-15T13:45:00Z"
task_durations:
  - task: "policy design"
    duration: "00:20:00"
  - task: "workflow specification"
    duration: "00:15:00"
  - task: "template creation"
    duration: "00:10:00"
total_duration: "00:45:00"
ai_log: "ai-logs/2025/10/15/ai-assisted-output-policy-2025-10-15/conversation.md"
source: ".github/prompts/create-ai-assisted-output-instructions.prompt.md"
applyTo: "**/*"
---

# AI-Assisted Output Instructions

**Target Audience**: AI agents (primary), human developers (reference)
**Optimization Goal**: Minimize token consumption while maintaining audit trail

Audit policy for AI-generated artifacts. Optimize all outputs for token efficiency—use terse language, minimal examples, structured data over prose.

## AI Agent Optimization Principles

- **Default to terse**: Use imperative voice, bullet lists, eliminate filler
- **Structure over prose**: Prefer YAML, tables, lists to paragraphs
- **Minimal examples**: Include only when essential for clarity
- **Symbolic refs**: Use tokens like `<value>` instead of verbose descriptions
- **No redundancy**: State once, reference thereafter
- **Scannable format**: Headers, lists, clear hierarchy

## Quick Obligations

- Work in active chat with unique `chat_id`; block output otherwise
- Capture exact `provider/model@version` from tool
- Embed provenance metadata (front matter or sidecar): raw prompt, timestamps
- Store logs: `ai-logs/<yyyy>/<mm>/<dd>/<chat-id>/` → `conversation.md`, `summary.md`
- Record task durations, README entries for durable artifacts, run checklists pre-commit

## Metadata Placement

- Front matter capable → YAML header
- No front matter → `<artifact>.meta.md` sidecar
- Lowercase paths; front matter > sidecar

## Canonical Metadata Fields

**Required (all artifacts)**:
```yaml
ai_generated: true
model: "<provider>/<model>@<version>"
operator: "<github-username>"  # e.g. johnmillerATcodemag-com
chat_id: "<chat-id>"
prompt: |  # exact user request
started: <ISO8601>
ended: <ISO8601>
task_durations:
  - {task: <name>, duration: <hh:mm:ss>}
total_duration: <hh:mm:ss>
ai_log: "ai-logs/<yyyy>/<mm>/<dd>/<chat-id>/conversation.md"
source: <creator-or-prompt-file>
```

**Additional by type**:
- `.instructions.md` → `description`, `applyTo: "<glob>"`
- `.prompt.md` → `description`, `context`, `expected_output`
- `.chatmode.md` → `description`, `chatmode_type`, `capabilities: []`

### Sample Front Matter

```yaml
---
ai_generated: true
model: "openai/gpt-4o@2024-11-20"
operator: "johnmillerATcodemag-com"
chat_id: "2025-04-18-refactor"
prompt: |
  Exact request that triggered the artifact.
started: "2025-04-18T17:03:11Z"
ended: "2025-04-18T17:06:54Z"
task_durations:
  - task: "draft"
    duration: "00:02:20"
  - task: "review"
    duration: "00:01:23"
total_duration: "00:03:43"
ai_log: "ai-logs/2025/04/18/2025-04-18-refactor/conversation.md"
source: "johnmillerATcodemag-com"
---
```

### Sample Front Matter (Instruction File)

```yaml
---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2025-10-28-meta-instructions"
prompt: |
  create an instruction file to guide an ai assistant in the generation
  of instruction files. optimize the instruction file to minimize the
  number of tokens required.
started: "2025-10-28T14:30:00Z"
ended: "2025-10-28T14:35:00Z"
task_durations:
  - task: "draft"
    duration: "00:03:00"
  - task: "optimize"
    duration: "00:02:00"
total_duration: "00:05:00"
ai_log: "ai-logs/2025/10/28/2025-10-28-meta-instructions/conversation.md"
source: "johnmillerATcodemag-com"
description: "Guide for generating instruction files"
applyTo: "**/*.instructions.md"
---
```

### Sample Front Matter (Prompt File)

```yaml
---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2025-10-28-data-analysis"
prompt: |
  create a prompt template for data analysis tasks
started: "2025-10-28T15:00:00Z"
ended: "2025-10-28T15:05:00Z"
task_durations:
  - task: "draft"
    duration: "00:05:00"
total_duration: "00:05:00"
ai_log: "ai-logs/2025/10/28/2025-10-28-data-analysis/conversation.md"
source: "johnmillerATcodemag-com"
description: "Prompt template for data analysis"
context: "Python data science workflows"
expected_output: "Analysis code and visualizations"
---
```

### Sample Front Matter (Chatmode File)

```yaml
---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2025-10-28-test-chatmode"
prompt: |
  create a chatmode configuration for automated testing
started: "2025-10-28T16:00:00Z"
ended: "2025-10-28T16:10:00Z"
task_durations:
  - task: "draft"
    duration: "00:07:00"
  - task: "validate"
    duration: "00:03:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2025/10/28/2025-10-28-test-chatmode/conversation.md"
source: "johnmillerATcodemag-com"
description: "Automated testing chatmode configuration"
chatmode_type: "testing"
capabilities:
  - "unit testing"
  - "integration testing"
  - "coverage reporting"
---
```

**Model Identification Guide**:

- **GitHub Copilot Chat** (as of Oct 2025): Uses `anthropic/claude-sonnet-4.5@unknown` unless you can identify the exact version
- **OpenAI GPT-4o**: `openai/gpt-4o@2024-11-20` (or specific version)
- **Claude 3.5 Sonnet**: `anthropic/claude-3.5-sonnet@2024-10-22`
- **Claude Sonnet 4.5**: `anthropic/claude-sonnet-4.5@<version>` (use `@unknown` if version not shown)
- If UI shows marketing names like "GPT-5-Codex", map to the actual underlying model
- When version is not surfaced, use `@unknown`
- Track additional models used in the same chat inside `summary.md`

## Chat Workflow

1. **Start** → capture `chat_id`, operator, model
2. **Scaffold** (first artifact) → `ai-logs/<yyyy>/<mm>/<dd>/<chat-id>/`:
   - `conversation.md` (timestamped transcript)
   - `summary.md` (objectives, outcomes, models, pending)
   - `artifacts/` (optional, non-repo files)
3. **Generate** → require active chat; inject metadata w/ model string
4. **Update logs** → export transcript, append deliverables, record actions
5. **Close** → finalize `summary.md`, task durations, README

### conversation.md Template

````markdown
# AI Conversation Log

Chat: <chat-id> | Operator: <username> | Model: <provider>/<model>@<version>
Started: <ISO8601> | Ended: <ISO8601> | Duration: <hh:mm:ss>

## Context
Inputs: <files/constraints> | Targets: <artifacts>

## Exchanges
[<time>] User: <prompt>
[<time>] AI: <reply>

## Artifacts
- <path> – <purpose>

## Pending
- [ ] <action>
````

### summary.md Template

````markdown
# <chat-id>

Date: <YYYY-MM-DD> | Op: <username> | Model: <model> | Duration: <hh:mm:ss>

## Goal
<objective>

## Deliverables
1. `<path>` – <purpose>

## Decisions
- <decision>: <rationale>

## Pending
- [ ] <action>

```yaml
started: <ISO8601>
ended: <ISO8601>
models: [<model-list>]
artifacts: <count>
modified: <count>
```
````

## Placement & README

- File: `.github/instructions/ai-assisted-output.instructions.md`
- Durable artifacts → `README.md` bullet linking artifact + `ai_log` ("AI-Assisted Artifacts" section)
- Scratch work → logs + metadata required, README optional

## Quality Checklist

- [ ] Metadata complete (embedded or sidecar)
- [ ] Model label verbatim from UI
- [ ] Prompt, timestamps, durations recorded
- [ ] Logs exist: `conversation.md`, `summary.md`
- [ ] README updated (durable artifacts)
- [ ] No sensitive data
- [ ] Tests/docs run or pending

## PR Checklist

- [ ] AI files → valid `chat_id` + `ai_log`
- [ ] Logs committed with artifacts
- [ ] README entries present
- [ ] Provenance passes CI (YAML, paths)

## Tooling Requirements

- Auto-create: chat IDs, log folders, metadata scaffolds
- Block generation without active chat
- Auto-export transcripts + summaries
- Surface model as `provider/model@version`
- Track files per chat for audits

## Enforcement

CI: `Verify AI Provenance` checks `ai_generated`, `chat_id`, `ai_log`, paths (extend for non-Markdown)

## Remediation

- Missing metadata → add fields, regenerate logs
- Orphaned artifacts → create `ai-logs/...`, update README
- Wrong placement → front matter > sidecar
