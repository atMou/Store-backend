using MassTransit;

using Microsoft.OpenApi.Models;

using Shared;
using Shared.Application.Extensions;

namespace Api;

internal class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        if (builder.Environment.IsDevelopment())
            builder.Configuration.AddUserSecrets<Program>();



        builder.Host.UseSerilog((ctx, config) =>
        {
            config.ReadFrom.Configuration(ctx.Configuration);
        });



        builder.Configuration.AddEnvironmentVariables();
        var productAssembly = typeof(ProductModule).Assembly;
        var basketAssembly = typeof(CartModule).Assembly;
        var orderAssembly = typeof(OrderModule).Assembly;
        var identityAssembly = typeof(IdentityModule).Assembly;

        builder.Services.AddSharedModule(builder.Configuration);
        builder.Services.AddIdentityModule(builder.Configuration);
        builder.Services.AddProductModule(builder.Configuration);
        builder.Services.AddBasketModule(builder.Configuration);
        builder.Services.AddOrderModule(builder.Configuration);

        builder.Services.AddMediatrWithAssemblies(
                productAssembly,
                basketAssembly,
                identityAssembly,
                orderAssembly);


        builder.Services.AddMassTransit(cfg =>
        {
            cfg.AddConsumers(identityAssembly);
            cfg.AddConsumers(basketAssembly);
            cfg.AddConsumers(productAssembly);
            cfg.AddConsumers(orderAssembly);


            cfg.SetKebabCaseEndpointNameFormatter();
            cfg.SetInMemorySagaRepositoryProvider();


            cfg.UsingRabbitMq((context, rmq) =>
            {
                rmq.Host(builder.Configuration["RabbitMq:Host"], h =>
                {
                    h.Username(builder.Configuration["RabbitMq:UserName"]!);
                    h.Password(builder.Configuration["RabbitMq:Password"]!);
                });

                rmq.ConfigureEndpoints(context);
            });
        });

        builder.Services.AddControllers()
            .AddApplicationPart(productAssembly)
            .AddApplicationPart(basketAssembly)
            .AddApplicationPart(identityAssembly)
            .AddApplicationPart(orderAssembly).AddControllersAsServices();


        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(c =>
        {
            c.EnableAnnotations();
            c.SupportNonNullableReferenceTypes();

            c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });

            c.TagActionsBy(api =>
            {
                var ns = api.ActionDescriptor.RouteValues["controller"];
                return new[] { ns ?? "API" };
            });// avoid conflicts
        });


        builder.Services.AddHttpClient("GeoIpClient", client =>
        {
            client.BaseAddress = new Uri("https://ipapi.co");
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });


        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.UseDeveloperExceptionPage();
        }


        app.UseIdentityModule();
        app.UseProductModule();
        app.UseBasketModule();
        app.UseOrderModule();
        app.UseAuthentication();
        app.UseAuthorization();

        app.UseSwagger();
        app.UseSwaggerUI();

        app.MapControllers();

        app.Use(async (ctx, next) =>
        {
            Console.WriteLine($"[AUTH] IsAuthenticated: {ctx.User?.Identity?.IsAuthenticated}");
            Console.WriteLine($"[AUTH] Claims: {string.Join(", ", ctx.User?.Claims.Select(c => $"{c.Type}={c.Value}") ?? [])}");
            await next();
        });
        app.UseSerilogRequestLogging();

        app.Run();
    }
}
