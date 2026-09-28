---
ai_generated: true
model: "github/copilot@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-09-27-register-academic-blog-post"
prompt: |
  write a blog post explaining the #file:ep-2-1-register-academic-implementation.prompt.md, how to execute the prompt and how to verify the implementation and how to showcase the new feature.
started: "2026-09-27T00:00:00Z"
ended: "2026-09-27T00:00:00Z"
task_durations:
  - task: "repository and prompt analysis"
    duration: "00:15:00"
  - task: "blog post drafting"
    duration: "00:30:00"
  - task: "traceability and editorial review"
    duration: "00:10:00"
total_duration: "00:55:00"
ai_log: "ai-logs/2026/09/27/2026-09-27-register-academic-blog-post/conversation.md"
source: ".github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md"
layout: post
title: "Executing the RegisterAcademic Slice with an AI Implementation Prompt"
date: 2026-09-27
categories: [ai-assisted-development, software-engineering, architecture]
tags: [vertical-slices, aspnet-core, mediatr, ef-core, sql-server, testing]
excerpt: "A practical guide to executing, verifying, and demonstrating the RegisterAcademic vertical slice in Zeus Academia."
description: "Learn how the RegisterAcademic implementation prompt coordinates agents, protects feature boundaries, verifies atomic persistence, and demonstrates the first academic lifecycle capability."
image: /assets/images/2026-09-27/register-academic-implementation.svg
---

RegisterAcademic is the first real academic-lifecycle command in Zeus Academia. It turns reference data, shared domain rules, and an available extension into a complete academic record that later slices can query and change. This post explains how to execute the [RegisterAcademic implementation prompt](../.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md), how to verify the result, and how to demonstrate its value without relying on a compile-only success.

<!--more-->

<figure>
  <img src="/assets/images/2026-09-27/register-academic-implementation.svg" alt="The RegisterAcademic slice connecting reference data, domain rules, persistence, and verification">
  <figcaption>RegisterAcademic is a boundary between reference-data contracts and the academic lifecycle.</figcaption>
</figure>

## Why RegisterAcademic Matters

A registration command is more than an insert into an `Academics` table. It combines identity, rank, qualifications, university references, and extension assignment. If those pieces are created independently, the system can produce partial records that look valid to one feature and unusable to the next.

The prompt treats RegisterAcademic as the first mandatory delivery gate for the academic lifecycle. A successful implementation must create a complete record atomically, derive access level from rank, and reject invalid or conflicting references before dependent slices begin. That boundary makes the prompt useful as an engineering contract rather than a checklist of files to generate.

## What the Prompt Defines

The [implementation prompt](../.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md) defines one bounded vertical slice. Its business outcome is precise: create an academic with a valid six-character employee number, a name within the domain limit, at least one degree and university pair, a rank-derived access level, and one available extension.

It deliberately excludes profile viewing, later employment changes, extension reassignment, and reporting. That scope matters because the implementation prompt is also a sequencing tool. RegisterAcademic must pass before dependent work starts, so unrelated lifecycle behavior should not be smuggled into the first gate.

What this prompt produces:

- A command, validator, handler, endpoint, mappings, and feature-local persistence workflow
- A public integration path for university resolution through `GetUniversityByCodeQuery`
- Atomic creation of the academic, qualification records, and extension linkage
- Unit and provider-backed integration tests for success, duplicates, invalid references, and conflicts
- Evidence that the result is reachable through the application host and ready for dependent slices

The prompt also names architectural prohibitions. The handler must consume ManageUniversities through its query contract; it must not inject `ManageUniversitiesDbContext` or reference `UniversityRecord` directly. This keeps feature ownership clear and prevents persistence details from leaking across slice boundaries.

## Read the Context Before Execution

Before invoking the prompt, I read the repository instructions and the current slice evidence. This step prevents an agent from inventing a new layout or silently bypassing an existing contract.

