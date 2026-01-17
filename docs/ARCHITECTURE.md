# ??? Architecture Deep Dive

## Table of Contents
- [Overview](#overview)
- [Modular Monolith Architecture](#modular-monolith-architecture)
- [Domain-Driven Design](#domain-driven-design)
- [Functional Programming Approach](#functional-programming-approach)
- [Event-Driven Architecture](#event-driven-architecture)
- [Database Design](#database-design)

## Overview

This project implements a **Modular Monolith** architecture with **Domain-Driven Design (DDD)** principles and **Functional Programming** paradigms using LanguageExt.

### Key Architectural Principles

1. **Separation of Concerns** - Each module is independent with clear boundaries
2. **Functional Core, Imperative Shell** - Pure functions at the core, side effects at the edges
3. **Immutability** - Domain models are immutable records
4. **Explicit Error Handling** - Using `Fin<T>` instead of exceptions
5. **Event-Driven Communication** - Modules communicate via events

## Modular Monolith Architecture

### Why Modular Monolith?

- ? **Easier deployment** than microservices
- ? **Lower operational complexity**
- ? **Better performance** (no network overhead)
- ? **Easier to refactor** into microservices later
- ? **Strong consistency** within bounded contexts

### Module Structure

Each module follows a consistent structure:

```
Module/
??? Domain/
?   ??? Models/           # Aggregates & Entities
?   ??? ValueObjects/     # Domain value objects
?   ??? Events/           # Domain events
?   ??? Enums/            # Domain enumerations
??? Application/
?   ??? Features/         # CQRS Commands & Queries
?   ?   ??? CreateX/
?   ?   ?   ??? CreateXCommand.cs
?   ?   ?   ??? CreateXHandler.cs
?   ?   ??? GetX/
?   ?       ??? GetXQuery.cs
?   ?       ??? GetXHandler.cs
?   ??? EventHandlers/    # Integration event consumers
?   ??? Contracts/        # DTOs and extensions
??? Infrastructure/
?   ??? (Module-specific infrastructure)
??? Persistence/
?   ??? EntityConfigurations/
?   ??? Migrations/
?   ??? ModuleDbContext.cs
??? Presentation/
    ??? Controllers/      # API endpoints
```

## Domain-Driven Design

### Tactical Patterns Implementation

#### 1. Aggregates

Aggregates are the consistency boundaries in our domain. They ensure invariants are maintained.

```csharp
public record Order : Aggregate<OrderId>
{
    // Private constructor ensures creation through factory methods
    private Order(UserId userId, CartId cartId) : base(OrderId.New)
    {
        UserId = userId;
        CartId = cartId;
        Status = OrderStatus.Pending;
        AddDomainEvent(new OrderCreatedDomainEvent(Id, UserId));
    }

    public UserId UserId { get; }
    public OrderStatus Status { get; private set; }
    public ICollection<OrderItem> OrderItems { get; private set; } = [];

    // Factory method with validation
    public static Fin<Order> Create(UserId userId, CartId cartId, IEnumerable<OrderItem> items)
    {
        return items.Any()
            ? FinSucc(new Order(userId, cartId) { OrderItems = items.ToList() })
            : FinFail<Order>(ValidationError.New("Order must have at least one item"));
    }

    // State transition with business rules
    public Fin<Order> MarkAsPaid(PaymentId paymentId, DateTime paidAt)
    {
        return Status.CanTransitionTo(OrderStatus.Paid)
            .Map(_ => {
                var updated = this with { 
                    Status = OrderStatus.Paid,
                    PaymentId = paymentId,
                    PaidAt = paidAt
                };
                updated.AddDomainEvent(new OrderPaidDomainEvent(Id, paymentId));
                return updated;
            });
    }
}
```

**Key Points:**
- Immutable records with `with` expressions
- Private setters to enforce encapsulation
- Domain events for state changes
- Validation in factory methods
- State transitions return `Fin<T>` for error handling

#### 2. Entities

Entities have identity and lifecycle, but are not aggregates.

```csharp
public record OrderItem : Entity<OrderItemId>
{
    private OrderItem(ProductId productId, int quantity, Money unitPrice) 
        : base(OrderItemId.New)
    {
        ProductId = productId;
        Quantity = quantity;
        UnitPrice = unitPrice;
    }

    public ProductId ProductId { get; }
    public int Quantity { get; private set; }
    public Money UnitPrice { get; }
    public Money LineTotal => UnitPrice * Quantity;

    public static Fin<OrderItem> Create(ProductId productId, int quantity, Money unitPrice)
    {
        return quantity > 0
            ? FinSucc(new OrderItem(productId, quantity, unitPrice))
            : FinFail<OrderItem>(ValidationError.New("Quantity must be positive"));
    }
}
```

#### 3. Value Objects

Value objects are immutable and equality is based on their values, not identity.

```csharp
public record Money
{
    private Money(decimal value) => Value = value;

    public decimal Value { get; }

    public static Money Zero => new(0);
    
    public static Money FromDecimal(decimal value) =>
        value >= 0 
            ? new Money(value)
            : throw new ArgumentException("Money cannot be negative");

    // Operators for domain operations
    public static Money operator +(Money a, Money b) => 
        FromDecimal(a.Value + b.Value);
    
    public static Money operator -(Money a, Money b) => 
        FromDecimal(a.Value - b.Value);
    
    public static Money operator *(Money money, int multiplier) => 
        FromDecimal(money.Value * multiplier);

    public static bool operator >(Money a, Money b) => a.Value > b.Value;
    public static bool operator <(Money a, Money b) => a.Value < b.Value;
}

public record Email
{
    private Email(string value) => Value = value;

    public string Value { get; }

    private static readonly Regex EmailRegex = new(
        @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
        RegexOptions.Compiled);

    public static Fin<Email> From(string value) =>
        string.IsNullOrWhiteSpace(value)
            ? FinFail<Email>(ValidationError.New("Email cannot be empty"))
            : EmailRegex.IsMatch(value)
                ? FinSucc(new Email(value))
                : FinFail<Email>(ValidationError.New("Invalid email format"));
}
```

**Benefits:**
- Type safety (can't mix Money with decimal)
- Domain operations are explicit
- Validation at construction
- Immutability guaranteed

#### 4. Domain Events

```csharp
// Domain Event (internal to module)
public record OrderCreatedDomainEvent(
    OrderId OrderId,
    UserId UserId,
    Money Total,
    DateTime CreatedAt) : IDomainEvent;

// Integration Event (cross-module communication)
public record OrderCreatedIntegrationEvent : IntegrationEvent
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public OrderDto OrderDto { get; init; }
}
```

**Event Flow:**
1. Domain event raised within aggregate
2. Event interceptor captures domain events
3. Domain event handler processes (same transaction)
4. Integration event published to message bus
5. Other modules consume integration events

#### 5. Repositories

Repositories use the **Db Monad** pattern for functional database access:

```csharp
// Functional repository operations
var db = from order in GetEntity<OrderDBContext, Order>(
            o => o.Id == orderId,
            NotFoundError.New($"Order not found"),
            opt => opt
                .AddInclude(o => o.OrderItems)
                .AddInclude(o => o.Payment))
         from updated in UpdateEntity<OrderDBContext, Order>(
            order,
            o => o.MarkAsShipped(shipmentId, DateTime.UtcNow))
         select updated;

// Execute with automatic transaction and save
var result = await db.RunSaveAsync(
    dbContext, 
    EnvIO.New(null, cancellationToken));

result.Match(
    order => logger.LogInformation("Order updated: {OrderId}", order.Id),
    error => logger.LogError("Update failed: {Error}", error));
```

### Strategic Patterns

#### Bounded Contexts

Each module represents a bounded context:

| Context | Language | Key Concepts |
|---------|----------|-------------|
| **Identity** | User, authentication, authorization | User, Role, Permission, Token |
| **Product** | Catalog, inventory items | Product, Variant, Brand, Material |
| **Basket** | Shopping, cart | Cart, LineItem, Coupon, Discount |
| **Order** | Purchase, fulfillment | Order, OrderItem, OrderStatus |
| **Payment** | Transaction, billing | Payment, PaymentIntent, Refund |
| **Inventory** | Stock, warehouse | Inventory, Stock, Warehouse |
| **Shipment** | Delivery, logistics | Shipment, Tracking, Carrier |

#### Context Mapping

Modules integrate using:
- **Integration Events** (asynchronous)
- **Shared Kernel** (common value objects)
- **Published Language** (shared DTOs)

```csharp
// Payment module publishes event
await publisher.Publish(new PaymentFulfilledIntegrationEvent
{
    PaymentId = payment.Id.Value,
    OrderId = payment.OrderId.Value,
    UserId = payment.UserId.Value
});

// Multiple modules can consume
public class PaymentFulfilledHandler : IConsumer<PaymentFulfilledIntegrationEvent>
{
    // Order module: Mark order as paid
    // Shipment module: Create shipment
    // Identity module: Update user state
    // Basket module: Deactivate cart
}
```

## Functional Programming Approach

### Core Principles

1. **Immutability** - All domain models are immutable records
2. **Pure Functions** - Business logic has no side effects
3. **Explicit Error Handling** - No exceptions for business logic
4. **Monadic Composition** - Chain operations with LINQ
5. **Type Safety** - Leverage the type system

### LanguageExt Integration

#### The `Fin<T>` Monad

`Fin<T>` (Finite) represents a computation that can succeed with a value or fail with an error.

```csharp
// Traditional approach with exceptions
public Order CreateOrder(Guid userId)
{
    var user = userRepo.Find(userId);
    if (user == null) throw new NotFoundException("User not found");
    
    var cart = cartRepo.GetActive(userId);
    if (cart == null) throw new NotFoundException("Cart not found");
    if (!cart.HasItems) throw new ValidationException("Cart is empty");
    
    var order = Order.Create(user, cart);
    orderRepo.Add(order);
    return order;
}

// Functional approach with Fin<T>
public async Task<Fin<Order>> CreateOrder(Guid userId)
{
    return await (
        from user in GetUser(userId)
        from cart in GetActiveCart(userId)
        from _ in cart.HasItems 
            ? FinSucc(unit)
            : FinFail<Unit>(ValidationError.New("Cart is empty"))
        from order in Order.Create(user.Id, cart.Id, cart.Items)
        from saved in SaveOrder(order)
        select saved
    ).RunAsync();
}

// Usage
var result = await CreateOrder(userId);
result.Match(
    order => Ok(order),           // Success path
    error => BadRequest(error)    // Error path
);
```

**Benefits:**
- ? Explicit error handling in type signature
- ? Errors are values, not exceptions
- ? Forces caller to handle errors
- ? Railway-oriented programming

#### The `Option<T>` Monad

Replaces nullable types with explicit presence/absence.

```csharp
// Traditional nullable
public User? FindUserByEmail(string email)
{
    return users.FirstOrDefault(u => u.Email == email);
}

// Functional Option
public Option<User> FindUserByEmail(string email)
{
    return Optional(users.FirstOrDefault(u => u.Email == email));
}

// Usage
var userOption = FindUserByEmail("test@test.com");
userOption.Match(
    Some: user => SendEmail(user),
    None: () => LogWarning("User not found")
);

// Or convert to Fin<T>
var result = userOption.ToFin(NotFoundError.New("User not found"));
```

#### The `IO<T>` Monad

Separates pure computations from side effects.

```csharp
// Email sending wrapped in IO
public K<M, Response> SendEmail<M>(EmailData data)
    where M : Fallible<M>, MonadIO<M>
{
    return from template in M.LiftIO(IO.liftAsync(e => 
                LoadTemplate("order-confirmation.html", e.Token)))
           from html in M.Pure(BuildHtml(template, data))
           from response in M.LiftIO(IO.liftAsync(e => 
                emailClient.SendAsync(html, e.Token)))
           select response;
}

// Usage
var io = SendEmail<IO>(emailData);
var result = await io.RunSafeAsync(EnvIO.New(null, cancellationToken));
```

#### The `Try<T>` Monad

Catches exceptions and converts them to values.

```csharp
Try.lift(async () =>
{
    await notificationService.NotifyShipmentStatus(userId, notification);
    logger.LogInformation("Notification sent");
}).Run().IfFail(ex => 
{
    logger.LogError(ex, "Failed to send notification");
});
```

### LINQ Query Expression Pattern

```csharp
public async Task<Fin<OrderResult>> ProcessOrder(ProcessOrderCommand cmd)
{
    var db = from cart in GetEntity<BasketDbContext, Cart>(
                c => c.Id == cmd.CartId,
                NotFoundError.New("Cart not found"),
                opt => opt.AddInclude(c => c.LineItems))
             from validCart in ValidateCart(cart)
             from order in CreateOrder(validCart, cmd.UserId)
             from payment in CreatePayment(order)
             from _ in UpdateEntity<BasketDbContext, Cart>(
                cart,
                c => c.SetCartCheckedOut())
             select (order, payment);

    return await db.RunSaveAsync(dbContext, EnvIO.New(null, ct))
        .Map(result => result.order.ToResult());
}
```

## Event-Driven Architecture

### Event Types

1. **Domain Events** - Internal module events
2. **Integration Events** - Cross-module events
3. **SignalR Events** - Real-time client notifications

### Event Flow

```
????????????????
?   Aggregate  ? ??? Domain Event
????????????????
       ?
       ?
????????????????
?  Interceptor ? ??? Captures events before SaveChanges
????????????????
       ?
       ?
????????????????
? Domain Event ? ??? Processed in same transaction
?   Handler    ?
????????????????
       ?
       ?
????????????????
? Integration  ? ??? Published to MassTransit
?    Event     ?
????????????????
       ?
       ?
????????????????
?  Consumers   ? ??? Other modules react
? (Other Mods) ?
????????????????
```

### MassTransit Configuration

```csharp
services.AddMassTransit(x =>
{
    // Register all consumers in assembly
    x.AddConsumers(Assembly.GetExecutingAssembly());

    x.UsingInMemory((context, cfg) =>
    {
        cfg.ConfigureEndpoints(context);
    });
});
```

## Database Design

### Per-Module Databases

Each module has its own database for:
- ? Strong module boundaries
- ? Independent scaling
- ? Technology flexibility
- ? Easier testing

### Entity Framework Configuration

```csharp
public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        
        builder.HasKey(o => o.Id);
        
        builder.Property(o => o.Id)
            .HasConversion(
                id => id.Value,
                value => OrderId.From(value));
        
        builder.Property(o => o.Total)
            .HasConversion(
                money => money.Value,
                value => Money.FromDecimal(value));
        
        builder.OwnsOne(o => o.ShippingAddress);
        
        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey("OrderId");
    }
}
```

### Interceptors

```csharp
public class DispatchDomainEventInterceptor : SaveChangesInterceptor
{
    private readonly IMediator _mediator;

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData,
        InterceptionResult<int> result,
        CancellationToken cancellationToken = default)
    {
        var context = eventData.Context;
        if (context == null) return result;

        var entities = context.ChangeTracker
            .Entries<IAggregate>()
            .Where(e => e.Entity.DomainEvents.Any())
            .Select(e => e.Entity)
            .ToList();

        var domainEvents = entities
            .SelectMany(e => e.DomainEvents)
            .ToList();

        entities.ForEach(e => e.ClearDomainEvents());

        foreach (var domainEvent in domainEvents)
        {
            await _mediator.Publish(domainEvent, cancellationToken);
        }

        return result;
    }
}
```

## Conclusion

This architecture provides:
- ? **Maintainability** - Clear separation of concerns
- ? **Testability** - Pure functions are easy to test
- ? **Scalability** - Modular design enables selective scaling
- ? **Reliability** - Explicit error handling reduces bugs
- ? **Flexibility** - Easy to add new features or refactor
