

namespace Payment;
public static class PaymentModule
{
    public static IServiceCollection AddPaymentModule(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddPaymentModuleServices(configuration);
        return services;
    }

    public static IApplicationBuilder UsePaymentModule(this IApplicationBuilder app)
    {
        return app;
    }


    private static IServiceCollection AddPaymentModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PaymentDbContext>((sp, options) =>
        {

            var mediatr = Optional(sp.GetService<IMediator>())
                .IfNone(() => throw new InvalidOperationException("IMediator is not registered"));
            var clock = Optional(sp.GetService<IClock>())
                .IfNone(() => throw new InvalidOperationException("IClock is not registered"));
            var userContext = Optional(sp.GetService<IUserContext>())
                .IfNone(() => throw new InvalidOperationException("IUserContext is not registered"));

            options.AddInterceptors(new AuditableEntityInterceptor(clock, userContext),
                new DispatchDomainEventInterceptor(mediatr));

            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors();

            options.LogTo(Log.Logger.Information,
                Microsoft.Extensions.Logging.LogLevel.Information,
                DbContextLoggerOptions.DefaultWithLocalTime);
        });
        return services;
    }



}
