namespace Shared.Application.Features.Order.Events;

public record OrderStatusChangedIntegrationEvent(
    Guid OrderId,
    string Status,
    DateTime StatusChangedAt) : IntegrationEvent
{
}
