---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-04-18-implementation-prompt-instructions"
prompt: |
  create an instruction file that describes the requirements for creating an implementation prompt. an implementation prompt is a prompt that specifies the implementation steps for a slice.  the implementation prompt should utilize custom agents specialized in the implementation roles. the implementation prompt includes acceptance criteria that agents and humans can use the verify the implementation. the implementation prompts includes step-by-step directions that a human can follow to showcase the value of the slice.
started: "2026-04-18T12:45:18.2891008-07:00"
ended: "2026-04-18T12:47:41.1104378-07:00"
task_durations:
  - task: "context analysis"
    duration: "00:00:50"
  - task: "draft instruction content"
    duration: "00:01:05"
  - task: "provenance and cross-reference updates"
    duration: "00:00:28"
total_duration: "00:02:23"
ai_log: "ai-logs/2026/04/18/2026-04-18-implementation-prompt-instructions/conversation.md"
source: "johnmillerATcodemag-com"
description: "Standards for generating slice implementation prompt files"
applyTo: ".github/prompts/**/*implementation*.prompt.md"
---

# Implementation Prompt Generation

## Purpose and Scope

- An **implementation prompt** defines the execution plan for one named vertical slice.
- The prompt MUST focus on a single slice or use-case, not an epic or mixed multi-slice backlog.
- The prompt MUST produce implementation guidance that is concrete enough for both AI agents and humans to execute and verify.
- The prompt MUST align with existing repository standards before prescribing code changes.
- The prompt MUST explicitly require the implementation to follow [.github/instructions/vertical-slice-implementation.instructions.md](vertical-slice-implementation.instructions.md) and keep the slice under `src/features/<Feature>/<UseCase>/` rather than splitting it across layer-oriented folders.
## Naming and Location

- Store reusable implementation prompts in `.github/prompts/`.
- Use kebab-case filenames: `<slice-name>-implementation.prompt.md`.
- Set the prompt `name` to `implement-<slice-name>`.
- Use `description`, `context`, and `expected_output` in front matter so the prompt is discoverable and executable.

## Metadata Requirements

### AI Provenance

See [ai-assisted-output.instructions.md](ai-assisted-output.instructions.md) for required provenance fields.

### Prompt Metadata

Implementation prompts MUST include:

- `name`
- `description`
- `author`
- `tags`
- `context`
- `expected_output`

Implementation prompts SHOULD include:

- `arguments`
- `tools`
- `mode`
- `examples`

Example front matter:

```yaml
---
name: implement-course-enrollment
description: Guide delivery of the Course Enrollment vertical slice
author: John Miller
tags: [implementation, vertical-slice, backend, frontend, testing]
arguments:
  - name: slice_name
    description: Canonical slice name
  - name: context_files
    description: Optional supporting files to read before execution
context: "zeus.academia slice delivery with vertical-slice boundaries and custom agent orchestration"
expected_output: "A step-by-step implementation plan with agent assignments, acceptance criteria, verification, and demo steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---
```

## Required Context Analysis

Before drafting the prompt, gather the implementation context in this order:

1. Read `.github/instructions/project-overview.instructions.md`.
2. Read `.github/instructions/vertical-slice-implementation.instructions.md`.
3. Read `.github/instructions/custom-agents.instructions.md`.
4. Read stack-specific instruction files for every layer the slice touches.
5. Inspect existing code or workflow files for analogous slices.
6. Identify the custom agents required for the slice and record any missing agents as explicit blockers.

If the slice touches backend C#, frontend Vue 3, Pinia stores, or tests, the prompt MUST pull in the corresponding instruction files before prescribing work.

## Agent Orchestration Requirements

- The prompt MUST use custom agents specialized in implementation roles rather than assigning the entire slice to one generic actor.
- The prompt MUST name the agent for each role and state that role's deliverable.
- The prompt MUST define handoff order so each agent knows when to start and what evidence to produce.
- The prompt MUST separate implementation from verification responsibility.

Every implementation prompt MUST include an agent matrix like this:

| Role                    | Agent                  | Responsibility                                            | Inputs                                 | Outputs                                       |
| ----------------------- | ---------------------- | --------------------------------------------------------- | -------------------------------------- | --------------------------------------------- |
| Scope and acceptance    | `product-manager`      | Confirm boundaries, dependencies, and acceptance criteria | Slice request, workflows, prior specs  | Approved slice scope and acceptance checklist |
| Backend implementation  | `<backend-agent>`      | Implement API, domain, persistence, validation            | Accepted scope, backend standards      | Code changes and tests                        |
| Frontend implementation | `<frontend-agent>`     | Implement UI, state, API integration                      | Accepted scope, frontend standards     | UI changes and tests                          |
| Verification            | `<qa-or-review-agent>` | Validate behavior, tests, and demo readiness              | Implemented slice, acceptance criteria | Verification result and residual risks        |

Rules:

- Use only roles needed for the slice; do not include a frontend role for a backend-only slice.
- If a required agent does not yet exist, the prompt MUST say so explicitly and either:
  - reference the `.github/agents/<agent-name>.agent.md` file that must be created first, or
  - mark the execution blocked until that agent exists.
- Do not hide missing-agent gaps behind generic phrasing like "engineering agent".

## Required Prompt Sections

Every implementation prompt MUST contain these sections, in order:

