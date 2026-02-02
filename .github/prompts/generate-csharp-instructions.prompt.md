---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-01-28-csharp-instructions-prompt"
prompt: |
  create a prompt file that will generate an instruction file to guide the implementation of C# code in this application
started: "2026-01-28T00:00:00Z"
ended: "2026-01-28T00:15:00Z"
task_durations:
  - task: "analyze context"
    duration: "00:05:00"
  - task: "design prompt structure"
    duration: "00:05:00"
  - task: "draft content"
    duration: "00:05:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/01/28/2026-01-28-csharp-instructions-prompt/conversation.md"
source: "johnmillerATcodemag-com"
name: generate-csharp-instructions
description: Generate comprehensive C# implementation instruction files
author: John Miller
tags: [csharp, instructions, code-generation, patterns]
arguments:
  - name: scope
    description: "Scope of instructions (auto-detected from context if omitted)"
    required: false
  - name: patterns
    description: "Architectural patterns to enforce (auto-detected from codebase if omitted)"
    required: false
  - name: frameworks
    description: "Frameworks and libraries in use (auto-detected from .csproj and imports if omitted)"
    required: false
  - name: conventions
    description: "Project-specific conventions (auto-detected from existing code; standard C# conventions always applied)"
    required: false
context: "C# application development with modern patterns and practices"
expected_output: "Complete .instructions.md file with metadata, rules, examples, and checklists"
tools: ["create_file", "read_file"]
mode: interactive
---

# Generate C# Implementation Instructions

Create a comprehensive `.instructions.md` file that guides AI assistants in implementing C# code following project-specific patterns, frameworks, and conventions.

## Context Analysis

Before generating instructions, analyze the repository to derive missing arguments:

### Derivation Strategy

**If arguments provided:** Use as-is
**If arguments omitted:** Auto-detect from context:

1. **{{scope}}** - Analyze:
   - User request/chat context (e.g., "create instructions for API controllers")
   - Existing instruction files in `.github/instructions/` (identify gaps)
   - Directory structure and namespace patterns
   - Focus area mentioned in conversation

2. **{{patterns}}** - Detect from:
   - Existing `.instructions.md` files (e.g., `cqrs-es-csharp-mediatr.instructions.md`)
   - Code structure: separate Domain/Application/Infrastructure folders → DDD/Clean Architecture
   - Base class names: `EntityBase`, `CommandHandler`, `QueryHandler` → specific patterns
   - Presence of MediatR `IRequestHandler` → CQRS
   - Repository interfaces → Repository pattern

3. **{{frameworks}}** - Extract from:
   - `.csproj` files: `<PackageReference>` elements
   - `using` statements across C# files
   - `global.usings` files
   - `Program.cs`/`Startup.cs` service registrations
   - Existing instruction file examples and imports

4. **{{conventions}}** - Derive from:
   - Existing code patterns (field naming, method naming, file organization)
   - Existing instruction files documenting conventions
   - **Standard C# Conventions** (always apply - see below)

### Standard C# Conventions (Always Apply)

Include these baseline conventions in every instruction file:

**Naming:**

- Interfaces: `I` prefix (IRepository, IService)
- Private fields: `_camelCase` (\_logger, \_context)
- Public/protected: PascalCase (Properties, Methods, Classes)
- Parameters/locals: camelCase
- Constants: PascalCase or UPPER_SNAKE_CASE (project-specific)
- Async methods: Suffix `Async` (GetOrderAsync)

**Organization:**

- One type per file (class/interface/record)
- File name matches type name
- Namespace matches folder structure
- Order: fields → constructors → properties → methods

**Language Features:**

- Enable nullable reference types (`<Nullable>enable</Nullable>`)
- Use `var` for obvious types, explicit for clarity
- Prefer expression-bodied members for simple cases
- Use `record` for immutable data, `class` for behavior
- Async/await for I/O operations (no `.Result` or `.Wait()`)

**Error Handling:**

