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
name: implement-academia-ep-1-1-manage-ranks
description: Implement the ManageRanks reference-data slice for rank administration and access-level mapping
author: John Miller
tags: [academia, implementation, reference-data, ranks]
context: "Zeus Academia Phase 1 reference-data implementation"
expected_output: "A slice-scoped implementation plan for ManageRanks"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement ManageRanks

## Slice Summary and Business Value

- Slice: ManageRanks
- Business outcome: provide the canonical rank reference data required by registration, rank changes, and reporting.
- Out of scope: update/delete rank administration, get-by-id rank retrieval, academic creation, employment-state mutations, and downstream reports.

## Context Files to Review First

- .github/models/workflows/academia-execution-plan.md
- .github/models/workflows/academia-implementation-plan.md
- .github/instructions/vertical-slice-implementation.instructions.md
- Follow the vertical-slice instructions and keep the implementation in a feature/use-case folder under `src/features/` with co-located command/query, validator, endpoint, and tests instead of splitting the slice across layer-oriented folders.
- .github/instructions/xunit-implementation.instructions.md
- .github/instructions/mediatr-implementation.instructions.md
- .github/instructions/fluentvalidation-implementation.instructions.md
- .github/instructions/aspnetcore-implementation.instructions.md

## Prerequisites and Dependency Checks

- Required prior slices: Shared Kernel
- Blocking risks: rank code representation may already exist in Shared Kernel; do not create a second source of truth.
- Existing patterns to reuse: command plus query slice structure, validator beside command, unique reference-data persistence, and rank-to-access mapping rules.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                              | Inputs                                       | Outputs                                           | Escalate when                                                                                |
| -------------------- | ------------------------------------------------------------- | -------------------------------------------- | ------------------------------------------------- | -------------------------------------------------------------------------------------------- |
| slice-coordinator    | confirm whether rank records are seeded, API-managed, or both | execution plan, current data setup           | approved slice boundary and file targets          | repo already stores rank reference data in a conflicting location                            |
| backend-domain       | build add and list rank behavior with validation              | shared kernel rank model, slice conventions  | commands, queries, handlers, responses, endpoints | code tries to bypass the canonical rank codes P, SL, L                                       |
| data-persistence     | implement rank persistence mapping and durable constraints    | canonical rank definition, persistence rules | EF Core mapping, durable constraint decisions     | a database rule would duplicate rank literals instead of deriving from the canonical mapping |
| testing-verification | verify uniqueness, allowed codes, and queryability            | implemented slice                            | tests and evidence                                | validator and persistence behavior disagree on allowed rank values                           |

## Ordered Implementation Steps

1. Confirm how rank data is stored and exposed.
   Targets: src/features/ReferenceData/ManageRanks/ or current equivalent, persistence registration, seed data path.
   Owner: slice-coordinator.
   Validation before next step: one canonical approach is selected for add/list behavior, the slice language is narrowed to add/list-only unless more handlers are explicitly in scope, and existing seed data conflicts are resolved.
2. Implement add-rank command behavior.
   Targets: AddRank command, validator, handler, response, endpoint, and mapping helpers within the ManageRanks slice folder.
   Owner: backend-domain.
   Validation before next step: only P, SL, and L are accepted, failures point to the `Code` property explicitly, whitespace-only `Code` values trigger the required-field message path before allowed-values checks, duplicates are rejected deterministically, and persistence exception handling translates only proven duplicate-code conflicts instead of masking unrelated `DbUpdateException` failures.
3. Implement persistence mapping and durable allowed-code enforcement.
   Targets: persistence configuration, any schema or model-constraint artifacts, and the canonical rank-code source used by validators and mappings.
   Owner: data-persistence.
   Validation before next step: any allowed-code rule derives from the shared rank mapping or enum source rather than hard-coded SQL or duplicated literal lists, public supported-rank catalogs cannot expose mutable backing arrays, and any schema-changing model update ships with a complete migration set (migration class + Designer metadata + snapshot), never snapshot-only metadata.
4. Implement rank listing query behavior.
   Targets: ListRanks query, handler, response contract, and endpoint.
   Owner: backend-domain.
   Validation before next step: returned data exposes stable rank codes and their access-level mapping.