### 1. Objective

State the slice name, the user or business value delivered, and the concrete outcome expected at completion.

### 2. Slice Boundary

Define:

- in-scope behavior
- explicit non-goals
- dependencies on shared kernel, contracts, or prerequisite slices
- interfaces or entry points touched by the slice

### 3. Required Context

List the files, workflows, standards, and existing code paths the executor must review before making changes.

### 4. Agent Plan

Include the agent matrix plus the expected handoff sequence. Each handoff must identify:

- owner
- prerequisite evidence
- output artifact
- stop condition

### 5. Implementation Steps

Provide an ordered list of concrete steps. Each step MUST identify:

- step number
- owning agent or human role
- goal
- files or directories to inspect or modify
- completion signal
- verification tied to the step

Preferred format:

| Step | Owner             | Action                                                  | Files                              | Done When                    | Verification                  |
| ---- | ----------------- | ------------------------------------------------------- | ---------------------------------- | ---------------------------- | ----------------------------- |
| 1    | `product-manager` | Confirm slice boundary and finalize acceptance criteria | `models/`, `.github/instructions/` | Scope is approved and stable | Acceptance checklist reviewed |

### 6. Acceptance Criteria

Acceptance criteria MUST be usable by both agents and humans. Each criterion MUST be:

- observable
- testable
- scoped to the slice outcome, not the implementation task list
- written as a checklist, Given/When/Then, or equivalent precise format

The section MUST cover, when applicable:

- happy path behavior
- validation and failure behavior
- persistence or side effects
- authorization or role restrictions
- user-visible feedback
- test coverage expectations
- result-wrapper invariants for success/failure access patterns (for example, `Result<T>.Value` must not be consumable on failure)
- database key/constraint intent without redundancy (for example, avoid unique indexes that duplicate the primary key columns)

Bad:

- "Create API endpoint"
- "Write tests"

Good:

- "Given valid enrollment data, when an authorized registrar submits the form, then the system creates the enrollment and returns the new enrollment identifier."
- "Given duplicate enrollment data, when the request is submitted, then the system rejects it with a conflict result and preserves existing data."

### 7. Verification Plan

Specify how the slice will be verified:

- automated tests to add or update
- commands to run
- manual checks
- evidence to collect
- residual-risk callouts if verification is partial
- behavior when environment prerequisites are missing (tests MUST fail explicitly with actionable diagnostics; no early return/skipped-by-default pattern)

The prompt MUST distinguish between required verification and optional follow-up checks.

### 8. Showcase Steps

This section is mandatory. It explains how a human demonstrates the value of the slice after implementation.

The showcase script MUST include:

- prerequisites or seed data
- environment setup steps
- step-by-step user actions
- expected result after each action
- one failure-path demonstration when relevant
- the specific value proven by the demo

Preferred format:

```markdown
1. Start the API and frontend for the target environment.
2. Sign in as a registrar test user.
3. Navigate to Course Enrollment.
4. Submit a new enrollment for Student A into Course B.
   Expected: Enrollment confirmation appears and the new row is visible in the enrollment list.
5. Repeat the same request.
   Expected: The UI shows a duplicate-enrollment error and no second row is created.

Value demonstrated: The slice supports successful enrollment while protecting data integrity.
```

### 9. Output Artifacts

List the expected outputs from execution:

- code changes
- tests
- updated docs or prompt files
- verification summary
- demo notes if they are part of the deliverable

### 10. Validation Checklist

End the prompt with a checklist that verifies prompt quality before use.

## Prompt Validation Checklist

- [ ] The prompt targets exactly one slice.
- [ ] Required repo instructions are listed in pre-work.
- [ ] Custom agents are named by role, not implied.
- [ ] Missing custom agents are called out explicitly.
- [ ] Each implementation step has an owner and completion signal.
- [ ] Acceptance criteria describe outcomes, not task completion.
- [ ] Verification covers both automated and manual checks where applicable.
- [ ] Showcase steps can be executed by a human without hidden knowledge.
- [ ] The value of the slice is demonstrated explicitly.
- [ ] Non-goals and dependency constraints are stated.
- [ ] Persistence rules avoid redundant uniqueness definitions (no PK + duplicate unique index on same columns unless explicitly justified).
- [ ] Verification instructions require explicit failure for missing infrastructure prerequisites (no silent pass/early return).
- [ ] Shared result contracts include invariant access rules for success/failure payloads.

## Anti-Patterns

- A single prompt covering multiple unrelated slices.
- Generic instructions such as "implement the feature" with no file targets, roles, or checkpoints.
- Agent roles listed without named agents or handoffs.
- Acceptance criteria that restate implementation tasks instead of behavior.
- Demo steps that omit expected outcomes.
- Verification sections that say "run tests" without naming the test scope or commands.
- Prompts that assume a missing custom agent already exists.
- Prompting persistence work that adds a unique index on the same columns as an existing primary key.
- Prompting integration/constraint tests to catch-and-return on connection/setup errors instead of failing explicitly.

## Output Standard

The generated implementation prompt should be concise, executable, and structured for delegation. Prefer tables and ordered steps over long prose. The result should let a human reviewer answer three questions quickly:

1. Which slice is being delivered?
2. Which agent or person owns each part of the work?
3. How do we prove the slice works and show why it matters?
