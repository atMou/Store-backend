namespace Shared.Application.Contracts.Carts.Dtos;
public record CartDto
{
    public Guid CartId { get; init; }
    public Guid UserId { get; init; }
    public decimal Total { get; init; }
    public decimal TotalSub { get; init; }
    public decimal Tax { get; init; }
    public decimal Discount { get; init; }
    public decimal TotalAfterDiscounted { get; init; }
    public decimal ShipmentCost { get; init; }
    public IEnumerable<Guid> CouponIds { get; init; }
    public Address DeliveryAddress { get; init; }
    public IEnumerable<LineItemDto> LineItems { get; init; }
}
