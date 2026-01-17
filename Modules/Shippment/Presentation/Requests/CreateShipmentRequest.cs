using Shipment.Application.Features.CreateShipment;
using Shared.Domain.ValueObjects;

namespace Shipment.Presentation.Requests;

public record CreateShipmentRequest
{
    public Guid OrderId { get; init; }
    public string Street { get; init; }
    public string City { get; init; }
    public uint PostalCode { get; init; }
    public uint HouseNumber { get; init; }
    public string? ExtraDetails { get; init; }

    public CreateShipmentCommand ToCommand()
    {
        return new CreateShipmentCommand
        {
            OrderId = Shared.Domain.ValueObjects.OrderId.From(this.OrderId),
            Street = Street,
            City = City,
            PostalCode = PostalCode,
            HouseNumber = HouseNumber,
            ExtraDetails = ExtraDetails
        };
    }
}
