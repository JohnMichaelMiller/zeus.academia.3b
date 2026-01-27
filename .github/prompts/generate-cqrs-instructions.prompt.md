---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-01-27-cqrs-instructions-prompt"
prompt: |
  create a prompt file that generates instruction files for a CQRS architecture. the prompt file should target ai agents and be optimized to reduce token consumption.
started: "2026-01-27T00:00:00Z"
ended: "2026-01-27T00:10:00Z"
task_durations:
  - task: "analyze requirements"
    duration: "00:02:00"
  - task: "draft prompt"
    duration: "00:06:00"
  - task: "optimize tokens"
    duration: "00:02:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2026/01/27/2026-01-27-cqrs-instructions-prompt/conversation.md"
source: "johnmillerATcodemag-com"
name: generate-cqrs-instructions
description: Generate CQRS architecture instruction files
author: John Miller
tags: [cqrs, architecture, event-sourcing, ddd]
context: "CQRS pattern with command/query separation, event sourcing support"
expected_output: "Instruction file (.instructions.md) with CQRS implementation rules"
tools: ["generate", "read_file"]
mode: interactive
arguments:
  - name: language
    description: "Target programming language (C#, TypeScript, Python, Java)"
  - name: framework
    description: "Framework or library (MediatR, NestJS, Axon, custom)"
  - name: include_event_sourcing
    description: "Include event sourcing patterns (true/false)"
  - name: persistence
    description: "Persistence layer (EF Core, Dapper, MongoDB, PostgreSQL)"
examples:
  - input: "Generate CQRS instructions for C# with MediatR and EF Core"
    output: "Instruction file with command/query handlers, validation, repository patterns"
  - input: "Generate CQRS instructions for TypeScript with NestJS and event sourcing"
    output: "Instruction file with CQRS+ES patterns, sagas, projection handlers"
---

# Generate CQRS Architecture Instruction File

Create instruction file defining CQRS (Command Query Responsibility Segregation) implementation rules for {{language}} using {{framework}}.

## Context

CQRS separates write (commands) from read (queries) operations. Commands modify state; queries retrieve data. Pattern enables scalable, maintainable systems with clear separation of concerns.

**Target:** {{language}} with {{framework}}
**Event Sourcing:** {{include_event_sourcing}}
**Persistence:** {{persistence}}

## Instructions

Generate `.instructions.md` with these sections:

### 1. Metadata Section

```yaml
ai_generated: true
model: "<captured>"
operator: "<github-username>"
chat_id: "<session-id>"
prompt: |
  <exact request>
started/ended: "<ISO8601>"
task_durations: [...]
total_duration: "<hh:mm:ss>"
ai_log: "ai-logs/<path>/conversation.md"
source: "<creator>"
description: "CQRS implementation rules for {{language}}"
applyTo: "<glob-pattern>" # e.g., "src/**/*.{cs,ts,py}"
```

### 2. Command Pattern

**Structure:**

- Command class: Immutable data transfer object with intent
- Handler: Single responsibility, validates → executes → persists
- Result: Success/failure with errors

**Rules:**

- One command = one handler
- Command names: verb + noun (`CreateOrder`, `UpdateInventory`)
- No return values except confirmation/error
- Validate before execution
- Publish domain events after persistence

**Example Template:**

```
Command: <Name>Command { <properties> }
Handler: <Name>CommandHandler
  - Validate: <rules>
  - Execute: <domain logic>
  - Persist: <aggregate/entity>
  - Publish: <DomainEvent>
Result: <Type>
```

### 3. Query Pattern

**Structure:**

- Query class: Criteria for data retrieval
- Handler: Reads from read model/projection
- Result: DTO with requested data

**Rules:**

- One query = one handler
- Query names: Get/List + noun (`GetOrderById`, `ListActiveUsers`)
- Never modify state
- Optimize for read performance
- Use projections/read models, not domain aggregates

