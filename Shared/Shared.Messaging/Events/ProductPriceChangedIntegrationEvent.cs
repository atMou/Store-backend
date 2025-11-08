using Shared.Messaging.Abstractions;

namespace Shared.Messaging.Events;
using System;

public record ProductPriceChangedIntegrationEvent(Guid ProductId, decimal NewPrice) : IntegrationEvent
{
}
