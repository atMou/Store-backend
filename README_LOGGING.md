# Serilog + Seq Setup - Complete ?

## ?? What This Setup Provides

Your **Store Backend** modular monolith now has enterprise-grade logging with:

### Infrastructure
- ? **Centralized Serilog configuration** across all modules
- ? **Seq** for powerful log aggregation and querying
- ? **Multiple sinks**: Console, Seq, and File logging
- ? **Structured logging** with queryable properties
- ? **Event IDs** organized by module (2000-9199)
- ? **Performance tracking** with automatic timing
- ? **Request logging** for all HTTP requests

### Developer Experience
- ? **Extension methods** for common logging operations
- ? **Code examples** and best practices
- ? **Comprehensive documentation**
- ? **Quick reference** cards

## ?? Quick Start (3 Steps)

### 1. Start Seq
```bash
cd Bootstrapper/Api
docker-compose up -d seq
```

### 2. Run Your Application
```bash
dotnet run --project Bootstrapper/Api
```

### 3. View Logs
Open: **http://localhost:5341**

That's it! Logs will appear automatically. ??

## ?? What Was Created

### Infrastructure Files
```
Shared/Infrastructure/Logging/
??? LogEvents.cs              # Event ID definitions (2000-9199)
??? LoggerExtensions.cs       # Module-specific extension methods
??? PerformanceLogging.cs     # Performance tracking helpers
??? LoggingExamples.cs        # Code examples
```

### Documentation
```
Docs/
??? IMPLEMENTATION_SUMMARY.md          # What was done (this file)
??? SERILOG_SEQ_COMPLETE_GUIDE.md     # Complete guide
??? LOGGING_QUICK_REFERENCE.md        # Daily reference
??? STARTUP_CHECKLIST.md              # Deployment checklist
??? LOGGING.md                         # Logging patterns
??? SEQ_QUICKSTART.md                  # Quick start
```

### Configuration Files
- ? `Bootstrapper/Api/appsettings.json` - Docker/Production config
- ? `Bootstrapper/Api/appsettings.Development.json` - Local dev config
- ? `Bootstrapper/Api/Program.cs` - Serilog initialization
- ? `Shared/Shared/Shared.csproj` - Serilog packages

## ?? Documentation Guide

**Start here** based on your need:

| If you want to... | Read this |
|-------------------|-----------|
| Get started quickly | `STARTUP_CHECKLIST.md` ? |
| Daily coding reference | `LOGGING_QUICK_REFERENCE.md` ?? |
| Understand architecture | `SERILOG_SEQ_COMPLETE_GUIDE.md` ?? |
| See code examples | `Shared/Infrastructure/Logging/LoggingExamples.cs` ?? |
| Query logs in Seq | `LOGGING_QUICK_REFERENCE.md` (Seq Queries section) ?? |
| Troubleshoot issues | `STARTUP_CHECKLIST.md` (Troubleshooting section) ?? |

## ?? Quick Examples

### Basic Logging
```csharp
// Inject ILogger<T>
private readonly ILogger<MyService> _logger;

// Log with structured properties
_logger.LogInformation("User {UserId} logged in from {IpAddress}", userId, ip);
```

### Using Extension Methods
```csharp
using Shared.Infrastructure.Logging;

// Identity
_logger.LogUserRegistration(userId, email);

// Product
_logger.LogProductCreated(productId, slug, price);

// Basket
_logger.LogCartCreated(userId, cartId);
```

### Performance Tracking
```csharp
using Shared.Infrastructure.Logging;

await _logger.LogPerformance(
    "GetProduct",
    async () => await _repository.GetByIdAsync(productId));
```

### Seq Queries
```sql
-- All errors
@Level = 'Error'

-- Specific user
UserId = 'guid-here'

-- Slow operations
ElapsedMs > 1000

-- Module logs
SourceContext like 'Product.%'
```

## ??? Architecture

```
Your Application (All Modules)
      ?
ILogger<T> (Microsoft.Extensions.Logging)
      ?
Serilog (with Event IDs, Extensions, Enrichers)
      ?
  ???????????????????
  ?       ?         ?
Console  Seq       File
```

### Event ID Organization
- 2000-2099: Identity Module
- 3000-3099: Product Module
- 4000-4099: Basket Module
- 5000-5099: Order Module
- 6000-6099: Payment Module
- 7000-7099: Shipment Module
- 8000-8099: Inventory Module
- 9000-9099: Integration Events
- 9100-9199: Database Operations

