namespace Order;
public static class OrderModule
{
    //public static IServiceCollection AddOrderModule(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services.AddMediatR(conf =>
    //    {
    //        conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    //        conf.AddOpenBehavior(typeof(LoggingBehaviour<,>));
    //    });
    //    services.AddOrderModuleServices(configuration);
    //    services.AddScoped<IDataSeeder, OrderDataSeeder>();
    //    return services;
    //}

    //public static IApplicationBuilder UseOrderModule(this IApplicationBuilder app)
    //{
    //    app.UseMigration<OrderDbContext>();
    //    return app;
    //}


    //private static IServiceCollection AddOrderModuleServices(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services.AddDbContext<OrderDbContext>((sp, options) =>
    //    {
    //        var utc = Optional(sp.GetService<GetUtcNow>()).Map(f => f).IfNone(() => DateTime.UtcNow);
    //        var mediatr = Optional(sp.GetService<IMediator>()).IfNone(() => throw new InvalidOperationException("IMediator is not registered"));

    //        options.AddInterceptors(new AuditableEntityInterceptor(utc), new DispatchDomainEventInterceptor(mediatr));
    //        options.UseSqlServer(configuration.GetConnectionString("DefaultConnection"))
    //            .EnableSensitiveDataLogging()
    //            .EnableDetailedErrors();
    //        options.LogTo(Log.Logger.Information,
    //            Microsoft.Extensions.Logging.LogLevel.Information,
    //            DbContextLoggerOptions.DefaultWithLocalTime);
    //    });
    //    return services;
    //}







}
