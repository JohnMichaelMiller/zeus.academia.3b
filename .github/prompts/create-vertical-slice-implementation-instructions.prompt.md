---
ai_generated: true
model: "anthropic/claude-sonnet-4.6@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-26-vertical-slices-prompt"
prompt: |
  create a new prompt file, that creates an instruction file, that provides guidance
  for implementing applications in vertical slices
started: "2026-02-26T00:00:00Z"
ended: "2026-02-26T00:15:00Z"
task_durations:
  - task: "design prompt structure"
    duration: "00:05:00"
  - task: "draft content"
    duration: "00:08:00"
  - task: "validate metadata"
    duration: "00:02:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/02/26/2026-02-26-vertical-slices-prompt/conversation.md"
source: "johnmillerATcodemag-com"
name: implement-vertical-slice
description: Generate an instruction file for implementing features as vertical slices
author: John Miller
tags: [vertical-slice, architecture, csharp, mediatr, instructions]
arguments:
  - name: stack
    description: "Target stack abbreviation: csharp-mediatr | csharp-minimal-api | fullstack (default: csharp-mediatr)"
    required: false
context: "ASP.NET Core application using MediatR and CQRS in a vertical-slice architecture"
expected_output: "Complete .instructions.md file governing vertical-slice structure, file layout, and implementation patterns"
tools: ["create_file", "read_file", "semantic_search"]
mode: agent
---

# Generate Vertical Slice Architecture Instruction File

Create a comprehensive `.instructions.md` file that guides AI assistants in implementing features as self-contained vertical slices, following conventions established in this project.

## Context Analysis

Before generating the instruction file, gather context:

1. Read `#file:.github/instructions/project-overview.instructions.md` — confirm tech stack and key patterns.
2. Read `#file:.github/instructions/cqrs-mediatr-efcore.instructions.md` — understand existing CQRS conventions to align slice structure.
3. Read `#file:.github/instructions/csharp-implementation.instructions.md` — apply C# coding standards inside slices.
4. Scan `src/backend/` for any existing feature folders to infer naming and layout conventions already in use.

**Stack argument** (default `csharp-mediatr`):

| Value                | Meaning                                                       |
| -------------------- | ------------------------------------------------------------- |
| `csharp-mediatr`     | ASP.NET Core + MediatR + EF Core                              |
| `csharp-minimal-api` | ASP.NET Core Minimal APIs + MediatR                           |
| `fullstack`          | Above + Vue 3 frontend slice (component + store + composable) |

## Output File

**Path**: `.github/instructions/vertical-slice-implementation.instructions.md`

**Metadata** (AI provenance + Copilot fields):

```yaml
---
ai_generated: true
model: "<provider>/<model>@<version>"
operator: "<operator>"
chat_id: "<chat-id>"
prompt: |
  <exact prompt used>
started: "<ISO8601>"
ended: "<ISO8601>"
task_durations:
  - task: "context analysis"
    duration: "<hh:mm:ss>"
  - task: "draft instruction content"
    duration: "<hh:mm:ss>"
total_duration: "<hh:mm:ss>"
ai_log: "ai-logs/<yyyy>/<mm>/<dd>/<chat-id>/conversation.md"
source: "<chat-id>"
description: "Vertical slice architecture implementation standards"
applyTo: "src/**/*.cs"
---
```

## Required Instruction Sections

The generated instruction file MUST include all of the following sections:

### 1. Core Principle

Define vertical slice in one paragraph:

- A slice = one cohesive feature/use-case spanning all layers (HTTP → handler → persistence).
- No cross-slice dependencies; shared kernel only for primitives.
- Prefer duplication within a slice over coupling between slices.

### 2. Folder Structure

Provide the canonical folder layout for a single slice. Example for `{{feature_name}}` (default: `FeatureName`):

```
src/backend/Features/
└── {{feature_name}}/
    ├── Commands/
    │   ├── Create{{feature_name}}/
    │   │   ├── Create{{feature_name}}Command.cs       # IRequest<Result>
    │   │   ├── Create{{feature_name}}Handler.cs       # IRequestHandler
    │   │   ├── Create{{feature_name}}Validator.cs     # AbstractValidator
    │   │   └── Create{{feature_name}}Response.cs      # DTO
    │   └── Update{{feature_name}}/
    │       └── ...
    ├── Queries/
    │   ├── Get{{feature_name}}ById/
    │   │   ├── Get{{feature_name}}ByIdQuery.cs
    │   │   ├── Get{{feature_name}}ByIdHandler.cs
    │   │   └── Get{{feature_name}}ByIdResponse.cs
    │   └── List{{feature_name}}s/
    │       └── ...
    ├── {{feature_name}}Endpoints.cs                   # Minimal API or Controller
    └── {{feature_name}}MappingProfile.cs              # AutoMapper / manual mapping
```

Rules:

- One folder per use-case (e.g., `CreateEnrollment`, `GetEnrollmentById`).
- File names match class names exactly.
- Validators live beside their command/query — never in a shared `Validators/` folder.
- Response DTOs are slice-private; never reuse across features without explicit shared-kernel promotion.

### 3. Naming Conventions

