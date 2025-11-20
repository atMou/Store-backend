using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

namespace Shared.Infrastructure.Authentication;

public class RoleHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {

        var roles = context.User.FindAll(ClaimTypes.Role)
            .Select(c => c.Value);

        var hasRole = requirement.Permissions.Any(p => roles.Contains(p));

        if (hasRole) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}