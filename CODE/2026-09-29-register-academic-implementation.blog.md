---
ai_generated: true
model: "github/copilot@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-09-29-register-academic-blog-execution-guide"
prompt: |
  write a blog post explaining the #file:ep-2-1-register-academic-implementation.prompt.md, how to execute the prompt and how to verify the implementation and how to showcase the new feature.
revision_prompt: |
  add to the blog post an explaination of the implementation of slice #file:ep-2-1-register-academic-implementation.prompt.md.
started: "2026-09-29T15:13:58-07:00"
ended: "2026-09-29T15:22:34-07:00"
task_durations:
  - task: "inspect prompt and implementation contracts"
    duration: "00:01:30"
  - task: "write execution and showcase guide"
    duration: "00:02:30"
  - task: "validate links and provenance"
    duration: "00:01:26"
  - task: "add implementation explanation and validate references"
    duration: "00:03:10"
total_duration: "00:08:36"
ai_log: "ai-logs/2026/09/29/2026-09-29-register-academic-blog-execution-guide/conversation.md"
source: ".github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md"
layout: post
title: "Executing, Verifying, and Understanding the RegisterAcademic Slice"
date: 2026-09-29
categories: [ai-assisted-development, software-engineering, architecture]
tags: [vertical-slices, aspnet-core, mediatr, ef-core, sql-server, testing]
excerpt: "A hands-on guide to the RegisterAcademic implementation, its verification gates, and a working API demonstration."
description: "Learn how the RegisterAcademic code path validates and resolves references, atomically claims an extension, and proves its behavior through route and SQL Server tests."
image: /assets/images/2026-09-29/register-academic-implementation.svg
---

A registration endpoint is only useful when the record it creates is complete, consistent, and safe to consume from the next feature. The RegisterAcademic prompt makes those requirements executable: it assigns implementation roles, defines the HTTP and persistence contract, and requires SQL Server evidence before dependent academic slices proceed.

<!--more-->

<img src="https://epsenterprise.blob.core.windows.net/permanent-files/FileAttachments/cdbdde06_02c0_4cbf_8adb_aac5ee8ac3e3/AIAGSD12_Header_Large.png" alt="AIAGSD12_Header_Large.png" />

## Why RegisterAcademic Is a Delivery Gate

RegisterAcademic is the first command that turns reference data into a durable academic lifecycle record. It needs a rank, at least one degree and university, and an available extension. If any part is missing or assigned independently, a later profile or employment operation can encounter incomplete state.

