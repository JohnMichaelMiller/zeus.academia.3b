---
ai_generated: true
model: "anthropic/claude-opus-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-09-28-pr46-review-instruction-hardening"
prompt: |
  I want to improve the instruction files and prompts to generate better code that won't be flagged by the
  Copilot code review. review the review comments on pull request 46. propose improvements to the
  instruction files and prompts that will improve the quality of the generated code.
  go ahead
started: "2026-09-28T22:41:00Z"
ended: "2026-09-28T22:53:00Z"
task_durations:
  - task: "gather contract facts from repository"
    duration: "00:04:00"
  - task: "rewrite prompt with Contract Sheet"
    duration: "00:08:00"
total_duration: "00:12:00"
ai_log: "ai-logs/2026/09/28/2026-09-28-pr46-review-instruction-hardening/conversation.md"
source: ".github/instructions/implementation-prompt.instructions.md"
previous_version:
  chat_id: "616990b5-0c5d-4735-a876-23fd1ebb4ff6"
  ai_log: "ai-logs/2026/04/20/616990b5-0c5d-4735-a876-23fd1ebb4ff6/conversation.md"
name: implement-academia-ep-2-1-register-academic
description: Implement the RegisterAcademic slice, the first mandatory delivery gate for the academic lifecycle
author: John Miller
tags: [academia, implementation, academics, registration]
context: "Zeus Academia Phase 2 academic registration implementation"
expected_output: "A slice-scoped implementation plan for RegisterAcademic"
tools: ["read", "search", "edit", "agent"]
mode: agent
---

# Implement RegisterAcademic

## Slice Summary and Business Value

- Slice: RegisterAcademic
- Business outcome: create an academic with valid identity, rank-derived access level, at least one qualification, and one available extension so every dependent slice has a real source record.
- Out of scope: profile viewing, later employment changes, extension reassignment, and reporting.

## Context Files to Review First

- src/models/orm/academia.txt (authoritative field constraints)
- src/models/workflows/academia-execution-plan.md
- src/models/workflows/migration-ownership-matrix.md
- .github/instructions/ai-dev-process.instructions.md (Canonical Rule Owners, Mechanical Verification Gate)
- .github/instructions/vertical-slice-implementation.instructions.md
- .github/instructions/csharp-implementation.instructions.md
- .github/instructions/aspnetcore-implementation.instructions.md
- .github/instructions/mediatr-implementation.instructions.md
- .github/instructions/fluentvalidation-implementation.instructions.md
- .github/instructions/xunit-implementation.instructions.md
- eng/verify-slice.ps1

## Prerequisites and Dependency Checks

- Required prior slices: ManageRanks, ManageDegrees, ManageUniversities, ProvisionExtension.
- Blocking risks: this slice is the first hard dependency gate; do not parallelize dependent slices until registration passes SQL Server integration tests. Escalate if any existing `Academics`, `AcademicQualifications`, or `Extensions` row has an `EmpNr`/`AssignedEmpNr` that is not exactly 6 characters.
- Bounded prerequisite increments owned by this slice:
  1. ManageDegrees has no resolve-by-code contract. Add `GetDegreeByCodeQuery(string Code) : IRequest<GetDegreeByCodeResponse>` with `GetDegreeByCodeResponse(bool IsFound, string? Code)` under `src/features/ReferenceData/ManageDegrees/GetDegreeByCode/`, mirroring `GetUniversityByCodeQuery`.
  2. Shared Kernel alignment: `SharedKernelFieldLengths.EmpNr` becomes `6` and is an exact length; `Academic.Create` enforces exact `empNr` length and at least one qualification (it currently accepts an empty collection).
- Existing patterns to reuse: `GetUniversityByCodeQuery` (found then active), `RankCodeCatalog.TryParseRank`, `Extension.AssignTo`, `ProvisionExtensionSqlServerTestDatabase` harness shape.
- Prohibited: injecting `SharedKernelDbContext`, `ProvisionExtensionDbContext`, `ManageDegreesDbContext`, or `ManageUniversitiesDbContext` into RegisterAcademic; validating degree existence with `Degree.Create` alone.

## Contract Sheet

