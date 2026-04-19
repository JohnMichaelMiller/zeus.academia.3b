---
ai_generated: true
model: "openai/gpt-5.4@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-04-18-academia-slice-agents-and-execution-plan"
prompt: |
  go ahead
started: "2026-04-18T14:05:00-07:00"
ended: "2026-04-18T14:30:00-07:00"
task_durations:
  - task: "agent design"
    duration: "00:10:00"
  - task: "execution plan authoring"
    duration: "00:10:00"
  - task: "provenance and catalog updates"
    duration: "00:05:00"
total_duration: "00:25:00"
ai_log: "ai-logs/2026/04/18/2026-04-18-academia-slice-agents-and-execution-plan/conversation.md"
source: "johnmillerATcodemag-com"
---

# Academia Slice Execution Plan

Dependency-ordered rollout plan for the slice prompts in `.github/prompts/academia/`.

## Preconditions

- Use `.github/agents/product-manager.agent.md` to lock scope and acceptance criteria when a prompt leaves ambiguity.
- Use `.github/agents/backend-slice-implementer.agent.md` for implementation.
- Use `.github/agents/slice-verifier.agent.md` for sign-off.
- Establish the Shared Kernel before any feature slice work begins.

## Wave 0 — Foundation

| Order | Slice         | Prompt | Why First                                                                                                                                      |
| ----- | ------------- | ------ | ---------------------------------------------------------------------------------------------------------------------------------------------- |
| 0.1   | Shared Kernel | N/A    | All slices depend on `Academic`, `Rank`, `AccessLevel`, `Degree`, `University`, `Extension`, `Result<T>`, domain events, and common exceptions |

Exit criteria:

- Shared-kernel types compile with nullability enabled.
- Exclusive employment rule is enforced in the aggregate.
- AccessLevel is derived only from rank.

## Wave 1 — Reference Data And Inventory

These can run in parallel once Shared Kernel is ready.

| Order | Slice              | Prompt                                                                  |
| ----- | ------------------ | ----------------------------------------------------------------------- |
| 1.1   | ManageRanks        | `.github/prompts/academia/manage-ranks-implementation.prompt.md`        |
| 1.2   | ManageDegrees      | `.github/prompts/academia/manage-degrees-implementation.prompt.md`      |
| 1.3   | ManageUniversities | `.github/prompts/academia/manage-universities-implementation.prompt.md` |
| 1.4   | ProvisionExtension | `.github/prompts/academia/provision-extension-implementation.prompt.md` |

Exit criteria:

- Rank, degree, university, and extension inventory endpoints exist.
- Uniqueness and deprovisioning guards are tested.

## Wave 2 — Core Registration Gate

| Order | Slice            | Prompt                                                                | Blocked By      |
| ----- | ---------------- | --------------------------------------------------------------------- | --------------- |
| 2.1   | RegisterAcademic | `.github/prompts/academia/register-academic-implementation.prompt.md` | Wave 1 complete |

Exit criteria:

- Academic onboarding works with required qualifications and extension assignment.
- Duplicate empNr, invalid extension, invalid rank, and exclusive employment failures are covered.

## Wave 3 — Independent Post-Registration Slices

These can run in parallel after RegisterAcademic.

| Order | Slice                  | Prompt                                                                       |
| ----- | ---------------------- | ---------------------------------------------------------------------------- |
| 3.1   | ViewAcademicProfile    | `.github/prompts/academia/view-academic-profile-implementation.prompt.md`    |
| 3.2   | UpdateAcademicName     | `.github/prompts/academia/update-academic-name-implementation.prompt.md`     |
| 3.3   | SearchListAcademics    | `.github/prompts/academia/search-list-academics-implementation.prompt.md`    |
| 3.4   | GrantTenure            | `.github/prompts/academia/grant-tenure-implementation.prompt.md`             |
| 3.5   | AssignContract         | `.github/prompts/academia/assign-contract-implementation.prompt.md`          |
| 3.6   | RemoveEmploymentStatus | `.github/prompts/academia/remove-employment-status-implementation.prompt.md` |
| 3.7   | ChangeRank             | `.github/prompts/academia/change-rank-implementation.prompt.md`              |
| 3.8   | RecordDegreeObtained   | `.github/prompts/academia/record-degree-obtained-implementation.prompt.md`   |
| 3.9   | AssignExtension        | `.github/prompts/academia/assign-extension-implementation.prompt.md`         |
| 3.10  | AcademicDirectory      | `.github/prompts/academia/academic-directory-implementation.prompt.md`       |
| 3.11  | ByRankReport           | `.github/prompts/academia/by-rank-report-implementation.prompt.md`           |

