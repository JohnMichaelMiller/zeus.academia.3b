---
ai_generated: true
model: "openai/gpt-5.3-codex@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-04-20-create-academia-execution-plan-prompt"
prompt: |
  create a prompt file that generates an execution plan from the #file:project-overview.instructions.md and the #file:academia-implementation-plan.md
started: "2026-04-20T18:55:00Z"
ended: "2026-04-20T19:10:00Z"
task_durations:
  - task: "analyze repository prompt conventions"
    duration: "00:05:00"
  - task: "design execution plan prompt structure"
    duration: "00:06:00"
  - task: "write prompt artifact and metadata"
    duration: "00:04:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/04/20/2026-04-20-create-academia-execution-plan-prompt/conversation.md"
source: "johnmillerATcodemag-com"
description: "Generate a repository-aware execution plan from project overview and implementation plan"
context: "Transforms architecture and slice dependencies into a phased, testable implementation roadmap"
expected_output: "Execution plan markdown with phases, dependency order, checkpoints, and risk controls"
tools: ["read", "analyze", "plan", "write"]
mode: agent
name: create-academia-execution-plan
author: John Miller
tags: [planning, execution-plan, vertical-slice, academia, backend]
arguments:
  - name: project_overview_file
    description: Path to the project overview instruction file
  - name: implementation_plan_file
    description: Path to the vertical slice implementation plan file
  - name: output_file
    description: Path where the generated execution plan markdown should be written
---

# Generate Academia Execution Plan

Create a practical execution plan by synthesizing the project context and slice dependency model.

## Inputs

- **Project Overview File**: {{project_overview_file}}
- **Implementation Plan File**: {{implementation_plan_file}}
- **Output File**: {{output_file}}

Default values if not provided:

- `project_overview_file`: `.github/instructions/project-overview.instructions.md`
- `implementation_plan_file`: `.github/models/workflows/academia-implementation-plan.md`
- `output_file`: `.github/models/workflows/academia-execution-plan.md`

## Required Workflow

1. Read and summarize project constraints from `project_overview_file`.
2. Parse slice dependencies, business rules, and rollout notes from `implementation_plan_file`.
3. Build a phased roadmap that respects all blockers and explicit predecessor constraints.
4. Convert phases into actionable implementation backlog items with measurable completion criteria.
5. Include testing and validation gates per phase.
6. Write the final output to `output_file`.

## Output Structure

The generated markdown must use this exact top-level structure:

1. `# Zeus Academia Execution Plan`
2. `## Scope and Inputs`
3. `## Planning Assumptions`
4. `## Dependency-Driven Phase Plan`
5. `## Phase Backlog`
6. `## Validation and Quality Gates`
7. `## Risks and Mitigations`
8. `## Exit Criteria`

## Phase Rules

- Phase 0 must cover Shared Kernel foundation work.
- Phase 1 must include all independent reference data slices that can run in parallel.
- Phase 2 must prioritize `RegisterAcademic` as the first mandatory sequential gate.
- Later phases must preserve dependency order from the implementation plan.
- Each phase must include:
  - Objective
  - Included slices
  - Blockers/Dependencies
  - Deliverables
  - Acceptance criteria
  - Test strategy

## Backlog Item Format

For each backlog item, use this template:

- **ID**: `EP-<phase>-<index>`
- **Slice**: `<slice-name>`
- **Type**: `Command | Query | Command+Query | Shared`
- **Why now**: one sentence tied to dependency logic
- **Implementation tasks**: 3-7 concise bullets
- **Definition of done**: explicit, testable checks

## Validation Requirements

The generated plan must explicitly validate:

- ExclusiveOr employment rule (`IsTenured` XOR `ContractEndDate`)
- AccessLevel derivation from Rank (P->INT, SL->NAT, L->LOC)
- Academic must retain at least one qualification
- Extension 1:1 uniqueness with Academic
- Contract dates are future-dated where required

## Constraints

- Do not invent slices not present in the input plan.
- Do not reorder required sequential dependencies.
- Do not omit report slices; place them after their data-producing predecessors.
- Keep language concise and implementation-focused.
- Use checklists for acceptance and quality gates.

## Quality Checklist

- [ ] All slices from the implementation plan are mapped to a phase
- [ ] No phase violates declared dependencies
- [ ] Shared Kernel appears before all slice phases
- [ ] RegisterAcademic is treated as a hard prerequisite for dependent slices
- [ ] Validation gates cover business rules and testing expectations
- [ ] Risks and mitigations reference real dependency or domain constraints
- [ ] Output written to `output_file`

## Success Criteria

1. A developer can execute the plan phase-by-phase without re-deriving dependency order.
2. Every slice has a clear position, rationale, and definition of done.
3. Validation checkpoints make business-rule regressions detectable early.
4. The plan is concise, deterministic, and directly actionable.
