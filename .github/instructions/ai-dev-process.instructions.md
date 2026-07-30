---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2025-12-28-ai-dev-process-generation"
prompt: |
  submit the #file:create-ai-dev-process.prompt.md
started: "2025-12-28T19:30:00Z"
ended: "2025-12-28T19:40:00Z"
task_durations:
  - task: "design workflow structure"
    duration: "00:03:00"
  - task: "draft directives"
    duration: "00:05:00"
  - task: "optimize tokens"
    duration: "00:02:00"
total_duration: "00:10:00"
ai_log: "ai-logs/2025/12/28/2025-12-28-ai-dev-process-generation/conversation.md"
source: ".github/prompts/create-ai-dev-process.prompt.md"
description: "AI-assisted software development workflow and review process"
applyTo: "**"
---

# AI-Assisted Software Development Process

## AI Code Generation

**When to Use:**

- Boilerplate, repetitive patterns, standard implementations
- Initial scaffolding, test skeletons, documentation templates
- Code transformations, refactoring, format conversions

**Requirements:**

- MUST include complete provenance metadata (see ai-assisted-output.instructions.md)
- MUST pass existing tests or include new passing tests
- MUST follow project style, patterns, and conventions
- MUST follow [.github/instructions/vertical-slice-implementation.instructions.md](vertical-slice-implementation.instructions.md) for all slice implementation work (feature-domain first, use-case folder ownership, no layer-root splitting)
- MUST document non-obvious logic, edge cases, assumptions
- MUST reference source prompt or instruction file

**Pre-PR Review-Prevention Checks (Required):**

- MUST verify reference integrity for every documented command/path before commit:
  - If documentation or agent guidance references a file, that file must be committed in the same change.
  - If guidance references an editor task (for example a VS Code task), the corresponding task file must exist in the repository; otherwise reference a committed script command instead.
- MUST validate platform assumptions for runtime tooling:
  - Windows-only fallbacks (for example LocalDB) must be explicitly guarded.
  - On non-Windows, require explicit environment configuration instead of silent fallback.
- MUST run a focused self-review for common correctness regressions before opening PR:
  - Vertical slice layout and boundaries match [.github/instructions/vertical-slice-implementation.instructions.md](vertical-slice-implementation.instructions.md).
  - Null argument validation for non-nullable API inputs.
  - Dependency package families remain version-compatible (for example xUnit core package major version aligned with its runner package major version).
  - No mutable collection escape through read-only interfaces.
  - No duplicate uniqueness enforcement on the same database key path (for example PK + duplicate unique index).
  - No duplicate project declarations in solution files; project name/path pairs must appear once with one GUID and one configuration block.
  - Any touched solution file must keep the required Visual Studio header as the first line with no leading blank line or stray BOM-only line.
  - Database constraint names must match predicate semantics; reserve "Xor" naming for strict exactly-one rules and use explicit mutual-exclusion naming when both-false is allowed.
  - Method and type naming remains compliant with language conventions (for example PascalCase in C#).
  - Exception and failure messages never include secrets (connection strings, credentials, tokens, keys).
  - Result-style failure factories enforce non-null failure payloads (for example guard `Failure(Error error)` inputs against nulls in both non-generic and generic result types).
  - Shared foundational primitives (for example Result/Error base types) retain direct tests for both non-generic and generic invariants when touched.
  - Value-object parse/creation APIs reject lossy coercion (for example silently truncating fractional inputs) unless the behavior is explicitly required and tested.
  - Integration tests that provision external resources (databases, containers, queues, files) perform best-effort cleanup in `finally` blocks.
  - Public/shared parse or mapping APIs retain direct acceptance tests when touched; do not remove only-path coverage without replacement.
  - Constrained-code parse/validation failures remain actionable by including allowed values (prefer constants over inline literals).
  - Validation messages must derive allowed values from a single source of truth rather than duplicating hard-coded literals across exception messages.
  - Database-backed tests must use unique test-scoped database names and safe connection-string handling; never use a provided connection string verbatim against a shared or non-test database.

**Prohibited Without Review:**

- Security-critical code (auth, crypto, permissions)
- Database schema changes, data migrations
- API contract modifications, breaking changes
- Production configuration, environment variables

## AI Code Reviews

**Required For:**

- All AI-generated code before commit
- All human code in PR before merge
- Any security-sensitive changes
- Cross-cutting refactors

**Review Focus:**

- **Correctness**: Logic errors, edge cases, type safety
- **Security**: Injection risks, auth bypasses, data exposure
- **Performance**: N+1 queries, memory leaks, blocking operations
- **Style**: Naming, structure, idioms, conventions
- **Tests**: Coverage, quality, edge cases

**Tools:**

- Use `review` tool for branch comparisons
- Use `reviewUnstaged` for working directory changes
- Use `reviewStaged` for pre-commit validation

**Interpretation:**

- High severity → MUST fix before commit
- Medium severity → Fix or document reason to skip
- Low severity → Consider for future improvement

## Human Code Reviews

**Mandatory For:**

- First use of new patterns, libraries, or architectures
- Security-critical changes (auth, permissions, data access)
- API contracts, database schemas, breaking changes
- Code flagged by AI review as high-risk or complex
- Any change touching >500 lines or >5 files

**Review Criteria:**

- [ ] Solves stated problem completely
- [ ] No unintended side effects or regressions
- [ ] Tests validate success and failure cases
- [ ] Documentation updated (README, API docs, comments)
- [ ] Follows project conventions and standards
- [ ] No security vulnerabilities introduced
- [ ] Performance implications acceptable

**Process:**

- AI review first → address findings → human review
- Reviewer requests changes → author updates → re-review
- Approved → ready for merge (pending PR approval)

## PR Approval Workflow

**Approval Gates:**

1. **AI Review Pass** (automated)
   - No high-severity issues unresolved
   - All required tests passing
   - Provenance metadata complete (AI-generated code)

2. **Human Approval** (1+ required)
   - Maintainer or code owner review
   - Approval indicates: correct, safe, maintainable, tested

**Who Can Approve:**

- Project maintainers: all PRs
- Code owners: files in their domain
- Senior devs: routine changes in their area

**Merge Requirements:**

- [ ] **Technical Gates**: All checks in [Git Workflow](git-workflow.instructions.md) passed
- [ ] **AI Review**: Completed with no unresolved high-severity issues
- [ ] **Human Approval**: ≥1 approval from qualified reviewer
- [ ] **Comments**: No unresolved review comments

**Special Cases:**

- Hotfix: 1 approval, AI review optional if critical
- Docs-only: AI review optional, 1 approval sufficient
- Bot/automation: Requires maintainer approval

## Quality Gates

**Technical Gates:**
Refer to [Git Workflow](git-workflow.instructions.md) for all CI, testing, and metadata requirements.

**Process Gates:**

- [ ] AI Review completed
- [ ] Human Review completed
- [ ] PR Approval granted

## Exceptions

**Emergency Hotfix:**

- May skip AI review if immediate deployment critical
- Requires post-merge review and follow-up PR if needed
- Document exception reason in commit message

**Experimental Branches:**

- Relaxed review requirements
- Must not merge to main/production branches
- Label as `experimental` or `wip`

**Automated Updates:**

- Dependency bumps: AI review + 1 approval
- Generated code (schemas, clients): Validate generation, spot-check output

## Integration

After creating this file, update `.github/instructions/project-overview.instructions.md`:

- Add reference in "Standards" or "Key Patterns" section
- Link as: `[AI-Assisted Development Process](.github/instructions/ai-dev-process.instructions.md)`
- Note: Defines code generation, review, and approval workflows
