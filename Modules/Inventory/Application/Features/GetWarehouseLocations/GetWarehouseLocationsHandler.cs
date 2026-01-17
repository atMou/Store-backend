using Shared.Application.Contracts.Inventory.Results;

namespace Inventory.Application.Features.GetWarehouseLocations;

public record GetWarehouseLocationsQuery : IQuery<Fin<IEnumerable<WarehouseResult>>>;

internal class GetWarehouseLocationsQueryHandler
    : IQueryHandler<GetWarehouseLocationsQuery, Fin<IEnumerable<WarehouseResult>>>
{
    public Task<Fin<IEnumerable<WarehouseResult>>> Handle(
        GetWarehouseLocationsQuery query,
        CancellationToken cancellationToken)
    {
        var warehouses = Warehouse.All
            .Where(w => w.Code != Shared.Domain.Enums.WarehouseCode.None)
            .Select(w => new WarehouseResult
            {
                Code = w.Code.ToString(),
                Name = w.Name,
                Address = w.Address,
                City = w.City,
                State = w.State,
                Country = w.Country,
                PostalCode = w.PostalCode,
                ContactPhone = w.ContactPhone,
                ContactEmail = w.ContactEmail
            });

        return Task.FromResult(FinSucc(warehouses));
    }
}
