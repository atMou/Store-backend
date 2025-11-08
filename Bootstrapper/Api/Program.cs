using Basket;

using Identity;

using Order;

using Product;

using Shared.Application.Extensions;
using Shared.Infrastructure.Clock;
using Shared.Messaging.Extensions;

namespace Api;

internal static class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        //builder.Host.UseSerilog((ctx, config) =>
        //{
        //    config.ReadFrom.Configuration(ctx.Configuration);
        //});
        // Add services to the container.
        var productAssembly = typeof(ProductModule).Assembly;
        var basketAssembly = typeof(CartModule).Assembly;
        var orderAssembly = typeof(OrderModule).Assembly;
        var userAssembly = typeof(UserModule).Assembly;
        //var orderingAssembly = typeof(OrderingModule).Assembly;

        builder.Services.AddMassTransitWithAssembles(
            builder.Configuration,
            productAssembly,
            basketAssembly,
            orderAssembly,
            userAssembly);
        builder.Services
            .AddMediatrWithAssemblies(
                productAssembly,
                basketAssembly,
                userAssembly);


        builder.Services.AddControllers()
            .AddApplicationPart(productAssembly)
            .AddApplicationPart(basketAssembly)
            .AddApplicationPart(userAssembly);

        builder.Services.AddScoped<IClock, Clock>();

        builder.Services.AddHttpClient("GeoIpClient", client =>
        {
            client.BaseAddress = new Uri("https://ipapi.co");
            client.Timeout = TimeSpan.FromSeconds(5);
            client.DefaultRequestHeaders.Add("Accept", "application/json");
        });

        //builder.Services.AddScoped<IGeoLocationService, GeoLocationService>();


        builder.Services.AddProductModule(builder.Configuration);
        builder.Services.AddBasketModule(builder.Configuration);
        //builder.Services.AddOrderModule(builder.Configuration);
        builder.Services.AddIdentityModule(builder.Configuration);

        var app = builder.Build();

        //app.UseMiddleware<GeoLocationMiddleware>();
        // Configure the HTTP request pipeline.
        //app.MapCarter();
        //app.UseSerilogRequestLogging();
        app.UseProductModule();
        app.UseBasketModule();
        //app.UseOrderModule();
        app.UseIdentityModule();


        app.Run();
    }
}