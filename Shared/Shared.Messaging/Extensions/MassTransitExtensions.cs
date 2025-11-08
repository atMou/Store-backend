namespace Shared.Messaging.Extensions;
using System.Reflection;

using MassTransit;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class MassTransitExtensions
{
    public static IServiceCollection AddMassTransitWithAssembles(this IServiceCollection service, IConfiguration configuration, params Assembly[] assemblies)
    {
        service.AddMassTransit(config =>
        {
            config.SetKebabCaseEndpointNameFormatter();
            config.SetInMemorySagaRepositoryProvider();
            config.AddConsumers(assemblies);
            config.AddSagaStateMachines(assemblies);
            config.AddSagas(assemblies);
            config.AddActivities(assemblies);


            //config.UsingInMemory((context, cfg) =>
            //{
            //    cfg.ConfigureEndpoints(context);
            //});

            //config.UsingRabbitMq((context, cfg) =>
            //{
            //    cfg.Host(new Uri(configuration["RabbitMQ:Host"]!), h =>
            //    {
            //        h.Username(configuration["RabbitMQ:Username"]!);
            //        h.Password(configuration["RabbitMQ:Password"]!);
            //    });
            //    cfg.ConfigureEndpoints(context);
            //});

            config.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(configuration.GetConnectionString("RabbitMq"));
                cfg.ConfigureEndpoints(context);
            });
        });
        return service;
    }

}
