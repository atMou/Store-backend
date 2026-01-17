using Microsoft.Extensions.Logging;

using Shared.Application.Features.Product.Events;
using Shared.Infrastructure.Hubs;
using Shared.Infrastructure.Hubs.Services;

namespace Product.Application.EventHandlers;

public class ProductCreatedIntegrationNotificationHandler(
    INotificationService notificationService,
    ILogger<ProductCreatedIntegrationNotificationHandler> logger)
    : IConsumer<ProductCreatedIntegrationEvent>
{
    public Task Consume(ConsumeContext<ProductCreatedIntegrationEvent> context)
    {
        var message = context.Message;

        Try.lift(async () =>
            {
                var notification = new NewProductNotification
                {
                    ProductId = message.ProductId,
                    Slug = message.Slug,
                    Brand = message.Brand,
                    ImageUrl = message.ImageUrl,
                    Message = $"🎉 New Arrival: {message.Brand}!"
                };

                await notificationService.BroadcastNewProductArrival(notification);
                logger.LogInformation($"New product arrival notification sent for Brand: {message.Brand}!");
            }).Run()
            .IfFail(ex => { logger.LogError(ex, $"Error sending new product notification for Brand: {message.Brand}!"); });

        return Task.CompletedTask;
    }
}