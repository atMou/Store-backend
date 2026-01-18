# ??? Store Backend - Architecture & Functionality Diagrams

## ?? System Architecture Overview

```mermaid
C4Context
    title System Context - E-Commerce Store Backend

    Person(customer, "Customer", "End user shopping online")
    Person(admin, "Admin", "Store administrator")
    
    System_Boundary(backend, "Store Backend System") {
        System(api, "API Gateway", "ASP.NET Core Web API<br/>Handles all HTTP requests")
        System(modules, "Business Modules", "Domain-driven modules<br/>with bounded contexts")
        System(events, "Event Bus", "MassTransit<br/>Integration events")
        System(signalr, "SignalR Hub", "Real-time notifications")
    }
    
    System_Ext(stripe, "Stripe", "Payment processing")
    System_Ext(sendgrid, "SendGrid", "Email delivery")
    System_Ext(twilio, "Twilio", "SMS notifications")
    System_Ext(cloudinary, "Cloudinary", "Image storage")
    SystemDb(database, "SQL Server", "Module databases")
    
    Rel(customer, api, "Uses", "HTTPS/REST")
    Rel(admin, api, "Manages", "HTTPS/REST")
    Rel(api, modules, "Routes to")
    Rel(modules, events, "Publishes/Subscribes")
    Rel(modules, database, "Reads/Writes", "EF Core")
    Rel(api, signalr, "Broadcasts")
    Rel(signalr, customer, "Notifies", "WebSocket")
    Rel(modules, stripe, "Processes payments")
    Rel(modules, sendgrid, "Sends emails")
    Rel(modules, twilio, "Sends SMS")
    Rel(modules, cloudinary, "Stores images")
```

## ?? Module Architecture (C4 Container)

```mermaid
C4Container
    title Container Diagram - Modular Monolith Architecture

    Container_Boundary(api_gateway, "API Gateway") {
        Container(web_api, "Web API", "ASP.NET Core 9", "REST endpoints, Swagger")
        Container(signalr_hub, "SignalR Hub", "SignalR", "Real-time notifications")
    }

    Container_Boundary(core_modules, "Core Business Modules") {
        Container(identity, "Identity Module", "C# 13", "Authentication<br/>Authorization<br/>User Management")
        Container(product, "Product Module", "C# 13", "Catalog<br/>Variants<br/>Reviews")
        Container(basket, "Basket Module", "C# 13", "Shopping Cart<br/>Coupons<br/>Discounts")
        Container(inventory, "Inventory Module", "C# 13", "Stock Management<br/>Warehouse")
    }

    Container_Boundary(transaction_modules, "Transaction Modules") {
        Container(order, "Order Module", "C# 13", "Order Processing<br/>Lifecycle Management")
        Container(payment, "Payment Module", "C# 13", "Stripe Integration<br/>Payment Intents")
        Container(shipment, "Shipment Module", "C# 13", "Shipping<br/>Tracking<br/>Delivery")
    }

    Container_Boundary(shared, "Shared Infrastructure") {
        Container(event_bus, "Event Bus", "MassTransit", "Integration Events<br/>Pub/Sub")
        Container(shared_kernel, "Shared Kernel", "C# 13", "Domain Primitives<br/>Value Objects<br/>Common Services")
    }

    ContainerDb(identity_db, "Identity DB", "SQL Server", "Users, Roles, Permissions")
    ContainerDb(product_db, "Product DB", "SQL Server", "Products, Variants")
    ContainerDb(basket_db, "Basket DB", "SQL Server", "Carts, Line Items")
    ContainerDb(order_db, "Order DB", "SQL Server", "Orders, Order Items")
    ContainerDb(payment_db, "Payment DB", "SQL Server", "Payments, Transactions")
    ContainerDb(inventory_db, "Inventory DB", "SQL Server", "Stock, Warehouses")
    ContainerDb(shipment_db, "Shipment DB", "SQL Server", "Shipments, Tracking")

    Rel(web_api, identity, "Routes", "HTTP")
    Rel(web_api, product, "Routes", "HTTP")
    Rel(web_api, basket, "Routes", "HTTP")
    Rel(web_api, inventory, "Routes", "HTTP")
    Rel(web_api, order, "Routes", "HTTP")
    Rel(web_api, payment, "Routes", "HTTP")
    Rel(web_api, shipment, "Routes", "HTTP")

    Rel(identity, identity_db, "Reads/Writes", "EF Core")
    Rel(product, product_db, "Reads/Writes", "EF Core")
    Rel(basket, basket_db, "Reads/Writes", "EF Core")
    Rel(order, order_db, "Reads/Writes", "EF Core")
    Rel(payment, payment_db, "Reads/Writes", "EF Core")
    Rel(inventory, inventory_db, "Reads/Writes", "EF Core")
    Rel(shipment, shipment_db, "Reads/Writes", "EF Core")

    Rel(identity, event_bus, "Publishes/Subscribes")
    Rel(product, event_bus, "Publishes/Subscribes")
    Rel(basket, event_bus, "Publishes/Subscribes")
    Rel(order, event_bus, "Publishes/Subscribes")
    Rel(payment, event_bus, "Publishes/Subscribes")
    Rel(inventory, event_bus, "Publishes/Subscribes")
    Rel(shipment, event_bus, "Publishes/Subscribes")

    Rel(event_bus, shared_kernel, "Uses")
```

