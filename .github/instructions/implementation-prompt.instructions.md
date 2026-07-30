---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "616990b5-0c5d-4735-a876-23fd1ebb4ff6"
prompt: |
  Follow instructions in #prompt:create-implementation-prompt-instructions.prompt.md
started: "2026-04-20T20:20:00Z"
ended: "2026-04-20T20:35:00Z"
task_durations:
  - task: "review repository standards and prompt requirements"
    duration: "00:04:00"
  - task: "draft implementation prompt instruction file"
    duration: "00:08:00"
  - task: "update project traceability and chat summary"
    duration: "00:03:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/04/20/616990b5-0c5d-4735-a876-23fd1ebb4ff6/conversation.md"
source: ".github/prompts/create-implementation-prompt-instructions.prompt.md"
description: "Requirements for authoring slice implementation prompts with role-based agents, acceptance criteria, and showcase steps"
applyTo: ".github/prompts/**/*implementation*.prompt.md"
name: "Implementation Prompt Standards"
author: "John Miller"
tags:
  [implementation, prompts, vertical-slice, custom-agents, acceptance-criteria]
---

# Implementation Prompt Standards

## Purpose and Scope

Use an implementation prompt to define the work plan for one slice or one explicitly bounded increment of a slice. The prompt must tell AI agents and humans what to inspect, what to build, in what order, how handoffs happen, how completion is verified, and how the slice's user value is demonstrated.

In this repository, a slice is a cohesive unit of behavior implemented through the established vertical-slice structure. Do not use one implementation prompt to cover multiple unrelated slices. Create a new prompt when the next unit of work has its own business outcome, dependency boundary, or verification path.

## Required Inputs

Before writing an implementation prompt, gather the minimum evidence:

- Slice name and one-sentence business outcome
- Prerequisite slices, shared-kernel dependencies, and blocked work
- Existing files, instructions, agents, and prompt patterns to reuse
- Backend, frontend, data, API, test, and demo surfaces affected
- Risks, constraints, assumptions, and explicit out-of-scope items

Ground the prompt in repository evidence. Reference real files, existing instructions, and current patterns. Do not fill gaps with guesses; mark missing context and define the escalation path instead.

## Required Context Review

Review these sources before drafting when they exist:

- `.github/instructions/project-overview.instructions.md`
- `.github/instructions/vertical-slice-implementation.instructions.md`
- `.github/instructions/custom-agents.instructions.md`
- `.github/instructions/ai-assisted-output.instructions.md`
- Existing files in `.github/agents/` for reusable implementation-role agents
- Existing implementation or execution prompts in `.github/prompts/`

If one of these sources is missing, say so in the implementation prompt and continue with the remaining repository evidence.

Include layer-specific instructions for every touched surface. At minimum:

- backend C#: `.github/instructions/csharp-implementation.instructions.md`
- ASP.NET Core endpoints: `.github/instructions/aspnetcore-implementation.instructions.md`
- MediatR/CQRS: `.github/instructions/mediatr-implementation.instructions.md`, `.github/instructions/cqrs-mediatr-efcore.instructions.md`, or `.github/instructions/cqrs-es-csharp-mediatr.instructions.md`
- FluentValidation: `.github/instructions/fluentvalidation-implementation.instructions.md`
- frontend Vue/TypeScript: `.github/instructions/vue3-implementation.instructions.md`, `.github/instructions/typescript-frontend-implementation.instructions.md`
- Pinia: `.github/instructions/pinia-implementation.instructions.md`
- backend tests: `.github/instructions/xunit-implementation.instructions.md`
- frontend tests: `.github/instructions/vitest-implementation.instructions.md`

## Agent-Oriented Roles

Implementation prompts must use role-specialized custom agents when the slice spans multiple concerns. For a multi-surface slice, define at least three roles. Each role must state responsibilities, expected inputs, expected outputs, handoff targets, and escalation triggers.

Recommended roles:

