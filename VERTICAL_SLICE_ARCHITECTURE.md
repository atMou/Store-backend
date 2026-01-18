# ??? Vertical Slice Architecture - Store Backend

## ?? Overview

This project implements **Vertical Slice Architecture** where each feature is a complete, independent slice through all architectural layers, promoting high cohesion and low coupling.

## ?? High-Level Architecture

```mermaid
graph TB
    subgraph Client["?? Client Layer"]
        HTTP[HTTP Request]
    end

    subgraph Gateway["?? API Gateway"]
        Controller[Controller<br/>Minimal APIs]
    end

    subgraph Feature["?? Feature Slice (Vertical)"]
        direction TB
        Command[Command/Query<br/>Input DTO]
        Handler[Handler<br/>Business Logic]
        Domain[Domain Models<br/>Aggregates & Events]
        Repository[Repository<br/>Data Access]
        Integration[Integration Events<br/>Cross-Module Communication]
    end

    subgraph Infrastructure["?? Infrastructure"]
        DB[(Database)]
        EventBus[Event Bus<br/>MassTransit]
        External[External Services<br/>Stripe, SendGrid]
    end

    HTTP --> Controller
    Controller --> Command
    Command --> Handler
    Handler --> Domain
    Handler --> Repository
    Repository --> DB
    Handler --> Integration
    Integration --> EventBus
    Handler --> External

    style Client fill:#e3f2fd
    style Gateway fill:#fff3e0
    style Feature fill:#e8f5e9
    style Infrastructure fill:#f3e5f5
```

## ?? Vertical Slice Example: Create Order

```mermaid
graph LR
    subgraph Request["?? Request"]
        Input["POST /api/orders<br/>CreateOrderCommand"]
    end

    subgraph Presentation["?? Presentation"]
        Controller["OrderController<br/>MapEndpoint"]
    end

    subgraph Application["?? Application"]
        Command["CreateOrderCommand<br/>{CartId, UserId, Address}"]
        Handler["CreateOrderHandler<br/>Business Logic"]
        Validation["Validation<br/>FluentValidation"]
    end

    subgraph Domain["?? Domain"]
        Aggregate["Order Aggregate<br/>Create() + Invariants"]
        Events["OrderCreatedEvent<br/>Domain Event"]
    end

    subgraph Persistence["?? Persistence"]
        DbOps["Database Operations<br/>Save Order + Items"]
        Transaction["Unit of Work<br/>Transaction"]
    end

    subgraph Integration["?? Integration"]
        PubEvent["Publish<br/>OrderCreatedIntegrationEvent"]
        EventBus["MassTransit<br/>Event Bus"]
    end

    subgraph Response["?? Response"]
        Output["200 OK<br/>OrderResult DTO"]
    end

    Input --> Controller
    Controller --> Command
    Command --> Validation
    Validation --> Handler
    Handler --> Aggregate
    Aggregate --> Events
    Handler --> DbOps
    DbOps --> Transaction
    Handler --> PubEvent
    PubEvent --> EventBus
    Handler --> Output

    style Request fill:#e3f2fd
    style Presentation fill:#fff3e0
    style Application fill:#e8f5e9
    style Domain fill:#c8e6c9
    style Persistence fill:#f3e5f5
    style Integration fill:#ffe0b2
    style Response fill:#c5cae9
```

## ??? Module Structure (Vertical Slices)

```mermaid
graph TB
    subgraph Module["?? Order Module"]
        subgraph Features["?? Features (Vertical Slices)"]
            CreateOrder["? CreateOrder/<br/>• CreateOrderCommand.cs<br/>• CreateOrderHandler.cs<br/>• CreateOrderValidator.cs"]
            
            UpdateOrder["?? UpdateOrder/<br/>• UpdateOrderCommand.cs<br/>• UpdateOrderHandler.cs"]
            
            GetOrder["?? GetOrderById/<br/>• GetOrderByIdQuery.cs<br/>• GetOrderByIdHandler.cs"]
            
            CancelOrder["? CancelOrder/<br/>• CancelOrderCommand.cs<br/>• CancelOrderHandler.cs"]
        end

        subgraph Shared["?? Shared (Horizontal)"]
            Domain["Domain/<br/>• Order.cs (Aggregate)<br/>• OrderItem.cs<br/>• OrderStatus.cs"]
            
            Infrastructure["Infrastructure/<br/>• OrderDbContext.cs<br/>• Configurations/"]
        end
    end

    CreateOrder --> Domain
    UpdateOrder --> Domain
    GetOrder --> Domain
    CancelOrder --> Domain
    
    CreateOrder --> Infrastructure
    UpdateOrder --> Infrastructure
    GetOrder --> Infrastructure
    CancelOrder --> Infrastructure

    style Features fill:#e8f5e9
    style Shared fill:#fff3e0
```

