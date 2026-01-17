namespace Shared.Infrastructure.Hubs.Services;

/// <summary>
/// Manages persistent SignalR subscriptions in Redis
/// </summary>
public interface ISignalRSubscriptionStore
{
    /// <summary>
    /// Store a user's subscription
    /// </summary>
    Task SubscribeAsync(string userId, string groupName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Remove a user's subscription
    /// </summary>
    Task UnsubscribeAsync(string userId, string groupName, CancellationToken cancellationToken = default);

    /// <summary>
    /// Get all subscriptions for a user
    /// </summary>
    Task<IEnumerable<string>> GetUserSubscriptionsAsync(string userId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Clear all subscriptions for a user
    /// </summary>
    Task ClearUserSubscriptionsAsync(string userId, CancellationToken cancellationToken = default);
}
