namespace Payment;
public static class PaymentModule
{
    //public static IServiceCollection AddPaymentModule(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services.AddMediatR(conf =>
    //    {
    //        conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    //        conf.AddOpenBehavior(typeof(LoggingBehaviour<,>));
    //    });
    //    services.AddPaymentModuleServices(configuration);
    //    services.AddScoped<IDataSeeder, PaymentDataSeeder>();
    //    return services;
    //}

    //public static IApplicationBuilder UsePaymentModule(this IApplicationBuilder app)
    //{
    //    app.UseMigration<PaymentDbContext>();
    //    return app;
    //}


    //private static IServiceCollection AddPaymentModuleServices(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services.AddDbContext<PaymentDbContext>((sp, options) =>
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
