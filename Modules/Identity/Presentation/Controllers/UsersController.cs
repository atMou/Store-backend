using Role = Shared.Infrastructure.Enums.Role;

namespace Identity.Presentation.Controllers;
[ApiController]
[Route("[controller]")]
public class UsersController(ISender sender) : ControllerBase
{


    [HttpGet]
    public async Task<ActionResult<UserResult>> Get([FromQuery] Guid userId)
    {
        var result = await sender.Send(new GetUserByIdQuery(UserId.From(userId)));

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPost]
    public async Task<ActionResult<RegisterCommandResult>> Create([FromBody] CreateUserRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPut("update/{id}")]
    public async Task<ActionResult<Unit>> Update([FromRoute] Guid id, [FromForm] UpdateUserRequest request)
    {
        var result = await sender.Send(request.ToCommand(id));
        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }


    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = (await sender.Send(new LoginCommand(request.Email, request.Password)))
            .Map(res =>
            {
                this.AddRefreshTokenToCookies(res.RefreshToken.RawToken, res.RefreshToken.ExpiresAt);
                return new LoginResponse(res.AccessToken);
            });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPost]
    [Route("logout")]
    public async Task<ActionResult<LogoutResponse>> Logout([FromBody] LogoutRequest request)
    {
        var result = (await sender.Send(new LogoutCommand(request.Email)))
            .Map(res =>
            {
                this.RemoveRefreshTokenFromCookies();
                return new LogoutResponse();
            });

        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpGet]
    [Route("verify")]
    public async Task<ActionResult<EmailVerificationResponse>> Verify([FromQuery] VerificationRequest request)
    {
        var result = (await sender.Send(new EmailVerificationCommand(request.email, request.token))).Map(res =>
        {
            this.AddRefreshTokenToCookies(res.RefreshToken.RawToken, res.RefreshToken.ExpiresAt);
            return new EmailVerificationResponse(res.AccessToken);
        });


        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }


    [HttpPost]
    [Route("refresh")]
    public async Task<ActionResult<RefreshTokenResponse>> Refresh([FromQuery] RefreshTokenRequest request)
    {
        var refreshToken = this.GetRefreshTokenFromCookies();
        var result = (await sender.Send(new RefreshTokenCommand(refreshToken, request.Email))).Map(res =>
        {
            this.AddRefreshTokenToCookies(res.RefreshToken.RawToken, res.RefreshToken.ExpiresAt);
            return new RefreshTokenResponse(res.AccessToken);
        });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
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

    [HttpPost]
    [HasRole(Role.Admin)]
    [Route("assign-permission")]
    public async Task<ActionResult<Unit>> AssignPermission([FromBody] AssignPermissionRequest request)
    {
        var result = await sender.Send(new AssignPermissionCommand
        {
            UserId = UserId.From(request.UserId),
            Permissions = request.Permissions

        });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }
    [HttpPost]
    [HasRole(Role.Admin)]
    [Route("assign-role")]
    public async Task<ActionResult<Unit>> AssignRole([FromBody] AssignRoleRequest request)
    {
        var result = await sender.Send(new AssignRoleCommand
        {
            UserId = UserId.From(request.UserId),
            Role = request.Role

        });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpDelete]
    [HasRole(Role.Admin)]
    [Route("delete-role")]
    public async Task<ActionResult<Unit>> DeleteRole([FromBody] DeleteRoleRequest request)
    {
        var result = await sender.Send(new DeleteUserRoleCommand
        {
            UserId = UserId.From(request.UserId),
            Role = request.Role

        });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpDelete]
    [HasRole(Role.Admin)]
    [Route("delete-permission")]
    public async Task<ActionResult<Unit>> DeletePermission([FromBody] DeletePermissionRequest request)
    {
        var result = await sender.Send(new DeletePermissionCommand()
        {
            UserId = UserId.From(request.UserId),
            Permissions = request.Permissions

        });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }



}