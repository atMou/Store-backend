# ?? Functional Programming with LanguageExt

## Table of Contents
- [Introduction](#introduction)
- [Why Functional Programming?](#why-functional-programming)
- [Core Concepts](#core-concepts)
- [LanguageExt Monads](#languageext-monads)
- [Practical Examples](#practical-examples)
- [Best Practices](#best-practices)

## Introduction

This project embraces **Functional Programming** principles using [LanguageExt](https://github.com/louthy/language-ext), bringing powerful functional programming concepts from languages like Haskell and F# to C#.

### Functional Programming Pillars

1. **Immutability** - Data cannot be modified after creation
2. **Pure Functions** - Same input always produces same output, no side effects
3. **First-Class Functions** - Functions as values
4. **Higher-Order Functions** - Functions that take/return functions
5. **Function Composition** - Building complex functions from simple ones
6. **Declarative Style** - Describe what to do, not how

## Why Functional Programming?

### Traditional Imperative Approach

```csharp
public Order CreateOrder(Guid userId, Guid cartId)
{
    // Lots of null checks
    var user = userRepository.Find(userId);
    if (user == null)
        throw new NotFoundException("User not found");

    var cart = cartRepository.Find(cartId);
    if (cart == null)
        throw new NotFoundException("Cart not found");

    // Validation
    if (cart.Items.Count == 0)
        throw new ValidationException("Cart is empty");

    if (cart.Total < 0)
        throw new ValidationException("Invalid cart total");

    // State mutation
    var order = new Order
    {
        Id = Guid.NewGuid(),
        UserId = userId,
        Status = OrderStatus.Pending
    };

    // More mutations
    foreach (var item in cart.Items)
    {
        order.Items.Add(new OrderItem
        {
            ProductId = item.ProductId,
            Quantity = item.Quantity
        });
    }

    // Side effects
    orderRepository.Add(order);
    cart.IsActive = false;
    
    return order;
}
```

**Problems:**
- ? Exceptions for control flow
- ? Hidden state mutations
- ? Difficult to test
- ? Hard to reason about
- ? Null reference risks

### Functional Approach

```csharp
public async Task<Fin<Order>> CreateOrder(Guid userId, Guid cartId)
{
    return await (
        from user in GetUser(userId)              // Explicit error handling
        from cart in GetCart(cartId)              // Railway-oriented
        from validCart in ValidateCart(cart)      // Composable validations
        from order in Order.Create(user.Id, validCart)  // Pure factory
        from _ in SaveOrder(order)                // Explicit side effects
        from __ in DeactivateCart(cart)          
        select order
    ).RunAsync();
}
```

**Benefits:**
- ? Explicit error handling in return type
- ? No exceptions for business logic
- ? Immutable data structures
- ? Easy to test (pure functions)
- ? Easy to reason about (declarative)
- ? Composable operations

## Core Concepts

### 1. Immutability

All domain models are defined as immutable records:

```csharp
// ? Mutable class
public class Order
{
    public Guid Id { get; set; }
    public OrderStatus Status { get; set; }
    public List<OrderItem> Items { get; set; }
}

// ? Immutable record
public record Order
{
    private Order(OrderId id) => Id = id;

    public OrderId Id { get; }
    public OrderStatus Status { get; init; }
    public ICollection<OrderItem> Items { get; init; } = [];

    // Updates create new instances
    public Order MarkAsShipped(DateTime shippedAt) =>
        this with 
        { 
            Status = OrderStatus.Shipped,
            ShippedAt = shippedAt
        };
}
```

### 2. Pure Functions

Functions with no side effects:

```csharp
// ? Impure - has side effects
public decimal CalculateTotal(Order order)
{
    logger.LogInformation("Calculating total");  // Side effect!
    var total = order.Items.Sum(i => i.Total);
    order.CalculatedAt = DateTime.Now;           // Mutation!
    return total;
}

// ? Pure - no side effects
public Money CalculateTotal(IEnumerable<OrderItem> items) =>
    items.Aggregate(Money.Zero, (acc, item) => acc + item.LineTotal);
```

### 3. Function Composition

Build complex operations from simple ones:

```csharp
// Simple pure functions
Func<string, Fin<Email>> parseEmail = 
    email => Email.From(email);

Func<Email, Fin<User>> findUser = 
    email => userRepo.FindByEmail(email)
        .ToFin(NotFoundError.New("User not found"));

Func<User, bool> isActive = 
    user => user.IsActive;

Func<User, Fin<Unit>> sendEmail = 
    user => emailService.SendWelcome(user);

// Compose into complex operation
var result = await (
    from email in parseEmail(emailString)
    from user in findUser(email)
    from _ in isActive(user) 
        ? FinSucc(unit) 
        : FinFail<Unit>(ValidationError.New("User is inactive"))
    from __ in sendEmail(user)
    select user
).RunAsync();
```

## LanguageExt Monads

### The `Fin<T>` Monad (Finite/Either)

Represents a computation that can **succeed** with a value of type `T` or **fail** with an `Error`.

```csharp
// Type signature shows it can fail
public Fin<Order> CreateOrder(CreateOrderRequest request)
{
    return from userId in UserId.From(request.UserId)      // Can fail
           from cartId in CartId.From(request.CartId)       // Can fail
           from order in Order.Create(userId, cartId)        // Can fail
           select order;
}

// Pattern matching on result
var result = CreateOrder(request);
result.Match(
    Succ: order => {
        logger.LogInformation("Order created: {OrderId}", order.Id);
        return Ok(order);
    },
    Fail: error => {
        logger.LogError("Failed to create order: {Error}", error);
        return BadRequest(error);
    }
);
```

#### Creating `Fin<T>` Values

```csharp
// Success
Fin<int> success = FinSucc(42);
Fin<Order> order = FinSucc(new Order());

// Failure
Fin<int> failure = FinFail<int>(Error.New("Something went wrong"));
Fin<Order> notFound = FinFail<Order>(NotFoundError.New("Order not found"));

// From validation
public static Fin<Money> From(decimal value) =>
    value >= 0
        ? FinSucc(new Money(value))
        : FinFail<Money>(ValidationError.New("Amount cannot be negative"));
```

#### Chaining Operations

```csharp
// Sequential operations
var result = 
    from order in GetOrder(orderId)               // Fin<Order>
    from payment in CreatePayment(order)           // Fin<Payment>
    from _ in UpdateOrderStatus(order, payment)    // Fin<Unit>
    select payment;

// If any step fails, entire chain short-circuits with the error
```

#### Transforming Values

```csharp
// Map - transform success value
Fin<int> number = FinSucc(5);
Fin<int> doubled = number.Map(n => n * 2);  // FinSucc(10)

// Bind - chain operations that return Fin
Fin<User> user = GetUser(userId);
Fin<Order> order = user.Bind(u => GetUserOrders(u.Id));

// Match - handle both cases
var message = order.Match(
    Succ: o => $"Order {o.Id}",
    Fail: e => $"Error: {e.Message}"
);
```

### The `Option<T>` Monad

Represents an optional value - either **Some** value or **None**.

```csharp
// Instead of nullable types
User? nullable = users.FirstOrDefault(u => u.Id == userId);
if (nullable != null)
{
    // Use nullable
}

// Use Option<T>
Option<User> maybeUser = Optional(users.FirstOrDefault(u => u.Id == userId));
maybeUser.Match(
    Some: user => SendEmail(user),
    None: () => LogWarning("User not found")
);
```

#### Creating `Option<T>` Values

```csharp
// Some
Option<int> some = Some(42);
Option<string> someString = Some("Hello");

// None
Option<int> none = None;

// From nullable
User? nullable = GetUser();
Option<User> option = Optional(nullable);

// Conditional
Option<User> admin = user.IsAdmin 
    ? Some(user) 
    : None;
```

#### Operations

```csharp
// Map
Option<int> number = Some(5);
Option<int> doubled = number.Map(n => n * 2);  // Some(10)

// Bind
Option<User> user = Some(new User { Id = 1 });
Option<Order> order = user.Bind(u => GetUserActiveOrder(u.Id));

// Filter
Option<User> activeUser = user.Filter(u => u.IsActive);

// IfNone - provide default
User user = maybeUser.IfNone(() => new GuestUser());

// IfNoneUnsafe - throw exception
User user = maybeUser.IfNoneUnsafe();  // ?? Use sparingly!

// Convert to Fin
Fin<User> finUser = maybeUser.ToFin(
    NotFoundError.New("User not found")
);
```

### The `IO<T>` Monad

Represents a side-effecting computation that will be executed later.

```csharp
// Define side effect
IO<string> readFile = IO.liftAsync(async e => 
    await File.ReadAllTextAsync("config.json", e.Token));

IO<Unit> writeLog = IO.lift(() => 
    logger.LogInformation("Processing order"));

// Compose IO operations
IO<string> processConfig = 
    from content in readFile
    from _ in writeLog
    from config in IO.lift(() => ParseConfig(content))
    select config;

// Execute when needed
var result = await processConfig.RunSafeAsync(EnvIO.New(null, cancellationToken));
```

#### Email Example with IO

```csharp
public K<M, Response> SendOrderEmail<M>(Order order, User user)
    where M : Fallible<M>, MonadIO<M>
{
    return from template in M.LiftIO(
                IO.liftAsync(e => LoadTemplate("order-confirmation.html", e.Token)))
           from emailData in M.Pure(BuildEmailData(order, user))
           from html in M.Pure(RenderTemplate(template, emailData))
           from response in M.LiftIO(
                IO.liftAsync(e => emailClient.SendAsync(html, user.Email, e.Token)))
           select response;
}

// Usage
var io = SendOrderEmail<IO>(order, user);
var result = await io.RunSafeAsync(EnvIO.New(null, ct));
```

### The `Try<T>` Monad

Catches exceptions and converts them to values.

```csharp
// Wrap dangerous code
Try<int> Parse(string s) => Try(() => int.Parse(s));

var result = Parse("42");
result.Match(
    Succ: value => Console.WriteLine($"Parsed: {value}"),
    Fail: ex => Console.WriteLine($"Error: {ex.Message}")
);

// Async operations
Try.lift(async () =>
{
    await SendNotification(userId, message);
    logger.LogInformation("Notification sent");
}).Run().IfFail(ex =>
{
    logger.LogError(ex, "Failed to send notification");
});
```

### The `Validation<Fail, Success>` Monad

Accumulates multiple validation errors.

```csharp
public Validation<Error, CreateUserRequest> ValidateCreateUser(
    string email, 
    string password, 
    int age)
{
    return (ValidateEmail(email), 
            ValidatePassword(password), 
            ValidateAge(age))
        .Apply((e, p, a) => new CreateUserRequest(e, p, a));
}

// Validation functions
Validation<Error, Email> ValidateEmail(string email) =>
    Email.From(email).ToValidation();

Validation<Error, Password> ValidatePassword(string pwd) =>
    pwd.Length >= 8
        ? Success<Error, Password>(Password.From(pwd))
        : Fail<Error, Password>(ValidationError.New("Password too short"));

Validation<Error, int> ValidateAge(int age) =>
    age >= 18
        ? Success<Error, int>(age)
        : Fail<Error, int>(ValidationError.New("Must be 18 or older"));

// Usage
var validation = ValidateCreateUser("test@test.com", "pass123", 17);
validation.Match(
    Succ: request => CreateUser(request),
    Fail: errors => errors.ToList().ForEach(e => logger.LogError(e.Message))
);
```

## Practical Examples

### Example 1: Order Creation with Full Validation

```csharp
public async Task<Fin<OrderResult>> CreateOrder(CreateOrderCommand command)
{
    var db = from cart in GetEntity<BasketDbContext, Cart>(
                c => c.Id == command.CartId && c.UserId == command.UserId,
                NotFoundError.New("Cart not found"),
                opt => opt
                    .AddInclude(c => c.LineItems)
                    .AddInclude(c => c.CouponIds))
             
             // Validate cart
             from _ in cart.LineItems.Any()
                ? FinSucc(unit)
                : FinFail<Unit>(ValidationError.New("Cart is empty"))
             
             from __ in !cart.IsCheckedOut
                ? FinSucc(unit)
                : FinFail<Unit>(InvalidOperationError.New("Cart already checked out"))
             
             // Create order items
             let orderItems = cart.LineItems.Select(li => 
                OrderItem.Create(
                    li.ProductId,
                    li.ColorVariantId,
                    li.SizeVariantId,
                    li.Quantity,
                    li.UnitPrice
                )).Sequence()  // Sequence converts IEnumerable<Fin<T>> to Fin<IEnumerable<T>>
             
             from items in orderItems
             
             // Create order
             from order in Order.Create(
                command.UserId,
                command.CartId,
                items,
                cart.DeliveryAddress,
                cart.Tax)
             
             // Save order
             from savedOrder in AddEntity<OrderDBContext, Order>(order)
             
             // Mark cart as checked out
             from ___ in UpdateEntity<BasketDbContext, Cart>(
                cart,
                c => c.SetCartCheckedOut())
             
             select savedOrder;

    return await db.RunSaveAsync(orderDbContext, EnvIO.New(null, command.CancellationToken))
        .Map(order => order.ToResult());
}
```

### Example 2: Payment Processing

```csharp
public async Task<Fin<PaymentResult>> ProcessPayment(ProcessPaymentCommand command)
{
    return await (
        from order in GetOrder(command.OrderId)
        
        from _ in order.Status == OrderStatus.Pending
            ? FinSucc(unit)
            : FinFail<Unit>(InvalidOperationError.New($"Cannot process payment for order in {order.Status} status"))
        
        from paymentIntent in CreateStripePaymentIntent(order)
        
        from payment in Payment.Create(
            order.Id,
            order.UserId,
            order.Total,
            paymentIntent.Id)
        
        from savedPayment in SavePayment(payment)
        
        from __ in PublishPaymentCreatedEvent(savedPayment)
        
        select savedPayment.ToResult()
    ).RunAsync();
}

private async Task<Fin<PaymentIntent>> CreateStripePaymentIntent(Order order)
{
    Try<PaymentIntent> tryCreate = TryAsync(async () =>
    {
        var options = new PaymentIntentCreateOptions
        {
            Amount = (long)(order.Total.Value * 100),
            Currency = "usd",
            Metadata = new Dictionary<string, string>
            {
                ["order_id"] = order.Id.Value.ToString(),
                ["user_id"] = order.UserId.Value.ToString()
            }
        };
        
        var service = new PaymentIntentService();
        return await service.CreateAsync(options);
    });

    var result = await tryCreate.Run();
    
    return result.Match(
        Succ: intent => FinSucc(intent),
        Fail: ex => FinFail<PaymentIntent>(
            InternalServerError.New($"Stripe error: {ex.Message}"))
    );
}
```

### Example 3: Notification System

```csharp
public Task SendShipmentNotification(Guid userId, ShipmentCreatedIntegrationEvent shipment)
{
    // Using Try monad to handle async exceptions
    Try.lift(async () =>
    {
        // Get user for notification
        var userResult = await GetUser(userId).RunAsync();
        
        await userResult.Match(
            async user =>
            {
                // Build notification
                var notification = new ShipmentStatusNotification
                {
                    ShipmentId = shipment.ShipmentId,
                    OrderId = shipment.OrderId,
                    Status = "Shipped",
                    TrackingCode = shipment.TrackingCode,
                    Message = $"Your order has been shipped! Tracking: {shipment.TrackingCode}",
                    UpdatedAt = DateTime.UtcNow
                };

                // Send via SignalR
                await notificationService.NotifyShipmentStatusChanged(userId, notification);
                
                logger.LogInformation(
                    "Shipment notification sent to user {UserId}", userId);
            },
            error => Task.Run(() => 
                logger.LogError("Failed to send notification: {Error}", error))
        );
    }).Run().IfFail(ex =>
    {
        logger.LogError(ex, "Exception sending shipment notification");
    });

    return Task.CompletedTask;
}
```

### Example 4: Cart Operations with Shipping Cost

```csharp
public record Cart : Aggregate<CartId>
{
    private const decimal FreeShippingThreshold = 29.50m;
    private const decimal StandardShippingCost = 4.50m;

    // Pure function to add line items
    public Cart AddLineItems(params LineItem[] lineItems)
    {
        var updated = this with
        {
            LineItems = [.. LineItems, .. lineItems]
        };
        
        return updated
            .RecalculateSubtotal()
            .UpdateShippingCost();
    }

    // Pure function to recalculate
    private Cart RecalculateSubtotal()
    {
        var subtotal = GetSubTotal(LineItems);
        return this with { TotalSub = subtotal };
    }

    // Pure function for shipping logic
    private Cart UpdateShippingCost()
    {
        var cost = TotalAfterDiscounted.Value >= FreeShippingThreshold
            ? Money.Zero
            : Money.FromDecimal(StandardShippingCost);
        
        return this with { ShipmentCost = cost };
    }

    // Pure function to calculate subtotal
    private static Money GetSubTotal(IEnumerable<LineItem> lineItems) =>
        lineItems.Aggregate(Money.Zero, (acc, li) => acc + li.LineTotal);
}
```

## Best Practices

### 1. Design with Types

```csharp
// ? Primitive obsession
public Order CreateOrder(Guid userId, Guid cartId, decimal total)
{
    // Easy to mix up GUIDs
    // No validation on decimal
}

// ? Strong types
public Fin<Order> CreateOrder(UserId userId, CartId cartId, Money total)
{
    // Type system prevents errors
    // Validation built into value objects
}
```

### 2. Make Errors Explicit

```csharp
// ? Hidden errors
public Order GetOrder(Guid id)
{
    // Can throw exception
    // Can return null
    // No indication in signature
}

// ? Explicit errors
public Fin<Order> GetOrder(OrderId id)
{
    // Signature shows it can fail
    // Caller must handle error case
    // No hidden exceptions
}
```

### 3. Prefer Composition Over Inheritance

```csharp
// ? Inheritance
public class Order : Entity
{
    public virtual void Process() { }
}

public class PremiumOrder : Order
{
    public override void Process() { }
}

// ? Composition
public record Order
{
    public OrderType Type { get; }
    
    public Fin<Order> Process() =>
        Type switch
        {
            StandardOrderType => ProcessStandard(),
            PremiumOrderType => ProcessPremium(),
            _ => FinFail<Order>(InvalidOperationError.New("Unknown type"))
        };
}
```

### 4. Keep Functions Small and Focused

```csharp
// ? Large function doing everything
public async Task<Order> CreateAndProcessOrder(CreateOrderRequest request)
{
    // Validate request
    // Get cart
    // Create order
    // Process payment
    // Send email
    // Update inventory
    // ... 100 lines of code
}

// ? Small, focused functions
public async Task<Fin<Order>> CreateOrder(CreateOrderRequest request) =>
    await (
        from validRequest in ValidateRequest(request)
        from cart in GetCart(validRequest.CartId)
        from order in BuildOrder(cart, validRequest)
        from saved in SaveOrder(order)
        select saved
    ).RunAsync();

public async Task<Fin<Unit>> ProcessOrder(Order order) =>
    await (
        from payment in ProcessPayment(order)
        from _ in SendConfirmationEmail(order)
        from __ in UpdateInventory(order)
        select unit
    ).RunAsync();
```

### 5. Use LINQ for Readability

```csharp
// ? Imperative
public Fin<Order> CreateOrder(Cart cart)
{
    if (!cart.Items.Any())
        return FinFail<Order>(ValidationError.New("Cart is empty"));
    
    if (cart.Total < 0)
        return FinFail<Order>(ValidationError.New("Invalid total"));
    
    var order = Order.Create(cart);
    if (order.IsSuccess)
        return order;
    else
        return FinFail<Order>(order.Error);
}

// ? Declarative with LINQ
public Fin<Order> CreateOrder(Cart cart) =>
    from _ in cart.Items.Any()
        ? FinSucc(unit)
        : FinFail<Unit>(ValidationError.New("Cart is empty"))
    from __ in cart.Total > Money.Zero
        ? FinSucc(unit)
        : FinFail<Unit>(ValidationError.New("Invalid total"))
    from order in Order.Create(cart)
    select order;
```

## Conclusion

Functional programming with LanguageExt provides:

- ? **Fewer Bugs** - Immutability and explicit errors
- ? **Better Testability** - Pure functions are easy to test
- ? **More Maintainable** - Declarative code is easier to understand
- ? **Type Safety** - Leverage the type system for correctness
- ? **Composability** - Build complex operations from simple ones

The functional approach makes code more **predictable**, **reliable**, and **maintainable** while keeping the benefits of C# and .NET.
