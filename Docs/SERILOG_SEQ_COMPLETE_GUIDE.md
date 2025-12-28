# Serilog + Seq Setup - Complete Guide

## ??? Architecture Overview

Your Store Backend uses a **modular monolith** architecture with centralized logging:

```
???????????????????????????????????????????????????????????????
?                     Serilog Configuration                    ?
?                     (Program.cs + appsettings)              ?
???????????????????????????????????????????????????????????????
                              ?
          ?????????????????????????????????????????
          ?                   ?                   ?
          ?                   ?                   ?
    ????????????        ????????????       ????????????
    ? Console  ?        ?   Seq    ?       ?   File   ?
    ?  Sink    ?        ?   Sink   ?       ?   Sink   ?
    ????????????        ????????????       ????????????
                             ?
                             ?
          ???????????????????????????????????????
          ?                                     ?
          ?                                     ?
    [Development]                         [Production]
    localhost:5341                        seq:5341
```

### Modules That Log:
- ? **Identity** - User registration, login, verification
- ? **Product** - CRUD, price changes, stock updates
- ? **Basket** - Cart operations, coupons
- ? **Order** - Order lifecycle
- ? **Payment** - Payment processing
- ? **Shipment** - Shipping operations
- ? **Inventory** - Stock management
- ? **Shared** - Infrastructure, integration events

## ?? Quick Start

### 1?? Start Seq Container

```bash
# From project root
cd Bootstrapper/Api
docker-compose up -d seq
```

Or manually:
```bash
docker run -d \
  --name seq \
  -e ACCEPT_EULA=Y \
  -e SEQ_FIRSTRUN_ADMINPASSWORD=SuperSecret123! \
  -p 5341:5341 \
  -p 9091:80 \
  -v seq-data:/data \
  datalust/seq:latest
```

### 2?? Verify Seq is Running

```bash
docker ps | grep seq
docker logs seq
```

### 3?? Access Seq UI

- **Development**: http://localhost:5341
- **Docker Network**: http://seq:5341
- **Alternative Port**: http://localhost:9091

**Login**: No authentication required on first run
**Admin Password** (if enabled): `SuperSecret123!` (from .env)

### 4?? Run Your Application

```bash
# From solution root
dotnet run --project Bootstrapper/Api

# Or from Api directory
cd Bootstrapper/Api
dotnet run
```

### 5?? Generate Logs

Make API requests and watch logs appear in Seq in real-time!

```bash
# Example: Register a user
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "Test123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

## ?? Log Structure

### Log Levels by Module

| Module | Default Level | Override Available |
|--------|--------------|-------------------|
| Identity | Information | ? |
| Product | Information | ? |
| Basket | Information | ? |
| Order | Information | ? |
| Payment | Information | ? |
| Shipment | Information | ? |
| Inventory | Information | ? |
| Microsoft.* | Warning | ? |
| EF Core | Warning | ? |
| MassTransit | Information | ? |

### Event IDs by Category

```csharp
// Identity Module (2000-2099)
2000 - UserRegistration
2001 - UserLogin
2002 - EmailVerification
2003 - PasswordReset

// Product Module (3000-3099)
3000 - ProductCreated
3001 - ProductUpdated
3003 - PriceChanged
3004 - StockUpdated
3005 - ImageUploaded

// Basket Module (4000-4099)
4000 - CartCreated
4001 - CartItemAdded
4004 - CouponApplied

// Order Module (5000-5099)
5000 - OrderCreated
5004 - PaymentProcessed

// Integration Events (9000-9099)
9000 - IntegrationEventPublished
9001 - IntegrationEventReceived
9002 - IntegrationEventFailed
```

## ?? Seq Query Examples

### By Module

```sql
-- Identity module logs
SourceContext like 'Identity.%'

-- Product module logs
SourceContext like 'Product.%'

-- All module logs
SourceContext like 'Basket.%' or SourceContext like 'Product.%'
```

### By Operation

```sql
-- User registrations
EventId = 2000

-- Product price changes
EventId = 3003

-- Cart creations
EventId = 4000

-- Integration events
EventId >= 9000 and EventId < 9100
```

### By User

```sql
-- All operations by specific user
UserId = '123e4567-e89b-12d3-a456-426614174000'

