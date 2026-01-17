namespace Order.Domain.Contracts;

public record UpdateOrderDto
{
    public string? Status { get; init; }
    public DateTime? StatusDate { get; init; }
    public Guid? PaymentId { get; init; }
    public Guid? ShipmentId { get; init; }
}
