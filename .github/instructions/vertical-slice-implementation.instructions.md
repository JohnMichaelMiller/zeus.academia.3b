---
ai_generated: true
model: "anthropic/claude-sonnet-4.6@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-26-vertical-slice-instructions-generation"
prompt: |
  submit #file:create-vertical-slice-implementation-instructions.prompt.md
started: "2026-02-26T00:30:00Z"
ended: "2026-02-26T00:50:00Z"
task_durations:
  - task: "context analysis"
    duration: "00:05:00"
  - task: "draft instruction content"
    duration: "00:12:00"
  - task: "validate and optimize"
    duration: "00:03:00"
total_duration: "00:20:00"
ai_log: "ai-logs/2026/02/26/2026-02-26-vertical-slice-instructions-generation/conversation.md"
source: ".github/prompts/create-vertical-slice-implementation-instructions.prompt.md"
description: "Vertical slice architecture implementation standards"
applyTo: "src/**/*.cs"
---

# Vertical Slice Architecture — Implementation Standards

Standards for implementing features as self-contained vertical slices in the zeus.academia ASP.NET Core + MediatR + EF Core application.

## 1. Core Principle

A **vertical slice** is one cohesive use-case that spans all application layers from HTTP endpoint through handler logic to database persistence. Each slice:

- Owns its own command/query, handler, validator, response DTO, and endpoint mapping.
- Has **no compile-time dependency on any other feature slice**.
- Communicates cross-feature only via **domain events** or the **shared kernel** — never by direct namespace reference.
- Prefers **duplication within a slice** over coupling between slices.

## 2. Folder Structure

One top-level folder per feature domain; one sub-folder per use-case:

```
src/backend/Features/
└── <FeatureName>/
    ├── Commands/
    │   ├── Create<FeatureName>/
    │   │   ├── Create<FeatureName>Command.cs        # IRequest<Result<T>>
    │   │   ├── Create<FeatureName>Handler.cs        # IRequestHandler
    │   │   ├── Create<FeatureName>Validator.cs      # AbstractValidator
    │   │   └── Create<FeatureName>Response.cs       # Slice-private DTO
    │   └── Update<FeatureName>/
    │       ├── Update<FeatureName>Command.cs
    │       ├── Update<FeatureName>Handler.cs
    │       ├── Update<FeatureName>Validator.cs
    │       └── Update<FeatureName>Response.cs
    ├── Queries/
    │   ├── Get<FeatureName>ById/
    │   │   ├── Get<FeatureName>ByIdQuery.cs
    │   │   ├── Get<FeatureName>ByIdHandler.cs
    │   │   └── Get<FeatureName>ByIdResponse.cs
    │   └── List<FeatureName>s/
    │       ├── List<FeatureName>sQuery.cs
    │       ├── List<FeatureName>sHandler.cs
    │       └── List<FeatureName>sResponse.cs
    ├── <FeatureName>Endpoints.cs                   # Minimal API route registration
    └── <FeatureName>MappingProfile.cs              # Mapping (AutoMapper or manual)
```

**Rules:**

- File name MUST match type name exactly (`CreateEnrollmentCommand.cs` → `CreateEnrollmentCommand`).
- Validators MUST live beside their command/query — never in a shared `Validators/` folder.
- Response DTOs are slice-private — never reference them from another feature's namespace.
- Empty `Update` or `Delete` use-case folders are fine; scaffold only what the slice needs.

## 3. Naming Conventions

| Artifact        | Pattern                     | Example                            |
| --------------- | --------------------------- | ---------------------------------- |
| Command         | `<Verb><Feature>Command`    | `CreateEnrollmentCommand`          |
| Query           | `<Verb><Feature>Query`      | `GetEnrollmentByIdQuery`           |
| Command handler | `<Command>Handler`          | `CreateEnrollmentHandler`          |
| Query handler   | `<Query>Handler`            | `GetEnrollmentByIdHandler`         |
| Validator       | `<CommandOrQuery>Validator` | `CreateEnrollmentCommandValidator` |
| Response DTO    | `<CommandOrQuery>Response`  | `CreateEnrollmentResponse`         |
| Endpoint class  | `<Feature>Endpoints`        | `EnrollmentEndpoints`              |
| Mapping profile | `<Feature>MappingProfile`   | `EnrollmentMappingProfile`         |
| Test class      | `<Handler>Tests`            | `CreateEnrollmentHandlerTests`     |

**Verb guidance:** `Create`, `Update`, `Delete`, `Enroll`, `Assign`, `Submit`, `Approve`, `Reject`, `Get`, `List`, `Search`.

## 4. Implementation Templates

### 4.1 Command

```csharp
namespace Zeus.Academia.Features.Enrollment.Commands.CreateEnrollment;

public sealed record CreateEnrollmentCommand(
    Guid StudentId,
    Guid CourseId,
    DateOnly RequestedDate) : IRequest<Result<CreateEnrollmentResponse>>;
```

