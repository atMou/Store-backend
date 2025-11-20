namespace Order.Application.Features.CreateOrder;

public record CreateOrderResult
{
    public Guid OrderId { get; init; }
    public IEnumerable<CreateOrderItemResult> OrderItems { get; init; }
}