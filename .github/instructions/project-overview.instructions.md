---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-02-24-project-overview-zeus-academia"
prompt: |
  Submit create-project-overview.prompt.md with arguments:
  project_name: zeus.academia
  project_type: web app
  primary_language: C#, TypeScript
  key_technologies: Vue 3, Pinia, REST, GraphQL, Vite, Azure Static Web Apps, ASP.NET Core, MediatR, FluentValidation
  project_purpose: Academic Management System
started: "2026-02-24T00:00:00Z"
ended: "2026-02-24T00:15:00Z"
task_durations:
  - task: "analyze project structure"
    duration: "00:03:00"
  - task: "generate overview content"
    duration: "00:10:00"
  - task: "optimize tokens"
    duration: "00:02:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/02/24/2026-02-24-project-overview-zeus-academia/conversation.md"
source: ".github/prompts/create-project-overview.prompt.md"
description: "zeus.academia - Academic Management System web application"
applyTo: "**"
---

# Project: Zeus Academia

**Purpose**: Academic Management System for educational institutions
**Type**: Full-stack web application
**Stack**: C# (ASP.NET Core) + TypeScript (Vue 3)

## Architecture

- **Structure**: Monorepo with application code organized by feature domain under `src/features/`
- **UI Stack**: Vue 3 + Pinia (state) + Vite (build) + TypeScript
- **Server Stack**: ASP.NET Core + MediatR (CQRS) + FluentValidation
- **API**: REST + GraphQL endpoints
- **Core Dirs**:
  - `src/features/` - Feature domains containing co-located use-case folders and their UI/API artifacts
  - `src/shared/` - Shared primitives, contracts, and cross-cutting building blocks
  - `.github/instructions/` - AI assistant directives
  - `.github/prompts/` - Reusable prompt templates
  - `ai-logs/` - AI conversation and session logs

## Standards

- **C#**: .NET 8+, async/await, nullable reference types
- **TypeScript**: Strict mode, explicit types, composition API
- **Patterns**: CQRS (MediatR), Repository pattern, dependency injection
- **Validation**: FluentValidation (backend), Vuelidate/custom (frontend)
- **Naming**: PascalCase (C#), camelCase (TypeScript), kebab-case (Vue components)
- **Tests**: xUnit (C#), Vitest (TypeScript), 80% coverage target
- **AI Workflows**:
  - [AI Development Process](.github/instructions/ai-dev-process.instructions.md)
  - [Git Workflow](.github/instructions/git-workflow.instructions.md)
  - [AI Output Policy](.github/instructions/ai-assisted-output.instructions.md)
  - [Custom Agents Standards](.github/instructions/custom-agents.instructions.md)
  - [Implementation Prompt Generation](.github/instructions/implementation-prompt-generation.instructions.md)
  - [Vertical Slice Implementation](.github/instructions/vertical-slice-implementation.instructions.md)

## Environment

- **Target**: Modern browsers (Chrome/Edge/Firefox/Safari latest 2 versions)
- **Deploy**: Azure Static Web Apps (frontend), Azure App Service (backend)
- **Database**: Azure SQL Database or Cosmos DB
- **Dependencies**: Azure AD B2C (auth), Azure Storage (files)

## Constraints

- **Security**: OAuth 2.0/OIDC authentication, RBAC authorization, HTTPS-only
- **Performance**: API response <500ms p95, initial page load <3s
- **Compatibility**: WCAG 2.1 AA accessibility compliance
- **Data Privacy**: FERPA/GDPR compliance for student data
- **Scalability**: Support 10k+ concurrent users

## Key Patterns

- **CQRS**: Commands via MediatR for writes, queries for reads
- **Vertical Slices**: Organize by feature domain first, then by use-case folder inside `src/features/`
- **Validation**: FluentValidation rules per command/query
- **State Management**: Pinia stores scoped to the relevant feature domain and use-case
- **API Communication**: Axios (REST), Apollo Client (GraphQL)
- **Error Handling**: Centralized error boundaries and middleware
- **AI Provenance**: Complete metadata required for all AI-assisted artifacts

## Critical Files

- `.github/instructions/ai-assisted-output.instructions.md` - AI provenance policy
- `.github/instructions/ai-dev-process.instructions.md` - Development workflow
- `.github/instructions/git-workflow.instructions.md` - Branching and PR requirements
- `.github/instructions/implementation-prompt-generation.instructions.md` - Standards for slice implementation prompt files
- `.github/instructions/cqrs-es-csharp-mediatr.instructions.md` - CQRS implementation guide
- `.github/instructions/csharp-implementation.instructions.md` - C# coding standards
- `.github/instructions/vertical-slice-implementation.instructions.md` - Feature-domain and use-case structure, naming, templates, and quality checklist
