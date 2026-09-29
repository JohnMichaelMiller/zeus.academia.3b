---
ai_generated: true
model: "github/copilot@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "edaf3bc8-e80f-4b74-b582-2aa3785f6a40"
prompt: |
    #file:ep-2-1-register-academic-implementation.prompt.md
started: "2026-09-28T23:48:22.455Z"
ended: "2026-09-29T00:37:03Z"
task_durations:
    - task: "contract and repository analysis"
        duration: "00:07:00"
    - task: "feature and host implementation"
        duration: "00:13:00"
    - task: "migrations and SQL Server tests"
        duration: "00:21:00"
    - task: "mechanical gates and handoff"
        duration: "00:07:00"
total_duration: "00:48:00"
ai_log: "ai-logs/2026/09/28/edaf3bc8-e80f-4b74-b582-2aa3785f6a40/conversation.md"
source: ".github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md"
previous_version:
    chat_id: "phase-0-step-6-downstream-documentation"
    ai_log: "ai-logs/2026/08/24/phase-0-step-6-downstream-documentation/conversation.md"
description: "Handoff document for EP-2-1 RegisterAcademic implementation"
---

# EP-2-1: RegisterAcademic Handoff Document

**Phase**: Phase 1
**Status**: BLOCKED until Phase 0 Complete ✅ — Now Ready
**Last Updated**: August 24, 2026
**Owner**: TBD (assign to implementation team)

## Executive Summary

RegisterAcademic is the **first downstream slice** that depends on the completed Phase 0 foundation. It implements the ability to register a new academic (faculty/staff member) in the system with rank and employment status.

This handoff document provides:

- ✅ Scope and business requirements
- ✅ Prerequisites (all Phase 0 items must be complete)
- ✅ Implementation steps (with exact file structure)
- ✅ Common mistakes to avoid
- ✅ Verification checklist before PR review
- ✅ Links to reference patterns

## Scope: What RegisterAcademic Implements

### Capabilities

1. **Register Academic** — Create a new academic record with:
   - Employee number (immutable, stable identifier)
   - Rank (from ManageRanks catalog)
   - Employment status (tenure track or contract)
   - Auto-computed AccessLevel (derived from Rank)

2. **Query Academic** — Retrieve academic details by employee number:
   - Full employment state
   - Current qualifications (via future episodes)
   - Extensions (via future episodes)

3. **Constraints Enforced**:
   - No duplicate employee numbers (unique constraint)
   - Rank must exist in ManageRanks catalog (via GetRankByCodeQuery)
   - Mutual exclusion: Cannot be both tenured AND on contract (domain invariant)
   - AccessLevel auto-computed from Rank (read-only, derived)

### NOT in Scope (Later Slices)

- Recording qualifications (→ RecordQualification, Phase 1 parallel)
- Managing rank changes (→ Future episode)
- Extension provisioning (→ ProvisionExtension, Phase 1)
- Deactivating academics (→ Future episode)

## Prerequisites Completed by Phase 0 ✅

- ✅ **Shared Kernel**
  - Academic aggregate (factory method, state management)
  - AcademicQualification entity (qualifications collection)
  - Extension entity (for future use)
  - Rank, Degree, University, Extension value objects
  - Configurations for all entities (reusable by feature-local contexts)
  - Result/Error types and base domain exceptions

- ✅ **Application Host** (src/Zeus.Academia.Api)
  - Program.cs with DI composition root
  - SQL Server configuration (with LocalDB fallback on Windows)
  - MediatR and FluentValidation registration
  - Health check endpoint
  - Migration orchestration strategy

- ✅ **ManageRanks** (Phase 0 reference data)
  - GetRankByCodeQuery + GetRankByCodeResponse contract
  - RanksDbContext (owns Ranks table)
  - AddManageRanksPersistence() extension

- ✅ **ManageDegrees** (Phase 0 reference data)
  - GetDegreeByCodeQuery + GetDegreeByCodeResponse contract
  - DegreesDbContext (owns Degrees table)
  - AddManageDegreesPersistence() extension

- ✅ **Pattern Documentation**
  - [Phase 1+ Downstream Consumer Pattern](./phase-1-downstream-consumer-pattern.md) — canonical pattern all features follow
  - [Shared Kernel Persistence Boundaries](../features/SharedKernel/PERSISTENCE_BOUNDARIES.md) — scope and constraints
  - [University Identity Resolution Contract](../features/ReferenceData/ManageUniversities/UNIVERSITY_RESOLUTION_CONTRACT.md) — reference data mapping pattern

