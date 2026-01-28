---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-01-27-cqrs-es-csharp-generation"
prompt: |
  Submit the prompt generate-cqrs-eventsourcing-instructions.prompt.md in the C# language,
  the MediatR framework, the EventStoreDB event_store and inline projections
started: "2026-01-27T15:30:00Z"
ended: "2026-01-27T15:45:00Z"
task_durations:
  - task: "parse prompt template"
    duration: "00:02:00"
  - task: "generate instruction content"
    duration: "00:10:00"
  - task: "validate structure"
    duration: "00:03:00"
total_duration: "00:15:00"
ai_log: "ai-logs/2026/01/27/2026-01-27-cqrs-es-csharp-generation/conversation.md"
source: ".github/prompts/generate-cqrs-eventsourcing-instructions.prompt.md"
description: "CQRS + Event Sourcing for C# with MediatR and EventStoreDB"
applyTo: "src/**/*.cs"
---

# CQRS + Event Sourcing Implementation Rules

C# implementation of CQRS with Event Sourcing using MediatR and EventStoreDB. Events are the source of truth; aggregates rebuild from event streams; inline projections provide immediate consistency.

## Core Concepts

| Concept     | Definition            | Purpose             |
| ----------- | --------------------- | ------------------- |
| Event       | Immutable fact        | State change record |
| Aggregate   | Consistency boundary  | Event producer      |
| Event Store | Append-only log       | Source of truth     |
| Projection  | Read model            | Query optimization  |
| Snapshot    | Aggregate state cache | Performance         |

## Command Pattern (Event Sourced)

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

**Template:**

```csharp
// Event
public record OrderCreated(Guid OrderId, Guid CustomerId, DateTime Timestamp, List<OrderItem> Items);

// Aggregate
public class Order
{
    private Guid _id;
    private string _status;

    public void Apply(OrderCreated e)
    {
        _id = e.OrderId;
        _status = "Created";
    }

    public static IEnumerable<object> Create(Guid customerId, List<OrderItem> items)
    {
        yield return new OrderCreated(Guid.NewGuid(), customerId, DateTime.UtcNow, items);
    }
}

// Handler
public class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, Guid>
{
    private readonly IEventStoreRepository _eventStore;

    public async Task<Guid> Handle(CreateOrderCommand cmd, CancellationToken ct)
    {
        // Validate
        if (cmd.Items.Count == 0) throw new InvalidOperationException();

        // Create events
        var events = Order.Create(cmd.CustomerId, cmd.Items);

        // Append to EventStoreDB
        var orderId = Guid.NewGuid();
        await _eventStore.AppendAsync($"order-{orderId}", events, ExpectedVersion.NoStream, ct);

        return orderId;
    }
}
```

## Event Store Operations

**Write:**

- `AppendAsync(streamId, events, expectedVersion)` → throw on version conflict
- Stream naming: `{aggregate}-{id}` (e.g., `order-123`)
- Event metadata: id, type, timestamp, version, correlationId

**Read:**

- `ReadStreamAsync(streamId)` → all events for aggregate
- `ReadStreamForwardAsync(streamId, fromVersion)` → event replay
- Reconstruct aggregate: `events.Aggregate(aggregate.Apply, initialState)`

**Template:**

```csharp
public async Task<Order> LoadAggregateAsync(Guid id, CancellationToken ct)
{
    var stream = await _eventStore.ReadStreamAsync($"order-{id}", ct);
    var order = new Order();

    foreach (var evt in stream.Events)
    {
        order.Apply((dynamic)evt);
    }

    return order;
}
```

## Query Pattern (Projection-Based)

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
- Use inline strategy

**Template:**

```csharp
// Projection (inline - same transaction)
public class OrderProjection
{
    private readonly ReadDbContext _readDb;

    public void On(OrderCreated evt)
    {
        _readDb.Orders.Add(new OrderReadModel
        {
            Id = evt.OrderId,
            Status = "Created",
            CustomerId = evt.CustomerId,
            CreatedAt = evt.Timestamp
        });
    }
}

// Query Handler
public class GetOrderQueryHandler : IRequestHandler<GetOrderQuery, OrderReadModel>
{
    private readonly ReadDbContext _readDb;

    public async Task<OrderReadModel> Handle(GetOrderQuery q, CancellationToken ct)
    {
        return await _readDb.Orders.FindAsync(new object[] { q.OrderId }, ct);
    }
}
```

