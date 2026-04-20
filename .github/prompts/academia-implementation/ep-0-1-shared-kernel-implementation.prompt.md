---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "616990b5-0c5d-4735-a876-23fd1ebb4ff6"
prompt: |
  Create an implementation prompt for each slice in the #file:academia-execution-plan.md
started: "2026-04-20T20:40:00Z"
ended: "2026-04-20T21:40:00Z"
task_durations:
  - task: "analyze slice dependencies"
    duration: "00:15:00"
  - task: "draft slice implementation prompt"
    duration: "00:35:00"
  - task: "traceability and review"
    duration: "00:10:00"
total_duration: "01:00:00"
ai_log: "ai-logs/2026/04/20/616990b5-0c5d-4735-a876-23fd1ebb4ff6/conversation.md"
source: ".github/models/workflows/academia-execution-plan.md"
name: implement-academia-ep-0-1-shared-kernel
description: Implement the Shared Kernel foundation for Zeus Academia before slice delivery starts
author: John Miller
tags: [academia, implementation, shared-kernel, cqrs, domain]
context: "Zeus Academia vertical-slice delivery plan and shared-kernel foundation"
expected_output: "An implementation-ready work plan for the Shared Kernel with explicit roles, ordered steps, acceptance criteria, and showcase steps"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement Shared Kernel

## Slice Summary and Business Value

- Slice: Shared Kernel
- Business outcome: establish the domain primitives, invariants, result types, and persistence constraints that every later slice depends on.
- Out of scope: feature endpoints, UI flows, reporting queries, and seed data beyond what is needed to validate foundational constraints.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/models/workflows/academia-implementation-plan.md
- .github/instructions/project-overview.instructions.md
- .github/instructions/vertical-slice-implementation.instructions.md
- .github/instructions/csharp-implementation.instructions.md
- .github/instructions/cqrs-mediatr-efcore.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: none
- Blocking risks: feature-root or persistence-root naming may differ from the plan; confirm the actual backend root before creating files.
- Existing patterns to reuse: nullable-enabled C#, Result/Error wrapper, domain event abstraction, EF Core uniqueness constraints, and aggregate guard methods.

## Assigned Agents and Role Boundaries

| Role | Responsibilities | Inputs | Outputs | Escalate when |
| --- | --- | --- | --- | --- |
| Slice coordinator | confirm folder roots, final type list, and sequence | execution plan, implementation plan, current source tree | approved artifact map and blocker list | current repo layout conflicts with the planned SharedKernel location |
| Backend/domain agent | implement aggregate, value objects, result types, exceptions, and domain events | approved artifact map, business rules | domain types and invariant logic | a rule cannot be expressed cleanly without clarifying the aggregate boundary |
| Data/persistence agent | implement EF Core mappings, indexes, and migration support | domain model, persistence standards | mappings, constraints, migration updates | a database rule would drift from the aggregate rule |
| Testing/verification agent | add invariant tests, mapping tests, and migration validation evidence | implemented kernel artifacts | passing tests and proof of enforced rules | tests expose ambiguity in XOR, access-level derivation, or qualification rules |

## Ordered Implementation Steps

1. Confirm the Shared Kernel boundary and file roots.
   Targets: src/backend/SharedKernel/, persistence project root, and tests/ root or current equivalents.
   Owner: Slice coordinator.
   Validation before next step: artifact list is approved for Academic, Rank, AccessLevel, Degree, University, Extension, AcademicQualification, Result<T>, Error, domain events, and common exceptions.
2. Implement the domain model and invariant methods.
   Targets: Shared Kernel aggregate and value-object files, especially Academic employment guards and Rank to AccessLevel derivation.
   Owner: Backend/domain agent.
   Validation before next step: the aggregate enforces tenured XOR contracted state and AccessLevel is derived only from Rank.
3. Implement persistence mappings and hard database constraints.
   Targets: EF Core entity configurations, indexes, and base migration updates for empNr uniqueness and extension uniqueness.
   Owner: Data/persistence agent.
   Validation before next step: mappings align with domain rules and no persistence rule contradicts the aggregate.
4. Add reusable error/result plumbing and domain event contracts.
   Targets: Shared Kernel result types, error primitives, event interfaces, and common exceptions.
   Owner: Backend/domain agent.
   Validation before next step: later slices can consume common result and exception types without redefining them.
5. Verify invariants and persistence behavior.
   Targets: unit tests, mapping tests, and migration validation.
   Owner: Testing/verification agent.
   Validation before next step: all foundational tests pass and failures clearly identify which invariant broke.

## Verification and Acceptance Criteria

- Creating or mutating an Academic cannot leave both IsTenured and ContractEndDate set at the same time.
- Rank values map only as P -> INT, SL -> NAT, and L -> LOC, and AccessLevel is never assigned directly.
- Shared Kernel types compile with nullable reference types enabled and are reusable by later slices.
- Database constraints back up the code-level uniqueness rules for empNr and extension assignment.
- Foundational tests cover invariant success and failure paths for employment guards, derivation, and result handling.

## Human Showcase Steps

1. Starting state: clean branch with no slice-specific code yet.
   Action: open the Shared Kernel project and inspect the Academic aggregate, value objects, and Result/Error types after implementation.
   Expected result: the domain foundation exists in one reusable location with explicit invariant methods and no feature-specific leakage.
   Value demonstrated: later slice work no longer needs to rediscover or duplicate core academic rules.
2. Starting state: test runner available.
   Action: run the Shared Kernel unit and mapping tests, including the cases for tenure/contract exclusivity and rank derivation.
   Expected result: passing tests prove the core rules are enforced before endpoint work begins.
   Value demonstrated: the highest-risk domain invariants are locked in before the backlog expands.

## Completion Checklist

- [ ] Shared Kernel scope is still limited to reusable domain and persistence foundations.
- [ ] Aggregate invariants and derived properties are enforced in code.
- [ ] Database constraints back up the critical uniqueness rules.
- [ ] Result, error, event, and exception primitives are reusable by later slices.
- [ ] Verification evidence exists for invariant and mapping behavior.
- [ ] Any repo-layout deviation from the plan is documented before dependent slice work begins.