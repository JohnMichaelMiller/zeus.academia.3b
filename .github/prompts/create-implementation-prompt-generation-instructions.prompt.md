---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-04-18-implementation-prompt-generation-prompt"
prompt: |
  create a prompt file for the prompt: create an instruction file that describes the requirements for creating an implementation prompt. an implementation prompt is a prompt that specifies the implementation steps for a slice.  the implementation prompt should utilize custom agents specialized in the implementation roles. the implementation prompt includes acceptance criteria that agents and humans can use the verify the implementation. the implementation prompts includes step-by-step directions that a human can follow to showcase the value of the slice.
started: "2026-04-18T12:55:27.8029009-07:00"
ended: "2026-04-18T12:56:36.8406016-07:00"
task_durations:
  - task: "context analysis"
    duration: "00:00:20"
  - task: "draft prompt content"
    duration: "00:00:40"
  - task: "provenance and reference updates"
    duration: "00:00:09"
total_duration: "00:01:09"
ai_log: "ai-logs/2026/04/18/2026-04-18-implementation-prompt-generation-prompt/conversation.md"
source: "johnmillerATcodemag-com"
name: create-implementation-prompt-generation-instructions
description: "Generate an instruction file defining how to create slice implementation prompt files"
author: John Miller
tags: [prompt, instructions, implementation, vertical-slice, custom-agents]
arguments:
  - name: instruction_filename
    description: "Target instruction filename (default: implementation-prompt-generation.instructions.md)"
    required: false
  - name: apply_to
    description: "Glob pattern for implementation prompt files (default: .github/prompts/**/*implementation*.prompt.md)"
    required: false
context: "Repository standards for prompt generation, custom agents, and vertical-slice delivery in zeus.academia"
expected_output: "A complete .instructions.md file that governs the creation of slice implementation prompts"
tools: ["read_file", "file_search", "semantic_search", "create_file"]
mode: agent
---

# Generate Implementation Prompt Instruction File

> Status: Deprecated alias. Use `.github/prompts/create-implementation-prompt-instructions.prompt.md` as the canonical prompt for ongoing updates.

Create a comprehensive `.instructions.md` file that defines the requirements for creating **implementation prompts** for vertical slices in this repository.

An implementation prompt is a prompt that specifies how a single slice should be implemented, which agents or humans own each part of the work, how the slice will be verified, and how a human can demonstrate the slice's value after delivery.

## Context Analysis

Before generating the instruction file, gather context in this order:

1. Read `#file:.github/instructions/project-overview.instructions.md` to confirm repository scope and delivery patterns.
2. Read `#file:.github/instructions/prompt-file-generation.instructions.md` to align with prompt metadata and structure rules.
3. Read `#file:.github/instructions/custom-agents.instructions.md` to align agent role naming and prompt expectations.
4. Read `#file:.github/instructions/vertical-slice-implementation.instructions.md` to align slice terminology and implementation boundaries.
5. Read `#file:.github/instructions/implementation-prompt-generation.instructions.md` if it exists, so the generated output can refine or replace it without drifting from current repository language.
6. Inspect `.github/agents/` and `.github/prompts/` for related agent profiles or implementation prompt examples.

If the repo lacks implementation-role custom agents, the generated instruction file MUST require prompts to call out those gaps explicitly rather than assuming the agents exist.

## Output File

**Path**: `.github/instructions/{{instruction_filename | default: implementation-prompt-generation.instructions.md}}`

**Metadata** (AI provenance + Copilot prompt/instruction metadata):

```yaml
---
ai_generated: true
model: "<provider>/<model>@<version>"
operator: "<operator>"
chat_id: "<chat-id>"
prompt: |
  <exact prompt used>
started: "<ISO8601>"
ended: "<ISO8601>"
task_durations:
  - task: "context analysis"
    duration: "<hh:mm:ss>"
  - task: "draft instruction content"
    duration: "<hh:mm:ss>"
total_duration: "<hh:mm:ss>"
ai_log: "ai-logs/<yyyy>/<mm>/<dd>/<chat-id>/conversation.md"
source: "<source>"
description: "Standards for generating slice implementation prompt files"
applyTo: "{{apply_to | default: .github/prompts/**/*implementation*.prompt.md}}"
---
```

## Required Instruction Content

The generated instruction file MUST define all of the following:

### 1. Purpose and Scope

Define what an implementation prompt is and constrain it to one slice or use-case at a time.

State that implementation prompts must:

- describe implementation steps for a slice
- use custom agents specialized in implementation roles
- include acceptance criteria that agents and humans can use to verify the slice
- include step-by-step showcase instructions that a human can follow to demonstrate the slice's value

### 2. Naming and Location Rules

Specify where implementation prompt files live and how they are named.

