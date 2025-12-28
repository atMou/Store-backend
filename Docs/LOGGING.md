# Serilog + Seq Logging Setup Guide

## ?? Overview

This project uses **Serilog** for structured logging with **Seq** as the log aggregation and analysis platform.

## ?? Prerequisites

- Docker Desktop running
- Seq container running on port 5341

## ?? Docker Setup

### Start Seq

```bash
docker run -d --name seq \
  -e ACCEPT_EULA=Y \
  -p 5341:80 \
  -p 5342:5341 \
  -v seq-data:/data \
  datalust/seq:latest
```

Or use Docker Compose:

```yaml
version: '3.8'

services:
  seq:
    image: datalust/seq:latest
    container_name: seq
    environment:
      - ACCEPT_EULA=Y
    ports:
      - "5341:80"      # Web UI
      - "5342:5341"    # Ingestion
    volumes:
      - seq-data:/data
    networks:
      - backend-network

volumes:
  seq-data:

networks:
  backend-network:
    driver: bridge
```

### Access Seq UI

Open browser: **http://localhost:5341**

## ?? Configuration

### appsettings.json (Already Configured)

```json
{
  "Serilog": {
    "Using": [ "Serilog.Sinks.Console", "Serilog.Sinks.Seq" ],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Information",
        "System": "Warning"
      }
    },
    "WriteTo": [
      { "Name": "Console" },
      {
        "Name": "Seq",
        "Args": { "serverUrl": "http://seq:5341" }
      }
    ],
    "Enrich": [ "FromLogContext", "WithMachineName", "WithProcessId", "WithThreadId" ],
    "Properties": {
      "Application": "Store Backend",
      "Environment": "Development"
    }
  }
}
```

### Program.cs (Already Configured)

```csharp
builder.Host.UseSerilog((ctx, config) =>
{
    config.ReadFrom.Configuration(ctx.Configuration);
});

// At the end of pipeline
app.UseSerilogRequestLogging();
```

## ?? Usage

### Basic Logging

```csharp
public class MyService
{
    private readonly ILogger<MyService> _logger;
    
    public MyService(ILogger<MyService> logger)
    {
        _logger = logger;
    }
    
    public void DoSomething()
    {
        // ? GOOD - Structured logging
        _logger.LogInformation("User {UserId} performed {Action}", userId, "Login");
        
        // ? BAD - String interpolation (not searchable in Seq)
        // _logger.LogInformation($"User {userId} performed Login");
    }
}
```

### Event Handlers

```csharp
public class CartCreatedEventHandler : IConsumer<CartCreatedIntegrationEvent>
{
    private readonly ILogger<CartCreatedEventHandler> _logger;
    
    public async Task Consume(ConsumeContext<CartCreatedIntegrationEvent> context)
    {
        _logger.LogInformation(
            "Processing CartCreatedIntegrationEvent for UserId: {UserId}, CartId: {CartId}", 
            context.Message.UserId, 
            context.Message.CartId);
        
        try
        {
            // Your logic
            
            _logger.LogInformation(
                "Successfully processed cart creation for User {UserId}", 
                context.Message.UserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(
                ex, 
                "Failed to process cart creation for User {UserId}", 
                context.Message.UserId);
            throw;
        }
    }
}
```

### Complex Objects

```csharp
// Use @ to destructure complex objects in Seq
_logger.LogInformation("Creating order {@Order}", order);

// This will show all properties of the order in Seq
```

### With Scopes

```csharp
using (_logger.BeginScope("OperationId: {OperationId}", Guid.NewGuid()))
{
    _logger.LogInformation("Starting operation");
    // All logs here will have OperationId property
    _logger.LogInformation("Step 1 complete");
    _logger.LogInformation("Step 2 complete");
}
```

## ?? Log Levels

