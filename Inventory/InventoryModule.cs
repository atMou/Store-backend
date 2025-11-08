namespace Inventory;
public static class InventoryModule
{
    //public static IServiceCollection AddInventoryModule(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services.AddMediatR(conf =>
    //    {
    //        conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
    //        conf.AddOpenBehavior(typeof(LoggingBehaviour<,>));
    //    });
    //    services.AddInventoryModuleServices(configuration);
    //    services.AddScoped<IDataSeeder, InventoryDataSeeder>();
    //    return services;
    //}

    //public static IApplicationBuilder UseInventoryModule(this IApplicationBuilder app)
    //{
    //    app.UseMigration<InventoryDbContext>();
    //    return app;
    //}


    //private static IServiceCollection AddInventoryModuleServices(this IServiceCollection services, IConfiguration configuration)
    //{
    //    services.AddDbContext<InventoryDbContext>((sp, options) =>
    //{
    //    services.AddDbContext<InventoryDbContext>((sp, options) =>
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