## Implementation Roadmap

### Phase 1a (Parallel): Reference Data Prerequisites

Before RegisterAcademic implementation can complete, two reference data slices must be finished:

- **EP-1-3: ManageUniversities** — Creates Universities catalog (status: planned)
  - Delivers: GetUniversityByCodeQuery, UniversitiesDbContext
  - Needed by: RecordQualification (qualifications require university)

- **EP-1-4: ProvisionExtension** — Creates Extensions persistence (status: planned)
  - Delivers: Extension entity persistence, handlers
  - Needed by: Future slices that assign extensions

### Phase 1b (Sequential): Domain Slices

After reference data ready:

1. **EP-2-1: RegisterAcademic** ← YOU ARE HERE
   - Implement: RegisterAcademic command/handler, Academic aggregate persistence
   - Dependencies: Shared Kernel ✅, ManageRanks ✅, Application Host ✅

2. **EP-2-2: RecordQualification** (parallel with future episodes)
   - Implement: RecordQualification command/handler
   - Dependencies: RegisterAcademic ✅, ManageDegrees ✅, ManageUniversities (EP-1-3)

## Implementation Steps

### Step 1: Create Feature Directory Structure

```
src/features/
├── RegisterAcademic/                      (← new feature folder)
│   ├── RegisterAcademic.csproj           (← new project)
│   ├── Register/
│   │   ├── RegisterAcademicCommand.cs
│   │   ├── RegisterAcademicCommandHandler.cs
│   │   ├── RegisterAcademicCommandValidator.cs
│   │   └── RegisterAcademicResponse.cs
│   ├── GetAcademic/
│   │   ├── GetAcademicByEmpNrQuery.cs
│   │   ├── GetAcademicByEmpNrQueryHandler.cs
│   │   └── GetAcademicByEmpNrResponse.cs
│   ├── Persistence/
│   │   ├── RegisterAcademicDbContext.cs
│   │   ├── AcademicConfiguration.cs (reuse from SK)
│   │   ├── RegisterAcademicServiceCollectionExtensions.cs
│   │   └── Migrations/
│   │       └── (empty, will be generated by `dotnet ef migrations add`)
│   ├── Endpoints/
│   │   ├── RegisterAcademicEndpoint.cs
│   │   └── GetAcademicEndpoint.cs
│   └── Tests/
│       ├── RegisterAcademic.Tests.csproj
│       ├── Unit/
│       │   ├── AcademicAggregateTests.cs
│       │   └── RegisterAcademicCommandValidatorTests.cs
│       └── Integration/
│           ├── RegisterAcademicCommandHandlerTests.cs
│           └── GetAcademicByEmpNrQueryHandlerTests.cs

tests/
└── Features/
    └── RegisterAcademic/
        ├── RegisterAcademicCommandHandlerTests.cs
        ├── GetAcademicQueryHandlerTests.cs
        └── AcademicAggregateTests.cs
```

### Step 2: Define Commands & Queries (MediatR Contracts)

```csharp
// RegisterAcademic/Register/RegisterAcademicCommand.cs
namespace Zeus.Academia.Features.RegisterAcademic.Register;

public record RegisterAcademicCommand(
    int EmpNr,
    string RankCode,
    bool IsTenured
) : IRequest<Result<RegisterAcademicResponse>>;

public record RegisterAcademicResponse(
    int EmpNr,
    string RankCode,
    string AccessLevel,
    bool IsTenured,
    DateTime CreatedAt
);
```

```csharp
// RegisterAcademic/GetAcademic/GetAcademicByEmpNrQuery.cs
namespace Zeus.Academia.Features.RegisterAcademic.GetAcademic;

public record GetAcademicByEmpNrQuery(int EmpNr) : IRequest<GetAcademicByEmpNrResponse>;

public record GetAcademicByEmpNrResponse(
    bool IsFound,
    int? EmpNr,
    string? RankCode,
    string? AccessLevel,
    bool? IsTenured,
    DateTime? CreatedAt
);
```

### Step 3: Create Feature-Local DbContext

