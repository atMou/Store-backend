using System.Reflection;

using Inventory;

using MassTransit;

using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Models;

using Payment;

using Serilog.Events;

using Shared;
using Shared.Application.Extensions;

using Shipment;

namespace Api;

internal class Program
{
    public static void Main(string[] args)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Information)
            .Enrich.FromLogContext()
            .WriteTo.Console()
            .CreateBootstrapLogger();

        try
        {
            Log.Information("Starting Store Backend API");

            var builder = WebApplication.CreateBuilder(args);

            if (builder.Environment.IsDevelopment())
                builder.Configuration.AddUserSecrets<Program>();

            builder.Host.UseSerilog((context, services, configuration) =>
            {
                configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                    .Enrich.WithProperty("Application", "Store Backend")
                    .Enrich.WithProperty("Environment", context.HostingEnvironment.EnvironmentName);
            });

            builder.Services.AddSignalR();

            builder.Configuration.AddEnvironmentVariables();
            var productAssembly = typeof(ProductModule).Assembly;
            var basketAssembly = typeof(CartModule).Assembly;
            var orderAssembly = typeof(OrderModule).Assembly;
            var identityAssembly = typeof(IdentityModule).Assembly;
            var paymentAssembly = typeof(PaymentModule).Assembly;
            var shipmentAssembly = typeof(ShipmentModule).Assembly;
            var inventoryAssembly = typeof(InventoryModule).Assembly;

            builder.Services.AddControllers()
                .ConfigureApiBehaviorOptions(options =>
                {
                    options.InvalidModelStateResponseFactory = context =>
                    {
                        var errors = context.ModelState
                            .Where(e => e.Value.Errors.Count > 0)
                            .Select(e => new
                            {
                                Field = e.Key,
                                Errors = e.Value.Errors.Select(err => err.ErrorMessage)
                            });

                        Log.Warning("Model validation failed: {@ValidationErrors}", errors);

                        return new BadRequestObjectResult(errors);
                    };
                })
                .AddApplicationPart(inventoryAssembly)
                .AddApplicationPart(productAssembly)
                .AddApplicationPart(basketAssembly)
                .AddApplicationPart(identityAssembly)
                .AddApplicationPart(orderAssembly)
                .AddControllersAsServices();


            builder.Services.AddSharedModule(builder.Configuration);
            builder.Services.AddIdentityModule(builder.Configuration);
            builder.Services.AddProductModule(builder.Configuration);
            builder.Services.AddBasketModule(builder.Configuration);
            builder.Services.AddOrderModule(builder.Configuration);
            builder.Services.AddShipmentModule(builder.Configuration);
            builder.Services.AddPaymentModule(builder.Configuration);
            builder.Services.AddInventoryModule(builder.Configuration);

            builder.Services.AddMediatrWithAssemblies(
                    productAssembly,
                    basketAssembly,
                    identityAssembly,
                    orderAssembly,
                    paymentAssembly,
                    shipmentAssembly,
                    inventoryAssembly
            );


            builder.Services.AddMassTransit(cfg =>
            {
                cfg.AddConsumers(identityAssembly);
                cfg.AddConsumers(basketAssembly);
                cfg.AddConsumers(productAssembly);
                cfg.AddConsumers(orderAssembly);
                cfg.AddConsumers(inventoryAssembly);
                cfg.AddConsumers(paymentAssembly);
                cfg.AddConsumers(shipmentAssembly);

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

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SupportNonNullableReferenceTypes();

                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Store Backend API", Version = "v1" });

                // include XML comments
                var asm = Assembly.GetExecutingAssembly();
                var xmlFile = $"{asm.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                if (File.Exists(xmlPath))
                {
                    c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
                }

                c.TagActionsBy(api =>
                {
                    var ns = api.ActionDescriptor.RouteValues["controller"];
                    return [ns ?? "API"];
                });

                // JWT Security
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT"
                });

                c.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    {
                        new OpenApiSecurityScheme
                        {
                            Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                        },
                        Array.Empty<string>()
                    }
                });
            });


            builder.Services.AddCors(options =>
            {
                options.AddPolicy("allow", policy =>
                {
                    policy.WithOrigins("http://localhost:3000")
                        .AllowAnyHeader()
                        .AllowAnyMethod()
                        .AllowCredentials();
                });
            });

            builder.Services.AddHttpClient("GeoIpClient", client =>
            {
                client.BaseAddress = new Uri("https://ipapi.co");
                client.Timeout = TimeSpan.FromSeconds(5);
                client.DefaultRequestHeaders.Add("Accept", "application/json");
            });


            var app = builder.Build();

            Log.Information("Application built successfully. Environment: {Environment}",
                app.Environment.EnvironmentName);

            if (app.Environment.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "Store Backend API v1");
                c.DocumentTitle = "Store Backend API Documentation";
            });

            app.UseCors("allow");

            // Add Serilog request logging with custom enrichment
            app.UseSerilogRequestLogging(options =>
            {
                options.MessageTemplate = "HTTP {RequestMethod} {RequestPath} responded {StatusCode} in {Elapsed:0.0000}ms";
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    diagnosticContext.Set("RequestHost", httpContext.Request.Host.Value);
                    diagnosticContext.Set("RequestScheme", httpContext.Request.Scheme);
                    diagnosticContext.Set("UserAgent", httpContext.Request.Headers["User-Agent"].ToString());

                    if (httpContext.User.Identity?.IsAuthenticated == true)
                    {
                        diagnosticContext.Set("UserId", httpContext.User.FindFirst("sub")?.Value);
                    }
                };
            });

            app.UseIdentityModule();
            app.UseProductModule();
            app.UseBasketModule();
            app.UseOrderModule();
            app.UseShipmentModule();
            app.UsePaymentModule();
            app.UseInventoryModule();
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            Log.Information("Store Backend API is ready to accept requests");

            app.Run();
        }
        catch (Exception ex)
        {
            Log.Fatal(ex, "Application terminated unexpectedly");
        }
        finally
        {
            Log.CloseAndFlush();
        }
    }
}
