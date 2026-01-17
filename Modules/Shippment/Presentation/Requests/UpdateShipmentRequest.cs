using Shipment.Application.Features.UpdateShipment;
using Shared.Domain.ValueObjects;

namespace Shipment.Presentation.Requests;

public record UpdateShipmentRequest
{
    public Guid ShipmentId { get; init; }
    public string? Street { get; init; }
    public string? City { get; init; }
    public uint? PostalCode { get; init; }
    public uint? HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }
    public string? TrackingCode { get; init; }
    public string? Status { get; init; } // "shipped", "delivered", "onhold", "cancelled"
    public DateTime? StatusDate { get; init; }

    public UpdateShipmentCommand ToCommand()
    {
        return new UpdateShipmentCommand
        {
            ShipmentId = Shared.Domain.ValueObjects.ShipmentId.From(this.ShipmentId),
            Street = Street,
            City = City,
            PostalCode = PostalCode,
            HouseNumber = HouseNumber,
            ExtraDetails = ExtraDetails,
            TrackingCode = TrackingCode,
            Status = Status,
            StatusDate = StatusDate
        };
    }
}