Route: `POST /api/academics/register` (existing route preserved).

### Request and response fields

| Field                             | Type                                      | Required             | Length/range                | Normalization | Canonical owner                                                       |
| --------------------------------- | ----------------------------------------- | -------------------- | --------------------------- | ------------- | --------------------------------------------------------------------- |
| `empNr`                           | string                                    | yes                  | exactly 6                   | trim, upper   | `SharedKernelFieldLengths.EmpNr` + `Academic.Create`                  |
| `empName`                         | string                                    | yes                  | 1–15                        | trim          | `SharedKernelFieldLengths.EmpName` + `Academic.Create`                |
| `rankCode`                        | string                                    | yes                  | `P`, `SL`, `L`              | trim, upper   | `RankCodeCatalog`                                                     |
| `qualifications`                  | array of `{ degreeCode, universityCode }` | yes                  | ≥ 1 item; no duplicate pair | per item      | `Academic.Create`                                                     |
| `qualifications[].degreeCode`     | string                                    | yes                  | 1–10                        | trim, upper   | `Degree.Create` (format) + `GetDegreeByCodeQuery` (existence)         |
| `qualifications[].universityCode` | string                                    | yes                  | 1–20                        | trim, upper   | `University.Create` (format) + `GetUniversityByCodeQuery` (existence) |
| `extNr`                           | int                                       | yes                  | > 0                         | none          | `Extension.Create`                                                    |
| `isTenured`                       | bool                                      | no (default `false`) | —                           | —             | `Academic.Create`                                                     |
| `contractEndDate`                 | `DateOnly?`                               | no                   | not with `isTenured = true` | —             | `Academic.Create`                                                     |

Response `201 Created`: `RegisterAcademicResponse(EmpNr, EmpName, RankCode, AccessLevel, IsTenured, ContractEndDate, Qualifications[{DegreeCode, UniversityCode}], ExtNr)`.

### Reference resolution

| Reference        | Owning slice                     | Public contract                                                                  | Not found           | Inactive/unavailable   |
| ---------------- | -------------------------------- | -------------------------------------------------------------------------------- | ------------------- | ---------------------- |
| `rankCode`       | ManageRanks                      | `RankCodeCatalog.TryParseRank` (closed enum catalog)                             | `InvalidRankCode`   | n/a                    |
| `degreeCode`     | ManageDegrees                    | `GetDegreeByCodeQuery` (new)                                                     | `InvalidDegree`     | n/a                    |
| `universityCode` | ManageUniversities               | `GetUniversityByCodeQuery`                                                       | `InvalidUniversity` | `UniversityNotActive`  |
| `extNr`          | ProvisionExtension (table owner) | `RegisterAcademicDbContext.Extensions` (mapping-only, Shared Kernel `Extension`) | `InvalidExtension`  | `ExtensionUnavailable` |

### Aggregate invariants

| Invariant                       | Aggregate enforcement                                     | Validator (early feedback)                         | DB constraint                                                       |
| ------------------------------- | --------------------------------------------------------- | -------------------------------------------------- | ------------------------------------------------------------------- |
| `empNr` exactly 6               | `Academic.Create`                                         | `.Length(SharedKernelFieldLengths.EmpNr)`          | `HasMaxLength(6)` + `CK_Academics_EmpNrLength` (`LEN([EmpNr]) = 6`) |
| `empName` ≤ 15                  | `Academic.Create`                                         | `.MaximumLength(SharedKernelFieldLengths.EmpName)` | `HasMaxLength(15)`                                                  |
| ≥ 1 qualification               | `Academic.Create` throws `BusinessRuleViolationException` | `NotEmpty()`                                       | none (deferred; see ledger)                                         |
| no duplicate qualification pair | `Academic.Create`                                         | unique (degreeCode, universityCode)                | existing `AcademicQualifications` key                               |
| tenured XOR contract            | `Academic.Create`                                         | cross-field rule                                   | `CK_Academics_EmploymentMutualExclusion`                            |
| `empNr` unique                  | handler pre-check                                         | —                                                  | PK `Academics.EmpNr`                                                |
| extension 1:1                   | `Extension.AssignTo`                                      | —                                                  | filtered unique index on `Extensions.AssignedEmpNr`                 |