## ?? Order Placement Flow (Sequence Diagram)

```mermaid
sequenceDiagram
    actor Customer
    participant API as API Gateway
    participant Basket as Basket Module
    participant Order as Order Module
    participant Payment as Payment Module
    participant Shipment as Shipment Module
    participant EventBus as Event Bus
    participant SignalR as SignalR Hub
    participant Email as Email Service
    participant Stripe

    Customer->>+API: Add items to cart
    API->>+Basket: POST /api/basket/items
    Basket-->>-API: Cart updated
    API-->>-Customer: 200 OK

    Customer->>+API: Checkout cart
    API->>+Basket: POST /api/basket/checkout
    Basket->>EventBus: Publish CartCheckedOutEvent
    Basket-->>-API: Cart checked out
    API-->>-Customer: 200 OK

    EventBus->>+Order: CartCheckedOutEvent
    Order->>Order: Create order (Pending)
    Order->>EventBus: Publish OrderCreatedEvent
    Order-->>-EventBus: Order created

    EventBus->>+Payment: OrderCreatedEvent
    Payment->>Stripe: Create payment intent
    Stripe-->>Payment: Payment intent created
    Payment-->>-EventBus: Payment intent ready

    Customer->>+API: Submit payment
    API->>+Payment: POST /api/payment/confirm
    Payment->>Stripe: Confirm payment
    Stripe-->>Payment: Payment succeeded
    Payment->>EventBus: Publish PaymentFulfilledEvent
    Payment-->>-API: Payment confirmed
    API-->>-Customer: Payment successful

    EventBus->>+Order: PaymentFulfilledEvent
    Order->>Order: Update order (Paid)
    Order-->>-EventBus: Order paid

    EventBus->>+SignalR: Notify customer
    SignalR-->>-Customer: ?? Payment successful

    EventBus->>+Shipment: PaymentFulfilledEvent
    Shipment->>Shipment: Create shipment
    Shipment->>EventBus: Publish ShipmentCreatedEvent
    Shipment-->>-EventBus: Shipment created

    EventBus->>+Email: ShipmentCreatedEvent
    Email->>Email: Build HTML email
    Email->>Email: Send order confirmation
    Email-->>-EventBus: Email sent

    EventBus->>+SignalR: Notify customer
    SignalR-->>-Customer: ?? Order shipped

    Note over Shipment: Later: Update tracking
    Shipment->>EventBus: Publish ShipmentDeliveredEvent
    EventBus->>+SignalR: Notify customer
    SignalR-->>-Customer: ?? Order delivered
```

## ??? DDD Layered Architecture (Per Module)

