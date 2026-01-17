using MediatR;
using Microsoft.AspNetCore.Mvc;
using Shared.Application.Contracts.Shipment.Queries;
using Shared.Application.Contracts.Shipment.Results;
using Shared.Presentation.Extensions;
using Shipment.Presentation.Requests;

namespace Shipment.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class ShipmentsController(ISender sender) : ControllerBase
{
    [HttpPost]
    public async Task<ActionResult<Unit>> Create([FromBody] CreateShipmentRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpGet("{shipmentId:guid}")]
    public async Task<ActionResult<ShipmentResult>> GetById(Guid shipmentId)
    {
        var query = new GetShipmentByIdQuery
        {
            ShipmentId = ShipmentId.From(shipmentId)
        };
        var result = await sender.Send(query);
        return result.ToActionResult(data => Ok(data), HttpContext.Request.Path);
    }

    [HttpGet("order/{orderId:guid}")]
    public async Task<ActionResult<ShipmentResult>> GetByOrderId(Guid orderId)
    {
        var query = new GetShipmentByOrderIdQuery
        {
            OrderId = OrderId.From(orderId)
        };
        var result = await sender.Send(query);
        return result.ToActionResult(data => Ok(data), HttpContext.Request.Path);
    }

    [HttpPut]
    public async Task<ActionResult<Unit>> Update([FromBody] UpdateShipmentRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }
}
