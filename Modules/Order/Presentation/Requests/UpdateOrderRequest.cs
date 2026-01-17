using Order.Application.Features.UpdateOrder;
using Shared.Domain.ValueObjects;

namespace Order.Presentation.Requests;

public record UpdateOrderRequest
{
    public Guid OrderId { get; init; }
    public string? Status { get; init; } // "paid", "shipped", "delivered", "cancelled", "refunded", "completed"
    public DateTime? StatusDate { get; init; }
    public Guid? PaymentId { get; init; }
    public Guid? ShipmentId { get; init; }

    public UpdateOrderCommand ToCommand()
    {
        return new UpdateOrderCommand
        {
            OrderId = Shared.Domain.ValueObjects.OrderId.From(this.OrderId),
            Status = Status,
            StatusDate = StatusDate,
            PaymentId = PaymentId,
            ShipmentId = ShipmentId
        };
    }
}
