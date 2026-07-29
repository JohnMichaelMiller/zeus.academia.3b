## Summary

Implements EP-0-1 Shared Kernel foundation for Academia Management.

This change introduces:

- Shared Kernel domain primitives and invariants.
- Result and error contracts for slice reuse.
- SQL Server-focused EF Core mappings and constraints.
- SQL Server verification tests and execution tooling.

## AI Provenance

- Chat ID: 2026-07-28-ep-0-1-shared-kernel-pr
- Model: openai/gpt-5.3-codex@unknown
- Operator: johnmillerATcodemag-com
- Logs: ai-logs/2026/07/28/2026-07-28-ep-0-1-shared-kernel-pr/conversation.md

## Changes

- Domain + reference data under src/features/SharedKernel/Foundation.
- Persistence mapping and baseline SQL under src/features/SharedKernel/Foundation/Persistence.
- Shared Kernel tests under tests/Features/SharedKernel/Foundation.
- Verification tooling in eng/ and VS Code task wiring.
- Agent updates for SQL Server verification policy.

## Testing

- [x] Shared Kernel SQL Server verification executed.
- [x] Latest run result: 17 passed, 0 failed, 0 skipped.

## Acceptance Criteria

- [x] XOR employment rule enforced for tenure and contract date.
- [x] Rank maps only P->INT, SL->NAT, L->LOC with derived access level.
- [x] Shared Kernel compiles with nullable reference types enabled.
- [x] Database constraints enforce empNr and extension uniqueness.
- [x] Tests cover invariant success/failure, derivation, and result handling.

## Review Focus

- Verify persistence mapping and uniqueness constraints.
- Verify SQL Server constraint tests and execution workflow.
- Verify agent/tooling updates are scoped to Shared Kernel verification and PR handoff.
