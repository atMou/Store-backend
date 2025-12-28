using System.Diagnostics;

using Microsoft.Extensions.Logging;

namespace Shared.Infrastructure.Logging;

public static class PerformanceLogging
{
    public static async Task<T> LogPerformance<T>(
        this ILogger logger,
        string operationName,
        Func<Task<T>> operation,
        EventId? eventId = null,
        Dictionary<string, object>? additionalProperties = null)
    {
        var sw = Stopwatch.StartNew();
        var logEventId = eventId ?? new EventId(0, operationName);

        using (logger.BeginScope(new Dictionary<string, object>
        {
            ["OperationName"] = operationName,
            ["StartTime"] = DateTime.UtcNow
        }))
        {
            logger.LogInformation(logEventId, "Starting operation {OperationName}", operationName);

            try
            {
                var result = await operation();
                sw.Stop();

                var props = new Dictionary<string, object>
                {
                    ["ElapsedMs"] = sw.ElapsedMilliseconds,
                    ["Success"] = true
                };

                if (additionalProperties != null)
                {
                    foreach (var kvp in additionalProperties)
                    {
                        props[kvp.Key] = kvp.Value;
                    }
                }

                using (logger.BeginScope(props))
                {
                    logger.LogInformation(
                        logEventId,
                        "Operation {OperationName} completed successfully in {ElapsedMs}ms",
                        operationName,
                        sw.ElapsedMilliseconds);
                }

                return result;
            }
            catch (Exception ex)
            {
                sw.Stop();

                var props = new Dictionary<string, object>
                {
                    ["ElapsedMs"] = sw.ElapsedMilliseconds,
                    ["Success"] = false
                };

                if (additionalProperties != null)
                {
                    foreach (var kvp in additionalProperties)
                    {
                        props[kvp.Key] = kvp.Value;
                    }
                }

                using (logger.BeginScope(props))
                {
                    logger.LogError(
                        logEventId,
                        ex,
                        "Operation {OperationName} failed after {ElapsedMs}ms",
                        operationName,
                        sw.ElapsedMilliseconds);
                }

                throw;
            }
        }
    }

    public static async Task LogPerformance(
        this ILogger logger,
        string operationName,
        Func<Task> operation,
        EventId? eventId = null,
        Dictionary<string, object>? additionalProperties = null)
    {
        await logger.LogPerformance(
            operationName,
            async () =>
            {
                await operation();
                return true;
            },
            eventId,
            additionalProperties);
    }
}
