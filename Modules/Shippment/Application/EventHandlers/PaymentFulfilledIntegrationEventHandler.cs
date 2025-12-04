using LanguageExt.Traits;
using MassTransit;
using Microsoft.Extensions.Logging;
using Shared.Application.Contracts.Order.Queries;
using Shared.Application.Features.Payment.Events;
using Shared.Infrastructure.Clock;
using Shared.Persistence.Db.Monad;
using Shipment.Persistence.Data;

namespace Shipment.Application.EventHandlers;

public class PaymentFulfilledIntegrationEventHandler(
    ShipmentDbContext dbContext,
    ILogger<PaymentFulfilledIntegrationEventHandler> logger,
    IClock clock,
    ISender sender
) : IConsumer<PaymentFulfilledIntegrationEvent>
{
    public async Task Consume(ConsumeContext<PaymentFulfilledIntegrationEvent> context)
    {
        K<Db<ShipmentDbContext>, Unit> db = from result in IO.liftAsync(async e =>
                await sender.Send(new GetOrderByIdCommand
                {
                    OrderId = context.Message.OrderId
                }, e.Token))
                                            from a in AddEntity<ShipmentDbContext, Domain.Models.Shipment>(result.Map(o =>
                                                Domain.Models.Shipment.Create(
                                                    OrderId.From(o.OrderId),
                                                    o.ShippingAddress,
                                                    o.TrackingCode
                                                )))
                                            select unit;
        await db.RunSaveAsync(dbContext, EnvIO.New(null, context.CancellationToken))
            .RaiseOnFail(err => logger.LogError($"Error occurred while saving shipment: {err.Message}"));
    }
}
