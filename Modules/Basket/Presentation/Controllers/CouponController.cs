
namespace Basket.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class CouponsController(ISender sender) : ControllerBase

{
    [HttpGet]
    [Route("{id}")]
    public async Task<ActionResult<GetCouponByIdResult>> Get([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetCouponByIdCommand(id));

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpGet]
    [Route("user/{id}")]
    public async Task<ActionResult<GetCouponByUserIdResult>> GetByUserId([FromRoute] Guid id)
    {
        var result = await sender.Send(new GetCouponByUserIdCommand(id));

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPost]
    [HasPermission(Permission.CreateCoupon)]
    public async Task<ActionResult<Unit>> Create([FromBody] CreateCouponRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpPost]
    [Route("assign")]
    public async Task<ActionResult<Unit>> Assign([FromBody] AssignCouponToUserRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpPost]
    [Route("expire")]
    public async Task<ActionResult<ExpireCouponResult>> Expire([FromBody] ExpireCouponRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }





}