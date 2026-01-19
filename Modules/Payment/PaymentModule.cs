using Payment.Infrastructure.Stripe;
using Payment.Infrastructure.Stripe.OptionsSetup;

namespace Payment;
public static class PaymentModule
{
    public static IServiceCollection AddPaymentModule(this IServiceCollection services, IConfiguration configuration)
    {

        services.AddPaymentModuleServices(configuration);
        services.AddStripeServices(configuration);
        return services;
    }

    public static IApplicationBuilder UsePaymentModule(this IApplicationBuilder app)
    {
        return app;
    }

    private static IServiceCollection AddStripeServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureOptions<StripOptionSetup>();
        services.AddScoped<IStripePaymentService, StripePaymentService>();
        return services;
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
