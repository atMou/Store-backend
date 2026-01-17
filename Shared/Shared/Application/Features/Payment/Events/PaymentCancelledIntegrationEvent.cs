using Shared.Application.Abstractions;
using Shared.Application.Contracts.Order.Dtos;

namespace Shared.Application.Features.Payment.Events;

public record PaymentCancelledIntegrationEvent : IntegrationEvent
{
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid CartId { get; init; }
    public DateTime CancelledAt { get; init; }
    public string? Reason { get; init; }
    public IEnumerable<OrderItemDto> OrderItems { get; init; } = [];
}