```mermaid
graph TB
    subgraph Presentation["?? Presentation Layer"]
        Controllers["Controllers<br/>API Endpoints<br/>Request/Response DTOs"]
    end

    subgraph Application["?? Application Layer"]
        Features["Features (CQRS)<br/>Commands & Queries<br/>Handlers"]
        EventHandlers["Event Handlers<br/>Integration Events<br/>Domain Events"]
        Contracts["Contracts<br/>DTOs<br/>Abstractions"]
    end

    subgraph Domain["?? Domain Layer"]
        Aggregates["Aggregates<br/>Order, Cart, Product"]
        Entities["Entities<br/>OrderItem, LineItem"]
        ValueObjects["Value Objects<br/>Money, Email, Address"]
        DomainEvents["Domain Events<br/>OrderCreated<br/>PaymentFulfilled"]
        DomainServices["Domain Services<br/>Business Rules<br/>Invariants"]
    end

    subgraph Infrastructure["?? Infrastructure Layer"]
        External["External Services<br/>Stripe, SendGrid<br/>Twilio, Cloudinary"]
    end

    subgraph Persistence["?? Persistence Layer"]
        DbContext["DbContext<br/>EF Core"]
        Configurations["Entity Configurations<br/>Fluent API"]
        Repositories["Repositories<br/>Db Monad Pattern"]
        Migrations["Migrations<br/>Database Schema"]
    end

    Controllers --> Features
    Controllers --> Contracts
    Features --> Domain
    EventHandlers --> Features
    EventHandlers --> Domain
    Features --> Repositories
    Features --> External
    Repositories --> DbContext
    DbContext --> Configurations
    Domain -.->|Raises| DomainEvents
    
    style Presentation fill:#e3f2fd
    style Application fill:#fff3e0
    style Domain fill:#e8f5e9
    style Infrastructure fill:#fce4ec
    style Persistence fill:#f3e5f5
```

## ?? Authentication & Authorization Flow

```mermaid
sequenceDiagram
    actor User
    participant API
    participant Identity as Identity Module
    participant JWT as JWT Service
    participant DB as Identity DB
    participant Module as Protected Module

    User->>+API: POST /api/auth/login
    API->>+Identity: Login request
    Identity->>+DB: Validate credentials
    DB-->>-Identity: User + Roles + Permissions
    Identity->>+JWT: Generate token
    JWT-->>-Identity: JWT token
    Identity-->>-API: Token + User info
    API-->>-User: 200 OK (JWT token)

    Note over User: Store JWT token

    User->>+API: GET /api/orders (+ JWT)
    API->>API: Validate JWT
    API->>API: Extract claims<br/>(UserId, Roles, Permissions)
    API->>+Module: Authorized request
    Module->>Module: Check permissions
    Module->>DB: Query orders for user
    DB-->>Module: Orders
    Module-->>-API: Orders data
    API-->>-User: 200 OK (Orders)
```

## ?? CQRS Pattern Implementation

```mermaid
graph LR
    subgraph Commands["?? Commands (Write Side)"]
        CreateOrder[CreateOrderCommand]
        UpdateOrder[UpdateOrderCommand]
        DeleteOrder[DeleteOrderCommand]
    end

    subgraph Queries["?? Queries (Read Side)"]
        GetOrder[GetOrderByIdQuery]
        GetOrders[GetOrdersQuery]
        GetOrderHistory[GetOrderHistoryQuery]
    end

    subgraph Handlers["?? Handlers"]
        CommandHandlers[Command Handlers<br/>Modify state<br/>Raise events]
        QueryHandlers[Query Handlers<br/>Read-only<br/>Optimized queries]
    end

    subgraph Domain["?? Domain"]
        Aggregates[Aggregates<br/>Business Logic<br/>Invariants]
        Events[Domain Events<br/>State changes]
    end

    subgraph Database["?? Database"]
        WriteDB[(Write Model<br/>Normalized)]
        ReadDB[(Read Model<br/>Denormalized)]
    end

    Commands --> CommandHandlers
    Queries --> QueryHandlers
    CommandHandlers --> Aggregates
    Aggregates --> WriteDB
    Aggregates -.->|Raises| Events
    QueryHandlers --> ReadDB
    Events -.->|Eventually| ReadDB

    style Commands fill:#ffebee
    style Queries fill:#e8f5e9
    style Handlers fill:#fff3e0
    style Domain fill:#e3f2fd
    style Database fill:#f3e5f5
```

