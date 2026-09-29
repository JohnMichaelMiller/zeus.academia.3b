# zeus.academia.3b

The third iteration of the Zeus Academia experiment

## Phase 0: Application Foundation — ✅ COMPLETE

**Status**: All 6 steps completed. Architecture finalized. Ready for Phase 1 implementation.

### What Phase 0 Established

- **Application Host**: `src/Zeus.Academia.Api/` with DI composition, configuration management, and migration orchestration
- **Shared Kernel**: Reusable domain types (Academic, AcademicQualification, Extension, value objects) and Entity Framework configurations
- **Feature-Local DbContexts**: ManageRanks, ManageDegrees, ManageUniversities (prepared), ProvisionExtension (prepared)
- **Catalog Resolution Pattern**: GetXxxByCodeQuery contracts for decoupled reference data consumption
- **Migration Ownership**: Each table owned by exactly one DbContext; no conflicts or ambiguity
- **University Identity**: Code-based, immutable identifier with historical data preservation (IsActive flag)

### Key Architectural Decisions

1. **One DbContext per feature** — Clear ownership, independent migrations, isolated testing
2. **Shared Kernel provides configurations, not DbContext** — Single source of truth; no duplication
3. **Catalog queries via IMediator** — Loose coupling between features; easy to mock and test
4. **Code-based identity for reference data** — Immutable, supports historical data and audit trails
5. **Host owns DI and configuration** — Centralized setup; environment-aware (LocalDB fallback on Windows)

### Documentation

- [Phase 0 Refactoring Plan](src/models/workflows/academia-refactoring-plan.md) — Original Phase 0 roadmap
- [Migration Ownership Matrix](src/models/workflows/migration-ownership-matrix.md) — Table ownership verification
- [Shared Kernel Persistence Boundaries](src/features/SharedKernel/PERSISTENCE_BOUNDARIES.md) — Scope and constraints
- [University Identity Resolution Contract](src/features/ReferenceData/ManageUniversities/UNIVERSITY_RESOLUTION_CONTRACT.md) — Identity mapping pattern
- [Phase 1+ Downstream Consumer Pattern](src/models/workflows/phase-1-downstream-consumer-pattern.md) — **Canonical pattern all downstream slices follow**
- [EP-2-1 RegisterAcademic Handoff](src/models/workflows/ep-2-1-register-academic-handoff.md) — First domain slice implementation guide
- [Architecture Decision Record (ADR)](src/models/workflows/adr-phase-0-architecture.md) — Decisions, trade-offs, alternatives considered
- [Phase 0 Completion Checklist](src/models/workflows/phase-0-completion-checklist.md) — Full verification status

### Quality Gates — All Passed ✅

- Full solution builds (0 warnings, 0 errors)
- All tests pass (25+ tests, 100% pass rate)
- No circular dependencies between features
- Each table owned by exactly one DbContext
- Shared Kernel free of host-specific code
- Domain invariants enforced in aggregates
- Feature-local DbContext pattern established
- Catalog resolution contracts documented

### What's Next

**Phase 1** implements reference data and the first domain slice using Phase 0 foundation:

- **EP-1-3**: ManageUniversities (catalog implementation)
- **EP-1-4**: ProvisionExtension (handlers and persistence)
- **EP-2-1**: RegisterAcademic (first domain slice using ManageRanks catalog) — [See handoff guide](src/models/workflows/ep-2-1-register-academic-handoff.md)

All Phase 1 slices follow the [canonical downstream consumer pattern](src/models/workflows/phase-1-downstream-consumer-pattern.md) **exactly**.

---

## SQL Server Setup

- The application and Shared Kernel verification flow use SQL Server for persistence and schema checks.
- Set `ZEUS_SQLSERVER_CONNECTION` when running on non-Windows hosts or CI environments that do not expose SQL Server LocalDB.
- On Windows, the verification script can fall back to SQL Server LocalDB `(localdb)\\MSSQLLocalDB` when no connection string is provided.
- Run the Shared Kernel verification script with:
  - `powershell -NoProfile -ExecutionPolicy Bypass -File eng/verify-shared-kernel-sqlserver.ps1`