-- User's cart operations
UserId = 'guid' and EventId >= 4000 and EventId < 5000
```

### Performance Analysis

```sql
-- Slow operations (>1s)
ElapsedMs > 1000

-- Average response time by endpoint
select avg(Elapsed), RequestPath 
from stream 
group by RequestPath
order by avg(Elapsed) desc

-- HTTP errors
StatusCode >= 400

-- Requests by user
IsAuthenticated = true | group count() by UserId
```

### Error Analysis

```sql
-- All errors
@Level = 'Error'

-- Errors by module
@Level = 'Error' | group count() by SourceContext

-- Integration event failures
EventId = 9002

-- Database errors
EventId = 9102
```

### Time-Based Queries

```sql
-- Last hour
@Timestamp > DateTime('now') - TimeSpan('01:00:00')

-- Today's logs
@Timestamp > DateTime('today')

-- Specific time range
@Timestamp >= DateTime('2024-01-01') and @Timestamp < DateTime('2024-01-02')
```

### Advanced Queries

```sql
-- Failed user registrations
EventId = 2000 and @Level = 'Warning'

-- Price changes over $100
EventId = 3003 and NewPrice > 100

-- Orders with high total
EventId = 5000 and Total > 500

-- Slow database queries
ElapsedMs > 500 and SourceContext like '%Repository%'

-- Integration event flow
EventId in [9000, 9001] | order by @Timestamp
```

## ?? Usage in Code

### Basic Logging

```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;
    
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public void DoSomething(Guid userId)
    {
        _logger.LogInformation("Processing request for User {UserId}", userId);
    }
}
```

### Using Extension Methods

```csharp
using Shared.Infrastructure.Logging;

public class AuthService
{
    public async Task<User> Register(RegisterDto dto)
    {
        var user = await CreateUser(dto);
        
        // Use extension method
        _logger.LogUserRegistration(user.Id, user.Email);
        
        return user;
    }
}
```

### Performance Logging

```csharp
using Shared.Infrastructure.Logging;

public async Task<Product> GetProduct(Guid productId)
{
    return await _logger.LogPerformance(
        "GetProduct",
        async () => await _repository.GetByIdAsync(productId),
        LogEvents.DatabaseQueryCompleted,
        new Dictionary<string, object> { ["ProductId"] = productId }
    );
}
```

### Event Handler Logging

```csharp
public class CartCreatedEventHandler : IConsumer<CartCreatedIntegrationEvent>
{
    private readonly ILogger<CartCreatedEventHandler> _logger;
    
    public async Task Consume(ConsumeContext<CartCreatedIntegrationEvent> context)
    {
        _logger.LogIntegrationEventReceived(
            nameof(CartCreatedIntegrationEvent),
            context.CorrelationId);
        
        try
        {
            // Process event
            _logger.LogCartCreated(context.Message.UserId, context.Message.CartId);
        }
        catch (Exception ex)
        {
            _logger.LogIntegrationEventFailed(
                nameof(CartCreatedIntegrationEvent), 
                ex);
            throw;
        }
    }
}
```

## ?? Configuration

### Environment-Specific Settings

#### Development (appsettings.Development.json)
- ? Seq URL: `http://localhost:5341`
- ? Minimum Level: `Debug`
- ? EF Core Queries: `Information` (to see SQL)
- ? File Logging: `logs/store-backend-dev-.log`

#### Docker (appsettings.json)
- ? Seq URL: `http://seq:5341`
- ? Minimum Level: `Information`
- ? EF Core Queries: `Warning` (reduce noise)
- ? File Logging: `logs/store-backend-.log`

### Adjusting Log Levels

Edit `appsettings.json` or `appsettings.Development.json`:

```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Product": "Debug",           // More verbose for Product module
        "Microsoft.EntityFrameworkCore": "Information"  // See EF queries
      }
    }
  }
}
```

### Adding Custom Sinks

```json
{
  "Serilog": {
    "WriteTo": [
      { "Name": "Console" },
      { "Name": "Seq", "Args": { "serverUrl": "http://seq:5341" } },
      {
        "Name": "File",
        "Args": {
          "path": "logs/errors-.log",
          "restrictedToMinimumLevel": "Error",
          "rollingInterval": "Day"
        }
      }
    ]
  }
}
```

