using Microsoft.AspNetCore.Authorization;

namespace Shared.Infrastructure.Authentication;

public class PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
	: DefaultAuthorizationPolicyProvider(options)
{
	public override async Task<AuthorizationPolicy?> GetPolicyAsync(
		string policyName)
	{
		AuthorizationPolicy? policy = await base.GetPolicyAsync(policyName);

		if (policy is not null)
		{
			return policy;
		}

		var roles = new List<string>();
		var permissions = new List<string>();

		foreach (var part in policyName.Split(';'))
		{
			if (part.StartsWith("Role:")) roles.Add(part["Role:".Length..]);
			if (part.StartsWith("Permission:")) permissions.Add(part["Permission:".Length..]);
		}

		var builder = new AuthorizationPolicyBuilder();

		if (roles.Any())
			builder.AddRequirements(new RoleRequirement(roles));

		if (permissions.Any())
			builder.AddRequirements(new PermissionRequirement(permissions));

		return builder.Build();
	}
}
