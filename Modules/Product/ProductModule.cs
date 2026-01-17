using Shared.Infrastructure.Images.Options;

namespace Product;
public static class ProductModule
{
    public static IServiceCollection AddProductModule(this IServiceCollection services, IConfiguration configuration)
    {

        services.ConfigureOptions<CloudinarySettingsSetup>();
        services.AddProductModuleServices(configuration);
        return services;
    }

    public static IApplicationBuilder UseProductModule(this IApplicationBuilder app)
    {

        return app;
    }



    private static IServiceCollection AddProductModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ProductDBContext>((sp, options) =>
        {

            var mediatr = Optional(sp.GetService<IMediator>())
                .IfNone(() => throw new InvalidOperationException("IMediator is not registered"));
            var clock = Optional(sp.GetService<IClock>())
                .IfNone(() => throw new InvalidOperationException("IClock is not registered"));
            var userContext = Optional(sp.GetService<IUserContext>())
                .IfNone(() => throw new InvalidOperationException("IUserContext is not registered"));

            options.AddInterceptors(new AuditableEntityInterceptor(clock, userContext),
                new DispatchDomainEventInterceptor(mediatr));

            options.UseSqlServer(
                configuration.GetConnectionString("DefaultConnection"),
                sqlServerOptionsAction: sqlOptions =>
                {
                    // Add connection resiliency for Docker/development scenarios
                    sqlOptions.EnableRetryOnFailure(
                        maxRetryCount: 5,
                        maxRetryDelay: TimeSpan.FromSeconds(10),
                        errorNumbersToAdd: null);
                    
                    sqlOptions.CommandTimeout(30);
                })
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();

            options.LogTo(Log.Logger.Information,
                Microsoft.Extensions.Logging.LogLevel.Information,
                DbContextLoggerOptions.DefaultWithLocalTime);
        });
        return services;
    }



}