## ?? Docker Compose Configuration

Your `docker-compose.yml` already includes Seq:

```yaml
seq:
  image: datalust/seq:latest
  container_name: seq
  environment:
    ACCEPT_EULA: Y
    SEQ_FIRSTRUN_ADMINPASSWORD: ${SEQ_ADMIN_PASSWORD}
  ports:
    - "5341:5341"  # Ingestion + UI
    - "9091:80"    # Alternative UI port
  volumes:
    - seq-data:/data
  healthcheck:
    test: ["CMD", "curl", "-f", "http://localhost/health"]
    interval: 20s
```

## ?? Troubleshooting

### Seq Not Accessible

```bash
# Check if running
docker ps | grep seq

# Check logs
docker logs seq

# Restart
docker restart seq

# Recreate
docker-compose down
docker-compose up -d seq
```

### No Logs Appearing

1. ? Verify Seq URL in appsettings matches your environment
2. ? Check Docker network connectivity: `docker network inspect store-backend_default`
3. ? Ensure API is making requests
4. ? Check minimum log level isn't filtering everything

### Connection Refused

**Development**: Use `http://localhost:5341`
**Docker**: Use `http://seq:5341`

Verify with:
```bash
curl http://localhost:5341/api
```

### Too Many Logs

Adjust override levels in appsettings:
```json
{
  "Override": {
    "Microsoft": "Warning",
    "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
  }
}
```

## ?? Best Practices

### ? DO

1. **Use structured logging**
   ```csharp
   _logger.LogInformation("User {UserId} registered", userId);
   ```

2. **Use event IDs for categorization**
   ```csharp
   _logger.LogInformation(LogEvents.UserRegistration, "User registered");
   ```

3. **Log exceptions properly**
   ```csharp
   _logger.LogError(ex, "Failed to process order {OrderId}", orderId);
   ```

4. **Use performance logging for slow operations**
   ```csharp
   await _logger.LogPerformance("DatabaseQuery", async () => ...);
   ```

5. **Add context with scopes**
   ```csharp
   using (_logger.BeginScope("RequestId: {RequestId}", requestId))
   {
       // All logs will have RequestId
   }
   ```

### ? DON'T

1. **Don't use string interpolation**
   ```csharp
   _logger.LogInformation($"User {userId}");  // ? Bad
   ```

2. **Don't log sensitive data**
   ```csharp
   _logger.LogInformation("Password: {Password}", password);  // ? Bad
   ```

3. **Don't catch without logging**
   ```csharp
   try { } catch (Exception) { }  // ? Bad - exception lost
   ```

4. **Don't log in tight loops**
   ```csharp
   foreach (var item in items)
       _logger.LogDebug("Item {Item}", item);  // ? Too many logs
   ```

## ?? Key Features Implemented

? **Centralized Configuration** - All settings in appsettings.json
? **Module-Specific Log Levels** - Control verbosity per module
? **Structured Logging** - Queryable properties in Seq
? **Event IDs** - Categorized by module (2000s, 3000s, etc.)
? **Performance Tracking** - Built-in timing helpers
? **Request Logging** - Automatic HTTP request/response logs
? **Error Enrichment** - Exception details captured
? **Multiple Sinks** - Console, Seq, and File
? **Docker Support** - Works in containers
? **Health Checks** - Seq has built-in health endpoint

## ?? Useful Commands

```bash
# Start all services
docker-compose up -d

# Start only Seq
docker-compose up -d seq

# View Seq logs
docker logs -f seq

# Stop all
docker-compose down

# Reset Seq data
docker-compose down -v
docker volume rm store-backend_seq-data

# Check Seq health
curl http://localhost:5341/api
```

## ?? Next Steps

1. ? Run `docker-compose up -d seq`
2. ? Access http://localhost:5341
3. ? Run your API
4. ? Make requests and watch logs
5. ? Explore Seq queries
6. ? Set up alerts (Seq UI > Settings > Alerts)

---

**Happy Logging! ??**

For questions, check:
- Seq Documentation: https://docs.datalust.co/
- Serilog Documentation: https://serilog.net/