| Artifact                                                                                                                               | Why it matters                                                                |
| -------------------------------------------------------------------------------------------------------------------------------------- | ----------------------------------------------------------------------------- |
| [RegisterAcademic implementation prompt](../.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md) | Defines scope, agent roles, steps, acceptance criteria, and showcase actions  |
| [Academia execution plan](../src/models/workflows/academia-execution-plan.md)                                                          | Establishes RegisterAcademic as the EP-2-1 sequential gate                    |
| [RegisterAcademic handoff](../src/models/workflows/ep-2-1-register-academic-handoff.md)                                                | Records the domain scope, dependencies, and expected feature structure        |
| [Downstream consumer pattern](../src/models/workflows/phase-1-downstream-consumer-pattern.md)                                          | Defines how feature handlers consume reference data through MediatR contracts |
| [Vertical-slice instructions](../.github/instructions/vertical-slice-implementation.instructions.md)                                   | Governs co-location, naming, boundaries, and verification                     |
| [MediatR instructions](../.github/instructions/mediatr-implementation.instructions.md)                                                 | Defines command and handler conventions                                       |
| [FluentValidation instructions](../.github/instructions/fluentvalidation-implementation.instructions.md)                               | Defines validator behavior and test expectations                              |
| [xUnit instructions](../.github/instructions/xunit-implementation.instructions.md)                                                     | Defines backend test structure and evidence                                   |

The prerequisite check is concrete. ManageRanks, ManageDegrees, ManageUniversities, and ProvisionExtension must expose the contracts and data needed by registration. At least one extension must be unassigned. If a prerequisite is incomplete, the coordinator stops and records the blocker instead of asking the backend agent to build against guesses.

## Execute the Prompt in Three Handoffs

The prompt assigns work to named roles so each handoff carries evidence forward. I execute it from the repository root, with the prompt and its context files open in the same Copilot conversation.

### 1. Coordinate the slice

The `slice-coordinator` confirms the route, the feature directory, the transaction boundary, and prerequisite readiness. The coordinator should identify the actual repository layout before changing it. The prompt suggests `src/features/Academics/RegisterAcademic/` or the current equivalent, so the existing structure wins when the two differ.

The coordinator hands the backend role an approved sequence and a blocker list. The handoff is complete when reference data can support a valid test request and the persistence owner is explicit.

### 2. Implement the domain path

The `backend-domain` role implements the request and response contract, validator, handler, endpoint, mappings, and persistence workflow. The handler resolves rank, degree, university, and extension references through their public contracts, creates the academic aggregate, derives access level from rank, and persists the complete operation atomically.

The validator should reject malformed employee numbers, overlong names, missing qualification pairs, and invalid request shapes before persistence. Domain factories remain responsible for domain invariants, and persistence mappings must stay aligned with those limits. The feature-local `RegisterAcademicDbContext` owns the registration tables and migrations; it does not reuse another feature's migration owner.

The endpoint is not finished when the handler compiles. The application host must register the feature services and map the route aggregator. The route prefix should follow the repository's current convention, and the response contract should accurately describe validation and conflict outcomes.

### 3. Verify the behavior

The `testing-verification` role receives the implemented contract and acceptance criteria. It verifies the valid path first, then deliberately exercises duplicate identity, invalid references, missing qualifications, and extension conflicts. It also checks that failed requests leave no partial academic, qualification, or assignment data behind.

The testing role returns test output, migration evidence, runtime reachability evidence, and residual risks. A failed infrastructure prerequisite is a blocker with an actionable diagnostic, not a silently skipped test.

## Use the Prompt Verbatim

The source prompt is the executable artifact for the work. In Copilot Chat, I open the file, reference it explicitly, and ask the agent to execute it against the current workspace:

```text
Execute #file:ep-2-1-register-academic-implementation.prompt.md from the repository root.
Read every context file named by the prompt before editing. Follow the named role handoffs, stop on missing prerequisites, implement only the RegisterAcademic slice, and return verification evidence plus showcase instructions.
```

The important part is not the wording of the invocation; it is the constraint that the agent reads the prompt's context before editing. The prompt itself contains the complete scope and handoff plan:

```text
The complete prompt is maintained verbatim in the repository artifact linked above. Execute that file directly so its provenance, acceptance criteria, and repository-relative paths remain authoritative.
```

This keeps the blog readable while preserving one source of truth. Copying a shortened version into a second prompt would create drift between the published explanation and the implementation artifact.

## Verify the Implementation

Verification proceeds from the outside in. I first prove that the host can reach the feature, then prove the domain behavior, and finally prove database durability and atomicity.

### Confirm composition and reachability

Inspect the application host and verify that the feature's service-registration extension and endpoint aggregator are called directly. Start the API with the repository's configured SQL Server connection. Submit one valid request through the actual route, not a handler test, and record the status code and response body.

A route that exists only in a feature folder is not delivered. The runtime smoke test must show that the application host maps it and that the response contract is reachable through HTTP.

### Run focused tests

