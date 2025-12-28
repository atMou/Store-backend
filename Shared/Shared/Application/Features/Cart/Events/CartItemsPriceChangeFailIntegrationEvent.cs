namespace Shared.Application.Features.Cart.Events;

public record CartItemsPriceChangeFailIntegrationEvent(Guid ProductId, string ErrorMessage) : IntegrationEvent
{
}
