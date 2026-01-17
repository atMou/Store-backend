namespace Product.Domain.Events;

public record ProductPriceChangedEvent(Models.Product Product, decimal NewPrice) : IDomainEvent
{
}
