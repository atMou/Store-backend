using Shared.Application.Contracts.Carts.Results;

namespace Basket.Application.Events;

internal record CartCheckedOutDomainEvent : IDomainEvent
{
    public Guid CartId { get; init; }
    public Guid UserId { get; init; }
    public decimal Total { get; init; }
    public decimal TotalSub { get; init; }
    public decimal Tax { get; init; }
    public decimal ShipmentCost { get; init; }
    public decimal Discount { get; init; }
    public decimal TotalDiscounted { get; init; }
    public List<Guid> CouponIds { get; init; }
    public List<LineItemResult> LineItems { get; init; }
}



