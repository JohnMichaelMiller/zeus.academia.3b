---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-01-27-cqrs-eventsourcing-prompt"
prompt: |
  create a prompt file that generates instruction files for a CQRS architecture
  including event sourcing. the prompt file should target ai agents and be optimized
  to reduce token consumption.
started: "2026-01-27T15:00:00Z"
ended: "2026-01-27T15:20:00Z"
task_durations:
  - task: "analyze event sourcing patterns"
    duration: "00:06:00"
  - task: "draft prompt structure"
    duration: "00:10:00"
  - task: "optimize tokens"
    duration: "00:04:00"
total_duration: "00:20:00"
ai_log: "ai-logs/2026/01/27/2026-01-27-cqrs-eventsourcing-prompt/conversation.md"
source: "user-request"
name: generate-cqrs-eventsourcing-instructions
description: Generate CQRS with Event Sourcing instruction files with token optimization
author: johnmillerATcodemag-com
tags: [cqrs, event-sourcing, architecture, instructions, ddd]
arguments:
  - name: language
    description: Programming language (C#, Java, TypeScript, Python)
  - name: framework
    description: Mediator framework (MediatR, Axon, EventStore, custom)
  - name: event_store
    description: Event store implementation (EventStoreDB, Marten, custom)
  - name: projections
    description: Projection strategy (inline, background, separate-service)
context: "CQRS + Event Sourcing with event store, aggregate reconstruction, projections, and snapshots"
expected_output: "Instruction file (.instructions.md) with CQRS+ES implementation rules"
tools: ["create_file", "read_file"]
mode: "code-generation"
---

# Generate CQRS + Event Sourcing Instruction File

Create instruction file for CQRS with Event Sourcing in {{language}} using {{framework}} and {{event_store}}.

## Context

**Architecture:** CQRS + Event Sourcing with {{event_store}}
**Projections:** {{projections}} read model updates
**Framework:** {{framework}} for command/query handling
**Target:** `.instructions.md` file for AI-assisted code generation

## Requirements

### File Structure

**Path:** `.github/instructions/cqrs-es-{{language}}-{{framework}}.instructions.md`
**Format:** Markdown with YAML front matter

### Metadata

```yaml
ai_generated: true
model: "<provider>/<model>@<version>"
operator: "<github-username>"
chat_id: "<chat-id>"
prompt: "<exact request>"
started: "<ISO8601>"
ended: "<ISO8601>"
task_durations: [...]
total_duration: "<hh:mm:ss>"
ai_log: "ai-logs/<path>/conversation.md"
source: "<creator>"
description: "CQRS + Event Sourcing for {{language}} with {{framework}} and {{event_store}}"
applyTo: "src/**/*.{{extension}}"
```

### Content Sections

#### 1. Title & Overview (2 sentences max)

Pattern purpose, event sourcing benefit, target use case.

#### 2. Core Concepts

Table:
| Concept | Definition | Purpose |
|---------|------------|---------|
| Event | Immutable fact | State change record |
| Aggregate | Consistency boundary | Event producer |
| Event Store | Append-only log | Source of truth |
| Projection | Read model | Query optimization |
| Snapshot | Aggregate state cache | Performance |

#### 3. Command Pattern (Event Sourced)

**Structure Table:**
| Component | Purpose | Location |
|-----------|---------|----------|
| Command | Intent to change | `Commands/<Aggregate>/` |
| Handler | Validate → load → execute → persist events | `Commands/<Aggregate>/` |
| Aggregate | Apply events → produce new events | `Domain/Aggregates/` |
| Event | State change record | `Domain/Events/` |

**Rules:**

- Command → handler loads aggregate from events
- Aggregate methods return new events (no direct state mutation)
- Handler appends events to store
- One command = one aggregate = atomic transaction
- Optimistic concurrency via version/stream position

**Template (minimal):**

```{{language}}
// Event
class OrderCreated {
    orderId, customerId, timestamp, items
}

// Aggregate
class Order {
    apply(OrderCreated e) { /* update internal state */ }
    static create(customerId, items) → [OrderCreated event]
}

// Handler
handle(CreateOrderCommand cmd) {
    validate(cmd)
    events = Order.create(cmd.customerId, cmd.items)
    eventStore.append("order-{id}", events, expectedVersion)
}
```

#### 4. Event Store Operations

**Write:**

- `append(streamId, events, expectedVersion)` → throw on version conflict
- Stream naming: `{aggregate}-{id}` (e.g., `order-123`)
- Event metadata: id, type, timestamp, version, correlationId

**Read:**

- `readStream(streamId)` → all events for aggregate
- `readStreamForward(streamId, fromVersion)` → event replay
- Reconstruct aggregate: `events.reduce(aggregate.apply, initialState)`

**Template:**

```{{language}}
loadAggregate(id) {
    stream = eventStore.readStream("order-{id}")
    return stream.events.reduce((agg, evt) => agg.apply(evt), new Order())
}
```

#### 5. Query Pattern (Projection-Based)

**Structure Table:**
| Component | Purpose | Location |
|-----------|---------|----------|
| Query | Read criteria | `Queries/<Context>/` |
| Handler | Read from projection DB | `Queries/<Context>/` |
| Projection | Event subscriber → update read model | `Projections/` |
| Read Model | Denormalized view | Separate DB/schema |

**Rules:**

- Queries NEVER read event store directly
- Projections subscribe to event stream
- Update read models on event arrival
- Eventual consistency between write/read
- Use {{projections}} strategy

**Template:**

```{{language}}
// Projection
on(OrderCreated evt) {
    readDb.insert({ id: evt.orderId, status: "Created", ... })
}

// Query Handler
handle(GetOrderQuery q) {
    return readDb.findById(q.orderId) // no event replay
}
```

#### 6. Event Schema

**Structure:**

- `eventType` (string): Fully qualified name
- `eventId` (UUID): Unique identifier
- `aggregateId` (UUID): Source aggregate
- `data` (JSON): Event payload
- `metadata`: timestamp, version, userId, correlationId
- `version` (int): Stream version

**Versioning:**

- V1 → V2: Upcasters transform old events on read
- Include version in event type or metadata
- Never mutate published events

#### 7. Aggregate Pattern

**Rules:**

- State rebuilt from events via `apply(event)` methods
- Command methods validate → return events (no side effects)
- No direct state setters (event sourcing invariant)
- One aggregate = one stream
- Keep aggregates small (≤20 events for good perf)

**Template:**

```{{language}}
class Order {
    private state = { id, status, items }

    // Apply past events (rebuild state)
    apply(OrderCreated e) { this.state = { id: e.orderId, status: "Created" } }
    apply(OrderShipped e) { this.state.status = "Shipped" }

    // Command methods return new events
    ship() {
        if (state.status != "Created") throw Error()
        return [new OrderShipped(state.id, now())]
    }
}
```

#### 8. Snapshots (Performance)

**When:** Aggregate has >50-100 events (configurable threshold)
**Strategy:** Periodic snapshot + events-since-snapshot
**Storage:** Separate snapshot table/stream

**Template:**

```{{language}}
loadAggregate(id) {
    snapshot = snapshotStore.getLatest("order-{id}")
    events = eventStore.readStreamForward("order-{id}", snapshot.version + 1)
    return events.reduce((agg, evt) => agg.apply(evt), snapshot.state)
}

saveSnapshot(aggregate, version) {
    snapshotStore.save("order-{id}", { state: aggregate.state, version })
}
```

#### 9. Projection Strategies

**Inline (Synchronous):**

- Update read model in same transaction as event append
- Guarantees consistency, limited scalability

**Background (Async Worker):**

- Subscription reads event stream → updates projections
- Eventual consistency, scales horizontally
- Track checkpoint (last processed event position)

**Separate Service:**

- Dedicated projection microservice
- Publishes events via message bus
- Best for multi-team systems

**Template (Background):**

```{{language}}
subscribeToAll(checkpoint) {
    while (true) {
        events = eventStore.readAllForward(checkpoint)
        foreach (evt in events) {
            projection.handle(evt) // update read model
            checkpoint = evt.position
            checkpointStore.save(checkpoint)
        }
    }
}
```

#### 10. Project Structure

```
src/
├── Application/
│   ├── Commands/
│   │   └── <Aggregate>/
│   │       ├── <Name>Command.{{ext}}
│   │       └── <Name>CommandHandler.{{ext}}
│   ├── Queries/
│   │   └── <Context>/
│   │       ├── <Name>Query.{{ext}}
│   │       └── <Name>QueryHandler.{{ext}}
│   └── Projections/
│       └── <Name>Projection.{{ext}}
├── Domain/
│   ├── Aggregates/
│   │   └── <Aggregate>.{{ext}}
│   └── Events/
│       └── <Name>Event.{{ext}}
├── Infrastructure/
│   ├── EventStore/
│   │   ├── EventStoreRepository.{{ext}}
│   │   └── SnapshotStore.{{ext}}
│   └── ReadModel/
│       └── ProjectionDbContext.{{ext}}
```

#### 11. Idempotency

**Problem:** Duplicate commands → duplicate events
**Solutions:**

- Command deduplication via `commandId` in metadata
- Check if aggregate already processed command (event exists)
- Use {{framework}}-specific idempotency middleware

#### 12. Anti-Patterns

| ❌ DON'T                            | ✅ DO                              |
| ----------------------------------- | ---------------------------------- |
| Query event store for reads         | Use projections/read models        |
| Mutate aggregate state directly     | Apply events to rebuild state      |
| Delete events                       | Mark with `Deleted` event          |
| Share events between aggregates     | Events belong to one stream        |
| Load multiple aggregates in command | One command = one aggregate        |
| Skip snapshot for large streams     | Snapshot at threshold (>50 events) |

#### 13. Event Schema Evolution

**Strategies:**

1. **Upcasting:** Transform V1 → V2 on read
2. **Weak Schema:** Use flexible JSON, add optional fields
3. **Event Versioning:** Include version in type (`OrderCreatedV2`)

**Template:**

```{{language}}
upcaster(evt) {
    if (evt.type == "OrderCreatedV1") {
        return { ...evt, type: "OrderCreatedV2", data: { ...evt.data, newField: default } }
    }
    return evt
}
```

#### 14. Testing

**Command Tests:** Verify events produced

```{{language}}
given([OrderCreated])
when(ShipOrderCommand)
then([OrderShipped])
```

**Projection Tests:** Verify read model updates

```{{language}}
given([OrderCreated, OrderShipped])
then(readModel.status == "Shipped")
```

**Aggregate Tests:** Given-When-Then with events

#### 15. Checklist

- [ ] Events immutable with all required fields
- [ ] Aggregates rebuild from events via `apply()`
- [ ] Commands validated before event production
- [ ] Event store append uses optimistic concurrency
- [ ] Projections subscribe to event stream
- [ ] Queries read from read model, not event store
- [ ] Snapshot strategy defined for large streams
- [ ] Event versioning/upcasting strategy in place
- [ ] Idempotency handled for commands
- [ ] Stream naming convention followed

## Token Optimization

**Eliminate:**

- Verbose explanations (use tables, bullets)
- Redundant examples (one per pattern)
- Marketing/introductory fluff

**Use:**

- Tables for structured data
- Code templates with minimal comments
- Imperative verbs (Load, Append, Apply, Subscribe)
- Abbreviations (ES, CQRS, evt, agg, cmd)
- Inline definitions

**Format:**

- Headers for navigation
- Lists for rules/steps
- Inline code for symbols (`OrderCreated`, `apply()`)

## Validation

- [ ] All metadata fields complete
- [ ] `applyTo` glob pattern defined for {{language}}
- [ ] Event sourcing concepts table included
- [ ] Command/Query/Projection patterns documented
- [ ] Event store operations specified
- [ ] Aggregate pattern with `apply()` shown
- [ ] Snapshot strategy covered
- [ ] Projection strategies explained ({{projections}})
- [ ] Anti-patterns identified
- [ ] Testing approach defined
- [ ] Token-optimized (tables, bullets, concise code)

## Output Format

Complete `.instructions.md` file with:

1. Valid YAML front matter
2. 15 structured sections
3. Working code templates in {{language}}
4. Clear directory structure
5. Actionable directives (verbs)
6. No unnecessary prose
7. Event sourcing patterns prioritized
