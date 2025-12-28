using Identity.Application.Features.GetUser;

namespace Identity.Presentation.Controllers;
[ApiController]
[Route("[controller]")]
public class UsersController(ISender sender) : ControllerBase
{


    [HttpGet("profile")]
    public async Task<ActionResult<UserResult>> Get([FromBody] Guid userId)
    {
        var result = await sender.Send(new GetUserByIdQuery(UserId.From(userId)));
        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserResult>> Get()
    {
        var result = await sender.Send(new GetLoggedInUserQuery());
        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }


    [HttpGet("check-user-email")]
    public async Task<ActionResult<Unit>> CheckUserEmail([FromQuery] string email)
    {
        var result = await sender.Send(new GetUserByEmailQuery()
        {
            Email = email
        });
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }



    [HttpPut("update/{id}")]
    public async Task<ActionResult<Unit>> Update([FromRoute] Guid id, [FromForm] UpdateUserRequest request)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }



    [HttpPost]
    [Route("add-phone")]
    public async Task<ActionResult<Unit>> AddPhone([FromBody] AddPhoneRequest request)
    {
        var result = (await sender.Send(new AddPhoneNumberCommand
        {
            UserId = UserId.From(request.UserId),
            PhoneNumber = request.PhoneNumber
        }));

        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }
    [Authorize]
    [HttpPost]
    [Route("toggle-liked-products")]
    public async Task<ActionResult<Unit>> ToggleLike([FromBody] ToggleLikeRequest request)
    {
        var result = await sender.Send(request.ToCommand());

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }


}