## ?? Key Features

### 1. Structured Logging
Properties are indexed and queryable:
```csharp
_logger.LogInformation("User {UserId} created order {OrderId} for ${Total}", 
    userId, orderId, total);
```

Query in Seq:
```sql
UserId = '...' and Total > 100
```

### 2. Performance Tracking
Automatic timing with context:
```csharp
await _logger.LogPerformance("DatabaseQuery", async () => ...);
```

Query in Seq:
```sql
ElapsedMs > 500 | select avg(ElapsedMs), OperationName group by OperationName
```

### 3. HTTP Request Logging
All requests automatically logged with:
- Method, Path, Status Code
- Elapsed time
- User ID (if authenticated)
- User Agent

Query in Seq:
```sql
StatusCode >= 400  // All errors
Elapsed > 1000     // Slow requests
```

### 4. Module-Specific Logging
Each module has its own namespace and event IDs:
```sql
SourceContext like 'Identity.%'  -- Identity logs
SourceContext like 'Product.%'   -- Product logs
EventId >= 4000 and EventId < 5000  -- All Basket events
```

## ?? Common Seq Queries

```sql
-- Recent errors
@Level = 'Error' and @Timestamp > DateTime('now') - TimeSpan('01:00:00')

-- User activity
UserId = 'guid' | order by @Timestamp

-- Integration events
EventId >= 9000 and EventId < 9100

-- Performance by endpoint
select avg(Elapsed), RequestPath group by RequestPath

-- Failed operations
@Level = 'Error' | group count() by SourceContext

-- Business metrics
EventId = 5000 | select count() group by bin(@Timestamp, 1h)
```

## ?? Configuration

### Environment Variables (.env)
```bash
SEQ_ADMIN_PASSWORD=SuperSecret123!
```

### Log Levels (appsettings.json)
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Product": "Debug",          // More verbose for Product
        "Microsoft": "Warning"        // Less noise from framework
      }
    }
  }
}
```

## ? Verification

### 1. Check Build
```bash
dotnet build
# Should succeed ?
```

### 2. Start Seq
```bash
docker-compose up -d seq
docker ps | grep seq
# Should show running container ?
```

### 3. Access Seq
```bash
curl http://localhost:5341/api
# Should return JSON ?
```

### 4. Run Application
```bash
dotnet run --project Bootstrapper/Api
# Should show logs in console ?
```

### 5. View in Seq
Open http://localhost:5341
Run query:
```sql
@Timestamp > DateTime('now') - TimeSpan('00:05:00')
# Should show logs ?
```

## ?? Troubleshooting

### Seq Not Accessible
```bash
docker logs seq
docker restart seq
```

### No Logs Appearing
1. Verify Seq URL in appsettings matches environment
2. Check application is making requests
3. Verify minimum log level isn't too high

### Build Errors
```bash
dotnet clean
dotnet restore
dotnet build
```

See `STARTUP_CHECKLIST.md` for detailed troubleshooting.

## ?? Learn More

### Essential Reading
1. **Start**: `STARTUP_CHECKLIST.md`
2. **Reference**: `LOGGING_QUICK_REFERENCE.md`
3. **Deep Dive**: `SERILOG_SEQ_COMPLETE_GUIDE.md`

### Code Examples
- `Shared/Infrastructure/Logging/LoggingExamples.cs`
- `Modules/Identity/Application/EventHandlers/*.cs`
- `Modules/Basket/Application/EventHandlers/*.cs`

### External Resources
- [Serilog Documentation](https://serilog.net/)
- [Seq Documentation](https://docs.datalust.co/)
- [Structured Logging Best Practices](https://github.com/serilog/serilog/wiki/Structured-Data)

## ?? You're Ready!

Everything is configured and ready to use. Just:

```bash
# 1. Start Seq
docker-compose up -d seq

# 2. Run your app
dotnet run --project Bootstrapper/Api

# 3. Open Seq
# http://localhost:5341
```

Your logs will flow automatically with:
- ? Structured, queryable properties
- ? Module-specific event IDs
- ? Performance metrics
- ? HTTP request tracking
- ? Integration event flow

**Happy Logging! ??**

---

**Questions?** Check the documentation:
- Quick issues: `STARTUP_CHECKLIST.md`
- How to log: `LOGGING_QUICK_REFERENCE.md`
- Architecture: `SERILOG_SEQ_COMPLETE_GUIDE.md`
