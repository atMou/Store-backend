using Shared.Messaging.Abstractions;

namespace Shared.Messaging.Events;

public record FailedChangeCartItemsPriceIntegrationEvent(ProductId productId, Error e) : IntegrationEvent
{
}
