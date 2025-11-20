using Shared.Messaging.Abstractions;

namespace Shared.Messaging.Events;
using System;

public record ChangeCartItemsPriceIntegrationEvent(Guid ProductId, decimal oldPrice, decimal NewPrice) : IntegrationEvent
{
}
