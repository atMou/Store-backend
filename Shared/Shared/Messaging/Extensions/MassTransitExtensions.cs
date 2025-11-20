// Shared.Messaging.Extensions/MassTransitExtensions.cs
using System.Reflection;

using MassTransit;

using Microsoft.Extensions.DependencyInjection;

namespace Shared.Messaging.Extensions;

//public static class MassTransitExtensions
//{
//    public static IServiceCollection AddMassTransitWithAssembles(
//        this IServiceCollection services,
//        IConfiguration configuration,
//        params Assembly[] assemblies)

//    {

//        services.AddMassTransit(cfg =>
//        {
//            cfg.SetKebabCaseEndpointNameFormatter();
//            cfg.SetInMemorySagaRepositoryProvider();

//            cfg.AddConsumers(assemblies);
//            cfg.AddSagaStateMachines(assemblies);
//            cfg.AddSagas(assemblies);
//            cfg.AddActivities(assemblies);
//            //cfg.AddEntityFrameworkOutbox<>(outbox =>
//            //{
//            //    outbox.UseBusOutbox();
//            //    outbox.QueryDelay = TimeSpan.FromSeconds(1);
//            //    outbox.DuplicateDetectionWindow = TimeSpan.FromMinutes(10);
//            //});

//            cfg.UsingRabbitMq((context, rabbit) =>
//            {
//                rabbit.Host(new Uri(configuration["RabbitMq:Host"]!), h =>
//                {
//                    h.Username(configuration["RabbitMq:UserName"]!);
//                    h.Password(configuration["RabbitMq:Password"]!);
//                });
//                rabbit.ConfigureEndpoints(context);
//            });
//        });

//        return services;
//    }
//}

public static class MassTransitModuleExtensions
{
    public static void AddModuleMassTransit<TDbContext>(
        this IServiceCollection services,
        Assembly assembly)
        where TDbContext : DbContext
        //where TBus : class, IBus
    {
        services.AddMassTransit(cfg =>
        {
            cfg.AddConsumers(assembly);

            cfg.AddEntityFrameworkOutbox<TDbContext>(outbox =>
            {
                outbox.UseBusOutbox();
                outbox.QueryDelay = TimeSpan.FromSeconds(1);
            });

            cfg.UsingRabbitMq((context, rmq) =>
            {
                var configuration = context.GetRequiredService<IConfiguration>();

                rmq.Host(configuration["RabbitMq:Host"], h =>
                {
                    h.Username(configuration["RabbitMq:UserName"]!);
                    h.Password(configuration["RabbitMq:Password"]!);
                });


                rmq.ConfigureEndpoints(context);
            });
        });
    }
}

