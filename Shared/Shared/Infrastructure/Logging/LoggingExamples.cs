using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Logging;

/// <summary>
/// Examples and best practices for using Serilog with Seq
/// </summary>
public static class LoggingExamples
{
    /// <summary>
    /// Example: Basic structured logging
    /// </summary>
    public static void LogBasicInfo(ILogger logger, string userId, string action)
    {
        // ? GOOD - Structured logging (properties are indexed in Seq)
        logger.LogInformation("User {UserId} performed {Action}", userId, action);
        
        // ? BAD - String interpolation (cannot query in Seq)
        // logger.LogInformation($"User {userId} performed {action}");
    }

    /// <summary>
    /// Example: Logging with complex objects
    /// Use @ to destructure objects in Seq
    /// </summary>
    public static void LogComplexObject(ILogger logger, object dto)
    {
        // ? GOOD - @ destructures the object, showing all properties in Seq
        logger.LogInformation("Processing DTO: {@Dto}", dto);
        
        // Without @, it just calls ToString()
        logger.LogInformation("Processing DTO: {Dto}", dto);
    }

    /// <summary>
    /// Example: Logging with exceptions
    /// </summary>
    public static void LogException(ILogger logger, Exception ex, string operationId)
    {
        // ? GOOD - Exception is properly captured with stack trace
        logger.LogError(ex, "Operation {OperationId} failed", operationId);
        
        // ? BAD - Exception details are lost
        // logger.LogError("Operation {OperationId} failed: {Error}", operationId, ex.Message);
    }

    /// <summary>
    /// Example: Using log scopes for correlation
    /// </summary>
    public static async Task LogWithScope(ILogger logger, Guid operationId, Func<Task> operation)
    {
        // All logs within this scope will have OperationId property
        using (logger.BeginScope("OperationId: {OperationId}", operationId))
        {
            logger.LogInformation("Starting operation");
            
            try
            {
                await operation();
                logger.LogInformation("Operation completed successfully");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Operation failed");
                throw;
            }
        }
    }

    /// <summary>
    /// Example: Different log levels
    /// </summary>
    public static void LogLevelsExample(ILogger logger, string userId)
    {
        // Trace - Very detailed (usually disabled in production)
        logger.LogTrace("Entering GetUser method with {UserId}", userId);

        // Debug - Diagnostic information
        logger.LogDebug("Fetching user {UserId} from database", userId);

        // Information - Normal application flow
        logger.LogInformation("User {UserId} logged in successfully", userId);

        // Warning - Unexpected but handled situation
        logger.LogWarning("User {UserId} attempted to access restricted resource", userId);

        // Error - Recoverable error
        logger.LogError("Failed to load profile for user {UserId}", userId);

        // Critical - Unrecoverable error
        logger.LogCritical("Database connection lost");
    }

    /// <summary>
    /// Example: Performance logging
    /// </summary>
    public static async Task<T> LogPerformance<T>(
        ILogger logger, 
        string operationName, 
        Func<Task<T>> operation)
    {
        var sw = System.Diagnostics.Stopwatch.StartNew();
        
        try
        {
            var result = await operation();
            sw.Stop();
            
            logger.LogInformation(
                "Operation {OperationName} completed in {ElapsedMs}ms", 
                operationName, 
                sw.ElapsedMilliseconds);
            
            return result;
        }
        catch (Exception ex)
        {
            sw.Stop();
            logger.LogError(
                ex, 
                "Operation {OperationName} failed after {ElapsedMs}ms", 
                operationName, 
                sw.ElapsedMilliseconds);
            throw;
        }
    }
}

/// <summary>
/// Common Seq queries for your logs
/// </summary>
public static class SeqQueryExamples
{
    /*
     * SEQ QUERY EXAMPLES:
     * 
     * 1. Find all errors:
     *    @Level = 'Error'
     * 
     * 2. Find logs for specific user:
     *    UserId = '123e4567-e89b-12d3-a456-426614174000'
     * 
     * 3. Find slow operations:
     *    ElapsedMs > 1000
     * 
     * 4. Find cart creation events:
     *    @MessageTemplate like '%Cart%' and @Level = 'Information'
     * 
     * 5. Find errors in last hour:
     *    @Level = 'Error' and @Timestamp > DateTime('now') - TimeSpan('01:00:00')
     * 
     * 6. Combine multiple conditions:
     *    UserId = '...' and @Level = 'Error' and @Timestamp > DateTime('2024-01-01')
     * 
     * 7. Find by operation ID (when using scopes):
     *    OperationId = 'guid-here'
     * 
     * 8. Group by error type:
     *    @Level = 'Error' | group count() by @Exception
     * 
     * 9. Find integration event failures:
     *    @MessageTemplate like '%IntegrationEvent%' and @Level = 'Error'
     * 
     * 10. Performance analysis:
     *     ElapsedMs > 500 | project @Timestamp, OperationName, ElapsedMs
     */
}