Exit criteria:

- Core read/update flows exist.
- Employment, rank, qualification-add, and extension-assign flows are working.
- Directory and by-rank reports read from current data.

## Wave 4 — Sequential Dependents

These require specific predecessors.

| Order | Slice                         | Prompt                                                                               | Blocked By           |
| ----- | ----------------------------- | ------------------------------------------------------------------------------------ | -------------------- |
| 4.1   | RenewContract                 | `.github/prompts/academia/renew-contract-implementation.prompt.md`                   | AssignContract       |
| 4.2   | ConvertContractToTenure       | `.github/prompts/academia/convert-contract-to-tenure-implementation.prompt.md`       | AssignContract       |
| 4.3   | UpdateDegreeUniversity        | `.github/prompts/academia/update-degree-university-implementation.prompt.md`         | RecordDegreeObtained |
| 4.4   | RemoveDegreeRecord            | `.github/prompts/academia/remove-degree-record-implementation.prompt.md`             | RecordDegreeObtained |
| 4.5   | ListQualifications            | `.github/prompts/academia/list-qualifications-implementation.prompt.md`              | RecordDegreeObtained |
| 4.6   | ReassignExtension             | `.github/prompts/academia/reassign-extension-implementation.prompt.md`               | AssignExtension      |
| 4.7   | ReleaseExtension              | `.github/prompts/academia/release-extension-implementation.prompt.md`                | AssignExtension      |
| 4.8   | ListAvailableExtensions       | `.github/prompts/academia/list-available-extensions-implementation.prompt.md`        | AssignExtension      |
| 4.9   | ByAccessLevelReport           | `.github/prompts/academia/by-access-level-report-implementation.prompt.md`           | ChangeRank           |
| 4.10  | AccessLevelDistributionReport | `.github/prompts/academia/access-level-distribution-report-implementation.prompt.md` | ChangeRank           |
| 4.11  | TenuredAcademicsReport        | `.github/prompts/academia/tenured-academics-report-implementation.prompt.md`         | GrantTenure          |
| 4.12  | ContractedAcademicsReport     | `.github/prompts/academia/contracted-academics-report-implementation.prompt.md`      | AssignContract       |
| 4.13  | ExpiringContractsReport       | `.github/prompts/academia/expiring-contracts-report-implementation.prompt.md`        | AssignContract       |
| 4.14  | QualificationReports          | `.github/prompts/academia/qualification-reports-implementation.prompt.md`            | RecordDegreeObtained |

Exit criteria:

- All dependent flows and reports are live and verified.
- Sequential rule enforcement is covered by focused tests.

## Wave 5 — Off-Boarding

| Order | Slice              | Prompt                                                                  | Blocked By                         |
| ----- | ------------------ | ----------------------------------------------------------------------- | ---------------------------------- |
| 5.1   | DeregisterAcademic | `.github/prompts/academia/deregister-academic-implementation.prompt.md` | RegisterAcademic, ReleaseExtension |

Exit criteria:

- Off-boarding releases the extension, preserves qualification history, and emits the deregistration event.

## Recommended Role Flow Per Slice

1. `product-manager`: resolve ambiguities and freeze acceptance criteria.
2. `backend-slice-implementer`: implement the slice and run focused verification.
3. `slice-verifier`: evaluate acceptance criteria, demo readiness, and residual risks.

## Fastest Safe Delivery Path

1. Finish Wave 0.
2. Run all Wave 1 slices in parallel.
3. Deliver RegisterAcademic immediately after Wave 1.
4. Run independent Wave 3 slices in parallel, prioritizing `AssignContract`, `GrantTenure`, `RecordDegreeObtained`, `AssignExtension`, and `ChangeRank` because they unlock later reports.
5. Run Wave 4 dependents as soon as each predecessor completes.
6. Finish with DeregisterAcademic once release behavior is proven.

## Verification Gates

- After Wave 0: domain invariants verified by tests.
- After Wave 1: reference-data and extension inventory endpoints verified.
- After Wave 2: onboarding happy path and critical failures verified.
- After each later wave: all new slices verified before unlocking dependents.

## Notes

- Reporting slices should use read-optimized projection queries, not aggregate loading.
- Extension uniqueness should be enforced in both handler logic and database constraints.
- If the repository scaffold is still absent when execution begins, create the minimum backend/test structure before Wave 0 and record that setup as part of the Shared Kernel milestone.
