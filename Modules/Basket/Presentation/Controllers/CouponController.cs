using Shared.Application.Contracts;
using Shared.Application.Contracts.Carts.Results;

namespace Basket.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class CouponsController(ISender sender) : ControllerBase

{
	[HttpGet]
	[Route("{id}")]
	public async Task<ActionResult<CouponResult>> Get([FromRoute] Guid id)
	{
		var result = await sender.Send(new GetCouponByIdCommand(CouponId.From(id)));

		return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
	}

	[HttpGet]
	[Route("user/{id}")]
	public async Task<ActionResult<PaginatedResult<CouponResult>>> GetByUserId([FromRoute] Guid id, [FromQuery] GetCouponsByUserIdRequest request)
	{
		var result = await sender.Send(new GetCouponsByUserIdQuery(UserId.From(id), request.PageNumber, request.PageSize));

		return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
	}

	[HttpPost("new")]
	[HasPermission(Permission.CreateCoupon)]
	public async Task<ActionResult<Unit>> Create([FromBody] CreateCouponRequest request)
	{
		var result = await sender.Send(request.ToCommand());
		return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
	}

	[HttpPost("assign")]
	public async Task<ActionResult<Unit>> Assign([FromBody] AssignCouponToUserRequest request)
	{
		var result = await sender.Send(request.ToCommand());
		return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
	}

	[HttpPost("expire")]
	public async Task<ActionResult<Unit>> Expire([FromBody] ExpireCouponRequest request)
	{
		var result = await sender.Send(request.ToCommand());
		return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
	}


	[HttpDelete]
	public async Task<ActionResult<Unit>> Delete([FromBody] DeleteCouponRequest request)
	{
		var result = await sender.Send(request.ToCommand());
		return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
	}




}