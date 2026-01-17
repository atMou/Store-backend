using Microsoft.Extensions.Logging;

using Shared.Application.Contracts.Order.Queries;
using Shared.Application.Features.Payment.Events;
using Shared.Application.Features.Shipment.Events;

using Shipment.Persistence;

namespace Shipment.Application.EventHandlers;

public class PaymentFulfilledIntegrationEventHandler(
    ISender sender,
    ShipmentDbContext dbContext,
    IPublishEndpoint endpoint,
    ILogger<PaymentFulfilledIntegrationEventHandler> logger
) : IConsumer<PaymentFulfilledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
    {
        var message = context.Message;


        var db = from orderResult in liftIO(async e =>
                await sender.Send(new GetOrderByIdQuery { OrderId = OrderId.From(message.OrderId) }, e.Token))
                 from order in orderResult
                 let address = new Address
                 {
                     Street = order.ShippingAddress.Street,
                     City = order.ShippingAddress.City,
                     PostalCode = order.ShippingAddress.PostalCode,
                     HouseNumber = order.ShippingAddress.HouseNumber,
                     ExtraDetails = order.ShippingAddress.ExtraDetails
                 }
                 let shipment = Domain.Models.Shipment.Create(
                     OrderId.From(message.OrderId),
                     address, order.TrackingCode)
                 from s in AddEntity<ShipmentDbContext, Domain.Models.Shipment>(shipment)
                 select s;

        var result = await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken))
            .RaiseOnSuccess(async s =>
            {
                var integrationEvent = new ShipmentCreatedIntegrationEvent(
                    s.Id.Value,
                    s.OrderId.Value,
                    s.TrackingCode
                );
                await endpoint.Publish(integrationEvent, context.CancellationToken);
                return unit;
            });


        result.Match(
            _ => logger.LogInformation($"Shipment created successfully for Order {message.OrderId}"),
            err => logger.LogError($"Error creating shipment for Order {message.OrderId}: {err.Message}")
        );
    }
}