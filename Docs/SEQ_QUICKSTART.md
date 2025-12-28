# Quick Start - Seq Setup

## 1?? Start Seq Container

```bash
# Start Seq
docker run -d --name seq \
  -e ACCEPT_EULA=Y \
  -p 5341:80 \
  -v seq-data:/data \
  datalust/seq:latest

# Verify Seq is running
docker ps | grep seq
```

## 2?? Access Seq UI

Open your browser: **http://localhost:5341**

## 3?? Run Your Application

```bash
dotnet run --project Bootstrapper/Api
```

## 4?? Test Logging

Make API requests and watch logs appear in Seq UI!

### Example: Register a User

```bash
POST http://localhost:5000/api/auth/register
Content-Type: application/json

{
  "email": "test@example.com",
  "password": "Test123!",
  "firstName": "John",
  "lastName": "Doe"
}
```

### Check Logs in Seq

1. Go to http://localhost:5341
2. You'll see logs appearing in real-time
3. Try these queries:

```sql
-- All logs
*

-- Errors only
@Level = 'Error'

-- User registration
@MessageTemplate like '%register%'

-- Specific user
UserId = 'guid-here'

-- Last 15 minutes
@Timestamp > DateTime('now') - TimeSpan('00:15:00')
```

## 5?? Common Seq Queries

### Find Integration Events
```sql
@MessageTemplate like '%IntegrationEvent%'
```

### Cart Operations
```sql
CartId != null
```

### Slow Operations
```sql
ElapsedMs > 500
```

### Errors by Exception Type
```sql
@Level = 'Error' | group count() by @Exception
```

## ??? Troubleshooting

### Seq not accessible
```bash
# Check if Seq is running
docker ps

# Check Seq logs
docker logs seq

# Restart Seq
docker restart seq
```

### No logs appearing
1. Check application is running
2. Verify Seq URL in appsettings.json
3. Make API requests to generate logs
4. Check console for any connection errors

### Connection Refused
- Make sure Seq is running: `docker ps | grep seq`
- Use `http://localhost:5341` for Development
- Use `http://seq:5341` for Docker network

## ?? Useful Docker Commands

```bash
# Stop Seq
docker stop seq

# Start Seq
docker start seq

# Remove Seq (data preserved in volume)
docker rm seq

# Remove Seq and data
docker rm seq
docker volume rm seq-data

# View Seq logs
docker logs -f seq
```

## ?? You're All Set!

Your logging is now fully configured with:
- ? Serilog for structured logging
- ? Seq for log aggregation and analysis
- ? Console output for development
- ? All event handlers instrumented
- ? Powerful query capabilities

Happy logging! ??
