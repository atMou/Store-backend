namespace Shared.Application.Features.Product.Events;

public record SubscriptionToProductIntegrationEvent(
    Guid UserId,
    string ProductId,
    string ColorCode,
    string SizeCode,
    bool IsSubscribing
) : IntegrationEvent;
