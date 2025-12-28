using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Shared.Infrastructure.Middleware;

public class ResponseAuthorizationMiddlewareResultHandler
	: IAuthorizationMiddlewareResultHandler
{
	private readonly AuthorizationMiddlewareResultHandler _defaultHandler = new();

	public async Task HandleAsync(
		RequestDelegate next,
		HttpContext context,
		AuthorizationPolicy policy,
		PolicyAuthorizationResult authorizeResult)
	{
		if (authorizeResult.Forbidden)
		{
			context.Response.StatusCode = StatusCodes.Status403Forbidden;
			context.Response.ContentType = "application/json";
			var requirementType = policy.Requirements.FirstOrDefault()?.GetType();

			ProblemDetails problem;

			if (requirementType == typeof(RoleRequirement))
			{
				problem = new ProblemDetails
				{
					Status = StatusCodes.Status403Forbidden,
					Title = "Forbidden",
					Detail = "You do not have the required role.",
					Instance = context.Request.Path,
					Extensions = new Dictionary<string, object?>
					{
						{
							"required roles",
							policy.Requirements
								.OfType<RoleRequirement>()
								.SelectMany(r => r.AllowedRoles)
						}
					}
				};
				await context.Response.WriteAsJsonAsync(problem);
				return;
			}

			if (requirementType == typeof(PermissionRequirement))
			{
				problem = new ProblemDetails
				{
					Status = StatusCodes.Status403Forbidden,
					Title = "Forbidden",
					Detail = "You do not have the required permission.",
					Instance = context.Request.Path,
					Extensions = new Dictionary<string, object?>
					{
						{
							"required permissions", policy.Requirements
								.OfType<PermissionRequirement>()
								.SelectMany(r => r.Permissions)
						}
					}
				};

				await context.Response.WriteAsJsonAsync(problem);
				return;
			}
		}

		if (authorizeResult.Challenged)
		{
			context.Response.StatusCode = StatusCodes.Status401Unauthorized;
			context.Response.ContentType = "application/json";

			var problem = new ProblemDetails
			{
				Status = StatusCodes.Status401Unauthorized,
				Title = "Unauthorized",
				Detail = "A valid JWT token is required.",
				Instance = context.Request.Path
			};

			await context.Response.WriteAsJsonAsync(problem);
			return;
		}

		await _defaultHandler.HandleAsync(next, context, policy, authorizeResult);
	}
}