### 4.2 Handler

```csharp
namespace Zeus.Academia.Features.Enrollment.Commands.CreateEnrollment;

public sealed class CreateEnrollmentHandler(AppDbContext db)
    : IRequestHandler<CreateEnrollmentCommand, Result<CreateEnrollmentResponse>>
{
    public async Task<Result<CreateEnrollmentResponse>> Handle(
        CreateEnrollmentCommand request,
        CancellationToken cancellationToken)
    {
        // 1. Domain guard — check for duplicate enrollment
        var exists = await db.Enrollments
            .AnyAsync(e => e.StudentId == request.StudentId
                        && e.CourseId  == request.CourseId, cancellationToken);
        if (exists)
            return Result<CreateEnrollmentResponse>.Failure("Student is already enrolled in this course.");

        // 2. Create aggregate
        var enrollment = Enrollment.Create(request.StudentId, request.CourseId, request.RequestedDate);

        // 3. Persist
        db.Enrollments.Add(enrollment);
        await db.SaveChangesAsync(cancellationToken);

        // 4. Return slice-private response
        return Result<CreateEnrollmentResponse>.Success(
            new CreateEnrollmentResponse(enrollment.Id, enrollment.Status));
    }
}
```

### 4.3 Validator

```csharp
namespace Zeus.Academia.Features.Enrollment.Commands.CreateEnrollment;

public sealed class CreateEnrollmentCommandValidator : AbstractValidator<CreateEnrollmentCommand>
{
    public CreateEnrollmentCommandValidator()
    {
        RuleFor(x => x.StudentId).NotEmpty();
        RuleFor(x => x.CourseId).NotEmpty();
        RuleFor(x => x.RequestedDate).GreaterThanOrEqualTo(DateOnly.FromDateTime(DateTime.UtcNow));
    }
}
```

### 4.4 Response DTO

```csharp
namespace Zeus.Academia.Features.Enrollment.Commands.CreateEnrollment;

public sealed record CreateEnrollmentResponse(Guid EnrollmentId, EnrollmentStatus Status);
```

### 4.5 Minimal API Endpoint

```csharp
namespace Zeus.Academia.Features.Enrollment;

public static class EnrollmentEndpoints
{
    public static IEndpointRouteBuilder MapEnrollmentEndpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/enrollments")
                       .WithTags("Enrollment")
                       .RequireAuthorization();

        group.MapPost("/", async (CreateEnrollmentCommand cmd, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(cmd, ct);
            return result.IsSuccess
                ? Results.Created($"/api/enrollments/{result.Value.EnrollmentId}", result.Value)
                : Results.Problem(result.Error);
        })
        .WithName("CreateEnrollment")
        .Produces<CreateEnrollmentResponse>(StatusCodes.Status201Created)
        .ProducesValidationProblem()
        .ProducesProblem(StatusCodes.Status409Conflict);

        group.MapGet("/{id:guid}", async (Guid id, ISender sender, CancellationToken ct) =>
        {
            var result = await sender.Send(new GetEnrollmentByIdQuery(id), ct);
            return result.IsSuccess ? Results.Ok(result.Value) : Results.NotFound();
        })
        .WithName("GetEnrollmentById")
        .Produces<GetEnrollmentByIdResponse>();

        return app;
    }
}
```

### 4.6 Program.cs Registration

```csharp
// Register all feature endpoints via a single aggregator
app.MapEnrollmentEndpoints();
app.MapGradingEndpoints();
// ...

// MediatR and validators auto-discovered — no per-handler registration needed:
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Program).Assembly));
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
```

## 5. Shared Kernel Rules

**Path:** `src/backend/SharedKernel/`

| Allowed                                                      | Prohibited                                |
| ------------------------------------------------------------ | ----------------------------------------- |
| Primitive value objects (`StudentId`, `CourseCode`)          | Feature-specific DTOs or responses        |
| `Result<T>` and `Error` types                                | Feature validators or business rules      |
| Domain event base types and interfaces                       | Cross-feature service interfaces          |
| Base entity / aggregate root                                 | Anything that imports a feature namespace |
| Common exceptions (`NotFoundException`, `ConflictException`) | —                                         |

**Promotion rule:** Promote a concept to the shared kernel only after it appears in ≥ 3 independent slices. Prefer inline duplication until then.

## 6. Cross-Slice Communication

**Never** call one feature's handler from another feature's handler. Use domain events:

```csharp
// In EnrollmentHandler — raise event after persist
enrollment.RaiseDomainEvent(new StudentEnrolledEvent(enrollment.Id, enrollment.StudentId));
await db.SaveChangesAsync(cancellationToken);   // event dispatched in SaveChanges pipeline

// In a different feature — react independently
public sealed class SendEnrollmentConfirmationHandler(IEmailService email)
    : INotificationHandler<StudentEnrolledEvent>
{
    public async Task Handle(StudentEnrolledEvent notification, CancellationToken ct)
        => await email.SendEnrollmentConfirmationAsync(notification.StudentId, ct);
}
```

