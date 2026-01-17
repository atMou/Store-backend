using Shared.Application.Contracts.Shipment.Queries;
using Shared.Application.Contracts.Shipment.Results;
using Shared.Domain.Errors;

using Shipment.Persistence;

namespace Shipment.Application.Features.GetShipmentByOrderId;

internal class GetShipmentByOrderIdQueryHandler(ShipmentDbContext dbContext)
    : IQueryHandler<GetShipmentByOrderIdQuery, Fin<ShipmentResult>>
{
    public async Task<Fin<ShipmentResult>> Handle(
        GetShipmentByOrderIdQuery query,
        CancellationToken cancellationToken)
    {
        var db = GetEntity<ShipmentDbContext, Domain.Models.Shipment>(
            shipment => shipment.OrderId == query.OrderId,
            NotFoundError.New($"Shipment for Order ID {query.OrderId.Value} not found."),
            null
        ).Map(s => s.ToResult());

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
