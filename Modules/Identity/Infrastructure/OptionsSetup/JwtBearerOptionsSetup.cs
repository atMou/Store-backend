namespace Identity.Infrastructure.OptionsSetup;

public class JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions, ILogger<JwtBearerOptionsSetup> logger) : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.ValidIssuer = _jwtOptions.Issuer;
        options.TokenValidationParameters.ValidAudience = _jwtOptions.Audience;
        options.TokenValidationParameters.IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);

        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                // Allow SignalR to pass tokens via query string
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;

                // If the request is for SignalR hub
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/hubs"))
                {
                    logger.LogDebug("Received SignalR token via query string for path: {Path}", path);
                    context.Token = accessToken;
                }

                return Task.CompletedTask;
            },

            OnAuthenticationFailed = context =>
            {
                var path = context.Request.Path;
                logger.LogWarning("JWT Authentication failed for path: {Path}, Exception: {ExceptionName}",
                    path, context.Exception.GetType().Name);
                logger.LogWarning("Message: {Message}", context.Exception.Message);

                if (context.Exception.InnerException != null)
                {
                    logger.LogWarning("Inner Exception: {Message}", context.Exception.InnerException.Message);
                }

                // For SignalR connections, provide more details
                if (path.StartsWithSegments("/hubs"))
                {
                    logger.LogError("SignalR authentication failed. Ensure token is valid and passed correctly.");
                }

                return Task.CompletedTask;
            },
        };
    }
}
