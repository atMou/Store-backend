namespace Shared.Application.Features.Cart.Events;

public record CartCreatedIntegrationEvent(Guid UserId, Guid CartId) : IntegrationEvent
{

}
