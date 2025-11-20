namespace Product.Application.Events;

public record ProductVariantsAddedEvent(IEnumerable<ProductId> VariantIds) : IDomainEvent
{
}
