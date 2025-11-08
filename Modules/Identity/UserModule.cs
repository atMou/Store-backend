using System.Reflection;

using Identity.Persistence;
using Identity.Persistence.Repositories;

using MediatR;

using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

using Serilog;

using Shared.Application.Behaviour;
using Shared.Infrastructure.Authentication;
using Shared.Infrastructure.Clock;
using Shared.Persistence;
using Shared.Persistence.Interceptors;

namespace Identity;
public static class UserModule
{
    public static IServiceCollection AddIdentityModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddHttpContextAccessor();
        services.AddScoped<IUserContext, UserContext>();
        services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            conf.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });

        services.AddIdentityModuleServices(configuration);
        services.AddScoped<IUserRepository, UserRepositories>();
        return services;
    }

    public static IApplicationBuilder UseIdentityModule(this IApplicationBuilder app)
    {
        app.UseMigration<IdentityDbContext>();
        return app;
    }


    private static IServiceCollection AddIdentityModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<UserDbContext>((sp, options) =>
        {
            var clock = Optional(sp.GetService<IClock>())
                .IfNone(() => throw new InvalidOperationException("IClock is not registered"));
            var mediatr = Optional(sp.GetService<IMediator>())
                .IfNone(() => throw new InvalidOperationException("IMediator is not registered"));

            var userContext = Optional(sp.GetService<IUserContext>())
                .IfNone(() => throw new InvalidOperationException("IUserContext is not registered"));

            options.AddInterceptors(new AuditableEntityInterceptor(clock, userContext), new DispatchDomainEventInterceptor(mediatr));

            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();

            options.LogTo(Log.Logger.Information,
                Microsoft.Extensions.Logging.LogLevel.Information,
                DbContextLoggerOptions.DefaultWithLocalTime);


        });

        services.AddAuthenticationServices();
        return services;
    }

    private static IServiceCollection AddAuthenticationServices(this IServiceCollection services)

    {
        services.AddAuthentication(o =>
        {
            o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        }).AddJwtBearer(o =>
        {
            o.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = "",
                ValidAudience = "yourdomain.com",
                IssuerSigningKey = new SymmetricSecurityKey("YourSuperSecretKey"u8.ToArray())
            };
        });
        return services;
    }







}