| Artifact        | Pattern                     | Example                            |
| --------------- | --------------------------- | ---------------------------------- |
| Command         | `<Verb><Feature>Command`    | `CreateEnrollmentCommand`          |
| Query           | `<Verb><Feature>Query`      | `GetEnrollmentByIdQuery`           |
| Handler         | `<CommandOrQuery>Handler`   | `CreateEnrollmentHandler`          |
| Validator       | `<CommandOrQuery>Validator` | `CreateEnrollmentCommandValidator` |
| Response DTO    | `<CommandOrQuery>Response`  | `CreateEnrollmentResponse`         |
| Endpoint class  | `<Feature>Endpoints`        | `EnrollmentEndpoints`              |
| Mapping profile | `<Feature>MappingProfile`   | `EnrollmentMappingProfile`         |

### 4. Implementation Templates

Provide minimal, copy-paste-ready templates for:

#### Command

```csharp
public sealed record Create{{feature_name}}Command(/* properties */) : IRequest<Result<Create{{feature_name}}Response>>;
```

#### Handler

```csharp
public sealed class Create{{feature_name}}Handler(AppDbContext db)
    : IRequestHandler<Create{{feature_name}}Command, Result<Create{{feature_name}}Response>>
{
    public async Task<Result<Create{{feature_name}}Response>> Handle(
        Create{{feature_name}}Command request, CancellationToken cancellationToken)
    {
        // 1. Validate domain rules
        // 2. Create/update aggregate
        // 3. Persist
        // 4. Return response
    }
}
```

#### Validator

```csharp
public sealed class Create{{feature_name}}CommandValidator : AbstractValidator<Create{{feature_name}}Command>
{
    public Create{{feature_name}}CommandValidator()
    {
        RuleFor(x => x./* property */).NotEmpty();
    }
}
```

#### Minimal API Endpoint

```csharp
public static class {{feature_name}}Endpoints
{
    public static IEndpointRouteBuilder Map{{feature_name}}Endpoints(this IEndpointRouteBuilder app)
    {
        var group = app.MapGroup("/api/{{feature_name | kebab-case}}").WithTags("{{feature_name}}");

        group.MapPost("/", async (Create{{feature_name}}Command cmd, ISender sender) =>
        {
            var result = await sender.Send(cmd);
            return result.IsSuccess ? Results.Created($"/api/{{feature_name | kebab-case}}/{result.Value.Id}", result.Value)
                                    : Results.Problem(result.Error);
        })
        .WithName("Create{{feature_name}}")
        .Produces<Create{{feature_name}}Response>(StatusCodes.Status201Created)
        .ProducesValidationProblem();

        return app;
    }
}
```

### 5. Shared Kernel Rules

- Allowed in shared kernel: primitive value objects, domain events, `Result<T>`, common exceptions, base entity.
- Prohibited in shared kernel: feature-specific DTOs, validators, or business logic.
- Shared kernel path: `src/backend/SharedKernel/`.
- Add to shared kernel only after a concept appears in ≥3 slices.

### 6. Registration Pattern

Describe how slices self-register:

- Use `IEndpointRouteBuilder` extension methods; call in `Program.cs` or a `FeatureModuleExtensions` aggregator.
- MediatR handlers auto-discovered via assembly scanning — no manual registration per handler.
- Validators registered via `AddValidatorsFromAssembly` — no manual registration per validator.

### 7. Testing Conventions

- One test class per handler: `Create{{feature_name}}HandlerTests`.
- Test file path mirrors source: `tests/backend/Features/{{feature_name}}/Commands/Create{{feature_name}}/`.
- Use an in-memory `AppDbContext` or SQL Server test container — never mock `DbContext`.
- Validate the full slice (command → handler → db round-trip) in integration tests; unit-test validators separately.

### 8. Anti-Patterns

| Anti-Pattern                                        | Instead                                                                   |
| --------------------------------------------------- | ------------------------------------------------------------------------- |
| Shared `Services/` folder across features           | Move logic into the handler; extract to shared kernel only when warranted |
| Generic `BaseHandler<T>`                            | Concrete, explicit handler per use-case                                   |
| Cross-slice handler calls                           | Publish domain events; let each slice's handler react independently       |
| Reusing response DTOs across features               | Keep DTOs slice-private; promote to shared kernel consciously             |
| Putting validators in a global `Validators/` folder | Co-locate validator with its command/query                                |
| Anemic domain model with all logic in handlers      | Encapsulate invariants in the domain entity                               |

### 9. Quality Checklist

Include a per-slice checklist at the end of the instruction file:

- [ ] Folder matches use-case name exactly
- [ ] Command/Query is a `sealed record`
- [ ] Handler registered via assembly scan (not manually)
- [ ] Validator co-located with command/query
- [ ] Response DTO is slice-private
- [ ] Endpoint maps to a distinct HTTP verb + route
- [ ] Integration test covers success and at least one failure path
- [ ] No direct dependency on another feature's namespace

## Validation

Before saving the output file, verify:

- [ ] All nine sections present
- [ ] `applyTo` glob covers backend feature files
- [ ] Templates compile (no missing using directives)
- [ ] Naming table is consistent with templates
- [ ] Anti-patterns list addresses real pitfalls in this stack
- [ ] AI provenance metadata complete and accurate
