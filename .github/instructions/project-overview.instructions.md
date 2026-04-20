---
ai_generated: true
model: "anthropic/claude-haiku-4.5@2024-10-22"
operator: "johnmillerATcodemag-com"
chat_id: "2026-04-20-project-overview-zeus-academia-generation"
prompt: |
  Follow instructions in create-project-overview.prompt.md with arguments:
    project_name: zeus.academia
    project_type: web app
    primary_language: C#, TypeScript
    key_technologies: Vue 3, Pinia, REST, GraphQL, Vite, Azure Static Web Apps, ASP.NET Core, MediatR, FluentValidation
    project_purpose: Academic Management System
started: "2026-04-20T18:00:00Z"
ended: "2026-04-20T18:18:00Z"
task_durations:
  - task: "analyze prompt template requirements"
    duration: "00:02:00"
  - task: "generate front matter and project header"
    duration: "00:03:00"
  - task: "structure architecture, standards, environment sections"
    duration: "00:06:00"
  - task: "compile constraints, patterns, critical files"
    duration: "00:04:00"
  - task: "optimize for tokens and validate"
    duration: "00:03:00"
total_duration: "00:18:00"
ai_log: "ai-logs/2026/04/20/2026-04-20-project-overview-zeus-academia-generation/conversation.md"
source: ".github/prompts/create-project-overview.prompt.md"
description: "zeus.academia - Academic Management System web application"
applyTo: "**"
---

# Project: Zeus Academia

**Purpose**: Academic Management System for educational institutions
**Type**: Full-stack web application
**Stack**: C# (ASP.NET Core) + TypeScript (Vue 3)

## Architecture

- **Structure**: Monorepo with application code organized by feature domain under `src/features/` with co-located use-case folders
- **UI Stack**: Vue 3 + Pinia (state management) + Vite (build) + TypeScript (strict mode)
- **Server Stack**: ASP.NET Core + MediatR (CQRS) + FluentValidation
- **API**: REST endpoints + GraphQL support
- **Core Dirs**:
  - `src/features/` – Feature domains containing use-case folders and UI/API artifacts
  - `src/shared/` – Shared primitives, contracts, cross-cutting building blocks
  - `.github/instructions/` – AI assistant directives and standards
  - `.github/prompts/` – Reusable prompt templates
  - `ai-logs/` – AI conversation and session logs

## Standards

- **C#**: .NET 8+, async/await, nullable reference types, PascalCase
- **TypeScript**: Strict mode, explicit types, composition API, camelCase
- **Vue**: Kebab-case component names, Pinia stores scoped to features
- **Patterns**: CQRS (MediatR), Repository pattern, dependency injection
- **Validation**: FluentValidation (backend), Vuelidate/custom (frontend)
- **Naming**: PascalCase (C#), camelCase (TypeScript), kebab-case (Vue components)
- **Testing**: xUnit (C#), Vitest (TypeScript), 80% coverage target
- **AI Workflows**:
  - [AI Development Process](.github/instructions/ai-dev-process.instructions.md)
  - [Git Workflow](.github/instructions/git-workflow.instructions.md)
  - [AI Output Policy](.github/instructions/ai-assisted-output.instructions.md)
  - [Custom Agents Standards](.github/instructions/custom-agents.instructions.md)
  - [Implementation Prompt Generation](.github/instructions/implementation-prompt-generation.instructions.md)
  - [Implementation Prompt Standards](.github/instructions/implementation-prompt.instructions.md)
  - [Vertical Slice Implementation](.github/instructions/vertical-slice-implementation.instructions.md)

## Environment

- **Target**: Modern browsers (Chrome/Edge/Firefox/Safari, latest 2 versions)
- **Deploy**: Azure Static Web Apps (frontend), Azure App Service (backend)
- **Database**: Azure SQL Database or Cosmos DB
- **Auth/Dependencies**: Azure AD B2C (OAuth 2.0/OIDC), Azure Storage (files)

## Constraints

- **Security**: OAuth 2.0/OIDC authentication, RBAC authorization, HTTPS-only
- **Performance**: API response <500ms p95, initial page load <3s
- **Compatibility**: WCAG 2.1 AA accessibility compliance
- **Privacy**: FERPA/GDPR compliance for student data
- **Scalability**: Support 10k+ concurrent users

## Key Patterns

- **CQRS**: Commands via MediatR for writes, queries for reads with validation per command/query
- **Vertical Slices**: Feature-domain organization first, then use-case folders (e.g., `src/features/enrollment/register-student/`)
- **Validation**: FluentValidation rules per MediatR command/query, frontend validation pre-submission
- **State Management**: Pinia stores scoped to feature domains with composable actions
- **API Communication**: Axios for REST, Apollo Client for GraphQL operations
- **Error Handling**: Centralized error boundaries (Vue), middleware (ASP.NET), consistent response contracts
- **AI Provenance**: Complete metadata required for all AI-assisted artifacts with chat ID and conversation logs

## Critical Files

- [ai-assisted-output.instructions.md](.github/instructions/ai-assisted-output.instructions.md) – AI provenance policy, metadata requirements
- [ai-dev-process.instructions.md](.github/instructions/ai-dev-process.instructions.md) – Development workflow, code generation, review process
- [git-workflow.instructions.md](.github/instructions/git-workflow.instructions.md) – Trunk-based development, AI branching, PR requirements
- [implementation-prompt-generation.instructions.md](.github/instructions/implementation-prompt-generation.instructions.md) – Standards for slice implementation prompt files
- [cqrs-es-csharp-mediatr.instructions.md](.github/instructions/cqrs-es-csharp-mediatr.instructions.md) – CQRS pattern implementation guide
- [csharp-implementation.instructions.md](.github/instructions/csharp-implementation.instructions.md) – C# coding standards and conventions
- [vertical-slice-implementation.instructions.md](.github/instructions/vertical-slice-implementation.instructions.md) – Feature-domain organization, naming, templates
- [custom-agents.instructions.md](.github/instructions/custom-agents.instructions.md) – AI agent definitions and behaviors
- [prompt-file-generation.instructions.md](.github/instructions/prompt-file-generation.instructions.md) – Prompt template standards
