namespace Product.Domain.Events;
internal record ProductReviewAddedEvent(ProductId ProductId, UserId UserId, ReviewId ReviewId, Rating Rating) : IDomainEvent
{
}