## ?? Event-Driven Communication

```mermaid
graph TB
    subgraph Modules["Modules"]
        Order[Order Module]
        Payment[Payment Module]
        Shipment[Shipment Module]
        Basket[Basket Module]
        Inventory[Inventory Module]
        Identity[Identity Module]
    end

    subgraph EventBus["Event Bus (MassTransit)"]
        Queue1[CartCheckedOut Queue]
        Queue2[OrderCreated Queue]
        Queue3[PaymentFulfilled Queue]
        Queue4[ShipmentCreated Queue]
    end

    Basket -->|Publish| Queue1
    Queue1 -->|Subscribe| Order
    
    Order -->|Publish| Queue2
    Queue2 -->|Subscribe| Payment
    Queue2 -->|Subscribe| Inventory
    
    Payment -->|Publish| Queue3
    Queue3 -->|Subscribe| Order
    Queue3 -->|Subscribe| Shipment
    Queue3 -->|Subscribe| Basket
    
    Shipment -->|Publish| Queue4
    Queue4 -->|Subscribe| Identity
    Queue4 -->|Subscribe| Order

    style Basket fill:#ffcc80
    style Order fill:#a5d6a7
    style Payment fill:#a5d6a7
    style Shipment fill:#a5d6a7
    style Inventory fill:#ffcc80
    style Identity fill:#ffcc80
```

## ?? Functional Programming with LanguageExt

```mermaid
graph LR
    subgraph Input["Input"]
        Command[CreateOrderCommand]
    end

    subgraph Pipeline["Railway-Oriented Pipeline"]
        Validate[Validate<br/>Fin&lt;Cart&gt;]
        Transform[Transform<br/>Fin&lt;Order&gt;]
        Save[Save<br/>Fin&lt;Order&gt;]
        Publish[Publish Events<br/>Fin&lt;Order&gt;]
    end

    subgraph Output["Output"]
        Success[Success<br/>Order]
        Failure[Failure<br/>Error]
    end

    Command --> Validate
    Validate -->|Success| Transform
    Validate -.->|Failure| Failure
    Transform -->|Success| Save
    Transform -.->|Failure| Failure
    Save -->|Success| Publish
    Save -.->|Failure| Failure
    Publish -->|Success| Success
    Publish -.->|Failure| Failure

    style Validate fill:#e8f5e9
    style Transform fill:#e8f5e9
    style Save fill:#e8f5e9
    style Publish fill:#e8f5e9
    style Success fill:#c8e6c9
    style Failure fill:#ffcdd2
```

## ?? Functional Error Handling

```mermaid
graph TD
    Start[Start Operation]
    
    Start --> GetCart{GetCart<br/>Fin&lt;Cart&gt;}
    GetCart -->|Success| GetUser{GetUser<br/>Fin&lt;User&gt;}
    GetCart -.->|NotFoundError| Error1[Return Error]
    
    GetUser -->|Success| Validate{Validate Cart<br/>Fin&lt;Unit&gt;}
    GetUser -.->|NotFoundError| Error2[Return Error]
    
    Validate -->|Success| CreateOrder{Create Order<br/>Fin&lt;Order&gt;}
    Validate -.->|ValidationError| Error3[Return Error]
    
    CreateOrder -->|Success| SaveOrder{Save Order<br/>Fin&lt;Order&gt;}
    CreateOrder -.->|BusinessError| Error4[Return Error]
    
    SaveOrder -->|Success| Result[Return Success]
    SaveOrder -.->|DatabaseError| Error5[Return Error]
    
    Error1 --> HandleError[Match Error Type]
    Error2 --> HandleError
    Error3 --> HandleError
    Error4 --> HandleError
    Error5 --> HandleError
    
    HandleError --> Return404[404 Not Found]
    HandleError --> Return400[400 Bad Request]
    HandleError --> Return500[500 Internal Error]

    style Start fill:#e3f2fd
    style Result fill:#c8e6c9
    style HandleError fill:#fff3e0
    style Return404 fill:#ffcdd2
    style Return400 fill:#ffcdd2
    style Return500 fill:#ffcdd2
```

