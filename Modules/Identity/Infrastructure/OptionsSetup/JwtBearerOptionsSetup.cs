namespace Identity.Infrastructure.OptionsSetup;

public class JwtBearerOptionsSetup(IOptions<JwtOptions> jwtOptions) : IPostConfigureOptions<JwtBearerOptions>
{
    private readonly JwtOptions _jwtOptions = jwtOptions.Value;

    public void PostConfigure(string? name, JwtBearerOptions options)
    {
        // Add diagnostic logging
        Console.WriteLine($"=== JWT Configuration ===");
        Console.WriteLine($"Issuer: {_jwtOptions.Issuer ?? "NULL"}");
        Console.WriteLine($"Audience: {_jwtOptions.Audience ?? "NULL"}");
        Console.WriteLine($"SecretKey Length: {_jwtOptions.SecretKey?.Length ?? 0}");
        Console.WriteLine($"Salt: {(_jwtOptions.Salt != null ? "Set" : "NULL")}");
        Console.WriteLine($"========================");

        options.TokenValidationParameters.ValidateIssuer = true;
        options.TokenValidationParameters.ValidateAudience = true;
        options.TokenValidationParameters.ValidateLifetime = true;
        options.TokenValidationParameters.ValidateIssuerSigningKey = true;
        options.TokenValidationParameters.ValidIssuer = _jwtOptions.Issuer;
        options.TokenValidationParameters.ValidAudience = _jwtOptions.Audience;
        options.TokenValidationParameters.IssuerSigningKey =
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtOptions.SecretKey));
        options.TokenValidationParameters.ClockSkew = TimeSpan.FromMinutes(5);

        // Add detailed error logging
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Authentication failed: {context.Exception.GetType().Name}");
                Console.WriteLine($"Message: {context.Exception.Message}");
                if (context.Exception.InnerException != null)
                {
                    Console.WriteLine($"Inner Exception: {context.Exception.InnerException.Message}");
                }
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine(" JWT Token validated successfully");
                var userId = context.Principal?.FindFirst("sub")?.Value;
                Console.WriteLine($"User ID: {userId}");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                Console.WriteLine($"JWT Challenge: {context.Error}, {context.ErrorDescription}");
                return Task.CompletedTask;
            },
            OnMessageReceived = context =>
            {
                var token = context.Request.Headers["Authorization"].ToString();
                Console.WriteLine($"Token received: {(string.IsNullOrEmpty(token) ? "NONE" : "Present")}");
                return Task.CompletedTask;
            }
        };
    }
}