| Role                                | Primary responsibility                                                    | Typical outputs                                |
| ----------------------------------- | ------------------------------------------------------------------------- | ---------------------------------------------- |
| slice-coordinator                   | Own scope, sequencing, and handoffs                                       | ordered plan, dependency decisions, blockers   |
| backend-domain                      | Implement contracts, handlers, validation, persistence                    | backend code, API changes, domain rules        |
| frontend-workflow                   | Implement UI, client flows, and interaction states                        | components, stores, composables, typed clients |
| testing-verification                | Define checks, run verification, capture evidence                         | test cases, verification notes, failure gaps   |
| data-persistence                    | Implement EF Core mappings, indexes, migrations, and database constraints | mappings, indexes, migrations, schema updates  |
| Report/projection agent             | Implement read models, grouped queries, and projection-backed reporting   | report queries, DTOs, projections, aggregates  |
| Optional data/integration/doc agent | Handle migrations, external integration, or user-facing docs when needed  | scripts, integration notes, showcase support   |

If a repository-specific agent does not exist, the prompt must say whether to:

- reuse the closest existing custom agent and narrow its role for this slice
- create a temporary role description inline for the current prompt
- stop and ask for a new reusable agent profile when the role will recur

Every prompt must define escalation triggers for missing prerequisites, contradictory repository patterns, failed verification, or ambiguity that changes the slice boundary.

For report-centric slices, prefer the reusable `report-projection` agent over the generic backend/domain role when the primary work is grouped analytics, projection storage, or report-only query behavior.

For schema-heavy or infrastructure-backed slices, prefer the reusable `data-persistence` agent when EF Core mappings, indexes, migrations, or database constraints are a primary concern rather than an incidental implementation detail.

## Standard Implementation Prompt Structure

Each implementation prompt must use this section order:

1. Slice summary and business value
2. Context files and repository evidence to inspect first
3. Prerequisites and dependency checks
4. Assigned agents and role boundaries
5. Ordered implementation steps
6. Verification workflow and acceptance criteria
7. Human showcase steps
8. Completion checklist

Keep the section order stable so humans and agents can scan prompts quickly.

## Step-by-Step Implementation Guidance

Implementation steps must be numbered and explicit. Each step must include:

- the objective of the step
- the target files, folders, or artifacts to inspect or update
- the responsible agent role
- required handoff or dependency from prior steps
- the validation expected before the next step begins

Reject vague instructions such as "implement the UI" or "wire up the backend" without file-level or artifact-level direction. A valid step is actionable by a human without inferring hidden work.

Use this step format:

| Step | Goal                                    | Targets                                   | Owner                | Validation before next step           |
| ---- | --------------------------------------- | ----------------------------------------- | -------------------- | ------------------------------------- |
| 1    | Confirm slice boundary and dependencies | named files and prior slices              | slice-coordinator    | boundary accepted and blockers listed |
| 2    | Implement backend behavior              | specific command/query/endpoint files     | backend-domain       | API behavior matches rules            |
| 3    | Implement frontend flow                 | specific component/store/composable files | frontend-workflow    | UI states align with API behavior     |
| 4    | Verify and capture evidence             | tests, logs, screenshots, notes           | testing-verification | evidence recorded and gaps called out |

## Acceptance Criteria Standards

Acceptance criteria must be observable and testable by both agents and humans. Write them as outcomes, not intentions.

Required coverage:

