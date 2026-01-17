# ?? Contributing to Store Backend

We love your input! We want to making to Store Backend as easy and transparent as possible, whether it's:

- Reporting a bug
- Discussing the current state of the code
- Submitting a fix
- Proposing new features
- Becoming a maintainer

## Development Process

We use GitHub to host code, track issues and feature requests, as well as accept pull requests.

## CI/CD Requirements

### Automated Checks

All pull requests must pass the following automated checks:

? **Code Formatting** - `dotnet format` verification  
? **Code Analysis** - Static analysis and linting  
? **Build** - Successful compilation  
? **Tests** - All unit tests pass  
? **Code Coverage** - Coverage reports generated  

### Setting Up CI/CD Locally

**First time setup:**

Windows:
```powershell
.\.github\scripts\setup-dev-environment.ps1
```

Linux/Mac:
```bash
./.github/scripts/setup-dev-environment.sh
```

This will:
- Configure pre-commit hooks
- Restore NuGet packages
- Verify formatting
- Set up your development environment

### Pre-Commit Hooks

Pre-commit hooks automatically run before each commit to ensure code quality:

1. **Format Check** - Verifies code formatting
2. **Build** - Ensures code compiles
3. **Tests** - Runs unit tests

If any check fails, the commit is blocked.

**Bypass (not recommended):**
```bash
git commit --no-verify -m "message"
```

### Before Creating a Pull Request

Run these commands locally:

```bash
# Format code
dotnet format "Store Backend.sln"

# Build solution
dotnet build "Store Backend.sln" --configuration Release

# Run tests with coverage
dotnet test "Store Backend.sln" --collect:"XPlat Code Coverage"
```

### CI/CD Documentation

For detailed CI/CD information:
- ?? **Complete Guide**: `.github/CI_CD_GUIDE.md`
- ?? **Quick Reference**: `.github/CI_CD_QUICKREF.md`
- ?? **Workflows**: `.github/workflows/README.md`

### Pull Request Process

1. Fork the repo and create your branch from `main`
2. If you've added code that should be tested, add tests
3. If you've changed APIs, update the documentation
4. Ensure the test suite passes
5. Make sure your code follows the coding standards
6. Issue that pull request!

## Coding Standards

### Functional Programming Principles

? **DO** use immutable records for domain models
```csharp
public record Order { ... }  // ?
public class Order { ... }   // ?
```

? **DO** use `Fin<T>` for operations that can fail
```csharp
public Fin<Order> CreateOrder(...) { ... }  // ?
public Order CreateOrder(...) { ... }        // ?
```

? **DO** keep functions pure when possible
```csharp
// ? Pure function
public Money CalculateTotal(IEnumerable<OrderItem> items) =>
    items.Aggregate(Money.Zero, (acc, item) => acc + item.LineTotal);

// ? Impure function
public Money CalculateTotal(Order order)
{
    logger.LogInformation("Calculating");  // Side effect!
    return order.Total;
}
```

### Domain-Driven Design

? **DO** encapsulate business logic in domain models
```csharp
public record Order
{
    public Fin<Order> MarkAsPaid(PaymentId paymentId) { ... }  // ?
}

// ? Don't put business logic in services
public class OrderService
{
    public void MarkAsPaid(Order order) { ... }
}
```

? **DO** use value objects for domain concepts
```csharp
public record Email { ... }    // ?
public class Email { ... }     // ?
string email;                  // ?
```

? **DO** raise domain events for important state changes
```csharp
public Fin<Order> MarkAsShipped(ShipmentId shipmentId)
{
    var updated = this with { Status = OrderStatus.Shipped };
    updated.AddDomainEvent(new OrderShippedDomainEvent(...));  // ?
    return updated;
}
```

### LINQ Query Expressions

? **DO** use LINQ expressions for monadic composition
```csharp
// ? Readable and composable
var result = 
    from order in GetOrder(orderId)
    from payment in CreatePayment(order)
    from _ in UpdateOrder(order, payment)
    select payment;

// ? Nested callbacks
GetOrder(orderId).Bind(order =>
    CreatePayment(order).Bind(payment =>
        UpdateOrder(order, payment).Map(_ => payment)));
```

### Naming Conventions

? **Commands** end with `Command`
```csharp
public record CreateOrderCommand : ICommand<Fin<OrderResult>> { }
```

? **Queries** end with `Query`
```csharp
public record GetOrderByIdQuery : IQuery<Fin<OrderResult>> { }
```

? **Handlers** end with `Handler` or `CommandHandler`/`QueryHandler`
```csharp
internal class CreateOrderCommandHandler : ICommandHandler<...> { }
```

? **Domain Events** end with `DomainEvent`
```csharp
public record OrderCreatedDomainEvent(...) : IDomainEvent;
```

? **Integration Events** end with `IntegrationEvent`
```csharp
public record OrderCreatedIntegrationEvent : IntegrationEvent { }
```

### File Organization

```
Module/
??? Domain/
?   ??? Models/              # Aggregates & Entities
?   ??? ValueObjects/        # Domain value objects
?   ??? Events/              # Domain events
?   ??? Enums/               # Domain enumerations
??? Application/
?   ??? Features/
?   ?   ??? CreateX/         # One folder per feature
?   ?       ??? CreateXCommand.cs
?   ?       ??? CreateXHandler.cs
?   ??? EventHandlers/       # Integration event consumers
??? Infrastructure/          # Module-specific infrastructure
??? Persistence/
?   ??? Configurations/      # EF configurations
?   ??? Migrations/          # EF migrations
?   ??? ModuleDbContext.cs
??? Presentation/
    ??? Controllers/         # API endpoints
```

