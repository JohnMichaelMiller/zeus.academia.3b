---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "616990b5-0c5d-4735-a876-23fd1ebb4ff6"
prompt: |
  create a prompt file for the prompt: create an instruction file that describes the requirements for creating an implementation prompt. an implementation prompt is a prompt that specifies the implementation steps for a slice. the implementation prompt should utilize custom agents specialized in the implementation roles. the implementation prompt includes acceptance criteria that agents and humans can use the verify the implementation. the implementation prompts includes step-by-step directions that a human can follow to showcase the value of the slice.
started: "2026-04-20T19:55:00Z"
ended: "2026-04-20T20:12:00Z"
task_durations:
  - task: "review prompt-generation conventions"
    duration: "00:05:00"
  - task: "design implementation-prompt instruction requirements"
    duration: "00:07:00"
  - task: "draft prompt artifact and traceability update"
    duration: "00:05:00"
total_duration: "00:17:00"
ai_log: "ai-logs/2026/04/20/616990b5-0c5d-4735-a876-23fd1ebb4ff6/conversation.md"
source: "johnmillerATcodemag-com"
name: create-implementation-prompt-instructions
description: Generate an instruction file that defines how to author slice implementation prompts with role-based custom agents, verifiable acceptance criteria, and showcase steps
author: John Miller
tags: [prompt, instructions, implementation, vertical-slice, agents, acceptance-criteria, showcase]
arguments:
  - name: instruction_filename
    description: Output instruction filename (example: implementation-prompt.instructions.md)
  - name: apply_to
    description: Glob for implementation prompt files governed by the generated instruction (example: .github/prompts/**/*implementation*.prompt.md)
  - name: slice_scope
    description: Scope of slices covered by the guidance (example: fullstack | backend-only | frontend-only)
context: "Repository with vertical-slice architecture, custom GitHub Copilot agents, and AI provenance requirements"
expected_output: "A .instructions.md file that defines the structure, constraints, agent orchestration, acceptance criteria, and showcase requirements for slice implementation prompts"
tools: ["read_file", "semantic_search", "create_file", "edit"]
mode: agent
---

# Prompt: Create Implementation Prompt Instruction File

Generate a token-efficient `.instructions.md` file at `.github/instructions/{{instruction_filename}}` that teaches contributors how to create high-signal implementation prompts for vertical slices.

An implementation prompt is a prompt artifact that tells AI assistants and humans exactly how to implement one slice, how role-specialized custom agents should collaborate, how success is verified, and how the slice's value is demonstrated.

## Context Analysis (Required Before Drafting)

Read these files first when they exist:

1. `.github/instructions/project-overview.instructions.md` for stack, architecture, and delivery constraints.
2. `.github/instructions/vertical-slice-implementation.instructions.md` for slice structure and naming.
3. `.github/instructions/custom-agents.instructions.md` for agent profile and role expectations.
4. `.github/instructions/ai-assisted-output.instructions.md` for provenance requirements.
5. Scan `.github/agents/` and `.github/prompts/` for any existing implementation-role agents or implementation prompt patterns that should be referenced rather than duplicated.

If any expected file is missing, state that explicitly in the generated instruction file and continue with the best available repository context.

## Output File Requirements

- **Path**: `.github/instructions/{{instruction_filename}}`
- **applyTo**: `{{apply_to}}`
- **Audience**: Contributors authoring implementation prompts for `{{slice_scope}}` slices
- **Tone**: Directive, concise, implementation-focused
- **Format**: Markdown with short sections, checklists, and one reusable prompt skeleton

## Core Objective of the Generated Instruction File

The instruction file must define how to write implementation prompts that:

- target exactly one slice or one tightly scoped increment of a slice
- specify ordered implementation steps instead of high-level goals only
- use custom agents aligned to implementation roles instead of one undifferentiated assistant
- include acceptance criteria that both agents and humans can use to verify completion
- include a step-by-step showcase or demo script that proves the slice's business value

## Required Sections

The generated instruction file MUST include all of the following sections.

### 1. Purpose and Definitions

Define:

- what an implementation prompt is
- what a slice means in this repository
- when to create a new implementation prompt versus updating an existing one
- the boundary rule: one prompt per slice or per clearly bounded slice increment

### 2. Required Inputs for an Implementation Prompt

List the minimum inputs an author must gather before drafting:

- slice name and business outcome
- dependencies and prerequisite slices
- affected backend, frontend, data, and test surfaces
- existing files, patterns, and instructions to reuse
- risks, constraints, and out-of-scope items

