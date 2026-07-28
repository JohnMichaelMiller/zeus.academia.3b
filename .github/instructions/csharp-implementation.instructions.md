---
ai_generated: true
model: "anthropic/claude-sonnet-4.5@unknown"
operator: "johnmillerATcodemag-com"
chat_id: "2026-01-28-csharp-instructions-generation"
prompt: |
  #file:generate-csharp-instructions.prompt.md submit this prompt
started: "2026-01-28T19:30:00Z"
ended: "2026-01-28T19:50:00Z"
task_durations:
  - task: "analyze repository context"
    duration: "00:05:00"
  - task: "generate instruction content"
    duration: "00:12:00"
  - task: "validate and optimize"
    duration: "00:03:00"
total_duration: "00:20:00"
ai_log: "ai-logs/2026/01/28/2026-01-28-csharp-instructions-generation/conversation.md"
source: ".github/prompts/generate-csharp-instructions.prompt.md"
description: "Modern C# implementation standards and best practices"
applyTo: "**/*.cs"
---

# Modern C# Implementation Standards

Foundational C# best practices for clean, maintainable, type-safe code using modern language features (C# 12+).

## Context

**Scope:** General C# implementation (all .cs files)
**Patterns:** Clean Architecture, SOLID principles, modern C# idioms
**Frameworks:** .NET 8+, nullable reference types enabled
**Conventions:** Standard C# + token-optimized structure

## Naming Conventions

| Element            | Pattern              | Example                   |
| ------------------ | -------------------- | ------------------------- |
| Interface          | `I` + PascalCase     | `IRepository`, `IService` |
| Private field      | `_camelCase`         | `_logger`, `_context`     |
| Public/Protected   | PascalCase           | `GetOrder`, `UserId`      |
| Parameter/Local    | camelCase            | `orderId`, `result`       |
| Const              | PascalCase           | `MaxRetries`              |
| Async method       | PascalCase + `Async` | `GetOrderAsync`           |
| Record (immutable) | PascalCase           | `OrderId`, `Address`      |

## File Organization

**Rules:**