## ?? Database Per Module Pattern

```mermaid
graph TB
    subgraph API["API Gateway"]
        Gateway[ASP.NET Core API]
    end

    subgraph Modules["Modules"]
        Identity[Identity Module]
        Product[Product Module]
        Basket[Basket Module]
        Order[Order Module]
        Payment[Payment Module]
        Shipment[Shipment Module]
    end

    subgraph Databases["Databases (One per Module)"]
        IdentityDB[(Identity DB)]
        ProductDB[(Product DB)]
        BasketDB[(Basket DB)]
        OrderDB[(Order DB)]
        PaymentDB[(Payment DB)]
        ShipmentDB[(Shipment DB)]
    end

    Gateway --> Identity
    Gateway --> Product
    Gateway --> Basket
    Gateway --> Order
    Gateway --> Payment
    Gateway --> Shipment

    Identity --> IdentityDB
    Product --> ProductDB
    Basket --> BasketDB
    Order --> OrderDB
    Payment --> PaymentDB
    Shipment --> ShipmentDB

    style IdentityDB fill:#ffcc80
    style ProductDB fill:#ffcc80
    style BasketDB fill:#ffcc80
    style OrderDB fill:#a5d6a7
    style PaymentDB fill:#a5d6a7
    style ShipmentDB fill:#a5d6a7
```

## ?? CI/CD Pipeline

```mermaid
graph LR
    subgraph Source["Source Control"]
        Push[Git Push]
    end

    subgraph CI["Continuous Integration"]
        Format[Format Check<br/>dotnet format]
        Lint[Code Analysis<br/>Static Analysis]
        Build[Build<br/>dotnet build]
        Test[Run Tests<br/>dotnet test]
        Coverage[Code Coverage<br/>Report]
    end

    subgraph CD["Continuous Deployment"]
        Publish[Publish<br/>dotnet publish]
        Docker[Build Docker Image]
        Deploy[Deploy to Server]
    end

    subgraph Monitoring["Monitoring"]
        Health[Health Checks]
        Logs[Logging<br/>Serilog ? Seq]
        Metrics[Metrics]
    end

    Push --> Format
    Format -->|Pass| Lint
    Lint -->|Pass| Build
    Build -->|Pass| Test
    Test -->|Pass| Coverage
    Coverage -->|Pass| Publish
    Publish --> Docker
    Docker --> Deploy
    Deploy --> Health
    Deploy --> Logs
    Deploy --> Metrics

    Format -.->|Fail| Notify1[Notify Team]
    Lint -.->|Fail| Notify1
    Build -.->|Fail| Notify1
    Test -.->|Fail| Notify1

    style Format fill:#e8f5e9
    style Lint fill:#e8f5e9
    style Build fill:#e8f5e9
    style Test fill:#e8f5e9
    style Deploy fill:#c8e6c9
```

---

## ?? How to Use These Diagrams

### **In GitHub:**
- Mermaid diagrams render automatically in `.md` files
- View them directly in your repository

### **Export as Image:**
```bash
# Using Mermaid CLI
npm install -g @mermaid-js/mermaid-cli
mmdc -i docs/ARCHITECTURE_DIAGRAM.md -o docs/architecture.png
```

### **In Documentation Sites:**
- Copy to Confluence, Notion, GitBook
- Diagrams auto-render in most platforms

### **In Presentations:**
- Screenshot the rendered diagrams
- Or use Mermaid Live Editor: https://mermaid.live

---

**?? These diagrams comprehensively describe your Store Backend's architecture and functionality using industry-standard notation!**
