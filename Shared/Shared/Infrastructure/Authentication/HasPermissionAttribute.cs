using Microsoft.AspNetCore.Authorization;

using Permission = Shared.Infrastructure.Enums.Permission;

namespace Shared.Infrastructure.Authentication;

public sealed class HasPermissionAttribute : AuthorizeAttribute
{
    public HasPermissionAttribute(Permission permission)
        : base(policy: $"Permission:{permission.ToString()}")
    {
    }
}
