using Microsoft.AspNetCore.Authorization;

using Shared.Infrastructure.ValueObjects;

namespace Shared.Infrastructure.Authentication;

public sealed class HasPermissionAttribute(Permission permission)
    : AuthorizeAttribute(policy: $"Permission:{permission}");
