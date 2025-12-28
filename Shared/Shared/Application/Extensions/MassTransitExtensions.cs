using MassTransit;

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


			config.UsingRabbitMq((context, cfg) =>
			{
				cfg.Host(new Uri(configuration["RabbitMq:Host"]!), h =>
				{
					h.Username(configuration["RabbitMq:UserName"]!);
					h.Password(configuration["RabbitMq:Password"]!);
				});
				cfg.ConfigureEndpoints(context);
			});


		});
		return service;
	}

}
