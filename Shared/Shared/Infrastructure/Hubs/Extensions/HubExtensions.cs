using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.Logging;

using Shared.Infrastructure.Hubs.Services;

using StackExchange.Redis;

namespace Shared.Infrastructure.Hubs.Extensions;

public static class HubExtensions
{
    public static IServiceCollection AddSharedHubs(this IServiceCollection services, IConfiguration configuration)
    {
        var redisConnectionString = configuration.GetConnectionString("Redis");

        if (string.IsNullOrEmpty(redisConnectionString))
        {
            throw new InvalidOperationException(
                "Redis connection string is required for SignalR. " +
                "Please add 'Redis' connection string to appsettings.json or appsettings.Development.json");
        }

        services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var logger = sp.GetRequiredService<ILogger<IConnectionMultiplexer>>();

            var configOptions = ConfigurationOptions.Parse(redisConnectionString);

            // Optimize timeouts - balance between responsiveness and reliability
            configOptions.AbortOnConnectFail = false;
            configOptions.ConnectTimeout = 3000;      // 3 seconds to connect (increased)
            configOptions.SyncTimeout = 2000;         // 2 seconds for sync operations (increased)
            configOptions.AsyncTimeout = 2000;        // 2 seconds for async operations (increased)
            configOptions.ConnectRetry = 3;           // Retry 3 times
            configOptions.KeepAlive = 60;             // Keep connection alive
            configOptions.ReconnectRetryPolicy = new ExponentialRetry(1000);

            // Create connection
            var connection = ConnectionMultiplexer.Connect(configOptions);

            // Monitor connection events
            connection.ConnectionFailed += (_, e) =>
            {
                logger.LogError("Redis connection failed: {Exception}", e.Exception?.Message ?? "Unknown");
            };

            connection.ConnectionRestored += (_, _) =>
            {
                logger.LogInformation("Redis connection restored");
            };

            connection.ErrorMessage += (_, e) =>
            {
                logger.LogWarning("Redis error: {Message}", e.Message);
            };

            logger.LogInformation("Redis connection established to {Endpoints}",
                string.Join(", ", connection.GetEndPoints().Select(ep => ep.ToString())));

            return connection;
        });

        services.AddSignalR(options =>
        {
            options.EnableDetailedErrors = true;
            options.MaximumReceiveMessageSize = 102400;
            options.StreamBufferCapacity = 10;
            options.KeepAliveInterval = TimeSpan.FromSeconds(10);
            options.ClientTimeoutInterval = TimeSpan.FromSeconds(30);
            options.HandshakeTimeout = TimeSpan.FromSeconds(5);
            options.MaximumParallelInvocationsPerClient = 10;
        })
        .AddStackExchangeRedis(redisConnectionString, options =>
        {
            options.Configuration.ChannelPrefix = RedisChannel.Literal("signalr");
            options.Configuration.AbortOnConnectFail = false;
            options.Configuration.ConnectTimeout = 3000;
            options.Configuration.SyncTimeout = 2000;
            options.Configuration.AsyncTimeout = 2000;
        });

        services.AddSingleton<ISignalRSubscriptionStore, RedisSignalRSubscriptionStore>();
        services.AddScoped<INotificationService, NotificationService>();

        return services;
    }

    public static IEndpointRouteBuilder MapSharedHubs(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapHub<NotificationHub>("/hubs/notifications", options =>
        {
            options.Transports = Microsoft.AspNetCore.Http.Connections.HttpTransportType.WebSockets |
                                Microsoft.AspNetCore.Http.Connections.HttpTransportType.ServerSentEvents |
                                Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling;

            options.LongPolling.PollTimeout = TimeSpan.FromSeconds(30);
            options.WebSockets.CloseTimeout = TimeSpan.FromSeconds(5);
        });

        return endpoints;
    }
}
