using Shared.Application.Contracts.Carts.Dtos;

namespace Shared.Application.Features.Cart.Events;

public record CartCheckedOutIntegrationEvent : IntegrationEvent
{
    public CartDto CartDto { get; init; }

}

