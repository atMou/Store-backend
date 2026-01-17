using Microsoft.Extensions.Logging;

using StackExchange.Redis;

namespace Shared.Infrastructure.Hubs.Services;

public class RedisSignalRSubscriptionStore : ISignalRSubscriptionStore
{
    private readonly IConnectionMultiplexer _redis;
    private readonly ILogger<RedisSignalRSubscriptionStore> _logger;
    private readonly TimeSpan _subscriptionExpiry = TimeSpan.FromDays(30);

    public RedisSignalRSubscriptionStore(IConnectionMultiplexer redis, ILogger<RedisSignalRSubscriptionStore> logger)
    {
        _redis = redis;
        _logger = logger;
    }

    public async Task SubscribeAsync(string userId, string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = GetRedisKey(userId);

            // Use FireAndForget for expiry update to improve performance
            var added = await db.SetAddAsync(key, groupName);
            _ = db.KeyExpireAsync(key, _subscriptionExpiry, flags: CommandFlags.FireAndForget);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Subscription added - UserId: {UserId}, Group: {GroupName}, IsNew: {IsNew}",
                    userId, groupName, added);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to add subscription - UserId: {UserId}, Group: {GroupName}",
                userId, groupName);
            throw;
        }
    }

    public async Task UnsubscribeAsync(string userId, string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = GetRedisKey(userId);

            await db.SetRemoveAsync(key, groupName);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Subscription removed - UserId: {UserId}, Group: {GroupName}", userId, groupName);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to remove subscription - UserId: {UserId}, Group: {GroupName}",
                userId, groupName);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetUserSubscriptionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = GetRedisKey(userId);

            var values = await db.SetMembersAsync(key);
            var subscriptions = values.Select(v => v.ToString()).ToList();

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Retrieved {Count} subscriptions for UserId: {UserId}", subscriptions.Count, userId);
            }

            return subscriptions;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to retrieve subscriptions - UserId: {UserId}", userId);
            return [];
        }
    }

    public async Task ClearUserSubscriptionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = _redis.GetDatabase();
            var key = GetRedisKey(userId);

            await db.KeyDeleteAsync(key);

            if (_logger.IsEnabled(LogLevel.Debug))
            {
                _logger.LogDebug("Cleared subscriptions for UserId: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to clear subscriptions - UserId: {UserId}", userId);
            throw;
        }
    }

    private static string GetRedisKey(string userId) => $"signalr:subscriptions:{userId}";
}
