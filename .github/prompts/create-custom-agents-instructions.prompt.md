---
ai_generated: true
model: "openai/gpt-5.3-codex@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-25-custom-agents-instructions-prompt"
prompt: |
  using (https://docs.github.com/en/copilot/reference/custom-agents-configuration), https://docs.github.com/en/copilot/concepts/agents/coding-agent/about-custom-agents, https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-custom-agents
   create a prompt that will create an instruction file for creating agents
started: "2026-02-25T00:00:00Z"
ended: "2026-02-25T00:15:00Z"
task_durations:
  - task: "analyze docs requirements"
    duration: "00:05:00"
  - task: "design prompt structure"
    duration: "00:07:00"
  - task: "write metadata and finalize"
    duration: "00:03:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/02/25/2026-02-25-custom-agents-instructions-prompt/conversation.md"
source: "johnmillerATcodemag-com"
description: "Prompt for generating custom-agent instruction files aligned to GitHub Copilot docs"
context: "GitHub Copilot custom agents authoring standards and profile configuration"
expected_output: ".instructions.md file describing how to create and maintain custom agent profiles"
tools: ["search", "read", "create", "edit"]
mode: agent
name: create-custom-agents-instructions
author: John Miller
tags: [copilot, agents, instructions, github-docs, mcp, tools, yaml, persona, skills, expertise]
arguments:
  - name: instruction_filename
    description: Output instruction filename (example: custom-agents.instructions.md)
  - name: apply_to
    description: Glob for files governed by the instruction (example: .github/agents/**/*.md)
  - name: agent_scope
    description: Scope target for guidance (repository|organization|enterprise|all)
  - name: include_ide_notes
    description: Whether to include VS Code/JetBrains/Eclipse/Xcode notes (true|false)
---

# Prompt: Create Custom Agents Instruction File

Generate a token-optimized `.instructions.md` file at `.github/instructions/{{instruction_filename}}` that teaches contributors how to create and maintain GitHub Copilot custom agents.

## Source Requirements (Must Use)

Base the instruction file on these authoritative sources:

- https://docs.github.com/en/copilot/reference/custom-agents-configuration
- https://docs.github.com/en/copilot/concepts/agents/coding-agent/about-custom-agents
- https://docs.github.com/en/copilot/how-tos/use-copilot-agents/coding-agent/create-custom-agents

Do not invent unsupported properties. If a property is environment-specific, label it explicitly.

## Output File Requirements

- **Path**: `.github/instructions/{{instruction_filename}}`
- **applyTo**: `{{apply_to}}`
- **Audience**: Engineers authoring `.agent.md` profiles
- **Tone**: Directive, concise, implementation-focused
- **Format**: Markdown with short sections and checklists

## Required Sections

### 1. Purpose and Scope

- What custom agents are and why to use them
- Scope: `{{agent_scope}}`
- Where custom agents run (GitHub.com, IDEs, CLI)

### 2. File Placement and Naming Rules

- Repository-level path: `.github/agents/<name>.agent.md`
- Organization/enterprise location in `.github-private` repo: `/agents/<name>.agent.md`
- Allowed filename characters and uniqueness guidance

### 3. Agent Profile Structure

Include a concise schema table for YAML frontmatter properties:

- `name` (optional)
- `description` (required)
- `target` (`vscode` or `github-copilot`)
- `tools` (omitted = all, `[]` = none, list = explicit)
- `disable-model-invocation` and `user-invocable`
- `infer` (retired/deprecated; explain migration)
- `mcp-servers` (GitHub.com behavior note)
- `metadata` (GitHub.com usage note)

If `{{include_ide_notes}}` is `true`, include IDE-only notes for `model`, `argument-hint`, and `handoffs` support differences.

### 4. Tools and MCP Guidance

- Tool alias strategy (`read`, `edit`, `search`, `execute`, `agent`, etc.)
- Namespaced tool usage (`server/tool`, `server/*`)
- Safe default tool sets by agent type
- MCP server configuration basics
- Environment variable/secret patterns for MCP settings

### 5. Prompt Authoring Guidance

- Role definition and boundaries
- Explicit responsibilities and constraints
- Output-format expectations
- Tone and communication style requirements
- Anti-patterns (overly broad scope, unsafe shell use, missing constraints)

#### 5a. Persona Definition (required for role/persona-type agents)

When the agent represents a professional role, include all of the following:

- **Skills**: Concrete capabilities the agent *can do* (e.g., backlog grooming, KPI definition, stakeholder communication). Annotate each with a proficiency level: `basic`, `intermediate`, or `advanced`.
- **Actions**: Observable behaviors and default workflows the agent *will do* (e.g., "asks clarifying questions before proposing solutions," "presents 2–3 options with tradeoffs," "defines acceptance criteria before handoff"). Classify each action as one of:
  - **Simple**: The action is fully defined inline within the `.agent.md` prompt body.
  - **Complex**: The action is defined in a separate prompt file. Note whether that prompt file already exists (provide the path) or needs to be created (provide the intended path and a proposed prompt).
- **Expertise**: Domain authority and depth the agent *embodies* — what it knows, how well, and in which subdomain (e.g., "senior PM in education SaaS, advanced in prioritization frameworks, intermediate in technical architecture").
- **Escalation triggers**: Explicit list of what is *out of scope* — when to decline, defer to a human, or hand off to another agent.
- **Evidence standards**: What inputs or signals the agent requires *before* making a recommendation (e.g., "do not propose priorities without impact/effort data," "do not claim stakeholder approval that was not provided").

### 6. Agent Behavior Testing

- Provide at least two representative prompts that exercise the agent's core behaviors.
- Include expected response patterns for each test prompt.
- Note boundary/refusal scenarios (inputs the agent should decline or escalate).
- Describe how to verify the agent stays within its defined expertise and escalation triggers.

### 7. Processing and Precedence Rules

- Name conflict behavior across repository/org/enterprise levels
- Tool processing behavior based on `tools` value
- Versioning behavior tied to Git commits/branches

### 8. Validation Checklist

Include a practical checklist that verifies:

- Required fields present and valid
- Tools are least-privilege for purpose
- MCP references are valid and scoped
- Prompt stays under character limits
- Environment compatibility notes are present
- For persona agents: skills, actions, and expertise are all defined
- Proficiency level annotated per skill
- Each action classified as simple (inline) or complex (prompt file)
- Complex actions reference an existing prompt file path or specify a path and description for one to be created
- Escalation triggers explicitly stated
- Evidence standards defined for any recommendation behavior

### 9. Example Profiles

Provide two short examples:

- Minimal profile (description + prompt only)
- Scoped profile (explicit tools + optional MCP configuration)

## Additional Requirements

1. Add a section called **Environment Differences** with GitHub.com vs IDE behavior notes.
2. Add a section called **Operational Safety** for secrets, least privilege, and avoid-overreach behavior.
3. Add a section called **Persona Guidance** that explains the skill/action/expertise model and includes a reusable persona table template.
4. Keep the instruction file practical and concise; prefer checklists and tables over long prose.
5. Include a closing **Maintenance** section for keeping agent profiles current as docs evolve.

## Cross-Reference Update (Required)

At the end of the generated instruction file, include this directive:

"After creating this instruction file, update `.github/instructions/project-overview.instructions.md` to add a reference to this custom-agents instruction in the Standards or Development Process section."

## Metadata Requirements for Generated Instruction File

Include complete provenance metadata per `.github/instructions/ai-assisted-output.instructions.md`, including:

- `ai_generated: true`
- `model`
- `operator`
- `chat_id`
- `prompt`
- `started`, `ended`
- `task_durations`
- `total_duration`
- `ai_log`
- `source`
- `description`
- `applyTo: {{apply_to}}`

## Success Criteria

The generated instruction file must:

- Be directly actionable for creating `.agent.md` files
- Accurately reflect the three GitHub docs listed above
- Distinguish GitHub.com behavior from IDE-specific behavior
- Provide safe defaults for tools and MCP usage
- Include concise examples and a validation checklist
- For persona agents: include skills (can do), actions (will do), and expertise (how well + in what domain)
- Require proficiency levels, escalation triggers, and evidence standards for all persona definitions
- Include at least two agent behavior test prompts with expected response patterns
