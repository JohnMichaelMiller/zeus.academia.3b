# 2026-01-28-csharp-instructions-prompt

Date: 2026-01-28 | Op: johnmillerATcodemag-com | Model: anthropic/claude-sonnet-4.5@unknown | Duration: 00:15:00

## Goal

Create a reusable prompt file that generates comprehensive instruction files for guiding C# code implementation with customizable architectural patterns, frameworks, and conventions.

## Deliverables

1. `.github/prompts/generate-csharp-instructions.prompt.md` – Complete prompt template with:
   - Four customizable arguments (scope, patterns, frameworks, conventions)
   - Complete metadata specifications (AI provenance + Copilot fields)
   - Detailed generation instructions with structure requirements
   - Token optimization guidelines
   - Concrete examples demonstrating usage
   - Validation checklist for generated outputs

## Decisions

- **Parameterized Design**: Used four arguments to make prompt reusable across different C# contexts (API, domain, data access, etc.)
- **Complete Metadata**: Included both AI provenance fields and Copilot-specific metadata for full integration
- **Example-Driven**: Provided concrete input/output example (Domain Entities with DDD) to clarify expected results
- **Token Optimized**: Emphasized imperative voice, tables, and minimal examples in generated instructions
- **Framework Agnostic**: Made frameworks configurable to support various tech stacks (ASP.NET Core, MediatR, EF Core, etc.)

## Pending

- [ ] Test prompt with different argument combinations
- [ ] Validate that generated instruction files meet all standards
- [ ] Consider creating additional specialized prompts for specific C# domains

```yaml
started: 2026-01-28T00:00:00Z
ended: 2026-01-28T00:15:00Z
models: [anthropic/claude-sonnet-4.5@unknown]
artifacts: 1
modified: 0
```
