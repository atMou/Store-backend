namespace Shared.Application.Features.Cart.Events;

public record DeliveryAddressChangedIntegrationEvent : IntegrationEvent
{
    public Guid CartId { get; init; }
    public Address Address { get; init; }
}
