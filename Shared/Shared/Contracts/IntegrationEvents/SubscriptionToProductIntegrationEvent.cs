namespace Shared.Contracts.IntegrationEvents;

public record SubscriptionToProductIntegrationEvent(
    Guid UserId,
    string ProductId,
    string ColorCode,
    string SizeCode,
    bool IsSubscribing
) : IntegrationEvent;
