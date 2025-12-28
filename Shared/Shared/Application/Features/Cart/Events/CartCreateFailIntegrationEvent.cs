namespace Shared.Application.Features.Cart.Events;

public record CartCreateFailIntegrationEvent(Guid UserId, string Error)
{
}