# Serilog + Seq Setup - Implementation Summary

## ? What Was Done

### 1. Package Installation
Added Serilog packages to `Shared.csproj`:
- Serilog v4.3.0
- Serilog.Extensions.Logging v8.0.0
- Serilog.Sinks.Console v6.0.0
- Serilog.Sinks.Seq v9.0.0
- Serilog.Sinks.File v6.0.0
- Serilog.Enrichers.Environment v3.0.1
- Serilog.Enrichers.Process v3.0.0
- Serilog.Enrichers.Thread v4.0.0
- Serilog.Settings.Configuration v9.0.0

### 2. Infrastructure Created

#### Logging Infrastructure (`Shared/Infrastructure/Logging/`)
```
Shared/Infrastructure/Logging/
??? LogEvents.cs              # Event ID definitions (2000-9199)
??? LoggerExtensions.cs       # Module-specific extension methods
??? PerformanceLogging.cs     # Performance tracking helpers
??? LoggingExamples.cs        # Code examples and patterns
```

**LogEvents.cs** - Organized event IDs:
- 2000-2099: Identity Module
- 3000-3099: Product Module
- 4000-4099: Basket Module
- 5000-5099: Order Module
- 6000-6099: Payment Module
- 7000-7099: Shipment Module
- 8000-8099: Inventory Module
- 9000-9099: Integration Events
- 9100-9199: Database Operations

**LoggerExtensions.cs** - Convenience methods:
```csharp
_logger.LogUserRegistration(userId, email);
_logger.LogProductCreated(productId, slug, price);
_logger.LogCartCreated(userId, cartId);
_logger.LogIntegrationEventPublished(eventType, data);
```

**PerformanceLogging.cs** - Automatic timing:
```csharp
await _logger.LogPerformance(
    "GetProduct",
    async () => await _repository.GetByIdAsync(id));
```

### 3. Configuration Files Updated

#### `appsettings.json` (Docker/Production)
```json
{
  "Serilog": {
    "Using": ["Serilog.Sinks.Console", "Serilog.Sinks.Seq", "Serilog.Sinks.File"],
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore": "Warning",
        "Product": "Information",
        "Basket": "Information",
        // ... all modules configured
      }
    },
    "WriteTo": [
      {
        "Name": "Console",
        "Args": {
          "outputTemplate": "[{Timestamp:HH:mm:ss} {Level:u3}] [{SourceContext}] {Message:lj}{NewLine}{Exception}"
        }
      },
      {
        "Name": "Seq",
        "Args": {
          "serverUrl": "http://seq:5341",
          "compact": true
        }
      },
      {
        "Name": "File",
        "Args": {
          "path": "logs/store-backend-.log",
          "rollingInterval": "Day",
          "retainedFileCountLimit": 7
        }
      }
    ],
    "Enrich": ["FromLogContext", "WithMachineName", "WithProcessId", "WithThreadId"],
    "Properties": {
      "Application": "Store Backend",
      "Environment": "Docker"
    }
  }
}
```

#### `appsettings.Development.json`
- Seq URL: `http://localhost:5341`
- More verbose logging (Debug level)
- EF Core query logging enabled

### 4. Program.cs Enhanced

```csharp
// Bootstrap logger for startup errors
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

// Serilog configuration from appsettings
builder.Host.UseSerilog((context, services, configuration) =>
{
    configuration
        .ReadFrom.Configuration(context.Configuration)
        .ReadFrom.Services(services)
        .Enrich.FromLogContext()
        .Enrich.WithProperty("Application", "Store Backend")
        .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
});

// Request logging middleware with enrichment
app.UseSerilogRequestLogging(options =>
{
    options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
    options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
    {
        diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
        diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
        }
    };
});
```

### 5. Docker Setup

#### `docker-compose.yml` - Already configured:
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
```

#### `.env` - Seq password already set:
```
SEQ_ADMIN_PASSWORD=SuperSecret123!
```

### 6. Event Handlers Updated

Updated event handlers in:
- ? `Identity/Application/EventHandlers/CartCreateIntegrationEventHandler.cs`
- ? `Basket/Application/EventHandlers/UserEmailVerifiedIntegrationEventHandler.cs`
- ? `Basket/Application/EventHandlers/ProductPriceChangedIntegrationEventHandler.cs`

Changed from:
```csharp
using ILogger = Serilog.ILogger;  // ? Direct Serilog dependency
```

To:
```csharp
using Microsoft.Extensions.Logging;  // ? ASP.NET Core abstraction
```

### 7. Documentation Created

```
Docs/
??? SERILOG_SEQ_COMPLETE_GUIDE.md    # Comprehensive guide (architecture, queries, examples)
??? LOGGING_QUICK_REFERENCE.md       # Quick reference card for developers
??? STARTUP_CHECKLIST.md             # Deployment and verification checklist
??? LOGGING.md                        # Original logging guide
??? SEQ_QUICKSTART.md                 # Quick start for Seq
```

## ?? Architecture Overview

```
???????????????????????????????????????????????????????????????
?                    Your Application                          ?
?                                                              ?
?  Identity ? Product ? Basket ? Order ? Payment ? etc.     ?
?     ?         ?        ?        ?        ?                  ?
?  ILogger<T>  (Microsoft.Extensions.Logging)                 ?
????????????????????????????????????????????????????????????????
                           ?
                           ?
