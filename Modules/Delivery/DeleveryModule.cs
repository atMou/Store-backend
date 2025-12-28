namespace Delivery;
public static class DeliveryModule
{
	//public static IServiceCollection AddDeliveryModule(this IServiceCollection services, IConfiguration configuration)
	//{
	//    services.AddMediatR(conf =>
	//    {
	//        conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
	//        conf.AddOpenBehavior(typeof(LoggingBehaviour<,>));
	//    });
	//    services.AddDeliveryModuleServices(configuration);
	//    services.AddScoped<IDataSeeder, DeliveryDataSeeder>();
	//    return services;
	//}

	//public static IApplicationBuilder UseDeliveryModule(this IApplicationBuilder app)
	//{
	//    app.UseMigration<DeliveryDbContext>();
	//    return app;
	//}


	//private static IServiceCollection AddDeliveryModuleServices(this IServiceCollection services, IConfiguration configuration)
	//{
	//    services.AddDbContext<DeliveryDbContext>((sp, options) =>
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
