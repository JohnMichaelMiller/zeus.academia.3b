---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-04-20-project-overview-zeus-academia-submission"
prompt: |
  Submit create-project-overview.prompt.md with arguments:
    project_name: zeus.academia
    project_type: web app
    primary_language: C#, TypeScript
    key_technologies: Vue 3, Pinia, REST, GraphQL, Vite, Azure Static Web Apps, ASP.NET Core, MediatR, FluentValidation
    project_purpose: Academic Management System
started: "2026-04-20T00:00:00Z"
ended: "2026-04-20T00:15:00Z"
task_durations:
  - task: "design comprehensive prompt structure"
    duration: "00:05:00"
  - task: "write detailed instructions with patterns"
    duration: "00:08:00"
  - task: "optimize for token efficiency"
    duration: "00:02:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/04/20/2026-04-20-project-overview-zeus-academia-submission/conversation.md"
source: ".github/prompts/create-project-overview.prompt.md"
description: "Generate comprehensive project overview instruction file for zeus.academia Academic Management System"
context: "Monorepo with feature-domain vertical slice architecture, CQRS pattern, full-stack TypeScript/C#"
expected_output: "YAML front matter + Markdown instruction file at .github/instructions/project-overview.instructions.md"
tools: [document-generation, architecture-planning]
mode: interactive
name: create-project-overview
author: John Miller
tags:
  [
    project-overview,
    documentation,
    web-app,
    architecture,
    instruction-generation,
  ]
