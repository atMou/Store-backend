using Identity.Application.Features.ResendVerification;

using Role = Shared.Infrastructure.Enums.Role;

namespace Identity.Presentation.Controllers;

[ApiController]
[Route("[controller]")]
public class AuthController(ISender sender) : ControllerBase
{
    [HttpPost("register")]
    public async Task<ActionResult<RegisterResult>> Register([FromForm] RegisterUserRequest request)
    {
        var result = await sender.Send(request.ToCommand());
        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }


    [HttpPost]
    [Route("login")]
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginRequest request)
    {
        var result = (await sender.Send(new LoginCommand(request.Email, request.Password)))
            .Map(res =>
            {
                this.AddRefreshTokenToCookies(res.RefreshToken.RawToken, res.RefreshToken.ExpiresAt);
                return new LoginResponse(res.User, res.AccessToken);
            });
        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPost]
    [Route("logout")]
    public async Task<ActionResult<Unit>> Logout([FromBody] LogoutRequest request)
    {
        var result = (await sender.Send(new LogoutCommand(request.Email)))
            .Map(res =>
            {
                this.RemoveRefreshTokenFromCookies();
                return res;
            });

        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
    }

    [HttpPost]
    [Route("verify")]
    public async Task<ActionResult<EmailVerificationResponse>> Verify([FromBody] VerificationRequest request)
    {
        var result = (await sender.Send(new EmailVerificationCommand(request.Email, request.Code, request.Token)))
            .Map(res =>
            {
                if (request.RememberMe.HasValue && request.RememberMe.Value)
                {
                    this.AddRefreshTokenToCookies(res.RefreshToken.RawToken, res.RefreshToken.ExpiresAt);
                }

                return new EmailVerificationResponse(res.User, res.AccessToken);
            });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }

    [HttpPost]
    [Route("resend-code")]
    public async Task<ActionResult<Unit>> Resend([FromBody] ResendVerificationRequest request)
    {
        var result = await sender.Send(new ResendVerificationCommand(request.Email));

        return result.ToActionResult(_ => Ok(), HttpContext.Request.Path);
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
        var result = await sender.Send(new DeletePermissionCommand
        {
            UserId = UserId.From(request.UserId),
            Permissions = request.Permissions
        });

        return result.ToActionResult(res => Ok(res), HttpContext.Request.Path);
    }
}