```csharp
// RegisterAcademic/Persistence/RegisterAcademicDbContext.cs
namespace Zeus.Academia.Features.RegisterAcademic.Persistence;

public class RegisterAcademicDbContext : DbContext
{
    public RegisterAcademicDbContext(DbContextOptions<RegisterAcademicDbContext> options)
        : base(options)
    {
    }

    public DbSet<Academic> Academics { get; set; } = default!;
    public DbSet<AcademicQualification> Qualifications { get; set; } = default!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // PATTERN: Reuse Shared Kernel configurations
        modelBuilder.ApplyConfiguration(new AcademicConfiguration());
        modelBuilder.ApplyConfiguration(new AcademicQualificationConfiguration());
    }
}
```

### Step 4: Implement Handler

```csharp
// RegisterAcademic/Register/RegisterAcademicCommandHandler.cs
namespace Zeus.Academia.Features.RegisterAcademic.Register;

public class RegisterAcademicCommandHandler : IRequestHandler<RegisterAcademicCommand, Result<RegisterAcademicResponse>>
{
    private readonly IMediator _mediator;
    private readonly RegisterAcademicDbContext _context;

    public RegisterAcademicCommandHandler(
        IMediator mediator,
        RegisterAcademicDbContext context)
    {
        _mediator = mediator;
        _context = context;
    }

    public async Task<Result<RegisterAcademicResponse>> Handle(
        RegisterAcademicCommand cmd,
        CancellationToken ct)
    {
        // Step 1: Validate command (FluentValidation runs before this)

        // Step 2: Check for duplicate employee
        var existing = await _context.Academics
            .FirstOrDefaultAsync(a => a.EmpNr == cmd.EmpNr, ct);

        if (existing is not null)
            return Result<RegisterAcademicResponse>.Failure(
                Error.Create("DuplicateEmployee", $"Employee {cmd.EmpNr} already registered"));

        // Step 3: Resolve rank from ManageRanks catalog
        var rankQuery = new GetRankByCodeQuery(cmd.RankCode);
        var rankResponse = await _mediator.Send(rankQuery, ct);

        if (!rankResponse.IsFound)
            return Result<RegisterAcademicResponse>.Failure(
                Error.Create("InvalidRank", $"Rank {cmd.RankCode} not found in catalog"));

        // Step 4: Create academic aggregate
        var academic = Academic.Create(
            cmd.EmpNr,
            rankCode: cmd.RankCode,
            isTenured: cmd.IsTenured
        );

        // Step 5: Persist
        try
        {
            _context.Academics.Add(academic);
            await _context.SaveChangesAsync(ct);
        }
        catch (DbUpdateException ex) when (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
        {
            return Result<RegisterAcademicResponse>.Failure(
                Error.Create("EmployeeAlreadyExists", $"Employee {cmd.EmpNr} already registered"));
        }

        // Step 6: Return response
        return Result<RegisterAcademicResponse>.Success(
            new RegisterAcademicResponse(
                academic.EmpNr,
                cmd.RankCode,
                academic.AccessLevel.ToString(),  // Auto-computed from rank
                academic.IsTenured,
                DateTime.UtcNow
            )
        );
    }
}
```

### Step 5: Create Validator

```csharp
// RegisterAcademic/Register/RegisterAcademicCommandValidator.cs
namespace Zeus.Academia.Features.RegisterAcademic.Register;

public class RegisterAcademicCommandValidator : AbstractValidator<RegisterAcademicCommand>
{
    public RegisterAcademicCommandValidator()
    {
        RuleFor(cmd => cmd.EmpNr)
            .GreaterThan(0)
            .WithMessage("Employee number must be positive");

        RuleFor(cmd => cmd.RankCode)
            .NotEmpty()
            .WithMessage("Rank code is required")
            .MaximumLength(20)
            .WithMessage("Rank code cannot exceed 20 characters");

        // Note: IsTenured is boolean (always valid), no explicit validation needed
        // Domain invariant (tenure XOR contract) is enforced in Academic.Create()
    }
}
```

### Step 6: Register in Application Host

```csharp
// In src/Zeus.Academia.Api/Program.cs

var builder = WebApplication.CreateBuilder(args);

// Phase 0: Core infrastructure
builder.Services.AddSharedKernel();

// Phase 0: Reference data features
builder.Services.AddManageRanksPersistence(builder.Configuration);
builder.Services.AddManageDegreesPersistence(builder.Configuration);

// Phase 1: Domain features
builder.Services.AddRegisterAcademicPersistence(builder.Configuration);      // ← Add this
builder.Services.AddRegisterAcademicMediatR();                               // ← Add this

var app = builder.Build();
app.Run();
```