Require the instruction file to tell authors to ground each prompt in repository evidence, not assumptions.

### 3. Agent-Oriented Implementation Roles

Require every implementation prompt to define a role model using custom agents.

The instruction file must require:

- at least three specialized roles when the slice spans multiple concerns
- explicit responsibilities, inputs, outputs, and handoff points per role
- escalation triggers for unresolved ambiguity, missing prerequisites, or conflicting patterns
- a fallback pattern when a repository-specific custom agent does not yet exist

Include guidance for a recommended role set such as:

- slice coordinator or implementation lead
- backend/API or domain agent
- frontend/UI or workflow agent
- testing/verification agent
- optional data, integration, or documentation agent when justified

### 4. Required Structure of an Implementation Prompt

Require the generated instruction file to define a standard section order for implementation prompts:

1. slice summary and business value
2. context files and repository evidence to inspect first
3. prerequisites and dependency checks
4. assigned custom agents and role boundaries
5. ordered implementation steps
6. verification and acceptance criteria
7. human showcase steps
8. completion checklist

State that implementation steps must be concrete enough that a human can follow them without inferring missing work.

### 5. Step-by-Step Implementation Guidance

Require implementation prompts to break the work into numbered steps with:

- the goal of each step
- target files or folders
- the responsible agent role
- validation expected before moving to the next step
- sequencing notes when a later step depends on an earlier artifact

The instruction file must explicitly reject vague steps such as "implement the UI" or "wire up the backend" without file-level or artifact-level direction.

### 6. Acceptance Criteria Standards

Require the generated instruction file to define acceptance criteria that are verifiable by both agents and humans.

The criteria must cover:

- functional behavior
- validation and error handling
- tests or checks to run
- integration points affected
- user-visible outcome or business rule satisfied

Require criteria to be written as observable outcomes, not intentions. Include examples of acceptable phrasing such as "Submitting an invalid enrollment request returns validation errors and does not persist data."

### 7. Verification Workflow

Require the instruction file to define a verification section for implementation prompts that includes:

- agent self-checks
- human review checks
- commands, tests, or inspection steps when available
- evidence to capture for completed work

Distinguish between "implemented" and "verified" so prompts do not stop at code generation.

### 8. Showcase and Value Demonstration

Require every implementation prompt to include a human-followable showcase sequence that demonstrates why the slice matters.

The generated instruction file must require showcase steps to include:

- starting state and prerequisites
- exact user actions or API calls
- expected visible outputs or state changes
- the business value or outcome each step proves

State that showcase steps are part of the definition of done, not optional documentation.

### 9. Reusable Prompt Template

Include one concise implementation-prompt template that authors can copy and fill in. The template must contain placeholders for:

- slice name
- business outcome
- context files
- agent roster
- numbered implementation steps
- acceptance criteria
- showcase steps

### 10. Anti-Patterns

Require a short anti-pattern section covering failures such as:

- assigning all work to one generic agent
- omitting repository context review
- acceptance criteria that cannot be observed or tested
- showcase steps that only describe code internals instead of user value
- prompts that span multiple unrelated slices

### 11. Validation Checklist

Require a final checklist that verifies:

- scope is limited to one slice or bounded increment
- required context files are listed
- custom agent roles and handoffs are explicit
- implementation steps are ordered and testable
- acceptance criteria are observable
- showcase steps demonstrate business value
- out-of-scope items are stated

## Additional Requirements

1. Keep the generated instruction file practical and terse; prefer checklists, tables, and prompt skeletons over long prose.
2. Include at least one example showing how backend, frontend, and testing roles divide work for a single slice.
3. Instruct authors to reuse existing custom agents from `.github/agents/` when available before inventing new roles.
4. Require prompts to state what a human should do if an agent gets blocked or produces conflicting output.
5. Require prompts to separate implementation instructions from demo instructions so both are independently testable.

## Cross-Reference Update (Required)

At the end of the generated instruction file, include this directive:

"After creating this instruction file, update `.github/instructions/project-overview.instructions.md` to reference the implementation-prompt instruction file in the Standards, Development Process, or Key Patterns section if that reference does not already exist."

## Metadata Requirements for the Generated Instruction File

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

- define implementation prompts as executable, slice-scoped work plans
- require role-specialized custom agents with clear handoffs
- require observable acceptance criteria usable by both agents and humans
- require a human-followable showcase script that demonstrates slice value
- provide a reusable template and validation checklist
- remain concise enough to be practical for repeated use
