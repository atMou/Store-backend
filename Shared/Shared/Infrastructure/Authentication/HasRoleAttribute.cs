
using Microsoft.AspNetCore.Authorization;

using Role = Shared.Infrastructure.Enums.Role;

namespace Shared.Infrastructure.Authentication;

public sealed class HasRoleAttribute(Role role)
    : AuthorizeAttribute(policy: $"Role:{role}");
