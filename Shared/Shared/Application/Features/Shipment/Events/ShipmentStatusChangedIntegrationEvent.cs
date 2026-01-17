namespace Shared.Application.Features.Shipment.Events;

public record ShipmentStatusChangedIntegrationEvent(
    Guid ShipmentId,
    Guid OrderId,
    string Status,
    DateTime? StatusChangedAt) : IntegrationEvent
{
}
