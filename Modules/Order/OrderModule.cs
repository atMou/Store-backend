using Order.Persistence;

namespace Order;
public static class OrderModule
{
    public static IServiceCollection AddOrderModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOrderModuleServices(configuration);
        return services;
    }

    public static IApplicationBuilder UseOrderModule(this IApplicationBuilder app)
    {
        return app;
    }

    private static IServiceCollection AddOrderModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<OrderDBContext>((sp, options) =>
        {
            var clock = Optional(sp.GetService<IClock>())
                .IfNone(() => throw new InvalidOperationException("IClock is not registered"));
            var mediatr = Optional(sp.GetService<IMediator>())
                .IfNone(() => throw new InvalidOperationException("IMediator is not registered"));

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
                LogLevel.Information,
                DbContextLoggerOptions.DefaultWithLocalTime);
        });

        return services;
    }







}
