using Shared.Application.Contracts.Order.Dtos;
using Shared.Application.Contracts.Order.Queries;
using Shared.Application.Features.Shipment.Events;
using Shared.Infrastructure.Email.Options;

namespace Identity.Application.EventHandlers;

public class ShipmentCreatedIntegrationEventHandler(
    ISender sender,
    IEmailService emailService,
    IEmailTemplateBuilder templateBuilder,
    IOptions<SendGridSettings> emailSettings,
    ILogger<ShipmentCreatedIntegrationEventHandler> logger)
    : IConsumer<ShipmentCreatedIntegrationEvent>
{
    private readonly SendGridSettings _emailSettings = emailSettings.Value;
    private static readonly Lazy<FileIO> _fileIO = new(() => fileIO.Default);

    public async Task Consume(ConsumeContext<ShipmentCreatedIntegrationEvent> context)
    {
        var message = context.Message;
        logger.LogInformation(
            "Processing shipment created event for Order {OrderId} - sending email notification",
            message.OrderId);

        var io = from orderResult in liftIO(async e =>
                    await sender.Send(new GetOrderByIdQuery { OrderId = OrderId.From(message.OrderId) }, e.Token))
                 from order in orderResult
                 from userResult in liftIO(async e =>
                    await sender.Send(new GetUserByIdQuery(UserId.From(order.UserId)), e.Token))
                 from user in userResult
                 let orderItems = order.OrderItems.Select(item => new OrderItemDto
                 {
                     OrderItemId = item.OrderItemId,
                     ProductId = item.ProductId,
                     ColorVariantId = item.ColorVariantId,
                     SizeVariantId = item.SizeVariantId,
                     Sku = item.Sku,
                     Size = item.Size,
                     Color = item.Color,
                     Slug = item.Slug,
                     ImageUrl = item.ImageUrl,
                     Quantity = item.Quantity,
                     UnitPrice = item.UnitPrice,
                     LineTotal = item.LineTotal
                 }).ToList()
                 from emailBody in templateBuilder.BuildOrderShippedEmailAsync(
                    $"{user.FirstName} {user.LastName}",
                    message.OrderId,
                    message.TrackingCode,
                    orderItems,
                    order.Subtotal,
                    order.Tax,
                    order.Total,
                    DateTime.UtcNow,
                    _fileIO.Value)
                 from response in emailService.Send<IO>(
                    new EmailAddress(_emailSettings.SenderEmail, _emailSettings.SenderName),
                    new EmailAddress(user.Email, $"{user.FirstName} {user.LastName}"),
                    $"Your Order #{message.OrderId.ToString()[..8]} Has Been Shipped! 📦",
                    "Your order has been shipped!",
                    emailBody,
                    context.CancellationToken)
                 select (user, message.OrderId, response);

        var result = await io.RunSafeAsync(EnvIO.New(null, context.CancellationToken));

        result.Match(
            Succ: r => logger.LogInformation(
                "Successfully sent order shipped email to {Email} for Order {OrderId}",
                r.user.Email,
                r.OrderId),
            Fail: err => logger.LogError(
                "Failed to send order shipped email for Order {OrderId}: {Error}",
                message.OrderId,
                err.Message)
        );
    }

}

