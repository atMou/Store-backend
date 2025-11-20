using Basket.Application.Features.Cart.DeleteCart;
using Basket.Application.Features.Cart.GetCart;

namespace Basket.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketsController(ISender sender) : ControllerBase

{
    [HttpGet(Name = "GetCartById")]
    [Route("{id:guid}")]
    public async Task<ActionResult<CartDto>> GetCartById([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetCartByCartIdQuery(CartId.From(id)));

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCartRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(id => CreatedAtRoute(nameof(GetCartById), new { id }), HttpContext.Request.Path);
    }

    [HttpDelete]
    public async Task<ActionResult<Unit>> Delete([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteCartCommand(CartId.From(id)));
        return result.ToActionResult(_ => NoContent(), HttpContext.Request.Path);
    }

    [HttpPost]
    [Route("addItem")]
    public async Task<ActionResult<Unit>> AddItem([FromBody] AddCartItemRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }


}

public record GetBasketByIdQuery(Guid Id);