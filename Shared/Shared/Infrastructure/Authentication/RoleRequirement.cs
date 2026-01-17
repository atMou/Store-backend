using Microsoft.AspNetCore.Authorization;

namespace Shared.Infrastructure.Authentication;

public class RoleRequirement(IEnumerable<string> allowedRoles) : IAuthorizationRequirement
{
    public IReadOnlyList<string> AllowedRoles { get; } = allowedRoles.ToList();
}