Deferral ledger: ≥ 1 qualification — enforced now by aggregate + validator; not enforced in database; owning follow-up: RemoveDegreeRecord (ep-4-4); risk: direct SQL writes can orphan an academic; evidence: aggregate unit test + handler test.

### Error → HTTP status

| Error code                                                                                                                        | Status                                              | `Produces*`                                                         | Route test                                                                                                            |
| --------------------------------------------------------------------------------------------------------------------------------- | --------------------------------------------------- | ------------------------------------------------------------------- | --------------------------------------------------------------------------------------------------------------------- |
| FluentValidation failures                                                                                                         | 400                                                 | `.ProducesValidationProblem()`                                      | `Register_InvalidPayload_Returns400WithFieldErrors`                                                                   |
| `InvalidRankCode`, `InvalidDegree`, `InvalidUniversity`, `UniversityNotActive`, `InvalidExtension`, `InvalidAcademicRegistration` | 400 (validation problem keyed to the request field) | `.ProducesValidationProblem()`                                      | `Register_UnknownDegree_Returns400`, `Register_InactiveUniversity_Returns400`, `Register_UnknownExtension_Returns400` |
| `AcademicAlreadyExists`                                                                                                           | 409                                                 | `.ProducesProblem(StatusCodes.Status409Conflict)`                   | `Register_DuplicateEmpNr_Returns409`                                                                                  |
| `ExtensionUnavailable`                                                                                                            | 409                                                 | `.ProducesProblem(StatusCodes.Status409Conflict)`                   | `Register_AssignedExtension_Returns409`                                                                               |
| success                                                                                                                           | 201                                                 | `.Produces<RegisterAcademicResponse>(StatusCodes.Status201Created)` | `Register_ValidRequest_Returns201`                                                                                    |

Unmapped error codes are a defect; do not fall back to a generic 400 `Results.Problem`.

### Persistence

- Feature context: `RegisterAcademicDbContext` in `src/features/Academics/RegisterAcademic/Persistence/`. Applies `AcademicConfiguration`, `AcademicQualificationConfiguration`, and `ExtensionConfiguration`, each mapped with `ExcludeFromMigrations()`. It owns no migrations.
- Migration owners stay unchanged: `SharedKernelDbContext` → `Academics`, `AcademicQualifications`; `ProvisionExtensionDbContext` → `Extensions`.
- Atomicity: academic, qualifications, and extension assignment are saved with one `SaveChangesAsync` on `RegisterAcademicDbContext`.
- Shared Kernel configurations touched and required migrations (migration class + Designer + snapshot for each):
  - `SharedKernelDbContext`: `Academics.EmpNr` and `AcademicQualifications.EmpNr` to length 6 with `CK_Academics_EmpNrLength`; `AcademicQualifications.UniversityName` → `UniversityCode` via `RenameColumn` (not drop/add), length 20.
  - `ProvisionExtensionDbContext`: `Extensions.AssignedEmpNr` to length 6.
- Update `migration-ownership-matrix.md` to list `RegisterAcademicDbContext` as mapping-only.

### Host composition

- `src/Zeus.Academia.Api/Zeus.Academia.Api.csproj`: `<ProjectReference>` to `src/features/Academics/RegisterAcademic/Zeus.Academia.Features.Academics.RegisterAcademic.csproj`.
- `Program.cs`: `AddRegisterAcademicPersistence(connectionString)`, `AddRegisterAcademicMediatR()`, `MapRegisterAcademicEndpoints()`; no `MigrateAsync` for `RegisterAcademicDbContext`; add `public partial class Program;` if needed for `WebApplicationFactory<Program>`.
- `appsettings.json`: `ConnectionStrings:DefaultConnection` is empty; the LocalDB value moves to `appsettings.Development.json`.

### Test matrix

