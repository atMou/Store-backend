namespace Product.Application.Events;

public record ProductPriceChangedEvent(ProductId ProductId, decimal NewPrice) : IDomainEvent
{
}
