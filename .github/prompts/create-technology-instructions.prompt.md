---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-24-project-overview-zeus-academia"
prompt: |
  Create a prompt template for generating technology-specific instruction files
  based on the technologies listed in project-overview.instructions.md
started: "2026-02-24T00:15:00Z"
ended: "2026-02-24T00:25:00Z"
task_durations:
  - task: "analyze technology requirements"
    duration: "00:03:00"
  - task: "design prompt structure"
    duration: "00:05:00"
  - task: "write prompt content"
    duration: "00:02:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2026/02/24/2026-02-24-project-overview-zeus-academia/conversation.md"
source: "johnmillerATcodemag-com"
description: "Generate technology-specific instruction files for project stack"
context: "Technology-specific coding standards and best practices"
expected_output: ".instructions.md file with technology-specific guidance"
tools: ["search", "read", "create"]
mode: agent
name: create-technology-instructions
author: John Miller
tags: [instructions, technology, standards, best-practices, token-optimization]
arguments:
  - name: technology_name
    description: Name of the technology/framework (e.g., Vue 3, ASP.NET Core, MediatR)
  - name: technology_category
    description: Category (frontend|backend|database|infrastructure|testing|validation)
  - name: primary_language
    description: Primary language for this technology (C#, TypeScript, etc.)
  - name: project_context
    description: How this technology fits in the project architecture
  - name: version_target
    description: Specific version or version range to target
---

# Generate Technology-Specific Instruction File

Create a token-optimized `.instructions.md` file with coding standards and best practices for a specific technology in the project stack.

## Input

- **Technology Name**: {{technology_name}}
- **Category**: {{technology_category}}
- **Primary Language**: {{primary_language}}
- **Project Context**: {{project_context}}
- **Version Target**: {{version_target}}

## Required Sections

### 1. Technology Identity

- Official name and version target
- Role in project architecture
- Primary use cases
- Related technologies in stack

### 2. Core Principles

- Fundamental patterns to follow
- Architecture approach (CQRS, composition, etc.)
- Key conventions specific to this technology
- Performance considerations

### 3. Implementation Standards

- File organization and naming
- Code structure patterns
- Required imports/dependencies
- Configuration approach

### 4. Common Patterns

- Standard implementations (commands, queries, components, services)
- Validation patterns
- Error handling
- Testing patterns

### 5. Anti-Patterns

- What NOT to do
- Common mistakes to avoid
- Performance pitfalls
- Security concerns

### 6. Integration Points

- How this technology connects to others in stack
- API contracts or interfaces
- State management interactions
- Authentication/authorization integration

## Output Requirements

**File**: `<technology-name-kebab-case>.instructions.md` in `.github/instructions/`

**Metadata**:

- Complete AI provenance (if AI-generated)
- `name`: Technology name and purpose
- `description`: One-line summary
- `applyTo`: Glob pattern matching relevant files
- `tags`: Technology category and related keywords

**Content Style**:

- Imperative, directive tone
- Code examples for non-obvious patterns
- Bullet/list format preferred
- Specific version features noted
- Token-optimized (no filler)

**applyTo Patterns**:

- Frontend (Vue): `src/frontend/**/*.{vue,ts}`
- Backend (C#): `src/backend/**/*.cs`
- Tests (C#): `tests/**/*.cs` or `**/*.test.cs`
- Tests (TS): `src/**/*.{test,spec}.ts`
- Config: `**/*.config.{js,ts}` or specific files

## Example Structure

````markdown
---
name: "[Technology] Standards"
description: "[Technology] coding standards and best practices for [purpose]"
applyTo: "[glob pattern]"
tags: [tech-name, category, language]
[...metadata...]
---

# [Technology Name] Standards

**Role**: [Purpose in architecture]
**Version**: [Target version]
**Language**: [Primary language]

## Core Principles

- [Principle 1]
- [Principle 2]
- [Pattern]: [Description]

## File Organization

- `path/` - [Purpose]
- Naming: [pattern]
- Structure: [approach]

## Standard Patterns

### [Pattern Name]

```[language]
// Example implementation
[code]
```
````

**Usage**: [When to use]
**Avoid**: [What not to do]

## Integration

- [Other tech]: [How they connect]
- [Interface]: [Contract description]

## Validation Checklist

- [ ] [Check 1]
- [ ] [Check 2]

## Anti-Patterns

❌ [Bad practice]
✅ [Correct approach]

````

## Technology-Specific Guidance

### Frontend (Vue 3, TypeScript)

**Focus on**:

- Composition API vs Options API (prefer composition)
- Component structure and organization
- Props typing and validation
- State management with Pinia
- Lifecycle hooks
- Composables for reusable logic
- Template syntax and directives
- Performance (v-once, v-memo, lazy loading)

**applyTo**: `src/frontend/**/*.{vue,ts}`

### Backend (ASP.NET Core, C#)

**Focus on**:

- Controller/minimal API patterns
- Dependency injection setup
- Middleware pipeline
- Configuration management
- Logging patterns
- Exception handling
- Async/await best practices
- Nullable reference types

**applyTo**: `src/backend/**/*.cs`

### CQRS (MediatR)

**Focus on**:

- Command structure and naming
- Query structure and naming
- Handler implementation
- Validation pipeline
- Request/response patterns
- Unit of work
- Transaction handling

**applyTo**: `src/backend/**/*.cs`

### Validation (FluentValidation)

**Focus on**:

- Validator structure
- Rule composition
- Custom validators
- Async validation
- Error message patterns
- Integration with MediatR

**applyTo**: `src/backend/**/*Validator.cs`

### State Management (Pinia)

**Focus on**:

- Store structure and naming
- State definition
- Getters vs computed
- Actions (sync/async)
- Store composition
- TypeScript typing
- Persistence patterns

**applyTo**: `src/frontend/stores/**/*.ts`

### Testing (xUnit, Vitest)

**Focus on**:

- Test naming conventions
- Arrange-Act-Assert pattern
- Fixture setup
- Mocking patterns
- Coverage targets
- Integration test patterns
- Parameterized tests

**applyTo**: `{tests,src}/**/*.{test,spec}.{cs,ts}`

### Build Tools (Vite)

**Focus on**:

- Configuration structure
- Plugin usage
- Environment variables
- Dev server setup
- Build optimization
- Asset handling

**applyTo**: `vite.config.ts`

## Validation Checklist

- [ ] Technology name and version specified
- [ ] Role in architecture documented
- [ ] `applyTo` glob pattern matches relevant files
- [ ] Core principles stated clearly
- [ ] Standard patterns with code examples
- [ ] Integration points documented
- [ ] Anti-patterns identified
- [ ] Token-optimized (no redundant text)
- [ ] Imperative tone throughout
- [ ] Testing guidance included

## Anti-Patterns to Avoid

❌ Generic advice applicable to any technology
❌ Copying documentation that's easily searchable
❌ Version-agnostic guidance (specify version features)
❌ Missing integration context with other stack technologies
❌ Overly broad `applyTo` patterns that match unrelated files
❌ Including basic syntax tutorials (assume competence)
❌ Omitting performance or security considerations
❌ Not specifying when to use vs avoid patterns

## Success Criteria

1. Instruction file is specific to technology and version
2. Patterns are actionable without external research
3. Integration with rest of stack is clear
4. `applyTo` glob matches only relevant files
5. Anti-patterns prevent common mistakes
6. Token count optimized (<1000 for typical tech)
7. Code examples demonstrate non-obvious patterns
8. Testing approach integrated throughout

## Example Technologies for Zeus Academia

**Priority 1** (Core Architecture):
- ASP.NET Core (backend framework)
- MediatR (CQRS implementation)
- FluentValidation (backend validation)
- Vue 3 (frontend framework)
- Pinia (state management)
- TypeScript (frontend language)

**Priority 2** (Infrastructure):
- Azure Static Web Apps (frontend hosting)
- Azure App Service (backend hosting)
- Azure AD B2C (authentication)
- Vite (build tool)

**Priority 3** (Testing & Supporting):
- xUnit (C# testing)
- Vitest (TypeScript testing)
- REST API patterns
- GraphQL patterns

## Usage Example

```bash
# Generate Vue 3 instruction file
submit #file:create-technology-instructions.prompt.md with arguments:
  technology_name: Vue 3
  technology_category: frontend
  primary_language: TypeScript
  project_context: SPA frontend for Academic Management System
  version_target: 3.4+

# Generate MediatR instruction file
submit #file:create-technology-instructions.prompt.md with arguments:
  technology_name: MediatR
  technology_category: backend
  primary_language: C#
  project_context: CQRS command/query handling
  version_target: 12.0+
````
