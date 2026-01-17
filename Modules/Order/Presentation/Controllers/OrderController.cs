using Microsoft.AspNetCore.Mvc;

using Order.Application.Features.DeleteOrder;
using Order.Application.Features.GetOrderByCartId;
using Order.Application.Features.GetOrdersByUserId;
using Order.Presentation.Requests;

using Shared.Application.Contracts;
using Shared.Application.Contracts.Order.Queries;
using Shared.Application.Contracts.Order.Results;
using Shared.Presentation.Extensions;

namespace Order.Presentation.Controllers;


[ApiController]
[Route("[controller]")]
public class OrdersController(ISender sender) : ControllerBase
{
    [HttpGet("{orderId:guid}")]
    public async Task<ActionResult<OrderResult>> GetById(Guid orderId)
    {
        var query = new GetOrderByIdQuery { OrderId = OrderId.From(orderId) };
        var result = await sender.Send(query);
        return result.ToActionResult(data => Ok(data), HttpContext.Request.Path);
    }
    [HttpGet("{cartId:guid}/cart")]
    public async Task<ActionResult<OrderResult>> GetByCartId(Guid cartId)
    {
        var query = new GetOrderByCartIdQuery() { CartId = CartId.From(cartId) };
        var result = await sender.Send(query);
        return result.ToActionResult(data => Ok(data), HttpContext.Request.Path);
    }


    [HttpGet("user/{userId:guid}")]
    public async Task<ActionResult<PaginatedResult<OrderResult>>> GetByUserId(
        Guid userId,
        [FromQuery] int pageNumber = 1,
        [FromQuery] int pageSize = 20)
    {
        var query = new GetOrdersByUserIdQuery
        {
            UserId = UserId.From(userId),
            PageNumber = pageNumber,
            PageSize = pageSize
        };
        var result = await sender.Send(query);
        return result.ToActionResult(data => Ok(data), HttpContext.Request.Path);
    }

    [HttpPut]
    public async Task<ActionResult<Unit>> Update([FromBody] UpdateOrderRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpDelete("{orderId:guid}")]
    public async Task<ActionResult<Unit>> Delete(Guid orderId)
    {
        var command = new DeleteOrderCommand { OrderId = OrderId.From(orderId) };
        var result = await sender.Send(command);
        return result.ToActionResult(_ => NoContent(), HttpContext.Request.Path);
    }
}