using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

namespace Shared.Infrastructure.Authentication;

public class PermissionRequirementHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        var userPermissions = context.User.FindAll(Claims.Permissions)
            .Select(c => c.Value);

        var rolePermissions = context.User.FindAll(ClaimTypes.Role)
            .SelectMany(c => Role.FromUnsafe(c.Value).Permissions).Select(p => p.Name);

        var hasPermission = requirement.Permissions.Any(p => userPermissions.Contains(p));
        var hasRolePermission = requirement.Permissions.Any(p => rolePermissions.Contains(p));

        if (hasPermission || hasRolePermission) context.Succeed(requirement);

        return Task.CompletedTask;
    }
}