## 7. Query Handler Pattern

Queries MUST NOT mutate state. Prefer direct EF Core projections over loading full aggregates:

```csharp
public sealed class GetEnrollmentByIdHandler(AppDbContext db)
    : IRequestHandler<GetEnrollmentByIdQuery, Result<GetEnrollmentByIdResponse>>
{
    public async Task<Result<GetEnrollmentByIdResponse>> Handle(
        GetEnrollmentByIdQuery request, CancellationToken cancellationToken)
    {
        var response = await db.Enrollments
            .Where(e => e.Id == request.EnrollmentId)
            .Select(e => new GetEnrollmentByIdResponse(e.Id, e.StudentId, e.CourseId, e.Status, e.CreatedAt))
            .FirstOrDefaultAsync(cancellationToken);

        return response is not null
            ? Result<GetEnrollmentByIdResponse>.Success(response)
            : Result<GetEnrollmentByIdResponse>.Failure("Enrollment not found.");
    }
}
```

## 8. Testing Conventions

- **One test class per handler:** `CreateEnrollmentHandlerTests`.
- **Test file path mirrors source:**
  `tests/backend/Features/Enrollment/Commands/CreateEnrollment/CreateEnrollmentHandlerTests.cs`
- **Never mock `DbContext`** — use `SqliteInMemory` or a local SQL Server test container.
- **Integration tests** validate the full slice: request → handler → db round-trip → response.
- **Unit tests** validate validators in isolation (no db required).

```csharp
public sealed class CreateEnrollmentHandlerTests(AppDbContextFactory factory)
    : IClassFixture<AppDbContextFactory>
{
    [Fact]
    public async Task Handle_ValidCommand_CreatesEnrollmentAndReturnsSuccess()
    {
        // Arrange
        using var db = factory.CreateContext();
        var handler = new CreateEnrollmentHandler(db);
        var cmd = new CreateEnrollmentCommand(Guid.NewGuid(), Guid.NewGuid(), DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeTrue();
        db.Enrollments.Should().ContainSingle(e => e.Id == result.Value.EnrollmentId);
    }

    [Fact]
    public async Task Handle_DuplicateEnrollment_ReturnsFailure()
    {
        // Arrange — seed existing enrollment
        using var db = factory.CreateContext();
        var studentId = Guid.NewGuid();
        var courseId  = Guid.NewGuid();
        db.Enrollments.Add(Enrollment.Create(studentId, courseId, DateOnly.FromDateTime(DateTime.UtcNow)));
        await db.SaveChangesAsync();

        var handler = new CreateEnrollmentHandler(db);
        var cmd = new CreateEnrollmentCommand(studentId, courseId, DateOnly.FromDateTime(DateTime.UtcNow.AddDays(1)));

        // Act
        var result = await handler.Handle(cmd, CancellationToken.None);

        // Assert
        result.IsSuccess.Should().BeFalse();
    }
}
```

## 9. Anti-Patterns

| Anti-Pattern                                           | Instead                                                                                      |
| ------------------------------------------------------ | -------------------------------------------------------------------------------------------- |
| Shared `Services/` folder used by multiple features    | Inline the logic in the handler; promote to shared kernel only at ≥3 uses                    |
| Generic `BaseHandler<T>` or `CrudHandler<T>`           | One concrete, explicit handler per use-case                                                  |
| Handler calls another feature's handler directly       | Publish a domain event; react in an `INotificationHandler`                                   |
| Response DTO imported from another feature's namespace | Keep DTOs slice-private; promote to shared kernel consciously                                |
| Validators in a global or shared `Validators/` folder  | Co-locate each validator with its command/query                                              |
| Anemic domain model — all logic in handlers            | Encapsulate invariants and guards in the domain entity/aggregate                             |
| `DbContext` mocked in tests                            | Use `SqliteInMemory` or test containers for realistic persistence tests                      |
| Registering handlers/validators manually in DI         | Use assembly-scan registration (`RegisterServicesFromAssembly`, `AddValidatorsFromAssembly`) |

## 10. Per-Slice Quality Checklist

Before marking a slice complete:

- [ ] Folder name matches the use-case name exactly
- [ ] Command/Query is a `sealed record`
- [ ] Handler is `sealed` and has no public state
- [ ] Validator is co-located with its command/query
- [ ] Response DTO is slice-private (no cross-feature imports)
- [ ] Endpoint method is in `<Feature>Endpoints` and maps to a distinct HTTP verb + route
- [ ] Handler registered via assembly scan (not manually)
- [ ] Integration test covers the happy path
- [ ] Integration test covers at least one failure/validation path
- [ ] No `using` import referencing another feature's namespace
- [ ] Domain events raised (not direct handler calls) for cross-feature side effects
