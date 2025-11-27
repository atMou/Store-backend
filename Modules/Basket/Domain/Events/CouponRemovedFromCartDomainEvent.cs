namespace Basket.Domain.Events;

public record CouponRemovedFromCartDomainEvent(CouponId CouponId) : IDomainEvent;