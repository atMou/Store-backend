using Order.Application.Features.CreateOrder;

namespace Shared.Domain.Contracts.Order;

public record OrderResult
{
    public Guid OrderId { get; init; }
    public Guid UserId { get; init; }
    public string Email { get; init; }
    public string? Phone { get; set; }
    public Guid? ShipmentId { get; init; }
    public Guid? PaymentId { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Tax { get; init; }
    public decimal Total { get; init; }
    public decimal Discount { get; init; }
    public string TrackingCode { get; init; }
    public string OrderStatus { get; set; }
    public IEnumerable<Guid> CouponIds { get; init; }

    public string Notes { get; init; }

    public IEnumerable<OrderItemResult> OrderItems { get; init; }
}