## ?? CQRS Vertical Slices

```mermaid
graph LR
    subgraph Commands["?? Commands (Write Operations)"]
        direction TB
        C1["CreateOrder<br/>Command + Handler"]
        C2["UpdateOrder<br/>Command + Handler"]
        C3["CancelOrder<br/>Command + Handler"]
        C4["MarkAsPaid<br/>Command + Handler"]
    end

    subgraph Queries["?? Queries (Read Operations)"]
        direction TB
        Q1["GetOrderById<br/>Query + Handler"]
        Q2["GetOrders<br/>Query + Handler"]
        Q3["GetOrderHistory<br/>Query + Handler"]
        Q4["SearchOrders<br/>Query + Handler"]
    end

    subgraph Domain["?? Shared Domain"]
        Aggregate["Order Aggregate<br/>Business Rules<br/>Invariants"]
    end

    subgraph Database["?? Database"]
        WriteDB[("Write Model<br/>Normalized<br/>EF Core")]
        ReadDB[("Read Model<br/>Denormalized<br/>Direct SQL")]
    end

    C1 & C2 & C3 & C4 --> Aggregate
    Aggregate --> WriteDB
    Q1 & Q2 & Q3 & Q4 --> ReadDB
    WriteDB -.->|Eventually| ReadDB

    style Commands fill:#ffcdd2
    style Queries fill:#c8e6c9
    style Domain fill:#fff3e0
    style Database fill:#e1bee7
```

## ?? Vertical Slice File Organization

```mermaid
graph TB
    subgraph OrderModule["?? Order Module"]
        direction TB
        
        subgraph CreateOrderSlice["?? Features/CreateOrder/"]
            CreateCommand["CreateOrderCommand.cs<br/><i>Input DTO</i>"]
            CreateHandler["CreateOrderHandler.cs<br/><i>Business Logic</i>"]
            CreateValidator["CreateOrderValidator.cs<br/><i>Validation Rules</i>"]
        end
        
        subgraph GetOrderSlice["?? Features/GetOrderById/"]
            GetQuery["GetOrderByIdQuery.cs<br/><i>Input DTO</i>"]
            GetHandler["GetOrderByIdHandler.cs<br/><i>Query Logic</i>"]
        end
        
        subgraph UpdateOrderSlice["?? Features/UpdateOrder/"]
            UpdateCommand["UpdateOrderCommand.cs<br/><i>Input DTO</i>"]
            UpdateHandler["UpdateOrderHandler.cs<br/><i>Business Logic</i>"]
        end
        
        subgraph SharedLayer["?? Shared Layer"]
            Domain["Domain/Models/<br/>Order.cs<br/>OrderItem.cs"]
            Infrastructure["Infrastructure/<br/>OrderDbContext.cs"]
            Events["EventHandlers/<br/>PaymentFulfilledHandler.cs"]
        end
    end

    CreateHandler --> Domain
    GetHandler --> Domain
    UpdateHandler --> Domain
    
    CreateHandler --> Infrastructure
    GetHandler --> Infrastructure
    UpdateHandler --> Infrastructure

    style CreateOrderSlice fill:#e8f5e9
    style GetOrderSlice fill:#e8f5e9
    style UpdateOrderSlice fill:#e8f5e9
    style SharedLayer fill:#fff3e0
```

## ?? Complete Feature Flow (End-to-End)

