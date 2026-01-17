using Shared.Application.Contracts.Order.Dtos;

namespace Shared.Application.Features.Payment.Events;

public record PaymentFulfilledIntegrationEvent : IntegrationEvent
{
    public Guid PaymentId { get; init; }
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public Guid CartId { get; init; }
    public string PaymentTransactionId { get; init; } = null!;
    public DateTime? PaymentPaidAt { get; init; }
    public IEnumerable<OrderItemDto> OrderItems { get; init; } = [];
}
