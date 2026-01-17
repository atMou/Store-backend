using Shared.Application.Contracts.Shipment.Queries;
using Shared.Application.Contracts.Shipment.Results;

using Shipment.Persistence;

namespace Shipment.Application.Features.GetShipmentById;

internal class GetShipmentByIdQueryHandler(ShipmentDbContext dbContext)
    : IQueryHandler<GetShipmentByIdQuery, Fin<ShipmentResult>>
{
    public async Task<Fin<ShipmentResult>> Handle(
        GetShipmentByIdQuery query,
        CancellationToken cancellationToken)
    {

        var db = GetEntity<ShipmentDbContext, Domain.Models.Shipment>(
            shipment => shipment.Id == query.ShipmentId,
            NotFoundError.New($"Shipment with ID {query.ShipmentId.Value} not found."),
            null

        ).Map(s => s.ToResult());

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
