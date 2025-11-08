using System.Reflection;

using MediatR;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using Product.Persistence.Data;
using Product.Persistence.Data.Seeder;

using Serilog;

using Shared.Application.Behaviour;
using Shared.Infrastructure.Authentication;
using Shared.Infrastructure.Clock;
using Shared.Persistence;
using Shared.Persistence.Interceptors;

namespace Product;
public static class ProductModule
{
    public static IServiceCollection AddProductModule(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMediatR(conf =>
        {
            conf.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            conf.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });
        services.AddProductModuleServices(configuration);
        services.AddScoped<IDataSeeder, ProductDataSeeder>();
        return services;
    }

    public static IApplicationBuilder UseProductModule(this IApplicationBuilder app)
    {
        app.UseMigration<ProductDBContext>();
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