```mermaid
sequenceDiagram
    participant Client
    participant API as API Controller
    participant Mediator as MediatR
    participant Handler as Feature Handler
    participant Domain as Domain Model
    participant DB as Database
    participant Events as Event Bus
    participant Other as Other Modules

    Client->>+API: POST /api/orders
    API->>API: Validate JWT
    API->>+Mediator: Send(CreateOrderCommand)
    Mediator->>+Handler: Handle(command)
    
    Note over Handler: Vertical Slice Execution
    
    Handler->>Handler: Validate input
    Handler->>+Domain: Order.Create(...)
    Domain->>Domain: Apply business rules
    Domain->>Domain: Raise domain events
    Domain-->>-Handler: Order aggregate
    
    Handler->>+DB: SaveChanges()
    DB->>DB: Begin transaction
    DB->>DB: Save order
    DB->>DB: Process domain events
    DB->>DB: Commit transaction
    DB-->>-Handler: Success
    
    Handler->>+Events: Publish(OrderCreatedEvent)
    Events-->>-Handler: Event published
    
    Handler-->>-Mediator: OrderResult
    Mediator-->>-API: OrderResult
    API-->>-Client: 200 OK + OrderResult
    
    Events->>Other: Notify other modules
    Other->>Other: React to event

    Note over Client,Other: Complete vertical slice executed
```

## ? Key Benefits of Vertical Slices

```mermaid
mindmap
    root((Vertical<br/>Slice<br/>Architecture))
        High Cohesion
            Related code together
            Single responsibility
            Easy to understand
        Low Coupling
            Minimal dependencies
            Independent features
            Parallel development
        Scalability
            Add features easily
            Remove features safely
            No shared layers
        Maintainability
            Easy to locate code
            Reduced side effects
            Clear boundaries
        Testing
            Test entire slice
            Mock only externals
            Integration friendly
        Team Productivity
            Clear ownership
            Avoid merge conflicts
            Faster onboarding
```

## ?? Comparison: Vertical vs Traditional Horizontal

```mermaid
graph TB
    subgraph Traditional["? Traditional Horizontal Layers"]
        direction TB
        PL[Presentation Layer<br/>All Controllers]
        AL[Application Layer<br/>All Services]
        DL[Domain Layer<br/>All Models]
        IL[Infrastructure Layer<br/>All Repositories]
        
        PL --> AL
        AL --> DL
        AL --> IL
        
        Note1["?? Scattered changes<br/>?? Shared dependencies<br/>?? Coupling issues"]
    end

    subgraph Vertical["? Vertical Slice Architecture"]
        direction LR
        
        Feature1["CreateOrder<br/>Feature"]
        Feature2["GetOrder<br/>Feature"]
        Feature3["UpdateOrder<br/>Feature"]
        
        SharedDomain["Shared<br/>Domain"]
        SharedInfra["Shared<br/>Infrastructure"]
        
        Feature1 --> SharedDomain
        Feature2 --> SharedDomain
        Feature3 --> SharedDomain
        
        Feature1 --> SharedInfra
        Feature2 --> SharedInfra
        Feature3 --> SharedInfra
        
        Note2["? Isolated changes<br/>? Minimal sharing<br/>? High cohesion"]
    end

    style Traditional fill:#ffcdd2
    style Vertical fill:#c8e6c9
```

## ?? Real Example from Your Project

### **Product Module - Vertical Slices**

```
Product/
??? Application/
?   ??? Features/                     ? Vertical Slices
?   ?   ??? CreateProduct/            ? Complete feature slice
?   ?   ?   ??? CreateProductCommand.cs
?   ?   ?   ??? CreateProductHandler.cs
?   ?   ?   ??? CreateProductValidator.cs
?   ?   ??? GetProductById/           ? Complete feature slice
?   ?   ?   ??? GetProductByIdQuery.cs
?   ?   ?   ??? GetProductByIdHandler.cs
?   ?   ??? UpdateProduct/            ? Complete feature slice
?   ?   ?   ??? UpdateProductCommand.cs
?   ?   ?   ??? UpdateProductHandler.cs
?   ?   ??? DeleteProduct/            ? Complete feature slice
?   ?       ??? DeleteProductCommand.cs
?   ?       ??? DeleteProductHandler.cs
?   ??? EventHandlers/                ? Cross-cutting concerns
?       ??? OrderCreatedHandler.cs
??? Domain/                           ? Shared horizontal layer
?   ??? Models/
?   ?   ??? Product.cs
?   ??? Events/
?       ??? ProductCreatedEvent.cs
??? Persistence/                      ? Shared horizontal layer
    ??? ProductDbContext.cs
    ??? Configurations/
```

## ?? Key Principles

1. **Feature Folders** - Each feature in its own folder
2. **Complete Slice** - All code for a feature together
3. **Minimal Sharing** - Share only domain models and infrastructure
4. **Independent Testing** - Test each slice independently
5. **Easy Discovery** - Find all related code in one place

---

**?? This vertical slice architecture makes your codebase more maintainable, scalable, and developer-friendly!**
