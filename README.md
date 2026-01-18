# 🛍️ Store Backend - E-Commerce Platform

[![.NET 9](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![C# 13](https://img.shields.io/badge/C%23-13.0-239120?logo=csharp)](https://docs.microsoft.com/en-us/dotnet/csharp/)
[![LanguageExt](https://img.shields.io/badge/LanguageExt-Functional-blue)](https://github.com/louthy/language-ext)
[![License](https://img.shields.io/badge/license-MIT-green)](LICENSE)

A modern, enterprise-grade e-commerce backend built with **Domain-Driven Design (DDD)**, **Functional Programming**, and **Event-Driven Architecture** using .NET 9 and C# 13.

## 🎯 Key Features

- ✅ **Domain-Driven Design (DDD)** - Modular monolith with bounded contexts
- 🔧 **Functional Core** - Leveraging LanguageExt for pure functional programming
- 📡 **Event-Driven Architecture** - Using MassTransit for reliable messaging
- 🔔 **Real-Time Notifications** - SignalR hubs for instant updates
- 💳 **Payment Integration** - Stripe payment processing
- 📧 **Email System** - SendGrid integration with HTML templates
- 📦 **Shipping Management** - Dynamic shipping cost calculation
- 🔐 **JWT Authentication** - Role and permission-based authorization
- 🎭 **CQRS Pattern** - Command Query Responsibility Segregation

## 🏗️ Architecture Overview

This project follows a **modular monolith architecture** with strict bounded contexts, implementing DDD principles with a functional programming approach.

```mermaid
graph TB
    subgraph API["🌐 API Gateway"]
        Gateway["<b>Bootstrapper/Api</b><br/>ASP.NET Core Web API<br/>SignalR Hubs"]
    end

    subgraph CoreModules["🔷 Core Business Modules"]
        Identity["👤 <b>Identity</b><br/>Authentication<br/>Authorization"]
        Product["📦 <b>Product</b><br/>Catalog<br/>Management"]
        Basket["🛒 <b>Basket</b><br/>Shopping Cart<br/>Coupons"]
        Inventory["📊 <b>Inventory</b><br/>Stock<br/>Management"]
    end

    subgraph TransactionModules["💰 Transaction Modules"]
        Order["📋 <b>Order</b><br/>Order<br/>Processing"]
        Payment["💳 <b>Payment</b><br/>Stripe<br/>Integration"]
        Shipment["🚚 <b>Shipment</b><br/>Delivery &<br/>Tracking"]
    end

    subgraph Infrastructure["⚙️ Shared Infrastructure"]
        Shared["🔧 <b>Shared Kernel</b><br/>• Domain Primitives<br/>• Event Bus (MassTransit)<br/>• Common Services<br/>• Email & SMS"]
    end

    Gateway -->|HTTP/REST| Identity
    Gateway -->|HTTP/REST| Product
    Gateway -->|HTTP/REST| Basket
    Gateway -->|HTTP/REST| Inventory
    Gateway -->|HTTP/REST| Order
    Gateway -->|HTTP/REST| Payment
    Gateway -->|HTTP/REST| Shipment

    Identity -.->|Domain Events| Shared
    Product -.->|Domain Events| Shared
    Basket -.->|Domain Events| Shared
    Inventory -.->|Domain Events| Shared
    Order -.->|Domain Events| Shared
    Payment -.->|Domain Events| Shared
    Shipment -.->|Domain Events| Shared

    Shared ==>|Integration Events| Identity
    Shared ==>|Integration Events| Product
    Shared ==>|Integration Events| Basket
    Shared ==>|Integration Events| Order
    Shared ==>|Integration Events| Payment
    Shared ==>|Integration Events| Shipment
    Shared ==>|Integration Events| Inventory

    style API fill:#e3f2fd,stroke:#1976d2,stroke-width:3px
    style CoreModules fill:#fff3e0,stroke:#f57c00,stroke-width:2px
    style TransactionModules fill:#e8f5e9,stroke:#388e3c,stroke-width:2px
    style Infrastructure fill:#f3e5f5,stroke:#7b1fa2,stroke-width:2px
    style Gateway fill:#42a5f5,stroke:#1565c0,stroke-width:2px,color:#fff
    style Shared fill:#ab47bc,stroke:#4a148c,stroke-width:2px,color:#fff
    style Identity fill:#ffcc80,stroke:#e65100,stroke-width:2px
    style Product fill:#ffcc80,stroke:#e65100,stroke-width:2px
    style Basket fill:#ffcc80,stroke:#e65100,stroke-width:2px
    style Inventory fill:#ffcc80,stroke:#e65100,stroke-width:2px
    style Order fill:#a5d6a7,stroke:#1b5e20,stroke-width:2px
    style Payment fill:#a5d6a7,stroke:#1b5e20,stroke-width:2px
    style Shipment fill:#a5d6a7,stroke:#1b5e20,stroke-width:2px
```

### 📦 Modules (Bounded Contexts)

| Module | Responsibility | Domain Entities |
|--------|---------------|-----------------|
| **Identity** | User management, authentication, authorization | `User`, `Role`, `Permission` |
| **Product** | Product catalog, variants, reviews | `Product`, `ColorVariant`, `SizeVariant` |
| **Basket** | Shopping cart, coupons, line items | `Cart`, `LineItem`, `Coupon` |
| **Inventory** | Stock management, warehouse operations | `Inventory`, `Stock` |
| **Order** | Order processing, order lifecycle | `Order`, `OrderItem` |
| **Payment** | Payment processing, Stripe integration | `Payment`, `PaymentIntent` |
| **Shipment** | Shipping, tracking, delivery | `Shipment`, `TrackingCode` |
| **Shared** | Cross-cutting concerns, infrastructure | Value Objects, Common Services |

## 🚀 Functional Programming with LanguageExt

This project heavily utilizes [LanguageExt](https://github.com/louthy/language-ext) to bring functional programming concepts to C#:

### Core Functional Patterns

#### 1️⃣ **Monadic Error Handling with `Fin<T>`**

```csharp
public async Task<Fin<Order>> CreateOrder(CreateOrderCommand command)
{
    return await (
        from cart in GetCart(command.CartId)
        from user in GetUser(command.UserId)
        from order in ValidateAndCreateOrder(cart, user)
        from _ in SaveOrder(order)
        select order
    ).RunAsync();
}
```

#### 2️⃣ **Railway-Oriented Programming**

```csharp
public Fin<Cart> AddLineItems(LineItem[] items)
{
    return from validItems in ValidateItems(items)
           from cart in UpdateCart(validItems)
           from _ in RecalculateTotals(cart)
           select cart;
}
```

#### 3️⃣ **Option Type for Null Safety**

```csharp
public Option<User> FindUserByEmail(string email) =>
    Optional(users.FirstOrDefault(u => u.Email == email));
```

#### 4️⃣ **IO Monad for Side Effects**

```csharp
var io = from template in ReadEmailTemplate()
         from email in BuildEmail(template, orderData)
         from response in SendEmail(email)
         select response;

var result = await io.RunSafeAsync();
```

#### 5️⃣ **Try Monad for Exception Handling**

```csharp
Try.lift(async () =>
{
    await notificationService.NotifyUser(userId, notification);
}).Run().IfFail(ex => logger.LogError(ex, "Notification failed"));
```

## 🏛️ Domain-Driven Design Implementation

### Tactical Patterns

#### 🎯 Aggregates & Entities

```csharp
public record Order : Aggregate<OrderId>
{
    private Order(UserId userId, CartId cartId, Money total) : base(OrderId.New)
    {
        UserId = userId;
        CartId = cartId;
        Total = total;
        AddDomainEvent(new OrderCreatedDomainEvent(Id, UserId, Total));
    }

    public UserId UserId { get; }
    public OrderStatus Status { get; private set; }
    public Money Total { get; }
    
    public Fin<Order> MarkAsPaid(PaymentId paymentId, DateTime paidAt)
    {
        return Status.CanTransitionTo(OrderStatus.Paid)
            .Map(_ => {
                var updated = this with { Status = OrderStatus.Paid };
                updated.AddDomainEvent(new OrderPaidDomainEvent(Id, paymentId, paidAt));
                return updated;
            });
    }
}
```

#### 💎 Value Objects

```csharp
public record Money
{
    private Money(decimal value) => Value = value;
    
    public decimal Value { get; }
    
    public static Money FromDecimal(decimal value) =>
        value >= 0 
            ? new Money(value)
            : throw new ArgumentException("Money cannot be negative");
    
    public static Money operator +(Money a, Money b) =>
        FromDecimal(a.Value + b.Value);
}

public record Email
{
    private Email(string value) => Value = value;
    
    public string Value { get; }
    
    public static Fin<Email> From(string value) =>
        IsValid(value)
            ? FinSucc(new Email(value))
            : FinFail<Email>(ValidationError.New("Invalid email format"));
}
```

#### 📢 Domain Events

```csharp
public record OrderCreatedDomainEvent(
    OrderId OrderId,
    UserId UserId,
    Money Total) : IDomainEvent;

public record PaymentFulfilledIntegrationEvent : IntegrationEvent
{
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public IEnumerable<OrderItemDto> OrderItems { get; init; }
}
```

#### 🗄️ Repositories with Functional Queries

```csharp
// Db Monad for database operations
var db = from order in GetEntity<OrderDBContext, Order>(
            o => o.Id == orderId,
            NotFoundError.New($"Order {orderId} not found"),
            opt => opt.AddInclude(o => o.OrderItems))
         from updated in UpdateEntity<OrderDBContext, Order>(
            order,
            o => o.MarkAsShipped(shipmentId, DateTime.UtcNow))
         select updated;

var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
```

### Strategic Patterns

#### 🔗 Bounded Context Integration

Modules communicate via **Integration Events** using MassTransit:

```csharp
// Payment Module publishes event
await publishEndpoint.Publish(new PaymentFulfilledIntegrationEvent
{
    PaymentId = payment.Id.Value,
    OrderId = payment.OrderId.Value,
    UserId = payment.UserId.Value
});

// Order Module consumes event
public class PaymentFulfilledIntegrationEventHandler : IConsumer<PaymentFulfilledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
    {
        // Update order status
        // Trigger shipment creation
        // Send notifications
    }
}
```

## 🛠️ Technology Stack

### Core Framework
- **.NET 9** - Latest .NET platform
- **C# 13** - Latest language features
- **Entity Framework Core 9** - ORM with migrations

### Functional Programming
- **LanguageExt** - Functional programming library
  - `Fin<T>` for error handling
  - `Option<T>` for null safety
  - `IO<T>` for side effects
  - `Try<T>` for exception handling
  - LINQ expressions for monadic composition

### Communication & Events
- **MassTransit** - Distributed application framework
- **SignalR** - Real-time web functionality
- **MediatR** - In-process messaging (CQRS)

### External Services
- **Stripe** - Payment processing
- **SendGrid** - Email delivery
- **Twilio** - SMS notifications
- **Cloudinary** - Image storage

### Database
- **SQL Server** - Relational database
- **EF Core** - Code-first migrations

## 🚀 Getting Started

### Prerequisites

```bash
# Required
.NET 9 SDK
SQL Server (LocalDB or full instance)
Node.js (for frontend integration)

# Optional
Docker (for containerized services)
```

### Installation

```bash
# Clone repository
git clone https://github.com/atMou/Store-Backend.git
cd Store-Backend

# Restore dependencies
dotnet restore

# Update connection strings in appsettings.json
# Configure external services (Stripe, SendGrid, etc.)

# Run migrations
dotnet ef database update --project Modules/Identity
dotnet ef database update --project Modules/Basket
dotnet ef database update --project Modules/Product
dotnet ef database update --project Modules/Order
dotnet ef database update --project Modules/Payment
dotnet ef database update --project Modules/Inventory
dotnet ef database update --project Modules/Shippment

# Run the application
dotnet run --project Bootstrapper/Api
```

### Configuration

Create `appsettings.json` with:

```json
{
  "ConnectionStrings": {
    "IdentityConnection": "Server=.;Database=StoreIdentity;Trusted_Connection=true;",
    "BasketConnection": "Server=.;Database=StoreBasket;Trusted_Connection=true;",
    // ... other modules
  },
  "Jwt": {
    "Secret": "your-secret-key",
    "Issuer": "Store-Backend",
    "Audience": "Store-App"
  },
  "Stripe": {
    "SecretKey": "sk_test_...",
    "PublishableKey": "pk_test_...",
    "WebhookSecret": "whsec_..."
  },
  "SendGrid": {
    "ApiKey": "SG...",
    "SenderEmail": "noreply@yourstore.com",
    "SenderName": "Your Store"
  }
}
```

## 📚 Key Features Documentation

### 🛒 Shopping Cart with Dynamic Shipping

- **Free Shipping**: Orders over $29.50
- **Standard Shipping**: $4.50 for orders under threshold
- **Real-time calculation** based on cart total after discounts

### 💳 Payment Processing

- Stripe integration with webhook support
- Payment intent creation and confirmation
- Automatic order status updates
- Real-time notifications

### 🔔 Real-Time Notifications

- **Payment success** notifications
- **Order status** updates
- **Shipment tracking** notifications
- **Delivery confirmations**

### 📧 Email System

- HTML email templates
- Order confirmation emails
- Shipping notification with tracking
- Beautiful, responsive design

### 📦 Shipment Management

- Automatic tracking code generation
- Status updates (Pending, Shipped, InTransit, Delivered)
- Integration with order lifecycle

## 📁 Project Structure

```
Store-Backend/
├── Bootstrapper/
│   └── Api/                          # API Gateway
│       ├── Program.cs
│       ├── Templates/
│       │   └── OrderShipped.html     # Email templates
│       └── appsettings.json
├── Modules/
│   ├── Identity/                     # User & Auth Module
│   │   ├── Domain/
│   │   │   ├── Models/
│   │   │   │   └── User.cs           # User aggregate
│   │   │   └── Events/
│   │   ├── Application/
│   │   │   ├── Features/             # CQRS handlers
│   │   │   └── EventHandlers/        # Integration events
│   │   ├── Infrastructure/
│   │   └── Presentation/
│   │       └── Controllers/
│   ├── Product/                      # Product Catalog Module
│   ├── Basket/                       # Shopping Cart Module
│   │   └── Domain/
│   │       └── Models/
│   │           └── Cart.cs           # Cart aggregate with shipping logic
│   ├── Order/                        # Order Management Module
│   ├── Payment/                      # Payment Processing Module
│   ├── Inventory/                    # Stock Management Module
│   └── Shipment/                     # Shipping Module
├── Shared/
│   └── Shared/                       # Shared Kernel
│       ├── Domain/
│       │   ├── ValueObjects/         # Shared value objects
│       │   ├── Abstractions/         # DDD base classes
│       │   └── Errors/               # Error types
│       ├── Application/
│       │   ├── Abstractions/         # CQRS interfaces
│       │   ├── Contracts/            # DTOs and queries
│       │   └── Features/             # Integration events
│       ├── Infrastructure/
│       │   ├── Email/                # Email services
│       │   ├── Hubs/                 # SignalR hubs
│       │   ├── Authentication/       # JWT & authorization
│       │   └── Sms/                  # SMS services
│       └── Persistence/
│           └── Db.Monad/             # Functional database access

```

## 📊 Architecture Diagrams

For detailed architecture diagrams including:
- System Context (C4 Model)
- Container Architecture
- Order Placement Flow
- CQRS Implementation
- Event-Driven Communication
- And more...

See the [Complete Architecture Diagrams](ARCHITECTURE_DIAGRAM.md)

## 🔄 Event Flow Example

### Order Placement Flow

```
1. User adds items to cart
2. User checks out → CartCheckedOutIntegrationEvent
3. Order module creates order → OrderCreatedIntegrationEvent
4. Payment module creates payment intent
5. User pays → PaymentFulfilledIntegrationEvent
6. Order marked as paid → Notification sent
7. Shipment created → ShipmentCreatedIntegrationEvent
8. Email sent with tracking → Notification sent
9. Shipment status updates → Real-time notifications
10. Delivered → ShipmentDeliveredIntegrationEvent
```

## 🧪 Testing

```bash
# Run unit tests
dotnet test

# Run specific module tests
dotnet test ModulesTest/Order.Tests
```

## 📖 Additional Documentation

- [Architecture Deep Dive](docs/ARCHITECTURE.md)
- [Functional Programming Guide](docs/FUNCTIONAL_PROGRAMMING.md)
- [API Documentation](docs/API_DOCUMENTATION.md)
- [Email Notifications](EMAIL_NOTIFICATION_IMPLEMENTATION.md)
- [Shipping Cost Logic](SHIPPING_COST_IMPLEMENTATION.md)
- [Real-Time Notifications](SHIPMENT_NOTIFICATIONS_IMPLEMENTATION.md)

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 👨‍💻 Author

**Ahmed Moustafa**

- GitHub: [@atMou](https://github.com/atMou)

## 🙏 Acknowledgments

- [LanguageExt](https://github.com/louthy/language-ext) - Functional programming library
- [MassTransit](https://masstransit-project.com/) - Distributed application framework
- Domain-Driven Design community
- Functional programming community

---

⭐ **Star this repository if you find it helpful!**
