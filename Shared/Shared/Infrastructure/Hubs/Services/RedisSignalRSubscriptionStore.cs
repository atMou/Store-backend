using Microsoft.Extensions.Logging;

using StackExchange.Redis;

namespace Shared.Infrastructure.Hubs.Services;

public class RedisSignalRSubscriptionStore(IConnectionMultiplexer redis, ILogger<RedisSignalRSubscriptionStore> logger)
    : ISignalRSubscriptionStore
{
    private readonly TimeSpan _subscriptionExpiry = TimeSpan.FromDays(30);

    public async Task SubscribeAsync(string userId, string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();
            var userKey = GetUserSubscriptionsKey(userId);
            var groupKey = GetGroupSubscribersKey(groupName);

            var added = await db.SetAddAsync(userKey, groupName);
            _ = db.KeyExpireAsync(userKey, _subscriptionExpiry, flags: CommandFlags.FireAndForget);

            await db.SetAddAsync(groupKey, userId);
            _ = db.KeyExpireAsync(groupKey, _subscriptionExpiry, flags: CommandFlags.FireAndForget);

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Subscription added - UserId: {UserId}, Group: {GroupName}, IsNew: {IsNew}",
                    userId, groupName, added);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to add subscription - UserId: {UserId}, Group: {GroupName}",
                userId, groupName);
            throw;
        }
    }

    public async Task UnsubscribeAsync(string userId, string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();
            var userKey = GetUserSubscriptionsKey(userId);
            var groupKey = GetGroupSubscribersKey(groupName);

            // Remove group from user's subscriptions
            await db.SetRemoveAsync(userKey, groupName);

            // Remove user from group's subscribers
            await db.SetRemoveAsync(groupKey, userId);

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Subscription removed - UserId: {UserId}, Group: {GroupName}", userId, groupName);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to remove subscription - UserId: {UserId}, Group: {GroupName}",
                userId, groupName);
            throw;
        }
    }

    public async Task<IEnumerable<string>> GetUserSubscriptionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();
            var key = GetUserSubscriptionsKey(userId);

            var values = await db.SetMembersAsync(key);
            var subscriptions = values.Select(v => v.ToString()).ToList();

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Retrieved {Count} subscriptions for UserId: {UserId}", subscriptions.Count, userId);
            }

            return subscriptions;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve subscriptions - UserId: {UserId}", userId);
            return [];
        }
    }

    public async Task<IEnumerable<string>> GetSubscribedUserIdsAsync(string groupName, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();
            var key = GetGroupSubscribersKey(groupName);

            var values = await db.SetMembersAsync(key);
            var userIds = values.Select(v => v.ToString()).ToList();

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Retrieved {Count} subscribers for Group: {GroupName}", userIds.Count, groupName);
            }

            return userIds;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to retrieve subscribers - Group: {GroupName}", groupName);
            return [];
        }
    }

    public async Task ClearUserSubscriptionsAsync(string userId, CancellationToken cancellationToken = default)
    {
        try
        {
            var db = redis.GetDatabase();

            var subscriptions = await GetUserSubscriptionsAsync(userId, cancellationToken);

            foreach (var groupName in subscriptions)
            {
                var groupKey = GetGroupSubscribersKey(groupName);
                await db.SetRemoveAsync(groupKey, userId);
            }

            var userKey = GetUserSubscriptionsKey(userId);
            await db.KeyDeleteAsync(userKey);

            if (logger.IsEnabled(LogLevel.Debug))
            {
                logger.LogDebug("Cleared subscriptions for UserId: {UserId}", userId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to clear subscriptions - UserId: {UserId}", userId);
            throw;
        }
    }

    private static string GetUserSubscriptionsKey(string userId) => $"signalr:subscriptions:user:{userId}";
    private static string GetGroupSubscribersKey(string groupName) => $"signalr:subscriptions:group:{groupName}";
}
