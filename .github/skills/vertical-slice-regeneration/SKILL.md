---
name: vertical-slice-regeneration
description: "Use when: regenerating one Zeus Academia slice, regenerating the complete solution from slice 0, or completely restarting from requirements; enforces reset-mode selection, confirmation-gated Git cleanup, impact manifests, and non-vacuous verification."
ai_generated: true
model: "github/copilot@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "66352ac8-c82d-4a5a-8ff7-b83cc7f482cf"
prompt: |
  make the recommended changes
  
  the regeneration process for a slice entails:
  Closing the pull request; deleting the slice branch and remote branch;
  creating a new slice branch; and running the slice prompt.
  A complete regeneration starts a new solution with everything but the
  implementation and begins with slice 0. A complete restart starts with
  only the .github folder and begins with specifying requirements.
started: "2026-09-29T17:55:25Z"
ended: "2026-09-29T18:47:15Z"
task_durations:
  - task: "update instructions prompts and agents"
    duration: "00:04:00"
  - task: "author regeneration workflow skill"
    duration: "00:01:00"
  - task: "strengthen deterministic verifier"
    duration: "00:02:40"
  - task: "run validation gates"
    duration: "00:02:00"
  - task: "clarify regeneration lifecycle modes"
    duration: "00:02:00"
total_duration: "00:11:40"
ai_log: "ai-logs/2026/09/29/66352ac8-c82d-4a5a-8ff7-b83cc7f482cf/conversation.md"
source: ".github/instructions/vertical-slice-implementation.instructions.md#regeneration-and-impact-closure"
---

# Zeus Academia Regeneration

Select exactly one regeneration mode before changing Git state or files.

## 1. Select the Regeneration Mode

### Slice regeneration

Use when replaying one slice prompt from a clean slice branch.

Required lifecycle:

1. Record the pull request, local slice branch, remote slice branch, base branch, dirty state, and unpushed commits.
2. Preserve required provenance, review findings, and handoff evidence outside the branch being deleted.
3. Present the exact destructive plan and obtain explicit approval. One approval may authorize the complete listed lifecycle; re-confirm if repository state or targets change.
4. Close the slice pull request.
5. Switch to and update the approved base branch so the checked-out slice branch can be deleted safely.
6. Delete the local slice branch.
7. Delete the remote slice branch.
8. Create a new slice branch from the approved base using the repository branch convention.
9. Confirm the new branch contains the slice prompt and prerequisites but not the discarded slice implementation.
10. Build the impact manifest, run prerequisite gates, and execute the slice prompt.

Do not restore implementation files from the deleted branch unless the user explicitly changes the mode from regeneration to recovery.

### Complete regeneration

Use when rebuilding the entire application in a new solution while retaining non-implementation assets.

1. Create a new destination solution/workspace; do not overwrite the current workspace by default.
2. Inventory and copy approved non-implementation assets, including `.github`, requirements, domain/ORM models, workflow plans, documentation, licenses, and provenance logs.
3. Exclude prior implementation artifacts: application/feature source, tests, migrations, generated solution/project files, build output, and implementation-specific host configuration.
4. Verify the destination has no prior implementation or migration artifacts.
5. Begin with the slice 0 prompt and proceed in the approved execution-plan order, creating a fresh manifest for every slice.

### Complete restart

Use when discarding both implementation and prior requirements/planning conclusions.

1. Create a new destination workspace containing only the approved `.github` customization tree.
2. Do not copy prior source, tests, migrations, solution/project files, requirements, models, workflow plans, or generated documentation.
3. Begin by specifying and approving requirements.
4. Generate plans/prompts from those approved requirements before any slice or solution implementation begins.

Complete regeneration and complete restart are workspace-creation operations. Destructive replacement of an existing workspace requires separate explicit approval and a verified backup or clean remote source.

## 2. Implementation Preflight

1. Read the slice prompt and canonical instruction owners.
2. Record:
   - `git branch --show-current`
   - resolved base ref
   - target source/test folders present on the current branch
   - prerequisite features
3. For slice regeneration, verify the new branch contains the prompt and prerequisites but not the discarded implementation. For ordinary implementation, stop when a target or prerequisite exists only on another branch/commit.
4. Run each prerequisite gate:

```powershell
pwsh eng/verify-slice.ps1 -Feature <Domain/Feature>
```

Compile-only evidence does not satisfy preflight.

## 3. Build the Impact Manifest

Complete every row before editing:

| Concern                                        | Value |
| ---------------------------------------------- | ----- |
| Branch / base ref                              |       |
| Target feature folders                         |       |
| Test projects                                  |       |
| Prerequisite features and gate results         |       |
| Shared Kernel constants/configurations changed |       |
| Every consuming DbContext                      |       |
| Migration owner per table                      |       |
| Schema state (`Fresh` or evidenced `Deployed`) |       |
| Host project reference / DI / route map        |       |
| Solution entries                               |       |
| Explicit final feature set                     |       |

A Shared Kernel constant or entity configuration expands the impact set to every consuming DbContext, even before Git shows changes in those feature folders.

## 4. Persistence Checkpoint

1. Read `src/models/workflows/migration-ownership-matrix.md`.
2. For `Fresh` schema, create or regenerate a normal initial migration.
3. For `Deployed` schema, require external evidence, baseline state, legacy column shape, and data-preserving upgrades.
4. For every migration owner, require:
   - migration class
   - matching Designer
   - owner snapshot
   - `dotnet ef migrations list`
   - `dotnet ef migrations has-pending-model-changes`
   - generated SQL inspection
   - SQL Server application to a unique test database
5. Update matrix IDs only after all evidence passes.

Do not continue to downstream behavior while an owner snapshot is stale.

## 5. Behavior Checkpoint

- Implement Contract Sheet fields and error mappings exactly.
- Register and invoke every validator advertised by endpoint metadata.
- Add endpoint-attributable `WebApplicationFactory` tests for every declared status and validated error key.
- Keep route tests separate from SQL Server persistence evidence.
- For claims/reservations/allocations, require a transactional atomic predicate update or concurrency token, zero-row conflict translation, narrow uniqueness translation, and a concurrent SQL Server test using separate claimant contexts plus fresh-context read-back.
- Search test fixtures after shared invariant changes; every seed value must satisfy current canonical rules.

## 6. Evidence-Gated Handoffs

| Owner                | Cannot hand off until                                                                     |
| -------------------- | ----------------------------------------------------------------------------------------- |
| Slice coordinator    | branch/base and manifest recorded; prerequisite gates pass                                |
| Data persistence     | all consuming contexts inventoried; artifacts, matrix, EF drift, and SQL application pass |
| Backend/domain       | runtime validation and atomic claim contracts pass focused tests                          |
| Testing/verification | route, SQL migration, concurrency, and traceability evidence are independently reproduced |

## 7. Final Verification

Run the explicit impact set first:

```powershell
pwsh eng/verify-slice.ps1 -Features <feature-a>,<feature-b>
```

Then run Git discovery:

```powershell
pwsh eng/verify-slice.ps1 -AllChangedFeatures
```

Reject completion when:

- explicit and Git-detected sets differ without explanation;
- a non-empty manifest produces `(none)`;
- any feature gate fails;
- the solution omits a source/test project;
- matrix migration IDs do not resolve to complete artifacts.

## 8. Handoff

Report:

- manifest and branch/base
- files changed
- migration owner/name/provider evidence
- route and SQL Server test counts
- explicit-feature and changed-feature command results
- acceptance criterion → file → test traceability
- unresolved risks or approved waivers
- selected regeneration mode and lifecycle evidence (PR/branch cleanup, new solution inventory, or requirements restart)
