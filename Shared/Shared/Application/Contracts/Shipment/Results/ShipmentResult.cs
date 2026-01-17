namespace Shared.Application.Contracts.Shipment.Results;

public record ShipmentResult
{
    public Guid Id { get; init; }
    public Guid OrderId { get; init; }
    public string Status { get; init; }
    public AddressResult ShippingAddress { get; init; }
    public string TrackingCode { get; init; }
    public DateTime? ShippedAt { get; init; }
    public DateTime? DeliveredAt { get; init; }
    public DateTime CreatedAt { get; init; }
}

public record AddressResult
{
    public string Street { get; init; }
    public string City { get; init; }
    public uint PostalCode { get; init; }
    public uint HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }
}