5. Add tests and verification evidence.
   Targets: validator tests, handler tests, integration tests for uniqueness and list behavior.
   Owner: testing-verification.
   Validation before next step: add and list flows both pass with valid and invalid inputs, file/type alignment is preserved, and persistence rules match the canonical rank definition.

## Verification and Acceptance Criteria

### Review-Prevention Guardrails

- Dependency compatibility is validated for coupled tooling packages when touched (for example xUnit core and runner major versions align).
- Result-style failure factories guard non-null failure payloads in both generic and non-generic wrappers when touched.
- Value-object parse/create APIs reject lossy coercion unless explicitly required and covered by tests.
- Integration tests that provision external resources include deterministic best-effort cleanup in `finally` blocks.
- C# source keeps one primary type per file and file names stay aligned with the primary type.
- Guard failures for invalid rank input identify the `Code` property rather than the enclosing command object.
- Required-string validation for `Code` treats null/empty/whitespace as missing input and preserves the required-field error message before allowed-values validation.
- Validators, mapping helpers, error messages, and any EF Core check constraints derive allowed rank codes from one canonical source instead of repeating literals.
- Public supported-rank catalogs and code lists expose immutable/read-only views that cannot be cast back to mutate shared array state.
- Adding a rank accepts only the codes P, SL, and L.
- Attempting to add a duplicate rank code fails without creating a second record.
- Duplicate/conflict results are returned only for proven duplicate-code cases; unrelated persistence failures are surfaced instead of being mislabeled as duplicate-rank errors.
- Listing ranks returns the canonical codes in a stable form that downstream slices can resolve.
- The slice exposes or documents the mapping from rank to access level so registration and reports do not redefine it.
- If the EF Core model now includes persisted rank records, the slice deliverable includes a complete committed migration set (migration class, Designer metadata, and snapshot) needed to provision the `Ranks` table in migration-based environments; snapshot-only changes are review blockers.
- Automated tests cover valid add, invalid code, duplicate code, and list-query behavior.
- Prompt, PR, and showcase wording stay aligned with the delivered add/list-only surface unless update/delete/get-by-id are explicitly added.

## Human Showcase Steps

1. Starting state: Shared Kernel is present and no conflicting rank seed exists.
   Action: call the add-rank endpoint or run the approved seed/admin path for P, SL, and L.
   Expected result: the canonical ranks are stored once and only once.
   Value demonstrated: every dependent slice can rely on one approved set of rank codes.
2. Starting state: rank records exist.
   Action: call the rank-list query endpoint.
   Expected result: rank data is returned with the expected codes and access-level relationship.
   Value demonstrated: registration and reporting can consume reference data instead of hardcoding it.

## Completion Checklist

- [ ] Review-prevention guardrails were evaluated and marked N/A where not applicable.
- [ ] If test packages changed, compatibility is verified (for example xUnit core and runner major versions align).
- [ ] If value-object parsing or creation changed, lossy coercion is rejected unless explicitly required and tested.
- [ ] If integration tests create external resources, teardown is enforced with best-effort `finally` cleanup.
- [ ] New C# files keep one primary type per file and filenames match the primary type.
- [ ] Invalid-rank guard failures point to `Code` rather than the enclosing command object.
- [ ] Required `Code` validation treats null/empty/whitespace as missing input and keeps required-field messaging ahead of allowed-values checks.
- [ ] Allowed rank codes are defined once and reused by validators, mappings, messages, and persistence constraints.
- [ ] Public supported-rank collections cannot mutate shared backing arrays through casts or direct collection access.
- [ ] ManageRanks stays limited to rank reference-data behavior.
- [ ] Rank validation is restricted to P, SL, and L.
- [ ] Duplicate codes are blocked at the application and persistence levels as appropriate.
- [ ] Duplicate-code error translation is narrow and does not mislabel unrelated persistence failures.
- [ ] List behavior returns stable rank data for downstream slices.
- [ ] Any schema-changing rank model update includes a complete migration artifact set (migration class + Designer + snapshot) with no standalone snapshot files.
- [ ] Prompt and PR wording do not claim CRUD or other unimplemented operations.
- [ ] Verification covers add and list success and failure paths.
- [ ] Any chosen seed strategy is documented for later environments.
