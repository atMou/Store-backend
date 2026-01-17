namespace Shipment.Domain.Contracts;

public record UpdateShipmentDto
{
    public Address? Address { get; init; }
    public string? TrackingCode { get; init; }
    public string? Status { get; init; } // "shipped", "delivered", "onhold", "cancelled"
    public DateTime? StatusDate { get; init; }
}
