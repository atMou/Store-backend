namespace Shared.Application.Features.Shipment.Events;

public record ShipmentCreatedIntegrationEvent(
    Guid ShipmentId,
    Guid OrderId,
    string TrackingCode) : IntegrationEvent
{
}