At minimum require:

- `.github/prompts/` as the default location
- kebab-case filenames
- prompt `name`, `description`, `context`, and `expected_output`

### 3. Metadata Requirements

Require:

- AI provenance fields for AI-generated files
- prompt-specific fields such as `name`, `description`, `author`, `tags`, `context`, and `expected_output`
- optional but recommended fields such as `arguments`, `examples`, `tools`, and `mode`

Include one concrete YAML example.

### 4. Required Context Analysis

The instruction file MUST tell prompt authors to gather repository and slice context before writing the prompt.

It MUST explicitly require reading:

- project overview
- vertical slice implementation guidance
- custom agent standards
- any stack-specific implementation instruction files relevant to the slice

At minimum, require the generated instruction file to name the concrete repository instruction files for backend C#, ASP.NET Core, MediatR/CQRS, FluentValidation, Vue 3, TypeScript, Pinia, xUnit, and Vitest whenever those surfaces are touched.

### 5. Agent Orchestration Requirements

Require implementation prompts to define:

- named agents by role
- role-specific responsibilities
- handoff order
- expected outputs for each role
- what to do when a required custom agent does not exist yet

The instruction file MUST require an agent matrix and MUST reject vague role names such as "engineering agent" unless they map to an actual custom agent file.

### 6. Required Prompt Sections

The instruction file MUST require implementation prompts to include these sections:

1. Objective
2. Slice Boundary
3. Required Context
4. Agent Plan
5. Implementation Steps
6. Acceptance Criteria
7. Verification Plan
8. Showcase Steps
9. Output Artifacts
10. Validation Checklist

For each section, describe what information belongs there.

### 7. Implementation Step Requirements

Require implementation steps to identify:

- step number
- owner
- action
- files or directories involved
- completion signal
- verification tied to that step

The instruction file SHOULD recommend a table format for step execution.

### 8. Acceptance Criteria Requirements

Require acceptance criteria to be:

- observable
- testable
- behavior-focused
- usable by both agents and humans

The instruction file MUST distinguish good behavioral acceptance criteria from bad task-based criteria.

It MUST also require acceptance criteria to cover scaffold cleanup and naming hygiene for newly created files, solution-file header and encoding hygiene when `.sln` files change, environment/setup helper hygiene when scripts or infra-backed tests read environment variables, precise parameter/property names in thrown argument exceptions or equivalent guard failures, and single-source reuse for constrained code or enum rules across validators, mappings, messages, and EF Core constraints.

It MUST also require acceptance criteria to cover persisted-identifier guard behavior at public domain APIs (for example create/assign/release methods enforce shared max-length and normalization before persistence) and to include explicit overlong-input verification.

It MUST also require acceptance criteria to prevent cross-concept normalization coupling (for example, one domain concept must not call another concept's normalization helper unless a neutral shared utility is explicitly introduced).

It MUST also require acceptance criteria to prevent mutable collection escape through read-only interfaces (including array-backed catalogs) and to preserve required-field validator intent for string inputs where whitespace should be treated as missing input.

### 9. Verification Requirements

Require implementation prompts to specify:

- automated test scope
- commands to run
- manual checks
- evidence expected from verification
- residual risks if verification is incomplete

The generated instruction file MUST say that verification includes a final scaffold audit whenever the slice creates new project, source, or test files.

### 10. Showcase Requirements

Require the prompt to include a human-run demo script with:

- prerequisites
- environment setup
- user actions
- expected result after each action
- at least one failure-path demonstration when relevant
- a closing statement of value demonstrated

### 11. Prompt Validation Checklist

Require a checklist that verifies the prompt itself is ready to use.

The checklist MUST cover:

- single-slice scope
- named custom agents
- missing-agent disclosure
- owned implementation steps
- behavioral acceptance criteria
- explicit verification
- human-executable showcase steps

### 12. Anti-Patterns

Require the instruction file to list anti-patterns such as:

- multi-slice prompts
- unnamed or generic agents
- task-based acceptance criteria
- showcase steps without expected outcomes
- verification instructions with no concrete commands or scope

## Writing Rules

The generated instruction file should:

- be concise and executable
- prefer lists and tables over long paragraphs
- use imperative language
- avoid vague guidance
- optimize for human and agent delegation

## Validation

Before saving the output file, verify:

- [ ] The file defines an implementation prompt in slice-specific terms.
- [ ] Custom-agent orchestration is required, not optional.
- [ ] Acceptance criteria are positioned as verification outcomes.
- [ ] Showcase guidance is concrete enough for a human to execute.
- [ ] The required prompt sections are complete and ordered.
- [ ] `applyTo` matches implementation prompt files.
- [ ] AI provenance metadata is complete and consistent.