### Step 7: Add Endpoints (REST API)

```csharp
// RegisterAcademic/Endpoints/RegisterAcademicEndpoint.cs
namespace Zeus.Academia.Features.RegisterAcademic.Endpoints;

public class RegisterAcademicEndpoint : Endpoint<RegisterAcademicRequest, RegisterAcademicResponse>
{
    private readonly IMediator _mediator;

    public RegisterAcademicEndpoint(IMediator mediator) => _mediator = mediator;

    public override void Configure()
    {
        Post("/api/academics/register");
        AllowAnonymous();
    }

    public override async Task HandleAsync(RegisterAcademicRequest req, CancellationToken ct)
    {
        var cmd = new RegisterAcademicCommand(req.EmpNr, req.RankCode, req.IsTenured);
        var result = await _mediator.Send(cmd, ct);

        if (!result.IsSuccess)
        {
            ThrowError(result.Error.Message, (int)result.Error.Code, StatusCodes.Status400BadRequest);
        }

        await SendAsync(result.Value, StatusCodes.Status201Created, ct);
    }
}
```

### Step 8: Generate Migrations

```bash
# From src/features/RegisterAcademic/ directory
dotnet ef migrations add InitialAcademicSchema \
    --project RegisterAcademic.csproj \
    --startup-project ../../Zeus.Academia.Api/Zeus.Academia.Api.csproj \
    --context RegisterAcademicDbContext

# Verify migration
dotnet ef migrations list
```

The migration will create:

- `Academics` table (PK: EmpNr, columns: EmpNr, RankCode, IsTenured, AccessLevel, CreatedAt)
- `AcademicQualifications` table (PK: AcademicId + DegreeCode + UniversityCode)

### Step 9: Add Tests

**Unit Tests** (domain logic, no persistence):

```csharp
// Tests/Unit/AcademicAggregateTests.cs
public class AcademicAggregateTests
{
    [Fact]
    public void Create_WithValidInputs_Succeeds()
    {
        var academic = Academic.Create(12345, "PROF", isTenured: true);

        Assert.Equal(12345, academic.EmpNr);
        Assert.True(academic.IsTenured);
        Assert.False(academic.IsContract);
        Assert.Equal("PROF", academic.RankCode);
    }

    [Fact]
    public void Create_WithNegativeEmpNr_Throws()
    {
        var ex = Assert.Throws<ArgumentException>(
            () => Academic.Create(-1, "PROF", isTenured: true));
        Assert.Contains("EmpNr must be positive", ex.Message);
    }
}
```

**Integration Tests** (handler + persistence):

```csharp
// Tests/Integration/RegisterAcademicCommandHandlerTests.cs
public class RegisterAcademicCommandHandlerTests
{
    private readonly RegisterAcademicDbContext _context;
    private readonly Mock<IMediator> _mediatorMock;
    private readonly RegisterAcademicCommandHandler _handler;

    public RegisterAcademicCommandHandlerTests()
    {
        var options = new DbContextOptionsBuilder<RegisterAcademicDbContext>()
            .UseInMemoryDatabase($"test-{Guid.NewGuid()}")
            .Options;

        _context = new RegisterAcademicDbContext(options);
        _mediatorMock = new Mock<IMediator>();
        _handler = new RegisterAcademicCommandHandler(_mediatorMock.Object, _context);
    }

    [Fact]
    public async Task Handle_WithValidRank_PersistsAndReturnsSuccess()
    {
        // Setup: Mock catalog response
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetRankByCodeQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetRankByCodeResponse(true, "PROF", "Professor", 1, 3));

        // Act
        var cmd = new RegisterAcademicCommand(12345, "PROF", isTenured: true);
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.True(result.IsSuccess);
        Assert.Equal(12345, result.Value.EmpNr);

        var stored = await _context.Academics.FirstAsync();
        Assert.Equal(12345, stored.EmpNr);
        Assert.Equal("PROF", stored.RankCode);
    }

    [Fact]
    public async Task Handle_WithInvalidRank_ReturnsError()
    {
        // Setup
        _mediatorMock
            .Setup(m => m.Send(
                It.IsAny<GetRankByCodeQuery>(),
                It.IsAny<CancellationToken>()))
            .ReturnsAsync(new GetRankByCodeResponse(false, null, null, 0, 0));

        // Act
        var cmd = new RegisterAcademicCommand(12345, "INVALID", isTenured: true);
        var result = await _handler.Handle(cmd, CancellationToken.None);

        // Assert
        Assert.False(result.IsSuccess);
        Assert.Contains("InvalidRank", result.Error.Code);
    }
}
```