- Functional behavior
- Validation and error handling
- Integration points or contracts affected
- Tests, checks, or inspections to run
- User-visible outcome or business rule satisfied
- Dependency compatibility checks for coupled tooling packages (for example xUnit core/runner major-version alignment)
- Test resource lifecycle rules for integration tests that provision external resources (must include teardown strategy)
- Ownership-safe aggregate mutation rules for linked entities (for example, release operations must verify current ownership and assignment operations must not overwrite a different existing link)
- No lossy value coercion in domain parse/create APIs unless explicitly required and tested
- Shared result/failure factories guard non-null failure payload invariants in both generic and non-generic forms
- Result semantics reserve `Error.None` for success only; failed results must carry actionable details and cannot be constructed with an empty success sentinel.
- EF Core schema changes require migration artifacts and metadata hygiene (for example, migration plus snapshot/Designer files when the project uses migrations) unless the prompt explicitly waives them and explains the tradeoff.
- Model metadata verification should inspect `context.Model` directly instead of relying on `context.GetService<IDesignTimeModel>()` in normal tests.
- Exception types should be split into dedicated files/types with names that stay aligned as the exception set grows.
- Solution hygiene when `.sln` files change: no duplicate project name/path entries, no duplicate GUID configuration blocks.
- Solution-file format hygiene when `.sln` files change: the `Microsoft Visual Studio Solution File` header remains on line 1 with no leading blank line.
- Foundational primitive coverage when shared base types are touched (for example `Result` and `Result<T>`): direct tests for non-generic and generic success/failure invariants.
- Scaffold cleanup and naming hygiene when new files are introduced: no leftover `Class1.cs`, `UnitTest1.cs`, `Placeholder` types, or similar starter artifacts; file names must match the primary type or test behavior.
- Script and setup-helper hygiene when verification touches infrastructure configuration: environment variables are read once per value and reused through a local variable or helper instead of duplicated lookups.
- Teardown failure isolation for integration tests: cleanup runs as best-effort in `finally` and teardown exceptions must not replace the primary assertion failure signal.

For each non-trivial business rule, the prompt must also name the intended enforcement layers. At minimum, state whether the rule is enforced in the aggregate, validator, handler, database constraint, or some explicit combination of those layers.

For every named database constraint in acceptance criteria, the constraint name must reflect the exact predicate semantics. Do not label a rule as XOR unless the predicate enforces strict exactly-one semantics.

For any persistence-backed field constraint such as max length, precision, scale, uniqueness, or required normalization, the prompt must say where the canonical rule lives and how drift is prevented. Prefer shared domain constants or a single canonical definition reused by factories, validators, and EF Core mappings rather than repeating raw values across layers.

Use a short enforcement matrix when the slice includes durable invariants, schema changes, or persistence-backed rules:

| Rule                        | Canonical layer      | Persistence backing required | Verification evidence                 |
| --------------------------- | -------------------- | ---------------------------- | ------------------------------------- |
| Employment mutual exclusion | Aggregate + database | Yes, CHECK constraint        | Unit test + schema or migration proof |

When a rule's final durable enforcement belongs to a later slice, the prompt must say so explicitly. Do not imply that a rule is fully durable now if the current slice only enforces it at the validator, handler, or aggregate level.

For each deferred durable invariant, the prompt must include a short deferral ledger with these fields: rule, enforced now by, not yet enforced in, owning follow-up slice, reviewer-visible risk, and verification evidence for the current temporary state.

If the current slice leaves a persistence gap open until a later slice, the prompt must explicitly choose one of these paths:

- add a temporary persistence safeguard now, such as a CHECK constraint or transitional uniqueness/index protection
- waive the safeguard for now and record the accepted gap, why it is acceptable, the owning follow-up slice, and the condition that removes the waiver

When a slice changes schema, constraints, indexes, or EF Core model shape, the prompt must explicitly say whether a committed migration artifact is required. If it is required, name the expected persistence root, the migration artifact to create or update, and the evidence that proves the migration is part of the slice deliverable.

When persisted data can drift outside domain expectations, read-path failure handling must be named explicitly. Treat invalid stored values as a persistence or data-corruption concern, not as an ordinary business-rule validation path, unless the prompt explicitly justifies a different contract.

When the slice depends on provider-specific EF Core behavior, such as SQL Server decimal precision, filtered indexes, collations, computed columns, or constraint translation, the prompt must require verification against the target provider or generated migration output. SQLite-only checks are not sufficient unless the prompt explicitly states that provider parity is out of scope.

