namespace Shared.Application.Features.Shipment.Events;

public record ShipmentDeliveredIntegrationEvent(
    Guid ShipmentId,
    Guid OrderId,
    DateTime DeliveredAt) : IntegrationEvent
{
}
