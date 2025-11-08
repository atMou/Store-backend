using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

namespace Shared.Application.Extensions;

public static class MediatrExtensions
{

    public static IServiceCollection AddMediatrWithAssemblies
        (this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssemblies(assemblies);
            //config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            //config.AddOpenBehavior(typeof(LoggingBehavior<,>));
        });


        return services;
    }
}