The [EP-2-1 implementation prompt](https://github.com/JohnMichaelMiller/zeus.academia.3b/blob/main/.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md) addresses that risk with a Contract Sheet, explicit persistence ownership, concurrency rules, route-level status requirements, and a final traceability table. It is more than a code-generation request: it defines what evidence must exist before dependent slices can start.

The delivery boundary is precise. RegisterAcademic creates an academic with an exactly six-character `empNr`, a normalized name of one to 15 characters, a rank-derived access level, one or more unique qualification pairs, and one available extension. It does not implement profile viewing, later employment changes, extension reassignment, or reporting.

## Read the Prompt as an Execution Contract

The prompt has four useful layers. The Contract Sheet defines each request field, normalization rule, canonical owner, and HTTP result. The persistence section identifies which context owns each table and which contexts only reuse mappings. The ordered steps specify the implementation sequence, and the test matrix translates the contract into validator, route, database, and concurrency evidence.

Several boundaries are easy to miss if an agent reads only the endpoint requirements. The `RegisterAcademicDbContext` is mapping-only and owns no migrations. `SharedKernelDbContext` remains the owner of `Academics` and `AcademicQualifications`; `ProvisionExtensionDbContext` remains the owner of `Extensions`. The handler must resolve rank, degree, and university through their approved public contracts, and it must not inject another feature's DbContext. Degree formatting alone does not prove that a degree code exists.

The prompt also specifies an atomic extension claim. A relational handler must conditionally update the extension only when it is still unassigned, inside a transaction with academic and qualification writes. A zero-row claim becomes `ExtensionUnavailable`. A duplicate employee number maps to a conflict only after rollback and a narrow existence check; unrelated database failures must not be mislabeled as duplicates.

## Prepare Before You Execute

I start by opening the prompt and each context file listed at its top: the ORM constraints, execution plan, migration ownership matrix, AI development process, C# and ASP.NET Core standards, MediatR and FluentValidation standards, xUnit standards, and `eng/verify-slice.ps1`. That sequence prevents the implementation from drifting away from the actual model, route, or migration owners.

The prerequisite set includes ManageRanks, ManageDegrees, ManageUniversities, ProvisionExtension, and Shared Kernel. The prompt also requires two bounded increments: add a public `GetDegreeByCodeQuery` contract under ManageDegrees, and align Shared Kernel so `empNr` is exactly six characters and an academic cannot be created without a qualification. Before editing, record the branch and base reference, inspect the ownership matrix and stored employee-number values, and run each prerequisite feature gate. Stop if an existing employee or extension-assignee value violates the new exact-length rule; do not convert that risk into an unreviewed migration.

The prompt's structure prevents two recurring implementation failures: convenient but invalid shortcuts across feature-owned data, and tests that prove only that code compiles. Its traceability requirement ties each Contract Sheet row to implementing files and named tests, so reviewers can identify a missing behavior without reverse-engineering the entire slice.

## Execute the Prompt in Agent Mode

The prompt declares `mode: agent` and lists the tools and roles it needs. In VS Code Copilot Chat, select an agent-capable mode, open the prompt, and invoke it by its repository path. I use an instruction that asks the agent to follow the prompt's role boundaries and stop on blockers:

*Execute #file:ep-2-1-register-academic-implementation.prompt.md from the repository root. Read every context file named in it before editing. Follow its ordered steps and role boundaries, preserve the recorded branch/base and migration ownership, stop if prerequisites or identifier data are incompatible, and do not report completion without the required SQL Server evidence and criterion-to-test traceability table.*

Treat this as one coordinated workflow, not permission to skip a role. First have the coordinator record branch/base, inspect stored identifier lengths, confirm reference data, and run prerequisite gates. Then hand off Shared Kernel and persistence changes to the appropriate domain and persistence roles. Implement the public degree lookup before relying on it. Only after model, migrations, and feature-local mapping are aligned should the handler, endpoint, and host composition be completed. Finally, testing-verification runs the test matrix and both mechanical gates.

The explicit feature set matters because the slice changes more than RegisterAcademic. It includes the bounded ManageDegrees lookup, Shared Kernel field rules, and ProvisionExtension schema. Git-based changed-feature discovery is supplementary; an empty discovered set does not prove the manifest impact set was checked.

## How the RegisterAcademic Implementation Works

The implementation follows the request from the HTTP boundary into the domain and then across the persistence boundary. Each layer has a narrow job: the endpoint owns HTTP translation, the validator provides early field feedback, the handler coordinates reference data and the write, and the Shared Kernel aggregate protects domain invariants.

### Define the request and validate its shape

[`RegisterAcademicCommand`](../src/features/Academics/RegisterAcademic/Register/RegisterAcademicCommand.cs) is the MediatR request. It carries the employee number and name, rank code, a collection of degree/university pairs, the extension number, and optional tenure or contract details. The response returns the normalized academic identity, rank and derived access level, employment state, qualifications, and extension number.

[`RegisterAcademicCommandValidator`](../src/features/Academics/RegisterAcademic/Register/RegisterAcademicCommandValidator.cs) rejects missing strings, employee numbers that are not exactly six characters after trimming, names outside one to 15 characters, unsupported rank codes, empty or duplicate qualification pairs, malformed reference codes, nonpositive extension numbers, and a request that combines tenure with a contract end date. This is fast request feedback; it does not replace existence checks or aggregate invariants.

### Resolve references and build the aggregate

[`RegisterAcademicHandler`](../src/features/Academics/RegisterAcademic/Register/RegisterAcademicHandler.cs) uses `RankCodeCatalog` for the closed rank set and sends `GetDegreeByCodeQuery` and `GetUniversityByCodeQuery` through MediatR. It checks that each reference exists, then checks that a found university is active. It uses the canonical codes from those query responses to create `Degree` and `University` values, rather than trusting formatted input as proof that a catalog entry exists.

The handler normalizes `empNr` and `empName`, rejects duplicate qualification pairs, checks that the requested extension exists and appears available, and checks for an existing normalized employee number. It then calls [`Academic.Create`](../src/features/SharedKernel/Foundation/Domain/Academic.cs). The aggregate applies the same normalization and length rules, requires at least one qualification, rejects duplicate pairs and the mutually exclusive tenure/contract combination, and creates the qualification entities. `AccessLevel` is computed from `Rank`; callers cannot submit a separate access-level value that could disagree with the rank.

### Claim the extension and persist atomically

An availability read by itself cannot prevent two requests from selecting the same extension. For relational providers, the handler starts a database transaction and performs a conditional `ExecuteUpdateAsync` that sets `AssignedEmpNr` only when the requested extension still has no assignee. If the update affects zero rows, the handler returns `ExtensionUnavailable`; otherwise it adds the academic aggregate, saves the academic and its qualifications, and commits. This single conditional update is the concurrency boundary, while the transaction keeps the extension claim and academic write together.

If a database update fails, the handler rolls the transaction back and clears tracked state before checking for the specific duplicate employee or extension assignment it can translate. It rethrows unrelated database failures instead of reporting every persistence error as a conflict. For nonrelational providers, the handler uses the tracked extension and aggregate save path; production concurrency evidence therefore comes from the SQL Server test, not an in-memory test alone.

[`RegisterAcademicDbContext`](../src/features/Academics/RegisterAcademic/Persistence/RegisterAcademicDbContext.cs) applies the Shared Kernel entity configurations for academics, qualifications, and extensions. It marks those tables `ExcludeFromMigrations()`, so it can query and write the model without becoming a second migration owner. Schema changes stay with `SharedKernelDbContext` and `ProvisionExtensionDbContext`.

### Translate the result into HTTP

[`RegisterAcademicEndpoints`](../src/features/Academics/RegisterAcademic/RegisterAcademicEndpoints.cs) maps `POST /api/academics/register` under `/api/academics`. It validates before dispatching the command, turns invalid fields and invalid references into a 400 validation problem, maps duplicate employees and unavailable extensions to 409, and returns 201 with a `Location` header when registration succeeds. The response includes the access level derived from the saved rank and the canonical qualification codes.

### Match tests to the layers

The tests cover different boundaries rather than repeating the same assertion at each layer:

| Test surface | What it demonstrates |
| --- | --- |
| [Validator tests](../tests/Features/Academics/RegisterAcademic/RegisterAcademicCommandValidatorTests.cs) | Request shape, boundary lengths, rank format, qualification rules, and employment-field validation |
| [Handler tests](../tests/Features/Academics/RegisterAcademic/RegisterAcademicHandlerTests.cs) | Successful coordination, duplicate employee rejection, and unavailable-extension behavior |
| [Route tests](../tests/Features/Academics/RegisterAcademic/RegisterAcademicRouteTests.cs) | HTTP status, response location, validation problem keys, and conflict responses |
| [SQL Server integration tests](../tests/Features/Academics/RegisterAcademic/RegisterAcademicSqlServerIntegrationTests.cs) | Migration-backed persistence and a fresh-context read-back of the academic, qualification, and extension assignment |
| [SQL Server concurrency tests](../tests/Features/Academics/RegisterAcademic/RegisterAcademicSqlServerConcurrencyTests.cs) | Two independent contexts competing for one extension produce one success and one conflict |

Together these layers explain why the acceptance criteria insist on both a happy-path record and clean failure behavior. The API response proves the route contract; the SQL Server read-back proves the committed database state; the concurrency test proves the extension cannot be double-claimed under a race.

## Verify the Slice from Contracts to SQL Server

Verification starts with narrow tests and ends with the repository gate. The implementation prompt requires actual SQL Server tests, route-level tests for declared statuses, migration discovery and application, a build, and traceability. Unit tests or EF Core InMemory tests alone do not prove this persistence boundary.

Run focused feature tests while iterating. The current registration test project is:

```powershell
dotnet test tests/Features/Academics/RegisterAcademic/Zeus.Academia.Tests.Features.Academics.RegisterAcademic.csproj
```

Also run the Shared Kernel and ManageDegrees tests affected by the bounded increments. Confirm that the validator covers the listed boundaries (including `empNr` lengths 5, 6, and 7; name lengths 15 and 16; missing or duplicate qualifications; invalid rank; invalid extension; and mutually exclusive employment fields). Route tests must assert both status and response body shape for every declared outcome: 201, 400 validation problems, and 409 conflicts.

The SQL Server integration database must be unique per test run. Its setup applies migrations from each owning context, not from the mapping-only registration context. After a write, use a fresh context to read the academic, qualifications, derived access level, and extension assignment. For rejected requests, prove there are no partial writes. Cleanup belongs in `finally` or deterministic async disposal, even when setup or assertions fail.

Concurrency needs a provider-backed race test, not just two sequential requests. Start two handlers using separate contexts and the same unassigned extension. Exactly one may succeed; the other must return `ExtensionUnavailable`. A fresh third context must see one academic and the extension assigned to the winner. Repeat duplicate-employee verification and confirm the losing operation does not retain a claimed extension.

Run the explicit impact gate and the full changed-feature gate from the repository root:

```powershell
pwsh eng/verify-slice.ps1 -Features Academics/RegisterAcademic,ReferenceData/ManageDegrees,SharedKernel/Foundation,Extensions/ProvisionExtension
pwsh eng/verify-slice.ps1 -AllChangedFeatures
```

The script checks repository composition, route and validator tests, migration ownership and artifacts, Entity Framework drift, SQL Server prerequisites, concurrency guards, provenance, build, and feature tests. Do not add `-SkipTests` or `-SkipEfChecks` to turn a failed required check into a green handoff. If the base reference is unavailable, resolve it explicitly and retain the output; do not interpret an empty change set as success.

The final handoff contains the script output and a table with one row for every Contract Sheet requirement: criterion, implementation file(s), test name(s), and status. Keep RegisterAcademic as a hard dependency gate until every row is covered and the SQL Server checks pass.

## Showcase the Registration Endpoint

A useful demo begins with a clean database and known reference data: valid rank `P`, degree `BSC`, active university `MIT`, and extension `101` provisioned but unassigned. The test fixtures demonstrate the same shape; the live environment must be seeded through its supported setup rather than assuming the API creates catalogs implicitly. Start the API with the development SQL Server connection and use the route from the Contract Sheet, `POST /api/academics/register`.

This PowerShell request follows the current command contract. Replace the host and port with the API URL shown by the active launch profile:

```powershell
$baseUrl = 'https://localhost:<port>'
$body = @{
  empNr = 'A00001'
  empName = 'Ada Lovelace'
  rankCode = 'P'
  qualifications = @(
    @{
      degreeCode = 'BSC'
      universityCode = 'MIT'
    }
  )
  extNr = 101
  isTenured = $false
  contractEndDate = $null
} | ConvertTo-Json -Depth 5

$response = Invoke-WebRequest `
  -Uri "$baseUrl/api/academics/register" `
  -Method Post `
  -ContentType 'application/json' `
  -Body $body

$response.StatusCode
$response.Headers.Location
$response.Content
```

The expected result is `201 Created` with the response body and a `Location` for `/api/academics/A00001`. Read the record through the available academic query and inspect the assigned extension. Confirm that the stored employee number is normalized, the access level matches rank `P`, the qualification stores canonical codes, and the extension points to `A00001`.

Then demonstrate the protection rules. Repost the same request and expect 409 for the duplicate employee number. Change `empNr` to `A00002` but keep the now-assigned extension `101`; expect 409 and no second record. Finally, use a well-formed unknown degree code such as `XYZ`; expect 400 keyed to the degree field and no database writes. The HTTP outcomes matter, but the most convincing demo also queries persistence afterward to show that each failed command left state unchanged.

The result is a complete source record that subsequent academic slices can consume, with identity, reference data, and extension assignment enforced at the API, domain, and database boundaries.

## What's Next?

The next academic-lifecycle work can build profile, search, employment, and qualification operations on top of this registered record. Those slices should remain sequenced behind the explicit SQL Server and traceability evidence from EP-2-1.

## Feedback Loop

Feedback is welcome at [john.miller@codemag.com](mailto:john.miller@codemag.com).

## Disclaimer

AI contributed to the writing of this post, but humans reviewed it, refined it, enhanced it, and gave it soul.

Prompts:

- "write a blog post explaining the #file:ep-2-1-register-academic-implementation.prompt.md, how to execute the prompt and how to verify the implementation and how to showcase the new feature."
- "add to the blog post an explaination of the implementation of slice ep-2-1-register-academic-implementation.prompt.md."
