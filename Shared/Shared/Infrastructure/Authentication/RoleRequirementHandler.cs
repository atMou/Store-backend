using System.Security.Claims;

using Microsoft.AspNetCore.Authorization;

namespace Shared.Infrastructure.Authentication;

// The handler that checks if the current user has any of the allowed roles
public class RoleRequirementHandler : AuthorizationHandler<RoleRequirement>
{
	protected override Task HandleRequirementAsync(
		AuthorizationHandlerContext context,
		RoleRequirement requirement)
	{
		var userRoles = context.User.FindAll(ClaimTypes.Role).Select(c => c.Value);

		if (requirement.AllowedRoles.Any(r => userRoles.Contains(r)))
		{
			context.Succeed(requirement);
		}

		return Task.CompletedTask;
	}
}