Do not let a schema-changing prompt stop at "migration support" or "schema evidence." The prompt must distinguish mapping-only persistence work from schema-changing persistence work.

Preferred phrasing:

- "Submitting an invalid enrollment request returns validation errors and does not persist data."
- "Saving a valid schedule update shows the updated state in the UI and persists the change through the slice endpoint."

Avoid phrasing like "Validation should work" or "The UI should be intuitive." Those are not verifiable.

## Verification Workflow

Implementation prompts must separate implementation from verification. The verification section must define:

- agent self-checks before handoff
- human review checks after implementation
- tests, commands, or manual inspection steps when available
- evidence to capture, such as logs, screenshots, response samples, or test output
- unresolved issues that block moving from implemented to verified

If the prompt claims persistence foundations, database constraints, or migration support, verification must include schema evidence. Passing unit tests or mapping tests alone is insufficient when the rule is supposed to be durable at the database level.

If the slice changes schema, verification must also confirm that a committed migration artifact exists in the diff unless the prompt explicitly waives migrations and explains why. For EF Core migrations, the verification path should call out the expected migration artifact, snapshot/Designer metadata, and provider-specific validation evidence.

For model metadata assertions, the verification section should require direct inspection of the EF Core model and avoid a normal test dependency on `IDesignTimeModel` service resolution.

Verification should also call out reviewer-facing hygiene when the slice produces durable artifacts or tests. At minimum, require durable README entries to link back to the artifact log when traceability applies, and require test names to describe the actual scenario and expectation rather than a nearby but different failure mode.

When a slice adds project, source, or test files, verification must include a final scaffold audit so starter placeholders are removed or renamed before review.

A slice is not complete because code exists. It is complete when the prompt's verification path has been executed and evidence has been captured or explicitly waived by a human.

## Showcase and Value Demonstration

Every implementation prompt must end with a human-followable showcase sequence that proves the slice's value. The showcase is part of the definition of done.

Each showcase step must include:

- starting state and prerequisites
- exact user actions or API calls
- expected visible output or state change
- the business value the step demonstrates

Use showcase steps to prove user or stakeholder value, not internal implementation details. A good showcase connects actions to an outcome such as reduced manual work, correct policy enforcement, or clearer user feedback.

## Example Role Split

Example for a single slice:

| Role                 | Scope for the slice                                                    | Handoff                                                        |
| -------------------- | ---------------------------------------------------------------------- | -------------------------------------------------------------- |
| slice-coordinator    | confirm scope, inspect dependencies, assign sequence                   | hands backend and frontend agents the approved work order      |
| backend-domain       | implement command, validator, handler, endpoint, and response contract | hands API contract and edge cases to frontend and verification |
| frontend-workflow    | implement component, composable, store, and request handling states    | hands user flow and failure states to verification             |
| data-persistence     | implement mappings, indexes, migrations, and integrity constraints     | hands persistence impacts and constraints to verification      |
| report-projection    | implement projection, grouped query, and report response contracts     | hands report shape and edge cases to verification              |
| testing-verification | define test cases, run checks, collect proof, raise failures           | returns pass/fail evidence to coordinator and human reviewer   |

## Reusable Prompt Template