**Example Template:**

```
Query: <Name>Query { <filters> }
Handler: <Name>QueryHandler
  - Validate: <criteria>
  - Fetch: <from read-model>
  - Map: <to DTO>
Result: <DTO>
```

### 4. Project Structure

```
{{language}}-specific hierarchy:
Commands/
  <Aggregate>/
    <Command>.cs (or .ts, .py)
    <Command>Handler.cs
    <Command>Validator.cs
Queries/
  <Context>/
    <Query>.cs
    <Query>Handler.cs
Models/
  Commands/  # Write models
  Queries/   # Read models/DTOs
```

### 5. Validation Rules

- Commands: Fail-fast with detailed errors before handler execution
- Queries: Validate input criteria; return empty/null for not-found
- Use {{framework}}-native validation or FluentValidation/class-validator
- Separate validation logic from handler logic

### 6. Event Handling (if event_sourcing = true)

**Domain Events:**

- Past tense: `OrderCreated`, `InventoryUpdated`
- Immutable, serializable
- Published after command succeeds

**Event Store:**

- Append-only log per aggregate
- Replay for state reconstruction
- Snapshotting for performance

**Projections:**

- Consume events → update read models
- Idempotent handlers
- Handle out-of-order events

### 7. Cross-Cutting Concerns

**Transaction Boundaries:**

- Commands: Unit of work wraps handler
- Queries: Read-only, no transactions

**Error Handling:**

- Commands: Return `Result<T>` or throw domain exceptions
- Queries: Return null/empty or 404

**Logging/Telemetry:**

- Log command execution start/end
- Track query performance metrics

### 8. Anti-Patterns to Avoid

❌ Queries modifying state
❌ Commands returning domain data
❌ Cross-aggregate transactions in single command
❌ Mixing read/write models
❌ Synchronous cross-aggregate communication

### 9. Framework-Specific Patterns

**For MediatR (C#):**

- `IRequest<TResponse>` for commands/queries
- `IRequestHandler<TRequest, TResponse>`
- Pipeline behaviors for validation/logging

**For NestJS (TypeScript):**

- `@CommandHandler()`, `@QueryHandler()` decorators
- `CommandBus`, `QueryBus` injection
- Sagas for process managers

**For Python:**

- Abstract base classes for handlers
- Type hints for requests/responses
- Dependency injection container

**For Java/Axon:**

- `@CommandHandler`, `@QueryHandler` annotations
- Aggregate lifecycle management
- Event upcasting support

### 10. Testing Guidelines

**Command Tests:**

- Given: Initial state
- When: Command executed
- Then: Events published, state changed

**Query Tests:**

- Given: Read model state
- When: Query executed
- Then: Expected DTO returned

**Integration:**

- Full command → event → projection → query flow

### 11. Success Criteria

Generated instruction file must:

- [ ] Define clear command/query boundaries
- [ ] Specify naming conventions
- [ ] Include validation requirements
- [ ] Provide structure templates
- [ ] Document error handling
- [ ] Include {{language}}/{{framework}} examples
- [ ] Address event sourcing (if enabled)
- [ ] Define testing approach
- [ ] List anti-patterns
- [ ] Use <200 tokens per major section

## Output Format

Single `.instructions.md` file with:

- YAML front matter (metadata)
- Markdown sections (patterns/rules)
- Code examples in {{language}}
- Glob pattern for applicability

## Validation

Before delivering:

1. Metadata complete per AI-assisted-output policy
2. All 11 sections present and concise
3. {{language}}/{{framework}} specifics included
4. Examples syntactically valid
5. Token count optimized (use tables, bullets, abbreviations)
6. ApplyTo glob matches target files

## Token Optimization Techniques

- Use tables for pattern summaries
- Abbreviate: DTO, ES (Event Sourcing), R/W (read/write)
- Code templates over full examples
- Bullet lists over paragraphs
- Section references instead of repetition