- Project: `tests/Features/Academics/RegisterAcademic/Zeus.Academia.Tests.Features.Academics.RegisterAcademic.csproj` with `Microsoft.EntityFrameworkCore.SqlServer`, `Microsoft.AspNetCore.Mvc.Testing`, and FluentValidation `TestHelper`.
- `RegisterAcademicCommandValidatorTests.cs`: null/empty/whitespace for each string; `empNr` lengths 5, 6, 7; `empName` lengths 15 and 16; invalid rank; empty and null `qualifications`; duplicate pair; `extNr` 0 and -1; tenured with contract date; one valid command.
- `RegisterAcademicEndpointsTests.cs` (`WebApplicationFactory<Program>` against an isolated SQL Server database): every route test in the Error → HTTP status table, asserting status and body shape.
- `RegisterAcademicSqlServerTestDatabase.cs` + `RegisterAcademicSqlServerIntegrationTests.cs`: unique database, `MigrateAsync` for every owner context (SharedKernel, ProvisionExtension, ManageDegrees, ManageUniversities), fresh-context read-back, no partial writes for each failure case, best-effort `EnsureDeletedAsync` in `finally`. No `EnsureCreated`.
- Shared Kernel tests: `Academic.Create` rejects empty qualifications and `empNr` of 5 and 7 characters; migration SQL contains `RenameColumn`-generated `sp_rename` for `UniversityName`.
- ManageDegrees tests: `GetDegreeByCodeQuery` found, not found, and normalization.

## Assigned Agents and Role Boundaries

| Role                 | Responsibilities                                                                                                        | Inputs                                                   | Outputs                                             | Escalate when                                                                                  |
| -------------------- | ----------------------------------------------------------------------------------------------------------------------- | -------------------------------------------------------- | --------------------------------------------------- | ---------------------------------------------------------------------------------------------- |
| slice-coordinator    | confirm prerequisites, data compatibility for `empNr` length 6, and ownership-matrix update                             | execution plan, ownership matrix, current database state | approved sequence and blocker list                  | existing data violates the 6-character rule or any prerequisite lacks integration proof        |
| backend-domain       | Shared Kernel alignment, `GetDegreeByCodeQuery`, command, validator, handler, endpoint status mapping, host composition | Contract Sheet, Shared Kernel, reference-data contracts  | code matching every Contract Sheet row              | a Contract Sheet row contradicts the ORM model or needs a Shared Kernel change not listed here |
| data-persistence     | `RegisterAcademicDbContext` (mapping-only) and the SharedKernel/ProvisionExtension migrations                           | Persistence section, ownership matrix                    | migrations with Designer + snapshot, updated matrix | a migration would drop data or a second context would own a table                              |
| testing-verification | test matrix, `eng/verify-slice.ps1`, traceability table                                                                 | implemented slice                                        | passing tests, script output, traceability table    | any criterion lacks a test or the script fails                                                 |

## Ordered Implementation Steps

1. Confirm prerequisites and data compatibility.
   Targets: ownership matrix, existing `EmpNr`/`AssignedEmpNr` values, reference-data availability.
   Owner: slice-coordinator.
   Validation before next step: no stored identifier violates the 6-character rule; blockers listed.
2. Align Shared Kernel invariants.
   Targets: `SharedKernelFieldLengths.cs`, `Academic.cs`, Shared Kernel tests.
   Owner: backend-domain.
   Validation before next step: `Academic.Create` rejects empty qualifications and non-6-character `empNr`; Shared Kernel tests pass.
3. Author migrations and the mapping-only feature context.
   Targets: SharedKernel and ProvisionExtension `Migrations/`, `RegisterAcademic/Persistence/RegisterAcademicDbContext.cs`, `migration-ownership-matrix.md`.
   Owner: data-persistence.
   Validation before next step: `dotnet ef migrations list` discovers the new migration for each owner context; generated SQL renames (not drops) `UniversityName`; `RegisterAcademicDbContext` produces no migration.
4. Add `GetDegreeByCodeQuery` to ManageDegrees.
   Targets: `ManageDegrees/GetDegreeByCode/`.
   Owner: backend-domain.
   Validation before next step: found/not-found tests pass.
5. Implement contract and validator exactly per the Contract Sheet.
   Targets: `RegisterAcademicCommand.cs`, `RegisterAcademicCommandValidator.cs`, `RegisterAcademicResponse.cs`.
   Owner: backend-domain.
   Validation before next step: every field row exists with its exact length/range; validator delegates to canonical owners.