## Common Mistakes to Avoid ❌

### 1. **Direct DbContext Access to ManageRanks**

```csharp
// ❌ WRONG
public async Task Handle(RegisterAcademicCommand cmd, CancellationToken ct)
{
    var rank = await _manageRanksDbContext.Ranks
        .FirstOrDefaultAsync(r => r.Code == cmd.RankCode);
    // This breaks module boundaries!
}

// ✅ CORRECT
public async Task Handle(RegisterAcademicCommand cmd, CancellationToken ct)
{
    var rankQuery = new GetRankByCodeQuery(cmd.RankCode);
    var rankResponse = await _mediator.Send(rankQuery, ct);
    // Clean, decoupled, testable
}
```

### 2. **Creating Academic DbContext in Shared Kernel**

```csharp
// ❌ WRONG (creates in shared kernel)
public class SharedKernelDbContext : DbContext
{
    public DbSet<Academic> Academics { get; set; }  // DO NOT DO THIS
    public DbSet<Rank> Ranks { get; set; }          // Two contexts own same table!
}

// ✅ CORRECT (feature-local context)
public class RegisterAcademicDbContext : DbContext
{
    public DbSet<Academic> Academics { get; set; }  // Only here
}

public class ManageRanksDbContext : DbContext
{
    public DbSet<Rank> Ranks { get; set; }         // Only here
}
```

### 3. **Storing Rank Objects Instead of Codes**

```csharp
// ❌ WRONG (stores full object)
public class Academic
{
    public Rank Rank { get; set; }  // This is a reference; shouldn't be stored
    // If Rank table changes, Academic is affected
}

// ✅ CORRECT (stores code)
public class Academic
{
    public string RankCode { get; set; }  // Just the identifier (immutable)
    // Creates value object on-demand: Rank.Create(academic.RankCode)
}
```

### 4. **Duplicate Shared Kernel Configurations**

```csharp
// ❌ WRONG (duplicates configuration)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.Entity<Academic>().HasKey(a => a.EmpNr);
    modelBuilder.Entity<Academic>().HasMany(a => a.Qualifications);
    // This is duplicated in Shared Kernel!
}

// ✅ CORRECT (reuses configuration)
protected override void OnModelCreating(ModelBuilder modelBuilder)
{
    modelBuilder.ApplyConfiguration(new AcademicConfiguration());
    modelBuilder.ApplyConfiguration(new AcademicQualificationConfiguration());
    // Single source of truth
}
```

### 5. **No Validation Before Database Access**

```csharp
// ❌ WRONG
public async Task Handle(RegisterAcademicCommand cmd, CancellationToken ct)
{
    var academic = Academic.Create(cmd.EmpNr, cmd.RankCode, cmd.IsTenured);
    // Validation not guaranteed; could throw at DB commit
}

// ✅ CORRECT
public async Task Handle(RegisterAcademicCommand cmd, CancellationToken ct)
{
    // FluentValidation runs before handler
    // Rank is validated against catalog before creating
    var rankResponse = await _mediator.Send(new GetRankByCodeQuery(cmd.RankCode), ct);
    if (!rankResponse.IsFound)
        return Result.Failure(...);

    // Now create with known valid inputs
    var academic = Academic.Create(cmd.EmpNr, cmd.RankCode, cmd.IsTenured);
}
```

### 6. **Forgetting to Register in Application Host**

```csharp
// ❌ WRONG (feature never registered)
// Program.cs has no:
//   builder.Services.AddRegisterAcademicPersistence(builder.Configuration);
//   builder.Services.AddRegisterAcademicMediatR();

// ✅ CORRECT
builder.Services.AddRegisterAcademicPersistence(builder.Configuration);
builder.Services.AddRegisterAcademicMediatR();
// DI now knows how to resolve handlers, DbContext, validators
```

## Verification Checklist

Before opening a pull request, verify:

- [ ] Feature directory structure matches specification above
- [ ] RegisterAcademicDbContext compiles and owns Academic + AcademicQualification tables only
- [ ] RegisterAcademicCommand and handler successfully invoke GetRankByCodeQuery via IMediator
- [ ] Academic aggregate created with correct state (EmpNr, RankCode, IsTenured, AccessLevel)
- [ ] Initial migration generated: `dotnet ef migrations list` shows new migration
- [ ] No direct dependencies on ManageRanksDbContext or other feature contexts
- [ ] Feature registration (AddRegisterAcademicPersistence) works in Application Host
- [ ] All unit tests pass (domain logic)
- [ ] All integration tests pass (handler + mocked catalogs + in-memory DB)
- [ ] No compilation errors or warnings
- [ ] Code follows C# conventions and project naming standards
- [ ] Endpoints added and tested (manual or integration)
- [ ] README updated if adding public endpoints

## Quality Gates

### Technical Gates

- ✅ Solution builds (0 warnings, 0 errors)
- ✅ All tests pass (100% success rate)
- ✅ Feature isolated (no direct DbContext coupling to other features)
- ✅ Migrations clean (can be applied to fresh database)
- ✅ DI registration complete (no unresolved dependencies)

### Architectural Gates

- ✅ Follows Phase 1+ Downstream Consumer Pattern exactly
- ✅ Shared Kernel untouched (Shared Kernel configuration reused, not duplicated)
- ✅ Catalog queries used (not direct DbContext access)
- ✅ Domain logic in aggregates (not handlers)
- ✅ Proper error handling (catalog not found, duplicate employee)

### Code Quality Gates

- ✅ Commands/queries are simple, focused (MediatR contracts)
- ✅ Handlers are readable (5-10 logical steps max)
- ✅ Validators use FluentValidation (consistent patterns)
- ✅ Configuration reuse (no duplication of AcademicConfiguration)
- ✅ Tests cover happy path + error cases

## References

- [Phase 0 Refactoring Plan](./academia-refactoring-plan.md) — overall roadmap
- [Phase 1+ Downstream Consumer Pattern](./phase-1-downstream-consumer-pattern.md) — canonical pattern (MUST follow)
- [Shared Kernel Persistence Boundaries](../features/SharedKernel/PERSISTENCE_BOUNDARIES.md) — what Shared Kernel owns
- [Migration Ownership Matrix](./migration-ownership-matrix.md) — table ownership verification
- [University Identity Resolution Contract](../features/ReferenceData/ManageUniversities/UNIVERSITY_RESOLUTION_CONTRACT.md) — for RecordQualification (future)
- [Phase 0 Completion Checklist](./phase-0-completion-checklist.md) — verify Phase 0 prerequisites

## Contact & Escalation

- **Stuck on catalog query**: Refer to GetRankByCodeQuery pattern in ManageRanks feature
- **Unclear domain invariant**: Review Academic aggregate in Shared Kernel and ask for clarification
- **Migration conflicts**: Check [Migration Ownership Matrix](./migration-ownership-matrix.md)
- **DI registration issues**: Reference ManageRanks service collection extension pattern
- **Architecture question**: Contact slice coordinator or architecture team

---

**Status**: Implemented and SQL Server verified on September 28, 2026
**Delivered Surface**: `POST /api/academics/register`; no academic retrieval or update endpoint is included.

## Implementation Traceability

