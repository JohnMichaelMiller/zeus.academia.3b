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

- Run `pwsh eng/verify-slice.ps1 -AllChangedFeatures` before handoff or PR (or `-Feature <Domain>/<Feature>` for one slice, for example `Academics/RegisterAcademic`). For regeneration, first run `-Features <feature-a>,<feature-b>` with every manifest impact feature so verification cannot pass vacuously when Git discovery is empty or incomplete. The script checks solution integrity, migration-matrix artifacts, documented paths and cited parameters, provenance, host wiring, configuration, persistence ownership, EF drift, concurrency guards, validator/route tests, provider-backed migration evidence, and feature tests.
- Options: `-SkipBuild`, `-SkipTests`, `-SkipEfChecks`, `-BaseRef` (default `origin/main`).

## AI-Assisted Artifacts

### Phase 0 Documentation (Complete)

- [Academia architecture issues and changes](docs/academia-architecture-issues-and-changes.md) - Summary of issues discovered and resolved during Phase 0. ([Log](ai-logs/2026/08/24/78bccef1-7df6-4b32-99b6-2cc4a743aecc/conversation.md))
- [Phase 0 Refactoring Plan](src/models/workflows/academia-refactoring-plan.md) - Original Phase 0 roadmap with 7 steps and verification gates
- [Phase 1+ Downstream Consumer Pattern](src/models/workflows/phase-1-downstream-consumer-pattern.md) - **Canonical integration pattern** all downstream slices follow (MUST READ)
- [EP-2-1 RegisterAcademic Handoff](src/models/workflows/ep-2-1-register-academic-handoff.md) - Implementation guide for first domain slice
- [Architecture Decision Record (ADR)](src/models/workflows/adr-phase-0-architecture.md) - Explains 5 key decisions, trade-offs, and alternatives
- [Phase 0 Completion Checklist](src/models/workflows/phase-0-completion-checklist.md) - Full verification status and quality gates

### Phase 1 Feature Slice

- [ProvisionExtension pool lifecycle slice](src/features/Extensions/ProvisionExtension/) - Adds extension provisioning and deprovisioning commands, route contract, validation, and persistence tests for duplicate protection and assignment guardrails ([Log](ai-logs/2026/08/26/2026-08-26-provision-extension-implementation/conversation.md))

### Publication Artifacts

- [Executing and verifying the RegisterAcademic vertical slice](CODE/2026-09-29-register-academic-implementation.blog.md) - Explains prompt execution, SQL Server verification, and the API showcase flow ([Log](ai-logs/2026/09/29/2026-09-29-register-academic-blog-execution-guide/conversation.md))

### Review-Prevention Tooling

- [Slice verification script](eng/verify-slice.ps1) - Mechanical pre-handoff checks derived from PR 46 Copilot review findings ([Log](ai-logs/2026/09/28/2026-09-28-pr46-review-instruction-hardening/conversation.md)); extended with `-AllChangedFeatures`, default EF drift checks, and concurrency/per-field route checks from PR 48 findings ([Log](ai-logs/2026/09/29/2026-09-29-pr48-review-instruction-hardening/conversation.md))
- [RegisterAcademic implementation prompt](.github/prompts/academia-implementation/ep-2-1-register-academic-implementation.prompt.md) - Regenerated with a Contract Sheet and traceability gate ([Log](ai-logs/2026/09/28/2026-09-28-pr46-review-instruction-hardening/conversation.md))
- [Vertical slice regeneration skill](.github/skills/vertical-slice-regeneration/SKILL.md) - Enforces branch preflight, impact manifests, migration-owner closure, atomic claims, and explicit final feature verification ([Log](ai-logs/2026/09/29/66352ac8-c82d-4a5a-8ff7-b83cc7f482cf/conversation.md))

### Phase 0 Planning & Coordination

- [Academia Slice Execution Plan](src/models/workflows/academia-execution-plan.md) - Phases 0-2 delivery sequence and dependencies ([Log](ai-logs/2026/04/18/2026-04-18-academia-slice-agents-and-execution-plan/conversation.md))
