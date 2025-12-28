# ?? Serilog + Seq Setup - Startup Checklist

## ? Pre-Deployment Checklist

### 1. Verify Package Installation
```bash
# Restore packages
dotnet restore

# Build solution
dotnet build
```

**Expected Output**: Build succeeded ?

### 2. Start Seq Container
```bash
cd Bootstrapper/Api

# Start Seq
docker-compose up -d seq

# Verify Seq is running
docker ps | grep seq
```

**Expected Output**:
```
CONTAINER ID   IMAGE                    STATUS
abc123...      datalust/seq:latest     Up 5 seconds (healthy)
```

### 3. Verify Seq is Accessible

**Development**:
```bash
curl http://localhost:5341/api
```

**Docker Network**:
```bash
docker exec -it seq curl http://localhost/api
```

**Expected Output**: JSON response from Seq API

### 4. Run Application
```bash
# From solution root
dotnet run --project Bootstrapper/Api

# Or from Api directory
cd Bootstrapper/Api
dotnet run
```

**Expected Console Output**:
```
[12:34:56 INF] Starting Store Backend API
[12:34:57 INF] Application built successfully. Environment: Development
[12:34:58 INF] Store Backend API is ready to accept requests
```

### 5. Verify Logs Appear in Seq

1. Open browser: **http://localhost:5341**
2. You should see logs appearing in real-time
3. Look for startup logs:
   - "Starting Store Backend API"
   - "Application built successfully"

### 6. Test with API Request

```bash
# Example: Health check (if you have one)
curl http://localhost:5000/api/health

# Or register a user
curl -X POST http://localhost:5000/api/auth/register \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "password": "Test123!",
    "firstName": "John",
    "lastName": "Doe"
  }'
```

### 7. Verify Logs in Seq

In Seq UI, run this query:
```sql
@Timestamp > DateTime('now') - TimeSpan('00:05:00')
```

You should see:
- ? HTTP request logs
- ? Application logs
- ? Integration event logs
- ? Database operation logs

---

## ?? Configuration Verification

### Check appsettings.json

**Docker Environment** (`appsettings.json`):
```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Seq",
        "Args": { "serverUrl": "http://seq:5341" }
      }
    ]
  }
}
```

**Development** (`appsettings.Development.json`):
```json
{
  "Serilog": {
    "WriteTo": [
      {
        "Name": "Seq",
        "Args": { "serverUrl": "http://localhost:5341" }
      }
    ]
  }
}
```

### Check Program.cs

Verify these lines exist:
```csharp
// Before builder
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
    .Enrich.FromLogContext()
    .WriteTo.Console()
    .CreateBootstrapLogger();

// After builder
builder.Host.UseSerilog((context, services, configuration) => { ... });

// In Configure
app.UseSerilogRequestLogging();
```

---

## ?? Test Logging Features

### 1. Structured Logging
```csharp
_logger.LogInformation("User {UserId} logged in", userId);
```

**Verify in Seq**: Properties panel shows `UserId` as a separate field

### 2. Event IDs
```csharp
_logger.LogInformation(LogEvents.UserLogin, "User logged in");
```

**Verify in Seq**: Filter by `EventId = 2001`

### 3. Performance Logging
```csharp
await _logger.LogPerformance("GetProduct", async () => ...);
```

**Verify in Seq**: Query `ElapsedMs != null`

### 4. Integration Events
```csharp
_logger.LogIntegrationEventReceived(nameof(CartCreatedIntegrationEvent));
```

**Verify in Seq**: Filter by `EventId >= 9000 and EventId < 9100`

---

## ?? Quick Seq Queries to Test

Run these in Seq UI to verify everything works:

```sql
-- 1. All logs from last 5 minutes
@Timestamp > DateTime('now') - TimeSpan('00:05:00')

-- 2. HTTP requests only
RequestPath != null

-- 3. Identity module logs
SourceContext like 'Identity.%'

-- 4. Errors only
@Level = 'Error'

-- 5. Integration events
EventId >= 9000 and EventId < 9100

-- 6. Performance metrics
ElapsedMs > 0 | select avg(ElapsedMs), OperationName group by OperationName

-- 7. Recent user registrations
EventId = 2000

-- 8. Cart operations
EventId >= 4000 and EventId < 5000
```

