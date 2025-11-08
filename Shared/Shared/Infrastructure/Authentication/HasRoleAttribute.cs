
using Microsoft.AspNetCore.Authorization;

using Shared.Infrastructure.ValueObjects;

namespace Shared.Infrastructure.Authentication;

public sealed class HasRoleAttribute(Role role)
    : AuthorizeAttribute(policy: $"Role:{role}");
