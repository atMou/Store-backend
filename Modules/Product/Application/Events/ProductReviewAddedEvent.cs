namespace Product.Application.Events;
internal record ProductReviewAddedEvent(ProductId ProductId, UserId UserId, ReviewId ReviewId, Rating Rating) : IDomainEvent
{
}