---

## ?? Troubleshooting Guide

### Issue: Seq Not Accessible

**Check**:
```bash
docker ps | grep seq
docker logs seq
```

**Fix**:
```bash
docker restart seq
# Or
docker-compose restart seq
```

### Issue: No Logs Appearing

**Checklist**:
- [ ] Seq is running (`docker ps`)
- [ ] Application is running
- [ ] Making API requests
- [ ] Correct Seq URL in appsettings
- [ ] No firewall blocking port 5341

**Verify Connection**:
```bash
# From your machine
curl http://localhost:5341/api

# From Docker network
docker exec -it api curl http://seq:5341/api
```

### Issue: Too Many Logs

**Solution**: Adjust log levels in `appsettings.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.EntityFrameworkCore.Database.Command": "Warning"
      }
    }
  }
}
```

### Issue: Build Errors

**Fix**:
```bash
# Clean and rebuild
dotnet clean
dotnet restore
dotnet build
```

### Issue: Package Restore Failed

**Fix**:
```bash
# Clear NuGet cache
dotnet nuget locals all --clear

# Restore again
dotnet restore
```

---

## ?? Files Created

### Infrastructure Files
- ? `Shared/Infrastructure/Logging/LogEvents.cs` - Event ID definitions
- ? `Shared/Infrastructure/Logging/LoggerExtensions.cs` - Extension methods
- ? `Shared/Infrastructure/Logging/PerformanceLogging.cs` - Performance tracking
- ? `Shared/Infrastructure/Logging/LoggingExamples.cs` - Code examples

### Documentation Files
- ? `Docs/SERILOG_SEQ_COMPLETE_GUIDE.md` - Complete setup guide
- ? `Docs/LOGGING_QUICK_REFERENCE.md` - Quick reference card
- ? `Docs/LOGGING.md` - Original logging guide
- ? `Docs/SEQ_QUICKSTART.md` - Quick start guide

### Configuration Files
- ? `Bootstrapper/Api/appsettings.json` - Docker config
- ? `Bootstrapper/Api/appsettings.Development.json` - Development config
- ? `Bootstrapper/Api/Program.cs` - Serilog setup
- ? `Shared/Shared/Shared.csproj` - Serilog packages

---

## ? Features Implemented

### Logging Infrastructure
- ? Centralized Serilog configuration
- ? Multiple sinks (Console, Seq, File)
- ? Structured logging with properties
- ? Event IDs by module (2000s, 3000s, etc.)
- ? Performance tracking helpers
- ? Request logging middleware
- ? Log enrichment (Machine, Process, Thread)

### Module-Specific Logging
- ? Identity: Registration, login, verification
- ? Product: CRUD, price changes, stock
- ? Basket: Cart operations, coupons
- ? Order: Order lifecycle
- ? Payment: Payment processing
- ? Shipment: Shipping operations
- ? Inventory: Stock management
- ? Integration Events: Event flow tracking

### Developer Experience
- ? Extension methods for common operations
- ? Consistent event IDs
- ? Queryable in Seq
- ? Performance metrics
- ? Error tracking
- ? Correlation via scopes

---

## ?? Success Indicators

You'll know everything is working when:

1. ? Build succeeds without errors
2. ? Seq UI loads at http://localhost:5341
3. ? Logs appear in Seq within seconds
4. ? Can query logs by module, user, time
5. ? Performance metrics are captured
6. ? Integration events are tracked
7. ? HTTP requests are logged
8. ? Errors have full stack traces

---

## ?? Next Steps

1. **Set up Alerts** in Seq for critical errors
2. **Create Dashboards** for key metrics
3. **Configure Retention** policies
4. **Add API Keys** for production
5. **Set up Log Forwarding** (if needed)
6. **Train Team** on Seq queries
7. **Document** custom queries

---

## ?? Quick Links

- **Seq UI**: http://localhost:5341
- **Complete Guide**: `Docs/SERILOG_SEQ_COMPLETE_GUIDE.md`
- **Quick Reference**: `Docs/LOGGING_QUICK_REFERENCE.md`
- **Serilog Docs**: https://serilog.net/
- **Seq Docs**: https://docs.datalust.co/

---

**Happy Logging! ??**

If you encounter any issues, check the troubleshooting section above or review the complete guide.
