# Session Summary – 2026-01-27-cqrs-eventsourcing-prompt

**Date**: 2026-01-27
**Operator**: johnmillerATcodemag-com
**Model**: anthropic/claude-sonnet-4.5@unknown
**Duration**: 00:20:00

## Objective

Create token-optimized prompt file for generating CQRS + Event Sourcing instruction files, covering event store operations, aggregate reconstruction, projections, and performance optimizations.

## Deliverables

1. `.github/prompts/generate-cqrs-eventsourcing-instructions.prompt.md` – Comprehensive prompt template with:
   - 15 structured sections (vs. 8 for standard CQRS)
   - Event sourcing core concepts table
   - Event-sourced aggregate pattern with apply() methods
   - Event store operations (append with versioning)
   - Three projection strategies (inline, background, separate service)
   - Snapshot optimization for large event streams
   - Event schema evolution and upcasting
   - Idempotency handling
   - Anti-patterns specific to event sourcing

## Key Differences from Standard CQRS

**Event Sourcing Additions:**

- Event store as source of truth (not traditional DB)
- Aggregate state reconstruction from events
- Event immutability and append-only semantics
- Optimistic concurrency via stream versioning
- Projection-based read models (eventual consistency)
- Snapshot strategies for performance (>50-100 events)
- Event schema versioning and upcasting

**Architecture Impact:**

- Commands produce events, not direct state changes
- Queries read from projections, never event store
- Aggregates rebuilt on every command (or from snapshot)
- Write and read models completely separated

## Decisions

- **Projection Strategies**: Parameterized (`{{projections}}`) to support inline, background worker, or separate service approaches
- **Event Store**: Abstracted to support EventStoreDB, Marten, custom implementations
- **Snapshot Threshold**: Recommended 50-100 events as configurable guideline
- **Language-Agnostic Templates**: Used pseudocode that adapts to C#, Java, TypeScript, Python
- **Token Optimization**: Tables, bullets, abbreviations (ES, evt, agg), minimal code comments

## Patterns Documented

1. **Event-Sourced Command Flow**: Validate → Load from events → Apply → Persist new events
2. **Aggregate Apply Pattern**: State reconstruction via `apply(event)` methods
3. **Event Store Operations**: `append()` with optimistic concurrency, `readStream()` for replay
4. **Projection Subscription**: Background worker reading event stream with checkpoint tracking
5. **Snapshot Strategy**: Periodic snapshots + events-since-snapshot for large aggregates
6. **Event Upcasting**: Transform old event versions on read
7. **Idempotency**: Command deduplication via metadata

## Follow-up

- [ ] Test prompt with different language/framework combinations (C# + Marten, Java + Axon)
- [ ] Consider adding CQRS+ES with SAGA pattern for distributed transactions
- [ ] Create example instruction files using this prompt to validate completeness
- [ ] Add event store comparison guide (EventStoreDB vs. Marten vs. Kafka)

## Metadata

```yaml
chat_id: 2026-01-27-cqrs-eventsourcing-prompt
started: 2026-01-27T15:00:00Z
ended: 2026-01-27T15:20:00Z
total_duration: 00:20:00
models_used:
  - anthropic/claude-sonnet-4.5@unknown
artifacts_count: 1
files_modified: 0
sections_count: 15
token_optimization: high
```
