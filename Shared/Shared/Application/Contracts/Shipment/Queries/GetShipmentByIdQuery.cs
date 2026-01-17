using Shared.Application.Contracts.Shipment.Results;

namespace Shared.Application.Contracts.Shipment.Queries;

public record GetShipmentByIdQuery : IQuery<Fin<ShipmentResult>>
{
    public ShipmentId ShipmentId { get; init; }
}