Use the test project created for the slice. The exact project path should match the implementation's final layout; the handoff's expected shape is `tests/Features/RegisterAcademic/`. A focused command is preferable to a full solution run while iterating:

```powershell
dotnet test tests/Features/RegisterAcademic/RegisterAcademic.Tests.csproj
```

The focused suite should cover:

- A valid request creates one academic with the expected rank-derived access level
- A duplicate `empNr` returns the documented conflict and creates no second record
- An invalid rank, degree, university, or extension reference fails through the intended contract
- A request with no degree and university pair is rejected
- An already assigned extension cannot be reused
- A failed multi-record operation leaves no partial writes

If package references change, confirm that coupled xUnit packages remain on compatible major versions. If integration tests provision databases or other external resources, cleanup runs in `finally` and does not replace the primary assertion failure.

### Verify SQL Server persistence

Because this slice owns persistence, unit tests alone are insufficient. Run the provider-backed integration tests against SQL Server LocalDB on Windows, or set `ZEUS_SQLSERVER_CONNECTION` explicitly in another environment. Use a unique test database name and never point tests at a shared development database without isolation.

Then inspect the migration lifecycle:

```powershell
dotnet ef migrations list --project src/features/RegisterAcademic/RegisterAcademic.csproj
 dotnet ef migrations script --project src/features/RegisterAcademic/RegisterAcademic.csproj --output register-academic.sql
```

The migration list must discover the expected migration. The generated SQL must contain the academic, qualification, and extension-linkage schema with the intended keys and constraints. Apply the migration to a fresh test database, execute the registration, create a fresh context, and read the record back. This catches missing migration artifacts, incorrect ownership, and mappings that only work inside one context.

### Review the failure path

Inspect the exception-to-response mapping for argument and normalization failures, validation errors, and conflicts. The handler must not translate every `DbUpdateException` into a duplicate response. It should translate only the proven conflict path and preserve unrelated persistence failures for diagnosis.

Finally, perform a scaffold audit. Remove starter files, align each C# filename with its primary type, keep one primary type per file, and confirm the solution contains each new project exactly once.

## Showcase the New Feature

The showcase starts with reference data already loaded: a valid rank, degree, and active university exist, and ProvisionExtension has created an unassigned extension. I also start the API against an isolated SQL Server database with the RegisterAcademic migration applied.

1. Submit a valid registration request with a unique six-character `empNr`, a name of 15 characters or fewer, a valid rank, one degree and university pair, and the available extension.
   Expected result: the API returns the documented success response, and the database contains one academic with the rank-derived access level, qualification, and extension assignment.
2. Retrieve the created academic through the available profile or list query.
   Expected result: the record is immediately visible with its canonical university code and derived access level.
3. Submit the same employee number again.
   Expected result: the API returns the documented validation or conflict response, and the database still contains only one academic for that `empNr`.
4. Attempt registration with the already assigned extension.
   Expected result: the request fails cleanly, and no second academic or partial qualification is created.
5. Inspect the test output, HTTP responses, and fresh-context database read-back.
   Expected result: the evidence shows both the user-visible behavior and the persistence guarantee.

The value demonstrated is simple and important: downstream academic-lifecycle slices now have a real, complete source record, while identity, reference-data, and extension invariants remain protected.

## What the Prompt Teaches

The RegisterAcademic prompt demonstrates a repeatable pattern for AI-assisted vertical-slice delivery. It starts with dependencies and boundaries, assigns work to specialized roles, and makes verification part of the implementation rather than an afterthought.

It also keeps architecture visible during execution. Public reference-data contracts prevent direct persistence coupling, feature-local contexts make migration ownership explicit, and integration tests prove that the operation behaves correctly across a real provider boundary. Those decisions reduce the chance that a fast generated implementation becomes a difficult dependency for every later slice.

## What's Next?

The next academic-lifecycle step is to build the query and mutation slices that consume the registered record: profile viewing, name updates, search, employment changes, qualification updates, and extension reassignment. Each should reuse the same downstream-consumer pattern and begin with the RegisterAcademic verification evidence as its prerequisite.

## Feedback Loop

Feedback is always welcome. Send your thoughts to [john.miller@codemag.com](mailto:john.miller@codemag.com).

## Disclaimer

AI contributed to the writing of this post, but humans reviewed it, refined it, enhanced it, and gave it soul.

Prompts:

- "write a blog post explaining the #file:ep-2-1-register-academic-implementation.prompt.md, how to execute the prompt and how to verify the implementation and how to showcase the new feature."