## Testing Standards

### Unit Tests

? **DO** test pure functions and domain logic
```csharp
[Fact]
public void AddLineItems_ShouldRecalculateShipping()
{
    // Arrange
    var cart = Cart.Create(userId, tax, address).ThrowIfFail();
    var items = new[] { lineItem1, lineItem2 };

    // Act
    var updated = cart.AddLineItems(items);

    // Assert
    updated.ShipmentCost.Should().Be(Money.Zero); // Total > $29.50
}
```

? **DO** use descriptive test names
```csharp
[Fact]
public void CreateOrder_WithEmptyCart_ShouldReturnValidationError()
```

? **DO** follow AAA pattern (Arrange, Act, Assert)

### Integration Tests

? **DO** test event handlers and database operations
```csharp
[Fact]
public async Task PaymentFulfilledHandler_ShouldMarkOrderAsPaid()
{
    // Arrange
    var order = await CreateTestOrder();
    var @event = new PaymentFulfilledIntegrationEvent { OrderId = order.Id };

    // Act
    await handler.Consume(@event);

    // Assert
    var updatedOrder = await GetOrder(order.Id);
    updatedOrder.Status.Should().Be(OrderStatus.Paid);
}
```

## Git Commit Messages

? **DO** write clear, descriptive commit messages

**Format:**
```
<type>(<scope>): <subject>

<body>

<footer>
```

**Types:**
- `feat`: New feature
- `fix`: Bug fix
- `docs`: Documentation changes
- `style`: Code style changes (formatting)
- `refactor`: Code refactoring
- `test`: Adding tests
- `chore`: Build process or auxiliary tool changes

**Examples:**
```
feat(order): add shipment tracking notification

Implement real-time SignalR notifications for shipment status changes.
Users now receive instant updates when their order is shipped.

Closes #123
```

```
fix(cart): correct shipping cost calculation

Fixed bug where shipping cost wasn't updated when removing items.
Shipping cost now recalculates on every cart modification.

Fixes #456
```

## Pull Request Template

```markdown
## Description
Brief description of changes

## Type of Change
- [ ] Bug fix
- [ ] New feature
- [ ] Breaking change
- [ ] Documentation update

## Testing
- [ ] Unit tests added/updated
- [ ] Integration tests added/updated
- [ ] Manual testing performed

## Checklist
- [ ] Code follows functional programming principles
- [ ] Domain logic encapsulated in domain models
- [ ] Used `Fin<T>` for error handling
- [ ] Added XML documentation
- [ ] Updated relevant documentation
- [ ] Tests pass locally
- [ ] No compiler warnings

## Related Issues
Closes #(issue number)
```

## Code Review Process

### What We Look For

? **Functional Correctness**
- Does the code do what it's supposed to do?
- Are edge cases handled?

? **Functional Programming**
- Are functions pure when possible?
- Is `Fin<T>` used for error handling?
- Are domain models immutable?

? **Domain-Driven Design**
- Is business logic in the domain?
- Are aggregates properly designed?
- Are domain events raised?

? **Code Quality**
- Is the code readable?
- Is it maintainable?
- Are there tests?

? **Performance**
- Are database queries optimized?
- Are there N+1 query problems?

## Documentation

### XML Documentation

? **DO** add XML documentation to public APIs
```csharp
/// <summary>
/// Creates a new order from the specified cart.
/// </summary>
/// <param name="cartId">The cart identifier</param>
/// <returns>A Fin containing the created order or an error</returns>
public async Task<Fin<Order>> CreateOrder(CartId cartId) { ... }
```

### README Updates

? **DO** update documentation when:
- Adding new features
- Changing APIs
- Modifying architecture
- Adding dependencies

## Setting Up Development Environment

### Prerequisites
```bash
.NET 9 SDK
SQL Server (LocalDB or full instance)
Visual Studio 2022 or VS Code
Git
```

### First Time Setup

```bash
# Clone repository
git clone https://github.com/atMou/Store-Backend.git
cd Store-Backend

# Restore packages
dotnet restore

# Update database connection strings in appsettings.Development.json

# Run migrations
dotnet ef database update --project Modules/Identity
dotnet ef database update --project Modules/Basket
dotnet ef database update --project Modules/Product
dotnet ef database update --project Modules/Order
dotnet ef database update --project Modules/Payment
dotnet ef database update --project Modules/Inventory
dotnet ef database update --project Modules/Shippment

# Run application
dotnet run --project Bootstrapper/Api
```

### Running Tests

```bash
# Run all tests
dotnet test

# Run specific test project
dotnet test ModulesTest/Order.Tests

# Run with coverage
dotnet test /p:CollectCoverage=true
```

## Getting Help

- ?? **Discussions**: Use GitHub Discussions for questions
- ?? **Bug Reports**: Create an issue with the bug template
- ?? **Feature Requests**: Create an issue with the feature template
- ?? **Email**: For security issues, email security@yourstore.com

## Code of Conduct

### Our Pledge

We pledge to make participation in our project a harassment-free experience for everyone.

### Our Standards

? Examples of behavior that contributes to a positive environment:
- Using welcoming and inclusive language
- Being respectful of differing viewpoints
- Gracefully accepting constructive criticism
- Focusing on what is best for the community

? Examples of unacceptable behavior:
- Trolling, insulting/derogatory comments
- Public or private harassment
- Publishing others' private information
- Other conduct which could reasonably be considered inappropriate

## License

By contributing, you agree that your contributions will be licensed under the MIT License.

## Recognition

Contributors will be recognized in:
- CONTRIBUTORS.md file
- Release notes
- Project documentation

Thank you for contributing to Store Backend! ??
