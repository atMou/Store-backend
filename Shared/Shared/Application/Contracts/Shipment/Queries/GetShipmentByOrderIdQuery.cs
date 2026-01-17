using Shared.Application.Contracts.Shipment.Results;

namespace Shared.Application.Contracts.Shipment.Queries;

public record GetShipmentByOrderIdQuery : IQuery<Fin<ShipmentResult>>
{
    public OrderId OrderId { get; init; }
}