| Level | When to Use | Example |
|-------|-------------|---------|
| **Trace** | Very detailed, debugging only | `_logger.LogTrace("Entering method with {Param}", param)` |
| **Debug** | Diagnostic information | `_logger.LogDebug("Fetching user {UserId}", userId)` |
| **Information** | Normal application flow | `_logger.LogInformation("User {UserId} logged in", userId)` |
| **Warning** | Unexpected but handled | `_logger.LogWarning("User {UserId} attempted invalid action", userId)` |
| **Error** | Recoverable errors | `_logger.LogError(ex, "Failed to process order {OrderId}", orderId)` |
| **Critical** | Unrecoverable errors | `_logger.LogCritical(ex, "Database connection lost")` |

## ?? Seq Query Examples

### Find Errors
```sql
@Level = 'Error'
```

### Find Logs for Specific User
```sql
UserId = '123e4567-e89b-12d3-a456-426614174000'
```

### Find Slow Operations
```sql
ElapsedMs > 1000
```

### Cart Creation Events
```sql
@MessageTemplate like '%CartCreatedIntegrationEvent%'
```

### Errors in Last Hour
```sql
@Level = 'Error' and @Timestamp > DateTime('now') - TimeSpan('01:00:00')
```

### Combined Filters
```sql
UserId = 'guid-here' and @Level = 'Error' and @Timestamp > DateTime('2024-01-01')
```

### Group by Error Type
```sql
@Level = 'Error' | group count() by @Exception
```

### Integration Event Failures
```sql
@MessageTemplate like '%IntegrationEvent%' and @Level = 'Error'
```

## ?? Best Practices

### ? DO

1. **Use structured logging**
   ```csharp
   _logger.LogInformation("User {UserId} logged in", userId);
   ```

2. **Pass exceptions as first parameter**
   ```csharp
   _logger.LogError(ex, "Failed to process {OrderId}", orderId);
   ```

3. **Use descriptive property names**
   ```csharp
   _logger.LogInformation("Order {OrderId} total: {TotalAmount}", orderId, total);
   ```

4. **Log at appropriate levels**
   - Information for normal flow
   - Warning for unexpected but handled
   - Error for exceptions

5. **Use scopes for correlation**
   ```csharp
   using (_logger.BeginScope("RequestId: {RequestId}", requestId))
   {
       // All logs will have RequestId
   }
   ```

### ? DON'T

1. **Don't use string interpolation**
   ```csharp
   // ? BAD
   _logger.LogInformation($"User {userId} logged in");
   ```

2. **Don't log sensitive data**
   ```csharp
   // ? BAD
   _logger.LogInformation("Password: {Password}", password);
   ```

3. **Don't log in tight loops**
   ```csharp
   // ? BAD
   foreach (var item in items)
   {
       _logger.LogDebug("Processing {Item}", item); // Too many logs
   }
   ```

4. **Don't catch and log without rethrowing**
   ```csharp
   // ? BAD
   try { }
   catch (Exception ex) 
   {
       _logger.LogError(ex, "Error");
       // Exception lost!
   }
   ```

## ?? Quick Start Checklist

- [x] Seq running on Docker
- [x] Serilog configured in appsettings.json
- [x] Serilog configured in Program.cs
- [x] ILogger<T> injected in services
- [x] Using structured logging with placeholders
- [x] Seq UI accessible at http://localhost:5341

## ?? Additional Resources

- [Serilog Documentation](https://serilog.net/)
- [Seq Documentation](https://docs.datalust.co/docs)
- [Structured Logging Best Practices](https://github.com/serilog/serilog/wiki/Structured-Data)

## ?? Troubleshooting

### Logs not appearing in Seq

1. Check Seq is running: `docker ps | grep seq`
2. Verify connection: `http://localhost:5341`
3. Check appsettings.json serverUrl
4. Check Docker network connectivity

### Too many logs

1. Adjust minimum level in appsettings.json
2. Override specific namespaces:
   ```json
   "Override": {
     "Microsoft": "Warning",
     "System": "Warning",
     "MassTransit": "Information"
   }
   ```

### Can't query logs in Seq

1. Use structured logging with placeholders, not string interpolation
2. Make sure properties have consistent naming
3. Use @ for complex object destructuring

---

**Happy Logging! ??**