- Use specific exception types, not generic `Exception`
- Validate at API boundaries
- Log exceptions before rethrowing
- Use `ArgumentNullException.ThrowIfNull()` (C# 11+)

**Documentation:**

- XML comments on public APIs
- `<summary>` minimum, `<param>` and `<returns>` for clarity
- Explain "why" in comments, not "what"

**Modern C# (C# 11+):**

- File-scoped namespaces
- Required members for constructors
- Raw string literals for JSON/SQL
- List/collection patterns where appropriate

### Derived Context Summary

After analysis, summarize findings:

**Scope:** [Detected or provided scope]
**Patterns:** [Detected architectural patterns]
**Frameworks:** [Detected framework versions and libraries]
**Project Conventions:** [Derived project-specific conventions]
**Standard Conventions:** [Reference to standard C# conventions above]

## Objective

Generate a token-optimized instruction file (`.github/instructions/<name>.instructions.md`) that:

1. Enforces architectural patterns and best practices (derived or provided)
2. Applies standard C# conventions plus project-specific conventions
3. Defines code structure and organization rules
4. Specifies framework usage patterns (detected from codebase)
5. Provides concrete implementation examples
6. Includes validation checklists

## Requirements

### Metadata (Front Matter)

Complete YAML front matter with:

- **AI Provenance**: `ai_generated`, `model`, `operator`, `chat_id`, `prompt`, timestamps, `task_durations`, `total_duration`, `ai_log`, `source`
- **Instruction Metadata**: `description`, `applyTo` (glob pattern for C# files)
- **Optional**: `tags`, `version`, `author`

### Content Structure

#### 1. Title & Overview

- Clear, specific title (e.g., "ASP.NET Core API Controller Patterns")
- One-sentence description of scope and purpose
- Brief context on architectural approach

#### 2. Core Rules

Organize by priority:

**Architecture & Patterns**

- Primary patterns (CQRS, DDD, Repository, etc.)
- Layer separation and dependencies
- Consistency boundaries and transactions

**Code Organization**

- Namespace conventions
- File structure and naming
- Project/folder organization

**Framework Usage**

- Dependency injection patterns
- Middleware/filter usage
- Configuration management
- Logging and error handling

**Language Features**

- Nullable reference types
- Record types vs classes
- Async/await patterns
- LINQ usage guidelines

**Naming Conventions**

- Apply standard C# conventions (see Context Analysis)
- Project-specific additions:
  - Domain-specific prefixes/suffixes
  - Aggregate/entity naming patterns
  - Event naming (past tense for event sourcing)
  - Command/query naming conventions

#### 3. Implementation Templates

Provide concrete code examples for:

- Common patterns (command handlers, queries, repositories)
- Class structures (entities, DTOs, value objects)
- API patterns (controllers, middleware)
- Data access patterns
- Error handling

Use tables for multi-component patterns:

| Component | Purpose | Location | Example |
| --------- | ------- | -------- | ------- |

#### 4. Anti-Patterns

List forbidden practices:

- ❌ Direct database access in controllers
- ❌ Business logic in DTOs
- ❌ Synchronous I/O in async methods
- ❌ Static mutable state

#### 5. Validation Checklist

Multi-step quality checks:

- [ ] Follows pattern requirements
- [ ] Uses correct namespaces
- [ ] Implements proper error handling
- [ ] Includes XML documentation
- [ ] Passes nullability analysis

## Output Format

````markdown
---
ai_generated: true
model: "<model>"
operator: "<username>"
chat_id: "<chat-id>"
prompt: |
  <exact prompt>
started: "<ISO8601>"
ended: "<ISO8601>"
task_durations:
  - task: "<name>"
    duration: "<hh:mm:ss>"
total_duration: "<hh:mm:ss>"
ai_log: "ai-logs/<yyyy>/<mm>/<dd>/<chat-id>/conversation.md"
source: "<source>"
description: "<clear description>"
applyTo: "<glob-pattern>"
---

# [Title]

[One-sentence overview with context]

## [Section 1: Architecture]

[Rules in imperative voice]

- MUST [critical requirement]
- MUST NOT [forbidden action]
- SHOULD [recommendation]

## [Section 2: Patterns]

[Implementation patterns with code examples]

```csharp
[Concrete example]
```
````

## [Section 3: Anti-Patterns]

❌ [Pattern to avoid]
✅ [Correct approach]

## [Section 4: Checklist]

- [ ] [Validation point]

````

## Success Criteria

**Completeness:**

- All architectural patterns documented
- Framework-specific rules included
- Examples cover common scenarios
- Anti-patterns identified

**Token Efficiency:**

- Imperative voice (no filler words)
- Code examples over prose explanations
- Tables for structured information
- Target 400-800 tokens

**Actionability:**

- Each rule testable/verifiable
- Clear pass/fail criteria
- Specific rather than general
- No ambiguous language

**Correctness:**

- Valid `applyTo` glob patterns
- Proper YAML syntax
- C# code examples compile
- Aligned with latest C# standards

## Examples

### Input

```text
Scope: Domain Entities
Patterns: DDD, Value Objects, Entity Base Classes
Frameworks: None (pure domain)
Conventions: Records for value objects, sealed classes, private setters
````

### Output Excerpt

````markdown
---
description: "Domain entity implementation with DDD patterns"
applyTo: "src/**/Domain/Entities/**/*.cs"
---

# Domain Entity Patterns

Entities are identified by unique ID with lifecycle tracked through events. Use records for value objects, sealed classes for entities.

## Entity Rules

- MUST inherit from `EntityBase<TId>`
- MUST have private setters (or init-only)
- MUST validate in constructors/factory methods
- MUST NOT have parameterless constructors
- MUST use value objects for complex properties

## Value Object Rules

- MUST use record types with positional parameters
- MUST validate in primary constructor
- MUST be immutable (no setters)
- MUST override Equals/GetHashCode (auto for records)

## Template

```csharp
public sealed class Order : EntityBase<OrderId>
{
    public CustomerId CustomerId { get; private set; }
    public Money TotalAmount { get; private set; }

    private Order() { } // EF

    public static Order Create(CustomerId customerId, List<OrderItem> items)
    {
        if (items.Count == 0) throw new DomainException("Order must have items");
        var order = new Order { Id = OrderId.New(), CustomerId = customerId };
        order.CalculateTotal(items);
        return order;
    }
}

public record OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());
}
```
````

## Anti-Patterns

❌ Public setters on entities
❌ Anemic domain models
❌ Validation in application layer
❌ Primitive obsession (string for OrderId)

```

## Validation

Before finalizing the instruction file:

- [ ] All required metadata fields present
- [ ] YAML syntax validates
- [ ] `applyTo` glob pattern tested and correct
- [ ] Rules are specific and actionable
- [ ] Code examples compile without errors
- [ ] No conflicting directives
- [ ] Token count optimized (imperatives, tables, minimal examples)
- [ ] Anti-patterns clearly distinguished
- [ ] Checklist covers critical validation points
- [ ] Follows instruction-file-generation.instructions.md guidelines

## Notes

- **Token Optimization**: Use tables, imperatives, eliminate filler
- **Specificity**: Prefer concrete requirements ("MUST use ILogger<T>") over vague ("use good logging")
- **Framework Versions**: Specify when behavior differs by version
- **Dependencies**: Document required NuGet packages
- **Testing**: Include unit test patterns if applicable

---

_Generated instruction file should be placed in `.github/instructions/` with descriptive name matching scope._
```
