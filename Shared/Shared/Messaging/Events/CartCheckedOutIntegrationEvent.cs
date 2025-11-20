using Shared.Domain.Contracts.Cart;
using Shared.Messaging.Abstractions;

namespace Shared.Messaging.Events;

public record CartCheckedOutIntegrationEvent : IntegrationEvent
{
    public Guid CartId { get; init; }
    public Guid UserId { get; init; }
    public decimal Total { get; init; }
    public decimal TotalSub { get; init; }
    public decimal Tax { get; init; }
    public decimal Discount { get; init; }
    public decimal TotalDiscounted { get; init; }
    public List<Guid> CouponIds { get; init; }
    public List<LineItemDto> LineItems { get; init; }
}