???????????????????????????????????????????????????????????????
?               Serilog (Shared Infrastructure)                ?
?                                                              ?
?  • Event IDs (2000-9199)                                    ?
?  • Extension Methods                                         ?
?  • Performance Tracking                                      ?
?  • Enrichers (Machine, Process, Thread)                     ?
????????????????????????????????????????????????????????????????
                           ?
          ???????????????????????????????????
          ?                ?                ?
          ?                ?                ?
    ????????????     ????????????    ????????????
    ? Console  ?     ?   Seq    ?    ?   File   ?
    ?  Sink    ?     ?   Sink   ?    ?   Sink   ?
    ????????????     ????????????    ????????????
    (Terminal)       (localhost:5341) (logs/*.log)
```

## ?? Key Features

### Structured Logging
```csharp
// ? Properties are indexed and queryable in Seq
_logger.LogInformation("User {UserId} logged in from {IpAddress}", userId, ip);

// Seq Query: UserId = 'guid-here'
// Seq Query: IpAddress like '192.168.%'
```

### Event IDs for Categorization
```csharp
_logger.LogInformation(LogEvents.UserRegistration, "User registered");

// Seq Query: EventId = 2000
// Seq Query: EventId >= 4000 and EventId < 5000  // All cart events
```

### Performance Tracking
```csharp
await _logger.LogPerformance("GetProduct", async () => 
{
    return await _repository.GetByIdAsync(productId);
});

// Seq Query: ElapsedMs > 1000  // Slow operations
// Seq Query: select avg(ElapsedMs), OperationName group by OperationName
```

### Module-Specific Extensions
```csharp
// Instead of:
_logger.LogInformation("User {UserId} registered with email {Email}", userId, email);

// Use:
_logger.LogUserRegistration(userId, email);

// Benefits:
// - Consistent event IDs
// - Consistent message templates
// - Type-safe parameters
```

### Request Logging
```csharp
// Automatic HTTP request/response logging
// No code needed - just add middleware in Program.cs

app.UseSerilogRequestLogging();

// Seq Query: StatusCode >= 400  // All errors
// Seq Query: Elapsed > 1000     // Slow requests
// Seq Query: select count(), RequestPath group by RequestPath
```

## ?? What You Can Now Do

### 1. Monitor Application Health
```sql
-- Errors in last hour
@Level = 'Error' and @Timestamp > DateTime('now') - TimeSpan('01:00:00')

-- Response time trends
select avg(Elapsed), bin(@Timestamp, 5m) group by bin(@Timestamp, 5m)

-- Most active users
select count(), UserId group by UserId order by count() desc
```

### 2. Debug Issues
```sql
-- Trace specific user's journey
UserId = 'guid-here' | order by @Timestamp

-- Find failed integration events
EventId = 9002

-- Database errors
EventId = 9102
```

### 3. Performance Analysis
```sql
-- Slow operations
ElapsedMs > 1000

-- Average performance by operation
select avg(ElapsedMs), OperationName group by OperationName

-- HTTP endpoint performance
select avg(Elapsed), RequestPath group by RequestPath
```

### 4. Business Intelligence
```sql
-- User registrations today
EventId = 2000 and @Timestamp > DateTime('today')

-- Products created this week
EventId = 3000 and @Timestamp > DateTime('now') - TimeSpan('7.00:00:00')

-- Orders by total value
EventId = 5000 | select Total, OrderId order by Total desc
```

## ?? Getting Started

### Quick Start (5 minutes)
```bash
# 1. Start Seq
cd Bootstrapper/Api
docker-compose up -d seq

# 2. Run application
dotnet run

# 3. Open Seq UI
# Browser: http://localhost:5341

# 4. Make API requests
# Watch logs appear in real-time!
```

### First Queries to Try
```sql
-- 1. All recent logs
@Timestamp > DateTime('now') - TimeSpan('00:05:00')

-- 2. Application startup
@MessageTemplate like '%Starting%'

-- 3. HTTP requests
RequestPath != null

-- 4. Your module logs
SourceContext like 'Product.%'
```

## ?? Documentation

| Document | Purpose |
|----------|---------|
| `SERILOG_SEQ_COMPLETE_GUIDE.md` | Architecture, setup, queries, examples |
| `LOGGING_QUICK_REFERENCE.md` | Quick reference for developers |
| `STARTUP_CHECKLIST.md` | Deployment and troubleshooting |
| `LOGGING.md` | Original logging patterns |
| `SEQ_QUICKSTART.md` | Get Seq running quickly |

## ?? Learning Path

1. **Start Here**: Read `STARTUP_CHECKLIST.md`
2. **Quick Reference**: Use `LOGGING_QUICK_REFERENCE.md` daily
3. **Deep Dive**: Study `SERILOG_SEQ_COMPLETE_GUIDE.md`
4. **Code Examples**: Check `Shared/Infrastructure/Logging/LoggingExamples.cs`

## ?? Access Points

- **Seq UI**: http://localhost:5341
- **Seq API**: http://localhost:5341/api
- **Alternative UI**: http://localhost:9091
- **Logs Directory**: `Bootstrapper/Api/logs/`

## ? Success Criteria

You'll know it's working when:
1. ? `dotnet build` succeeds
2. ? Seq UI loads without errors
3. ? Logs appear within seconds of starting app
4. ? Can query logs by module, user, time
5. ? HTTP requests are automatically logged
6. ? Performance metrics show ElapsedMs
7. ? Integration events have EventId >= 9000

## ?? You're All Set!

Your Store Backend now has:
- ? Enterprise-grade logging infrastructure
- ? Powerful querying with Seq
- ? Module-specific log categorization
- ? Performance tracking out of the box
- ? Comprehensive documentation
- ? Best practices baked in

**Start Seq, run your app, and watch the logs flow!** ??

---

For questions or issues, refer to:
- `Docs/STARTUP_CHECKLIST.md` for troubleshooting
- `Docs/SERILOG_SEQ_COMPLETE_GUIDE.md` for detailed explanations
- Seq documentation: https://docs.datalust.co/