| Criterion / Contract Sheet row                                                                                                      | Implementing file(s)                                                                                                                             | Test name(s)                                                                                                                                                                                                                              | Status                  |
| ----------------------------------------------------------------------------------------------------------------------------------- | ------------------------------------------------------------------------------------------------------------------------------------------------ | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- | ----------------------- |
| Valid request returns 201; access level is rank-derived; qualifications and extension assignment persist                            | `src/features/Academics/RegisterAcademic/RegisterAcademicHandler.cs`, `RegisterAcademicEndpoints.cs`, `Persistence/RegisterAcademicDbContext.cs` | `Register_ValidRequest_Returns201`                                                                                                                                                                                                        | Passed                  |
| `empNr` is normalized and exactly six characters                                                                                    | `src/features/SharedKernel/Foundation/Domain/SharedKernelFieldLengths.cs`, `Academic.cs`; `RegisterAcademicCommandValidator.cs`                  | `Create_WithEmployeeNumberOfInvalidLength_ThrowsBusinessRuleViolationException`; `Validate_WithEmployeeNumberOfWrongLength_HasError`; `Register_InvalidPayload_Returns400WithFieldErrors`                                                 | Passed                  |
| At least one qualification; duplicate degree prohibited by the existing `(EmpNr, DegreeCode)` key                                   | `Academic.cs`; `RegisterAcademicCommandValidator.cs`                                                                                             | `Create_WithoutQualifications_ThrowsBusinessRuleViolationException`; `Create_WithSameDegreeMoreThanOnce_ThrowsBusinessRuleViolationException`; `Validate_WithNullOrEmptyQualifications_HasError`; `Validate_WithDuplicateDegree_HasError` | Passed                  |
| Well-formed but unknown degree returns 400 without writes                                                                           | `ManageDegrees/GetDegreeByCode/*`; `RegisterAcademicHandler.cs`; `RegisterAcademicEndpoints.cs`                                                  | `Handle_WithUnknownCode_ReturnsNotFound`; `Register_UnknownDegree_Returns400`                                                                                                                                                             | Passed                  |
| Unknown and inactive universities return 400; code, not name, persists                                                              | `RegisterAcademicHandler.cs`; `RegisterAcademicEndpoints.cs`; university owning query                                                            | `Register_UnknownUniversity_Returns400`; `Register_InactiveUniversity_Returns400`; success persistence assertions in `Register_ValidRequest_Returns201`                                                                                   | Passed                  |
| Unknown extension returns 400; unavailable extension returns 409; neither creates an academic                                       | `RegisterAcademicHandler.cs`; `RegisterAcademicEndpoints.cs`                                                                                     | `Register_UnknownExtension_Returns400`; `Register_AssignedExtension_Returns409`                                                                                                                                                           | Passed                  |
| Duplicate employee number returns 409 and preserves existing academic and extension state                                           | `RegisterAcademicHandler.cs`; `RegisterAcademicEndpoints.cs`                                                                                     | `Register_DuplicateEmpNr_Returns409`                                                                                                                                                                                                      | Passed                  |
| Shared Kernel alignment renames `UniversityName`, tightens employee number fields, and adds exact-length constraint                 | `src/features/SharedKernel/Foundation/Persistence/Migrations/20260928235923_AcademicRegistrationAlignment.cs`                                    | `AcademicRegistrationAlignment_RenamesUniversityColumnAndPreservesData`                                                                                                                                                                   | Passed on fresh LocalDB |
| Extension assignment length is six; migration owner remains ProvisionExtension                                                      | `src/features/Extensions/ProvisionExtension/Shared/Migrations/20260928235954_ExtensionEmployeeNumberLength.cs`; `migration-ownership-matrix.md`  | `ProvisionExtensionSqlServerIntegrationTests`                                                                                                                                                                                             | Passed                  |
| Host references/registers feature, maps route, and does not migrate the mapping-only context; production appsettings has no LocalDB | `src/Zeus.Academia.Api/Program.cs`, `Zeus.Academia.Api.csproj`, `appsettings.json`, `appsettings.Development.json`                               | `Register_ValidRequest_Returns201`; `eng/verify-slice.ps1`                                                                                                                                                                                | Passed                  |

## Verification Results

- `dotnet build zeus.academia.3b.sln`: passed.
- RegisterAcademic suite: 49 tests passed, including 11 SQL Server-backed route/migration cases, validator coverage, fresh-context persistence, failure atomicity, and migration data preservation.
- Shared Kernel SQL Server suite: 34 tests passed.
- ManageDegrees SQL Server suite: 24 tests passed, including TestServer route coverage for AddDegree.
- ProvisionExtension SQL Server suite: 29 tests passed.
- LocalDB compatibility scan: `Academics` and `AcademicQualifications` each had zero rows. The existing `Extensions` table did not have the current `AssignedEmpNr` column and the database had no migration history matching the source model; it was left untouched. Development configuration now uses `Zeus_Academia_RegisterAcademic_Dev`.
- `eng/verify-slice.ps1 -BaseRef HEAD` passes for Academics/RegisterAcademic, ReferenceData/ManageDegrees, and SharedKernel/Foundation. ProvisionExtension passes build and tests but reports only the two explicitly listed PR 46 route-test carry-overs (`ProvisionExtensionEndpoint` and `DeprovisionExtensionEndpoint`).
- The default `origin/main` provenance scan also reports four pre-existing duration mismatches in unrelated ManageUniversities/ProvisionExtension prompt and log files; they were not changed.

**Next Handoff**: RegisterAcademic's gate passes; dependent slices may proceed after normal review. The two ProvisionExtension route-test carry-overs remain separate follow-up work.
