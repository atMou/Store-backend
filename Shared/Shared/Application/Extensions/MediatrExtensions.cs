using System.Reflection;

using Microsoft.Extensions.DependencyInjection;

using Shared.Application.Behaviour;

namespace Shared.Application.Extensions;

public static class MediatrExtensions
{

    public static IServiceCollection AddMediatrWithAssemblies
        (this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(config =>
        {
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            config.RegisterServicesFromAssemblies(assemblies);
            config.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());
            //config.AddOpenBehavior(typeof(ValidationBehavior<,>));
            config.AddOpenBehavior(typeof(LoggingBehaviour<,>));
        });


        return services;
    }
}