## Event Schema

**Structure:**

- `eventType` (string): Fully qualified name
- `eventId` (Guid): Unique identifier
- `aggregateId` (Guid): Source aggregate
- `data` (JSON): Event payload
- `metadata`: timestamp, version, userId, correlationId
- `version` (long): Stream version

**Versioning:**

- V1 → V2: Upcasters transform old events on read
- Include version in event type or metadata
- Never mutate published events

## Aggregate Pattern

**Rules:**

- State rebuilt from events via `Apply(event)` methods
- Command methods validate → return events (no side effects)
- No direct state setters (event sourcing invariant)
- One aggregate = one stream
- Keep aggregates small (≤20 events for good perf)

**Template:**

```csharp
public class Order
{
    private Guid _id;
    private string _status;
    private List<OrderItem> _items = new();

    // Apply past events (rebuild state)
    public void Apply(OrderCreated e)
    {
        _id = e.OrderId;
        _status = "Created";
        _items = e.Items;
    }

    public void Apply(OrderShipped e)
    {
        _status = "Shipped";
    }

    // Command methods return new events
    public IEnumerable<object> Ship()
    {
        if (_status != "Created")
            throw new InvalidOperationException("Cannot ship order in status: " + _status);

        yield return new OrderShipped(_id, DateTime.UtcNow);
    }
}
```

## Snapshots (Performance)

**When:** Aggregate has >50-100 events (configurable threshold)
**Strategy:** Periodic snapshot + events-since-snapshot
**Storage:** Separate snapshot table/stream

**Template:**

```csharp
public async Task<Order> LoadAggregateAsync(Guid id, CancellationToken ct)
{
    var snapshot = await _snapshotStore.GetLatestAsync($"order-{id}", ct);
    var fromVersion = snapshot?.Version + 1 ?? 0;

    var events = await _eventStore.ReadStreamForwardAsync($"order-{id}", fromVersion, ct);

    var order = snapshot?.State ?? new Order();
    foreach (var evt in events)
    {
        order.Apply((dynamic)evt);
    }

    return order;
}

public async Task SaveSnapshotAsync(Order aggregate, long version, CancellationToken ct)
{
    await _snapshotStore.SaveAsync($"order-{aggregate.Id}",
        new Snapshot<Order> { State = aggregate, Version = version }, ct);
}
```

## Projection Strategies

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

**Template (Inline):**

```csharp
public class InlineProjectionService : IProjectionService
{
    private readonly ReadDbContext _readDb;
    private readonly OrderProjection _orderProjection;

    public async Task ProjectAsync(IEnumerable<object> events, CancellationToken ct)
    {
        foreach (var evt in events)
        {
            switch (evt)
            {
                case OrderCreated e:
                    _orderProjection.On(e);
                    break;
                case OrderShipped e:
                    _orderProjection.On(e);
                    break;
            }
        }

        await _readDb.SaveChangesAsync(ct);
    }
}

// In command handler
await _eventStore.AppendAsync(streamId, events, expectedVersion, ct);
await _projectionService.ProjectAsync(events, ct); // inline - same transaction scope
```

## Project Structure

```
src/
├── Application/
│   ├── Commands/
│   │   └── <Aggregate>/
│   │       ├── <Name>Command.cs
│   │       └── <Name>CommandHandler.cs
│   ├── Queries/
│   │   └── <Context>/
│   │       ├── <Name>Query.cs
│   │       └── <Name>QueryHandler.cs
│   └── Projections/
│       └── <Name>Projection.cs
├── Domain/
│   ├── Aggregates/
│   │   └── <Aggregate>.cs
│   └── Events/
│       └── <Name>Event.cs
├── Infrastructure/
│   ├── EventStore/
│   │   ├── EventStoreRepository.cs
│   │   └── SnapshotStore.cs
│   └── ReadModel/
│       └── ProjectionDbContext.cs
```

