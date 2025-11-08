using System.Reflection;

using Basket.Persistence.Repositories;
using Basket.Persistence.Seeder;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Serilog;

using Shared.Application.Behaviour;
using Shared.Infrastructure.Clock;
using Shared.Persistence;
using Shared.Persistence.Interceptors;

namespace Basket;
public static class CartModule
{
    public static IServiceCollection AddBasketModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            conf.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });
        services.AddBasketModuleServices(configuration);
        services.AddScoped<IDataSeeder, BasketDataSeeder>();
        services.AddScoped<ICartRepository, CartRepository>();
        return services;
    }

    public static IApplicationBuilder UseBasketModule(this IApplicationBuilder app)
    {
        //app.UseMigration<BasketDbContext>();
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

            options.AddInterceptors(new AuditableEntityInterceptor(clock, userContext), new DispatchDomainEventInterceptor(mediatr));
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
