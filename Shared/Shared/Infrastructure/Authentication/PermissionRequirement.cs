using Microsoft.AspNetCore.Authorization;

namespace Shared.Infrastructure.Authentication;

public class PermissionRequirement(IEnumerable<string> permissions)
	: IAuthorizationRequirement
{
	public IReadOnlyList<string> Permissions { get; } = permissions.ToList();
}

