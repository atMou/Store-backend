namespace Product.Application.Events;

public record ProductPriceChangedEvent(Domain.Models.Product Product, decimal NewPrice) : IDomainEvent
{
}
