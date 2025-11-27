using Basket.Application.Features.Cart.DeleteCart;
using Basket.Application.Features.Cart.GetCart;

using Shared.Application.Contracts.Carts.Results;

namespace Basket.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class BasketsController(ISender sender) : ControllerBase

{
    [HttpGet("{id:guid}", Name = "GetCartById")]
    public async Task<ActionResult<CartResult>> GetCartById([FromRoute] Guid id, [FromQuery] string? include)
    {
        var result = await sender.Send(new GetCartByCartIdQuery()
        {
            CartId = CartId.From(id),
            Include = include
        });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPost]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateCartRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(id => CreatedAtRoute(nameof(GetCartById), new { id }), HttpContext.Request.Path);
    }

    [HttpPost("add-coupon")]
    public async Task<ActionResult<Unit>> AddCouponToCart([FromBody] AddCouponToCartRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpPost("remove-coupon")]
    public async Task<ActionResult<Unit>> DeleteCouponFromCart([FromBody] DeleteCouponFromCartRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpPost("checkout")]
    public async Task<ActionResult<Unit>> CheckOut([FromBody] CartCheckoutRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpDelete("{id}")]
    public async Task<ActionResult<Unit>> Delete([FromRoute] Guid id)
    {
        var result = await sender.Send(new DeleteCartCommand(CartId.From(id)));
        return result.ToActionResult(_ => NoContent(), HttpContext.Request.Path);
    }

    [HttpPost]
    [Route("add-line-item")]
    public async Task<ActionResult<Unit>> AddItem([FromBody] AddLineItemRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }


    [HttpPost]
    [Route("change-delivery-address")]
    public async Task<ActionResult<Unit>> ChangeDeliveryAddress([FromBody] ChangeDeliveryAddressRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }


}