- MUST have one type per file (class/interface/record)
- MUST name file to match type name (`Order.cs`, `IOrderRepository.cs`)
- MUST use file-scoped namespaces (C# 10+)
- MUST organize members: fields → constructors → properties → methods
- MUST match namespace to folder structure

**Template:**

```csharp
namespace Zeus.Academia.Domain.Orders;

public sealed class Order
{
    // Fields
    private readonly List<OrderItem> _items = [];

    // Constructors
    public Order(Guid id, DateTime createdAt)
    {
        Id = id;
        CreatedAt = createdAt;
    }

    // Properties
    public Guid Id { get; }
    public DateTime CreatedAt { get; }
    public IReadOnlyList<OrderItem> Items => _items;

    // Methods
    public void AddItem(OrderItem item) => _items.Add(item);
}
```

## Nullable Reference Types

**Rules:**

- MUST enable nullable reference types (`<Nullable>enable</Nullable>` in .csproj)
- MUST use `?` suffix for nullable types: `string?`, `Order?`
- MUST NOT use `!` null-forgiving operator without validation
- MUST use null-conditional (`?.`) and null-coalescing (`??`) operators
- MUST validate parameters with `ArgumentNullException.ThrowIfNull(param)` (C# 12+)

**Examples:**

```csharp
// ✅ Correct
public Order? FindOrder(Guid id)
{
    var order = _repository.GetById(id);
    return order; // Explicit nullability
}

public void ProcessOrder(Order order)
{
    ArgumentNullException.ThrowIfNull(order);
    // Proceed with non-null order
}

// ❌ Avoid
public Order FindOrder(Guid id)
{
    return _repository.GetById(id)!; // Null-forgiving without validation
}
```

## String normalization and validation

**Rules:**

- Before calling `.Trim()`, `.ToUpperInvariant()`, `.ToLowerInvariant()`, or similar string methods, guard against `null`, empty, and whitespace input first.
- Prefer `string.IsNullOrWhiteSpace(value)` or `ArgumentNullException.ThrowIfNull(value)` plus a whitespace check when a caller-supplied string is required.
- For public or handler-facing APIs, throw `ArgumentException` for null/empty/whitespace input and keep the message specific; do not allow a `NullReferenceException` from a later `.Trim()` call.
- Example:

```csharp
public static string NormalizeCode(string? value)
{
    if (string.IsNullOrWhiteSpace(value))
        throw new ArgumentException("Degree code must not be empty.", nameof(value));

    return value.Trim().ToUpperInvariant();
}
```

## Type Selection

| Use Case                   | Type     | Rationale                             |
| -------------------------- | -------- | ------------------------------------- |
| Immutable data (DTOs, IDs) | `record` | Value equality, concise syntax        |
| Mutable entities           | `class`  | Identity equality, lifecycle          |
| Single-value wrapper       | `record` | Strong typing, no primitive obsession |
| Behavior + state           | `class`  | Methods + encapsulation               |
| Collection of values      | `record` | Structural equality                   |

**Examples:**

```csharp
// Record for value object
public record OrderId(Guid Value)
{
    public static OrderId New() => new(Guid.NewGuid());
}

// Record for DTO
public record CreateOrderRequest(Guid CustomerId, List<OrderItemDto> Items);

// Class for entity
public class Order
{
    public Guid Id { get; private set; }
    public OrderStatus Status { get; private set; }

    public void MarkAsShipped() => Status = OrderStatus.Shipped;
}
```

## Async/Await Patterns

**Rules:**

- MUST use `async`/`await` for I/O operations (database, HTTP, file system)
- MUST suffix async methods with `Async`
- MUST propagate `CancellationToken` through call chain
- MUST NOT use `.Result`, `.Wait()`, or `.GetAwaiter().GetResult()`
- SHOULD use `ConfigureAwait(false)` in libraries (not needed in ASP.NET Core)

**Template:**

```csharp
public async Task<Order?> GetOrderAsync(Guid orderId, CancellationToken ct = default)
{
    ArgumentNullException.ThrowIfNull(orderId);

    var order = await _repository.GetByIdAsync(orderId, ct);
    return order;
}

// ❌ Avoid blocking
public Order GetOrder(Guid orderId)
{
    return _repository.GetOrderAsync(orderId).Result; // Deadlock risk
}
```

## Error Handling

**Rules:**

- MUST use specific exception types (`ArgumentException`, `InvalidOperationException`, custom)
- MUST NOT throw generic `Exception`
- MUST validate at API boundaries (controllers, handlers)
- MUST log exceptions before rethrowing or wrapping
- SHOULD create domain-specific exceptions for business rule violations

**Template:**

```csharp
public class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
}

public void AddItem(OrderItem item)
{
    ArgumentNullException.ThrowIfNull(item);

    if (_status == OrderStatus.Shipped)
        throw new DomainException("Cannot modify shipped order");

    _items.Add(item);
}
```

## Expression-Bodied Members

**Use for:**

- Simple property getters: `public int Total => Items.Sum(i => i.Price);`
- Single-statement methods: `public void Clear() => _items.Clear();`
- Read-only properties: `public bool IsEmpty => _items.Count == 0;`

**Avoid for:**

- Multi-statement logic
- Complex conditionals
- Mutation with side effects

```csharp
// ✅ Good use
public decimal TotalPrice => Items.Sum(i => i.Price);
public string DisplayName => $"{FirstName} {LastName}";

// ❌ Avoid
public void Process() => _logger.LogInfo("Start"); ProcessInternal(); _logger.LogInfo("End");
```

## Modern C# Features (C# 12+)

**Required Members:**

```csharp
public class Order
{
    public required Guid Id { get; init; }
    public required string CustomerName { get; init; }
}

var order = new Order { Id = Guid.NewGuid(), CustomerName = "John" }; // Enforced
```

**Raw String Literals:**

```csharp
var json = """
    {
        "orderId": "123",
        "status": "shipped"
    }
    """;
```

**List Patterns:**

```csharp
if (items is [var first, .. var rest])
{
    // Deconstruct list
}
```

**File-Scoped Namespaces:**

```csharp
namespace Zeus.Academia.Orders; // No braces, reduced indentation

public class Order { }
```

## Dependency Injection

**Rules:**

- MUST use constructor injection for required dependencies
- MUST NOT use service locator pattern
- MUST register with appropriate lifetime (Singleton, Scoped, Transient)
- SHOULD use `ILogger<T>` for logging
- SHOULD use `IOptions<T>` for configuration

**Template:**

```csharp
public class OrderService
{
    private readonly IOrderRepository _repository;
    private readonly ILogger<OrderService> _logger;

    public OrderService(
        IOrderRepository repository,
        ILogger<OrderService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<Order> CreateOrderAsync(CreateOrderRequest request, CancellationToken ct)
    {
        _logger.LogInformation("Creating order for customer {CustomerId}", request.CustomerId);
        // Implementation
    }
}
```

## LINQ Best Practices

**Rules:**

- PREFER method syntax over query syntax (more composable)
- MUST NOT use `.ToList()` unnecessarily (deferred execution)
- SHOULD use `Any()` instead of `Count() > 0`
- SHOULD use `FirstOrDefault()` over `Where().First()`
- MUST avoid multiple enumeration of IEnumerable

**Examples:**

```csharp
// ✅ Efficient
var activeOrders = orders.Where(o => o.Status == OrderStatus.Active);
var hasOrders = orders.Any(o => o.CustomerId == customerId);
var firstOrder = orders.FirstOrDefault(o => o.Id == orderId);

// ❌ Inefficient
var count = orders.Where(o => o.Status == OrderStatus.Active).Count(); // Use Count(predicate)
var exists = orders.Count() > 0; // Use Any()
```

## XML Documentation

**Rules:**

- MUST document public APIs (classes, methods, properties)
- MUST include `<summary>` minimum
- SHOULD include `<param>` and `<returns>` for non-obvious methods
- SHOULD explain "why" not "what" in comments

**Template:**

```csharp
/// <summary>
/// Represents an order in the system with items and total calculation.
/// </summary>
public class Order
{
    /// <summary>
    /// Adds an item to the order. Recalculates total.
    /// </summary>
    /// <param name="item">The item to add. Must not be null.</param>
    /// <exception cref="ArgumentNullException">Thrown when item is null.</exception>
    /// <exception cref="DomainException">Thrown when order is already shipped.</exception>
    public void AddItem(OrderItem item)
    {
        ArgumentNullException.ThrowIfNull(item);
        // Implementation
    }
}
```

## Anti-Patterns

| ❌ Anti-Pattern              | ✅ Correct Approach              |
| ---------------------------- | -------------------------------- |
| Public setters on entities   | Private/init-only setters        |
| Primitive obsession          | Value objects (records)          |
| `.Result` or `.Wait()`       | `await` with async               |
| Catch generic `Exception`    | Specific exception types         |
| `var` for non-obvious types  | Explicit type declaration        |
| Mutable static state         | Dependency injection             |
| Service locator              | Constructor injection            |
| String concatenation in loop | `StringBuilder` or interpolation |

## Validation Checklist

Before committing C# code:

- [ ] Nullable reference types enabled and warnings resolved
- [ ] No compiler warnings (treat warnings as errors in CI)
- [ ] File-scoped namespaces used (C# 10+)
- [ ] Async methods suffixed with `Async` and propagate `CancellationToken`
- [ ] XML documentation on public APIs
- [ ] Specific exception types (no generic `Exception`)
- [ ] Constructor injection for dependencies
- [ ] One type per file, file name matches type
- [ ] Private fields use `_camelCase`, public use `PascalCase`
- [ ] No `.Result`, `.Wait()`, or blocking async calls
- [ ] Expression-bodied members used for simple cases
- [ ] Records used for immutable data, classes for entities

## Integration

This instruction file provides general C# standards. For specialized patterns:

- **CQRS + Event Sourcing:** [cqrs-es-csharp-mediatr.instructions.md](cqrs-es-csharp-mediatr.instructions.md)
- Additional pattern-specific instructions can be added to `.github/instructions/`

---

_Apply these standards consistently across all C# code. Specialized patterns build upon these foundations._
