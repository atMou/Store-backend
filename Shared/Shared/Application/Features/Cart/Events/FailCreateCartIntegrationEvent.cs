namespace Shared.Application.Features.Cart.Events;

public record FailCreateCartIntegrationEvent(Guid UserId, string[] Errors)
{
}