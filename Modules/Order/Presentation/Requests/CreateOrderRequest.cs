namespace Order.Presentation.Requests;

public record CreateOrderRequest
{
    public Guid UserId { get; init; }
    public decimal Subtotal { get; init; }
    public decimal Total { get; init; }
    public decimal Tax { get; init; }
    public decimal Discount { get; init; }
    public IEnumerable<CreateOrderItemRequest> OrderItems { get; init; }
}