6. Implement handler and endpoint.
   Targets: `RegisterAcademicHandler.cs`, `RegisterAcademicEndpoints.cs`.
   Owner: backend-domain.
   Validation before next step: every reference is resolved through its public contract; every error code maps to its declared status.
7. Compose the host.
   Targets: `Zeus.Academia.Api.csproj`, `Program.cs`, `appsettings.json`, `appsettings.Development.json`, `RegisterAcademicServiceCollectionExtensions.cs`.
   Owner: backend-domain.
   Validation before next step: `dotnet build zeus.academia.3b.sln` succeeds.
8. Implement the test matrix.
   Owner: testing-verification.
   Validation before next step: all listed test files exist and pass against SQL Server.
9. Run the mechanical gate and build the traceability table.
   Command: `pwsh eng/verify-slice.ps1 -Feature <f>` for `Academics/RegisterAcademic`, `ReferenceData/ManageDegrees`, `SharedKernel/Foundation`, `Extensions/ProvisionExtension`.
   Owner: testing-verification.
   Validation: any failure in a feature touched by this slice blocks handoff, including pre-existing failures. Escalate for explicit sign-off if a failure cannot be resolved in scope.

## Verification and Acceptance Criteria

- Given a valid request with an available extension, when it is posted, then the API returns 201, and a fresh context reads one academic with a 6-character `empNr`, derived access level, every submitted qualification, and the extension assigned to that `empNr`.
- Given `empNr` of 5 or 7 characters, the API returns 400 keyed to `empNr` and nothing is persisted.
- Given an empty `qualifications` array, the API returns 400; calling `Academic.Create` directly with no qualifications throws.
- Given a well-formed but unknown degree code (`XYZ`), the API returns 400 keyed to the degree field and nothing is persisted.
- Given an unknown or inactive university, the API returns 400 with `InvalidUniversity` or `UniversityNotActive` and the canonical university code (not name) is what gets persisted on success.
- Given an unknown `extNr`, the API returns 400; given an already assigned `extNr`, the API returns 409; neither case writes an academic.
- Given a duplicate `empNr`, the API returns 409 and the existing academic is unchanged.
- The SharedKernel and ProvisionExtension migrations are discovered by EF, apply to a fresh SQL Server database, and preserve `AcademicQualifications` data through the column rename.
- The host builds, maps the route, and `appsettings.json` contains no LocalDB connection string.
- `eng/verify-slice.ps1` passes for `Academics/RegisterAcademic`.

Handoff must include the traceability table: `criterion / Contract Sheet row | implementing file(s) | test name(s) | status`.

## Human Showcase Steps

1. Starting state: reference data is seeded and extension `101` is provisioned and unassigned.
   Action: `POST /api/academics/register` with `empNr` `A00001`, one qualification, and `extNr` `101`.
   Expected result: 201 with the response body; extension `101` now shows as assigned.
   Value demonstrated: a complete academic record exists for every dependent slice.
2. Starting state: step 1 completed.
   Action: resubmit with `empNr` `A00001`, then submit a new `empNr` with `extNr` `101`, then submit a degree code `XYZ`.
   Expected result: 409, 409, and 400 respectively; no new rows.
   Value demonstrated: identity, extension, and reference-data rules hold at the API boundary.

## Completion Checklist

- [ ] Every Contract Sheet row is implemented and appears in the traceability table with a test.
- [ ] No RegisterAcademic file references `SharedKernelDbContext` or another feature's DbContext.
- [ ] Degree, university, rank, and extension references are resolved through their public contracts, not value-object creation alone.
- [ ] `Academic.Create` enforces exact `empNr` length and at least one qualification.
- [ ] SharedKernel and ProvisionExtension migrations include migration class, Designer, and snapshot, and are discovered by EF.
- [ ] Every declared status code has a route-level test.
- [ ] `RegisterAcademicCommandValidatorTests.cs` covers every validator branch listed in the test matrix.
- [ ] Host `.csproj` references the feature project; the solution builds; `appsettings.json` has no LocalDB value.
- [ ] `eng/verify-slice.ps1` output is attached to the handoff.
- [ ] Dependent slices stay blocked until registration verification passes.
