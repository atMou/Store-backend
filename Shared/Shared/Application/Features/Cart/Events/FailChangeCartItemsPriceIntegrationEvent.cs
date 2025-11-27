using Shared.Application.Abstractions;

namespace Shared.Application.Features.Cart.Events;

public record FailedChangeCartItemsPriceIntegrationEvent(ProductId productId, Error e) : IntegrationEvent
{
}
