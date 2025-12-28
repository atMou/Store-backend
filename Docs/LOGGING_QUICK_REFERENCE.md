# Serilog + Seq Quick Reference

## ?? Start Seq
```bash
docker-compose up -d seq
```

## ?? Access
- Development: http://localhost:5341
- Docker: http://seq:5341

## ?? Basic Logging

```csharp
// Inject logger
private readonly ILogger<MyClass> _logger;

// Log levels
_logger.LogTrace("Detailed debug info");
_logger.LogDebug("Diagnostic info");
_logger.LogInformation("Normal flow");
_logger.LogWarning("Unexpected but handled");
_logger.LogError(ex, "Recoverable error");
_logger.LogCritical(ex, "Unrecoverable");

// Structured logging (? GOOD)
_logger.LogInformation("User {UserId} logged in", userId);

// String interpolation (? BAD)
_logger.LogInformation($"User {userId} logged in");

// With event ID
_logger.LogInformation(LogEvents.UserLogin, "User logged in");

// Complex objects (@ destructures)
_logger.LogInformation("Creating {@Order}", order);
```

## ?? Extension Methods

```csharp
using Shared.Infrastructure.Logging;

// Identity
_logger.LogUserRegistration(userId, email);
_logger.LogUserLogin(userId, email, success);
_logger.LogEmailVerification(userId, success);

// Product
_logger.LogProductCreated(productId, slug, price);
_logger.LogPriceChanged(productId, oldPrice, newPrice);
_logger.LogImageOperation("Upload", count, productId);

// Basket
_logger.LogCartCreated(userId, cartId);
_logger.LogCartItemAdded(cartId, productId, quantity);
_logger.LogCouponApplied(cartId, couponId, discount);

// Integration Events
_logger.LogIntegrationEventPublished(eventType, eventData);
_logger.LogIntegrationEventReceived(eventType, correlationId);
_logger.LogIntegrationEventFailed(eventType, exception);
```

## ? Performance Logging

```csharp
using Shared.Infrastructure.Logging;

// Async operations
await _logger.LogPerformance(
    "GetProduct",
    async () => await _repository.GetByIdAsync(productId),
    LogEvents.DatabaseQueryCompleted);

// With additional properties
await _logger.LogPerformance(
    "ProcessOrder",
    async () => await ProcessOrderAsync(order),
    LogEvents.OrderCreated,
    new Dictionary<string, object> 
    { 
        ["OrderId"] = orderId,
        ["Total"] = total 
    });
```

## ?? Seq Queries

### By Level
```sql
@Level = 'Error'
@Level = 'Warning'
@Level in ['Error', 'Warning']
```

### By Module
```sql
SourceContext like 'Identity.%'
SourceContext like 'Product.%'
SourceContext like 'Basket.%'
```

### By Event ID
```sql
EventId = 2000              -- User registration
EventId = 3003              -- Price changed
EventId >= 4000 and EventId < 5000  -- All cart events
```

### By User
```sql
UserId = '123e4567-e89b-12d3-a456-426614174000'
UserId != null and @Level = 'Error'
```

### Performance
```sql
ElapsedMs > 1000
select avg(Elapsed), RequestPath 
from stream 
group by RequestPath
```

### Time-Based
```sql
@Timestamp > DateTime('now') - TimeSpan('01:00:00')
@Timestamp > DateTime('today')
@Timestamp >= DateTime('2024-01-01')
```

### HTTP Requests
```sql
StatusCode >= 400
RequestMethod = 'POST'
RequestPath like '/api/auth/%'
```

### Combined
```sql
UserId = 'guid' and 
@Level = 'Error' and 
@Timestamp > DateTime('now') - TimeSpan('01:00:00')
```

## ?? Event IDs

| Range | Module | Examples |
|-------|--------|----------|
| 2000-2099 | Identity | 2000: Registration, 2001: Login |
| 3000-3099 | Product | 3000: Created, 3003: Price Change |
| 4000-4099 | Basket | 4000: Cart Created, 4004: Coupon |
| 5000-5099 | Order | 5000: Created, 5004: Payment |
| 6000-6099 | Payment | 6000: Initiated, 6002: Failed |
| 7000-7099 | Shipment | 7000: Created, 7002: Delivered |
| 8000-8099 | Inventory | 8000: Stock Added, 8001: Reserved |
| 9000-9099 | Integration | 9000: Published, 9002: Failed |
| 9100-9199 | Database | 9102: Error, 9103: Save Changes |

## ?? Configuration

### Change Log Level
Edit `appsettings.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Product": "Debug",
        "Microsoft.EntityFrameworkCore": "Information"
      }
    }
  }
}
```

### Docker Commands
```bash
# Start Seq
docker-compose up -d seq

# View logs
docker logs -f seq

# Restart
docker restart seq

# Stop
docker-compose stop seq

# Remove (keeps data)
docker-compose rm seq

# Remove with data
docker-compose down -v
```

## ? Best Practices

### DO
```csharp
// ? Structured logging
_logger.LogInformation("User {UserId} action {Action}", userId, action);

// ? Exception as first parameter
_logger.LogError(ex, "Failed to process {OrderId}", orderId);

// ? Use event IDs
_logger.LogInformation(LogEvents.UserLogin, "Login successful");

// ? Use scopes for correlation
using (_logger.BeginScope("RequestId: {RequestId}", requestId))
{
    // All logs will have RequestId
}
```

### DON'T
```csharp
// ? String interpolation
_logger.LogInformation($"User {userId}");

// ? Sensitive data
_logger.LogInformation("Password: {Password}", password);

// ? Catch without logging
try { } catch (Exception) { }

// ? Log in loops
foreach (var item in items)
    _logger.LogDebug("Item {Item}", item);
```

## ?? Common Patterns

### Controller Logging
```csharp
[HttpPost("register")]
public async Task<IActionResult> Register(RegisterDto dto)
{
    _logger.LogInformation(LogEvents.UserRegistration, 
        "Registration attempt for {Email}", dto.Email);
    
    var result = await _mediator.Send(new RegisterCommand(dto));
    
    return result.Match<IActionResult>(
        Succ: user => {
            _logger.LogUserRegistration(user.Id, user.Email);
            return Ok();
        },
        Fail: err => {
            _logger.LogWarning("Registration failed: {@Error}", err);
            return BadRequest(err);
        });
}
```

### Event Handler Logging
```csharp
public async Task Consume(ConsumeContext<CartCreatedIntegrationEvent> context)
{
    _logger.LogIntegrationEventReceived(
        nameof(CartCreatedIntegrationEvent), 
        context.CorrelationId);
    
    try
    {
        // Process
        _logger.LogCartCreated(context.Message.UserId, context.Message.CartId);
    }
    catch (Exception ex)
    {
        _logger.LogIntegrationEventFailed(
            nameof(CartCreatedIntegrationEvent), ex);
        throw;
    }
}
```

---

**Access Seq**: http://localhost:5341
**Docs**: `Docs/SERILOG_SEQ_COMPLETE_GUIDE.md`
