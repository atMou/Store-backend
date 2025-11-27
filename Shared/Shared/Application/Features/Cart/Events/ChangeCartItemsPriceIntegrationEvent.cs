namespace Shared.Application.Features.Cart.Events;
using System;

using Shared.Application.Abstractions;

public record ChangeCartItemsPriceIntegrationEvent(Guid ProductId, decimal oldPrice, decimal NewPrice) : IntegrationEvent
{
}