arguments:
  - name: project_name
    description: Project name (e.g., zeus.academia)
    required: true
  - name: project_type
    description: Type of project (e.g., web app, API, service, library)
    required: true
  - name: primary_language
    description: Primary programming languages (comma-separated, e.g., C#, TypeScript)
    required: true
  - name: key_technologies
    description: Key technologies and frameworks (comma-separated)
    required: true
  - name: project_purpose
    description: Clear statement of project purpose and domain
    required: true
---

# Generate Project Overview Instruction File

## Purpose

Create a comprehensive project overview instruction file that documents project architecture, standards, coding patterns, constraints, and critical references for a full-stack web application. This serves as the repository-wide context for all AI-assisted development tasks.

## Input Arguments

- **Project Name**: {{project_name}}
- **Project Type**: {{project_type}}
- **Primary Language(s)**: {{primary_language}}
- **Key Technologies**: {{key_technologies}}
- **Project Purpose**: {{project_purpose}}

## Context

**File Location**: `.github/instructions/project-overview.instructions.md`

**Audience**: Development team, AI assistants, onboarding documentation

**Organization**: Monorepo with feature-domain vertical slice architecture (`src/features/`, `src/shared/`)

**Scope**: Repository-wide reference document with `applyTo: "**"`

## Output Requirements

### 1. Front Matter Metadata

**Required YAML Fields**:

- `ai_generated: true`
- `model`: `<provider>/<model>@<version>`
- `operator`: `<username>`
- `chat_id`: `<chat-id>`
- `prompt`: Full prompt text as provided
- `started`/`ended`: ISO8601 timestamps
- `task_durations`: Array with task names and hh:mm:ss durations
- `total_duration`: Sum of task durations
- `ai_log`: Path to conversation log file
- `source`: Reference to `.github/prompts/create-project-overview.prompt.md`
- `description`: `"{{project_name}} - {{project_purpose}}"`
- `applyTo: "**"`

### 2. Content Sections

#### Project Header (3-4 lines)

```markdown
# Project: {{project_name}}

**Purpose**: {{project_purpose}}
**Type**: {{project_type}}
**Stack**: <Primary Language(s)> + <Key Framework(s)>
```

#### Architecture (8-12 bullets)

- **Structure**: Monorepo description, feature-domain organization
- **UI Stack**: Frontend framework, state management, build tool, language
- **Server Stack**: Backend framework, patterns, validation
- **API**: REST, GraphQL, or hybrid
- **Core Dirs**: `src/features/`, `src/shared/`, `.github/instructions/`, `.github/prompts/`, `ai-logs/`

#### Standards (3-4 groups, 2-3 bullets each)

- **Language Conventions**: PascalCase (C#), camelCase (TypeScript), kebab-case (Vue components)
- **Patterns**: CQRS (MediatR), Repository pattern, dependency injection
- **Validation**: FluentValidation (backend), framework-specific (frontend)
- **Testing**: xUnit (C#), Vitest (TypeScript), 80% coverage target
- **AI Workflows**: Links to 5+ instruction files in `.github/instructions/`

#### Environment (3-4 bullets)

- **Target**: Browsers/platforms
- **Deploy**: Azure Static Web Apps (frontend), Azure App Service (backend), or equivalent
- **Database**: Azure SQL Database or Cosmos DB
- **Auth/Dependencies**: OAuth 2.0/OIDC, RBAC, storage services

#### Constraints (5 bullets)

- **Security**: OAuth 2.0/OIDC, RBAC, HTTPS
- **Performance**: API response <500ms p95, page load <3s
- **Compatibility**: WCAG 2.1 AA, browser versions
- **Privacy**: FERPA/GDPR compliance or equivalent
- **Scalability**: Concurrent user targets

#### Key Patterns (6-8 bullets)

- **CQRS**: Commands/queries via MediatR framework
- **Vertical Slices**: Feature-domain organization, use-case folders
- **Validation**: Per-command/query validation framework
- **State Management**: Feature-scoped stores (Pinia)
- **API Communication**: REST client (Axios), GraphQL (Apollo Client)
- **Error Handling**: Centralized middleware/boundaries
- **AI Provenance**: Complete metadata for all AI-assisted artifacts

#### Critical Files (8-12 entries)

Link to `.github/instructions/` files:

- `ai-assisted-output.instructions.md` - AI provenance policy
- `ai-dev-process.instructions.md` - Development workflow
- `git-workflow.instructions.md` - Branching, PR requirements
- Language/framework-specific: `csharp-implementation.instructions.md`, `vue3-implementation.instructions.md`
- Pattern-specific: `cqrs-mediatr-efcore.instructions.md`, `vertical-slice-implementation.instructions.md`
- Tool-specific: `mediatr-implementation.instructions.md`, `pinia-implementation.instructions.md`

### 3. Token Optimization

- **Target**: 1,500–2,000 tokens (balance between completeness and conciseness)
- **Format**: Bullet lists over prose, tables for structured data
- **Brevity**: Short descriptions (1 sentence max per line item)
- **Specificity**: Measurable criteria, exact names, explicit paths
- **Links**: Use markdown relative links to related `.github/instructions/` files

## Validation Checklist

- [ ] Front matter has all required YAML fields with AI provenance
- [ ] `ai_log` path references conversation log in `ai-logs/<yyyy>/<mm>/<dd>/<chat-id>/`
- [ ] `description` field shows project name and purpose
- [ ] `applyTo: "**"` for repository-wide scope
- [ ] Architecture section includes all technologies from `key_technologies` argument
- [ ] Standards section links to relevant `.github/instructions/` files
- [ ] Stack breakdown clearly separates UI, server, infrastructure layers
- [ ] Folder structure documented with feature-domain and use-case organization
- [ ] CQRS pattern explained with specific framework (MediatR, etc.)
- [ ] Vertical slice architecture documented with example paths
- [ ] Environment includes all deployment platforms mentioned in `key_technologies`
- [ ] Constraints specify security, performance, compliance, scalability
- [ ] Critical files section uses markdown links with relative paths
- [ ] Naming conventions specified for each language
- [ ] Testing approach and coverage targets included
- [ ] AI Workflows section references 5+ instruction files
- [ ] No sensitive data, credentials, or API keys included
- [ ] Markdown syntax valid, all links work
- [ ] Token count between 1,500–2,000 tokens
- No redundant explanations
- Specific over general
- Token-optimized (eliminate filler)

**Optimization Rules**:

- Use abbreviations where unambiguous
- Prefer lists over prose
- Omit obvious context
- State facts, not descriptions
- Group related items

## Example Structure

```markdown
---
description: "[Project name] - [one-line purpose]"
applyTo: "**"
[...metadata...]
---

# Project: [Name]

**Purpose**: [1-2 sentence mission]
**Type**: [category]
**Stack**: [lang] + [frameworks]

## Architecture

- **Structure**: [organization pattern]
- **Core Dirs**: `dir/` – [purpose]; `dir2/` – [purpose]
- **Build**: [tool/process]

## Standards

- [Language]: [version], [key conventions]
- Naming: [pattern]
- Tests: [framework], [coverage target]

## Environment

- Target: [platforms]
- Deploy: [method]
- Dependencies: [critical services]

## Constraints

- [constraint type]: [requirement]
```

## Validation Checklist

- [ ] All required sections present
- [ ] Metadata complete with `description` and `applyTo: "**"`
- [ ] Token-optimized (no filler words)
- [ ] Specific technologies/versions stated
- [ ] Directory structure documented
- [ ] Testing approach defined
- [ ] Critical constraints listed
- [ ] Imperative tone throughout

## Anti-Patterns to Avoid

❌ Verbose explanations of obvious concepts
❌ Redundant section introductions
❌ Vague technology references ("modern framework")
❌ Missing version information
❌ Prose format instead of lists
❌ Including implementation details (save for specific instruction files)
❌ Omitting key dependencies or constraints

## Success Criteria

1. AI agent can understand project scope in <200 tokens
2. Contains all information needed for initial orientation
3. Links to more detailed instruction files where appropriate
4. No ambiguous or vague statements
5. Every sentence provides unique, essential information
