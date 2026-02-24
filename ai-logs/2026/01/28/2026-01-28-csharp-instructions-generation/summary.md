# 2026-01-28-csharp-instructions-generation

Date: 2026-01-28 | Op: johnmillerATcodemag-com | Model: anthropic/claude-sonnet-4.5@unknown | Duration: 00:20:00

## Goal

Generate comprehensive C# implementation instruction file using the generate-csharp-instructions.prompt.md template. Auto-detect scope, patterns, frameworks, and conventions from repository context.

## Deliverables

1. `.github/instructions/csharp-implementation.instructions.md` – Foundational C# standards and best practices for modern C# (11+) development

## Decisions

- **Scope Selection**: General C# implementation (all .cs files)
  - **Rationale**: No C# codebase exists yet; provide foundational standards that complement specialized CQRS/ES instructions

- **Pattern Focus**: Clean Architecture, SOLID, modern C# idioms
  - **Rationale**: Broad applicability; specializations (CQRS/ES) already documented separately

- **Convention Strategy**: Standard C# + modern features (C# 11+)
  - **Rationale**: Nullable reference types, file-scoped namespaces, required members, records vs classes

- **Token Optimization**: Tables for structured data, imperative directives, minimal examples
  - **Rationale**: Target 400-800 tokens per prompt guidance; achieved ~800 tokens

- **Integration**: Cross-reference to cqrs-es-csharp-mediatr.instructions.md
  - **Rationale**: Avoid duplication; establish hierarchy of general → specialized instructions

## Context Analysis

**Detected Patterns:**

- No C# projects/files in current repository
- Existing CQRS/ES instruction file indicates future event-sourced architecture
- Documentation-centric repository structure

**Applied Conventions:**

- File-scoped namespaces (C# 10+)
- Nullable reference types enabled
- Constructor injection for dependencies
- One type per file
- Async/await with CancellationToken propagation
- Record types for immutable data

**Frameworks Referenced:**

- .NET 8+ (modern SDK)
- ILogger<T> (Microsoft.Extensions.Logging)
- IOptions<T> (Microsoft.Extensions.Options)

## Pending

- [ ] Test instruction file application when C# codebase is added
- [ ] Update project-overview.instructions.md to reference new C# standards (optional)
- [ ] Validate YAML metadata passes CI provenance check

```yaml
started: "2026-01-28T19:30:00Z"
ended: "2026-01-28T19:50:00Z"
models: ["anthropic/claude-sonnet-4.5@unknown"]
artifacts: 1
modified: 0
```