```markdown
# Implement {{slice_name}}

## Slice Summary and Business Value

- Slice: {{slice_name}}
- Business outcome: {{business_outcome}}
- Out of scope: {{out_of_scope}}

## Context Files to Review First

- {{file_or_instruction_1}}
- {{file_or_instruction_2}}
- {{file_or_instruction_3}}

## Prerequisites and Dependency Checks

- Required prior slices: {{dependencies}}
- Blocking risks: {{risks}}
- Existing patterns to reuse: {{patterns}}

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities      | Inputs                 | Outputs                 | Escalate when              |
| -------------------- | --------------------- | ---------------------- | ----------------------- | -------------------------- |
| slice-coordinator    | {{coordinator_scope}} | {{coordinator_inputs}} | {{coordinator_outputs}} | {{coordinator_escalation}} |
| backend-domain       | {{backend_scope}}     | {{backend_inputs}}     | {{backend_outputs}}     | {{backend_escalation}}     |
| frontend-workflow    | {{frontend_scope}}    | {{frontend_inputs}}    | {{frontend_outputs}}    | {{frontend_escalation}}    |
| data-persistence     | {{data_scope}}        | {{data_inputs}}        | {{data_outputs}}        | {{data_escalation}}        |
| report-projection    | {{report_scope}}      | {{report_inputs}}      | {{report_outputs}}      | {{report_escalation}}      |
| testing-verification | {{test_scope}}        | {{test_inputs}}        | {{test_outputs}}        | {{test_escalation}}        |

## Ordered Implementation Steps

1. {{step_1_goal}}
   Targets: {{step_1_targets}}
   Owner: {{step_1_owner}}
   Validation before next step: {{step_1_validation}}
2. {{step_2_goal}}
   Targets: {{step_2_targets}}
   Owner: {{step_2_owner}}
   Validation before next step: {{step_2_validation}}
3. {{step_3_goal}}
   Targets: {{step_3_targets}}
   Owner: {{step_3_owner}}
   Validation before next step: {{step_3_validation}}

## Verification and Acceptance Criteria

- {{acceptance_criterion_1}}
- {{acceptance_criterion_2}}
- {{acceptance_criterion_3}}

## Human Showcase Steps

1. Starting state: {{showcase_start}}
   Action: {{showcase_action_1}}
   Expected result: {{showcase_result_1}}
   Value demonstrated: {{showcase_value_1}}
2. Starting state: {{showcase_start_2}}
   Action: {{showcase_action_2}}
   Expected result: {{showcase_result_2}}
   Value demonstrated: {{showcase_value_2}}

## Completion Checklist

- [ ] Scope is still limited to this slice or bounded increment
- [ ] Agent roles and handoffs are explicit
- [ ] Implementation steps are ordered and concrete
- [ ] Acceptance criteria are observable
- [ ] Verification evidence is captured
- [ ] Schema-changing work names the required migration artifact and how it will be verified
- [ ] Deferred durable invariants are either temporarily safeguarded now or tracked with an explicit owning follow-up slice and risk note
- [ ] Showcase steps demonstrate business value
```

## Anti-Patterns

Do not author implementation prompts that:

- assign all work to one generic agent without role boundaries
- skip repository context review and invent new patterns unnecessarily
- use acceptance criteria that cannot be observed, tested, or inspected
- imply a rule is durably enforced without stating whether it is enforced now, temporarily safeguarded, or deferred to a later slice
- describe showcase steps only in terms of code internals
- span multiple unrelated slices in one prompt
- stop at implementation instructions and omit verification or showcase paths
- suggest schema changes without naming the migration artifact or metadata hygiene expectations
- define result/failure contracts that allow `Error.None` on failures or allow failure access to `Value` without explicit guards
- rely on `IDesignTimeModel` in normal model tests instead of inspecting the EF Core model directly

## Validation Checklist

Before using an implementation prompt, verify:

- [ ] The scope is one slice or one clearly bounded increment
- [ ] Repository context files are listed explicitly
- [ ] Dependencies, risks, and out-of-scope items are stated
- [ ] Custom agent roles, outputs, and handoffs are explicit
- [ ] Each implementation step names targets, owner, and validation
- [ ] Acceptance criteria are observable and testable
- [ ] Verification steps include evidence capture
- [ ] Showcase steps are human-followable and prove business value
- [ ] The prompt states what to do when an agent is blocked or outputs conflict

## Maintenance

Keep implementation prompts aligned with current slice structure, agent inventory, and verification practices. Prefer updating shared agent profiles and this instruction file when a pattern repeats instead of re-explaining the same behavior in each prompt.

After creating this instruction file, update `.github/instructions/project-overview.instructions.md` to reference the implementation-prompt instruction file in the Standards, Development Process, or Key Patterns section if that reference does not already exist.
