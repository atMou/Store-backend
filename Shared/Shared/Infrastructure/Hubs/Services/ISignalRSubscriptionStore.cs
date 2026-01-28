namespace Shared.Infrastructure.Hubs.Services;

public interface ISignalRSubscriptionStore
{
    Task SubscribeAsync(string userId, string groupName, CancellationToken cancellationToken = default);
    Task UnsubscribeAsync(string userId, string groupName, CancellationToken cancellationToken = default);
    Task<IEnumerable<string>> GetUserSubscriptionsAsync(string userId, CancellationToken cancellationToken = default);
    Task ClearUserSubscriptionsAsync(string userId, CancellationToken cancellationToken = default);

    Task<IEnumerable<string>> GetSubscribedUserIdsAsync(string groupName, CancellationToken cancellationToken = default);
}
