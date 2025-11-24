namespace Basket;
public static class CartModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
    {
        //services.AddMediatR(conf =>
        //{
        //    conf.RegisterServicesFromAssembly(typeof(SharedModule).Assembly);
        //});
        //services.AddModuleMassTransit<BasketDbContext>(typeof(CartModule).Assembly);

        //services.AddMassTransit(cfg =>
        //{
        //    cfg.AddConsumers(Assembly.GetExecutingAssembly());

        //    cfg.AddEntityFrameworkOutbox<BasketDbContext>(o =>
        //    {
        //        o.UseBusOutbox();
        //    });

        //    cfg.UsingRabbitMq((context, rabbit) =>
        //    {
        //        rabbit.ConfigureEndpoints(context);
        //    });
        //});
        services.AddBasketModuleServices(configuration);
        //services.AddScoped<ICartRepository, CartRepository>();
        //services.AddScoped<ICouponRepository, CouponRepository>();
        return services;
    }

    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {
        return app;
    }


    private static IServiceCollection AddBasketModuleServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<BasketDbContext>((sp, options) =>
        {
            var clock = Optional(sp.GetService<IClock>())
                .IfNone(() => throw new InvalidOperationException("IClock is not registered"));
            var mediatr = Optional(sp.GetService<IMediator>())
                .IfNone(() => throw new InvalidOperationException("IMediator is not registered"));

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

//public interface IBasketBus : IBus { }