## Idempotency

**Problem:** Duplicate commands → duplicate events
**Solutions:**

- Command deduplication via `CommandId` in metadata
- Check if aggregate already processed command (event exists)
- Use MediatR-specific idempotency middleware

**Template:**

```csharp
public class IdempotencyBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ICommandIdempotencyStore _store;

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct)
    {
        if (request is IIdempotentCommand cmd)
        {
            if (await _store.ExistsAsync(cmd.CommandId, ct))
                return await _store.GetResultAsync<TResponse>(cmd.CommandId, ct);

            var result = await next();
            await _store.SaveAsync(cmd.CommandId, result, ct);
            return result;
        }

        return await next();
    }
}
```

## Anti-Patterns

| ❌ DON'T                            | ✅ DO                              |
| ----------------------------------- | ---------------------------------- |
| Query event store for reads         | Use projections/read models        |
| Mutate aggregate state directly     | Apply events to rebuild state      |
| Delete events                       | Mark with `Deleted` event          |
| Share events between aggregates     | Events belong to one stream        |
| Load multiple aggregates in command | One command = one aggregate        |
| Skip snapshot for large streams     | Snapshot at threshold (>50 events) |

## Event Schema Evolution

**Strategies:**

1. **Upcasting:** Transform V1 → V2 on read
2. **Weak Schema:** Use flexible JSON, add optional fields
3. **Event Versioning:** Include version in type (`OrderCreatedV2`)

**Template:**

```csharp
public class EventUpcaster
{
    public object Upcast(object evt, string eventType)
    {
        return eventType switch
        {
            "OrderCreatedV1" => UpcastOrderCreatedV1((OrderCreatedV1)evt),
            _ => evt
        };
    }

    private OrderCreatedV2 UpcastOrderCreatedV1(OrderCreatedV1 evt)
    {
        return new OrderCreatedV2(
            evt.OrderId,
            evt.CustomerId,
            evt.Timestamp,
            evt.Items,
            NewField: "default-value"
        );
    }
}
```

## Testing

**Command Tests:** Verify events produced

```csharp
[Fact]
public void Ship_ProducesOrderShippedEvent()
{
    // Given
    var order = new Order();
    order.Apply(new OrderCreated(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new()));

    // When
    var events = order.Ship().ToList();

    // Then
    Assert.Single(events);
    Assert.IsType<OrderShipped>(events[0]);
}
```

**Projection Tests:** Verify read model updates

```csharp
[Fact]
public async Task OrderCreated_UpdatesReadModel()
{
    // Given
    var evt = new OrderCreated(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new());

    // When
    _projection.On(evt);
    await _readDb.SaveChangesAsync();

    // Then
    var readModel = await _readDb.Orders.FindAsync(evt.OrderId);
    Assert.Equal("Created", readModel.Status);
}
```

**Aggregate Tests:** Given-When-Then with events

```csharp
[Fact]
public void GivenCreated_WhenShip_ThenShipped()
{
    // Given
    var order = new Order();
    order.Apply(new OrderCreated(Guid.NewGuid(), Guid.NewGuid(), DateTime.UtcNow, new()));

    // When
    var events = order.Ship().ToList();

    // Then
    Assert.Contains(events, e => e is OrderShipped);
}
```

## Checklist

- [ ] Events immutable with all required fields (records preferred)
- [ ] Aggregates rebuild from events via `Apply()` methods
- [ ] Commands validated before event production
- [ ] Event store append uses optimistic concurrency (ExpectedVersion)
- [ ] Projections update read model inline (same transaction)
- [ ] Queries read from read model, not event store
- [ ] Snapshot strategy defined for large streams (>50 events)
- [ ] Event versioning/upcasting strategy in place
- [ ] Idempotency handled via MediatR pipeline behavior
- [ ] Stream naming convention followed (`{aggregate}-{id}`)
- [ ] MediatR `IRequestHandler` used for commands/queries
- [ ] EventStoreDB client configured with connection string
- [ ] Inline projection service updates read DB in command handler
- [ ] No direct aggregate state mutations (events only)
- [ ] One command handler = one aggregate = one stream
