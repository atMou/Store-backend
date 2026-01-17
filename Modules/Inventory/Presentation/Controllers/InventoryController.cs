using Inventory.Application.Features.GetInventories;
using Inventory.Application.Features.GetWarehouseLocations;
using Inventory.Application.Features.UpdateInventory;

using MediatR;

using Microsoft.AspNetCore.Mvc;

using Shared.Application.Contracts;
using Shared.Application.Contracts.Inventory.Queries;
using Shared.Application.Contracts.Inventory.Results;
using Shared.Presentation.Extensions;

namespace Inventory.Presentation.Controllers;
[ApiController]
[Route("[controller]")]
public class InventoriesController(ISender sender) : ControllerBase
{

    //[HttpPost]
    //public async Task<ActionResult<Unit>> Create([FromBody] CreateStockRequest request)
    //{
    //	var result = await sender.Send(request.ToCommand());

    //	return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    //}

    [HttpGet]
    public async Task<ActionResult<PaginatedResult<InventoryResult>>> GetAll([FromQuery] GetInventoriesQuery query)
    {
        var result = await sender.Send(query);

        return result.ToActionResult(data => Ok(data), HttpContext.Request.Path);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<InventoryResult>> GetByVariantId(Guid id)
    {
        var query = new GetInventoryByColorVariantIdQuery
        {
            InventoryId = InventoryId.From(id)
        };
        var result = await sender.Send(query);

        return result.ToActionResult(data => Ok(data), HttpContext.Request.Path);
    }

    [HttpGet("warehouses")]
    public async Task<ActionResult<IEnumerable<WarehouseResult>>> GetWarehouses()
    {
        var query = new GetWarehouseLocationsQuery();
        var result = await sender.Send(query);

        return result.ToActionResult(data => Ok(data), HttpContext.Request.Path);
    }

    //[HttpDelete("{colorVariantId:guid}")]
    //public async Task<ActionResult<Unit>> Delete(Guid variantId)
    //{
    //    var request = new DeleteStockRequest { VariantId = variantId };
    //    var result = await sender.Send(request.ToCommand());

    //    return result.ToActionResult(_ => NoContent(), HttpContext.Request.Path);
    //}

    [HttpPut]
    public async Task<ActionResult<Unit>> Update([FromBody] UpdateInventoryCommand request)
    {
        var result = await sender.Send(request);

        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }
}