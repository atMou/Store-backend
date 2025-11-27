using Shared.Application.Abstractions;

namespace Shared.Application.Features.Cart.Events;

public record CartCreatedIntegrationEvent(Guid CartId, Guid UserId) : IntegrationEvent
{

}
