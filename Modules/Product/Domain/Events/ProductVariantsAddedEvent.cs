namespace Product.Domain.Events;

public record ProductVariantsAddedEvent(IEnumerable<ProductId> VariantIds) : IDomainEvent
{
}