## Slice Verification Gate

- Run `pwsh eng/verify-slice.ps1 -Feature <Domain>/<Feature>` (for example `Academics/RegisterAcademic`) before handoff or PR. It fails on missing host project references, LocalDB in `appsettings.json`, `SharedKernelDbContext` use in feature slices, incomplete migrations, missing validator or route tests, InMemory-only persistence tests, placeholder `Try*` outputs, and inconsistent provenance timestamps, then builds the solution and runs the feature tests.
- Options: `-SkipBuild`, `-SkipTests`, `-RunEfChecks` (runs `dotnet ef migrations list` per context), `-BaseRef` (default `origin/main`).

## AI-Assisted Artifacts

### Phase 0 Documentation (Complete)

- [Academia architecture issues and changes](docs/academia-architecture-issues-and-changes.md) - Summary of issues discovered and resolved during Phase 0. ([Log](ai-logs/2026/08/24/78bccef1-7df6-4b32-99b6-2cc4a743aecc/conversation.md))
- [Phase 0 Refactoring Plan](src/models/workflows/academia-refactoring-plan.md) - Original Phase 0 roadmap with 7 steps and verification gates
- [Phase 1+ Downstream Consumer Pattern](src/models/workflows/phase-1-downstream-consumer-pattern.md) - **Canonical integration pattern** all downstream slices follow (MUST READ)
- [EP-2-1 RegisterAcademic Handoff](src/models/workflows/ep-2-1-register-academic-handoff.md) - Implementation guide for first domain slice
- [Architecture Decision Record (ADR)](src/models/workflows/adr-phase-0-architecture.md) - Explains 5 key decisions, trade-offs, and alternatives
- [Phase 0 Completion Checklist](src/models/workflows/phase-0-completion-checklist.md) - Full verification status and quality gates

### Phase 1 Feature Slice

- [RegisterAcademic slice](src/features/Academics/RegisterAcademic/) - Registers academics with verified reference data and atomically assigns an available extension through a mapping-only context ([Log](ai-logs/2026/09/28/edaf3bc8-e80f-4b74-b582-2aa3785f6a40/conversation.md))
- [AIAGSD Part 12: Registering Academics Across Vertical Slices](jekyll-src/_posts/2026-09-28-AIAGSD12-RegisterAcademic.blog.md) - Describes the RegisterAcademic route, owner-managed reference resolution, atomic persistence, and migration boundaries ([Log](ai-logs/2026/09/28/edaf3bc8-e80f-4b74-b582-2aa3785f6a40/conversation.md))
- [ProvisionExtension pool lifecycle slice](src/features/Extensions/ProvisionExtension/) - Adds extension provisioning and deprovisioning commands, route contract, validation, and persistence tests for duplicate protection and assignment guardrails ([Log](ai-logs/2026/08/26/2026-08-26-provision-extension-implementation/conversation.md))

### Review-Prevention Tooling

- [Slice verification script](eng/verify-slice.ps1) - Mechanical pre-handoff checks derived from PR 46 Copilot review findings ([Log](ai-logs/2026/09/28/2026-09-28-pr46-review-instruction-hardening/conversation.md))
- [RegisterAcademic implementation prompt](.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md) - Regenerated with a Contract Sheet and traceability gate ([Log](ai-logs/2026/09/28/2026-09-28-pr46-review-instruction-hardening/conversation.md))

### Phase 0 Planning & Coordination

- [Academia Slice Execution Plan](.github/prompts/academia/execution-plan.md) - Phases 0-2 delivery sequence and dependencies ([Log](ai-logs/2026/04/18/2026-04-18-academia-slice-agents-and-execution-plan/conversation.md))
