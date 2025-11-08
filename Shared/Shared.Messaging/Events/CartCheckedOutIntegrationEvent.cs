using Shared.Messaging.Abstractions;

namespace Shared.Messaging.Events;
using System;

public record CartCheckedOutIntegrationEvent(
    Guid UserId,
    Guid CartId,
    Guid? CouponId,
    decimal Subtotal,
    decimal TotalTax,
    decimal Total,
    decimal TotalDiscount,
    IEnumerable<(
        Guid ProductId,
        string Sku,
        string Slug,
        string ImageUrl,
        int Quantity,
        Guid? CouponId,
        decimal UnitPrice,
        decimal LineTotal,
        decimal LineTotalAfterDiscount,
        decimal UnitPriceAfterDiscount,
        decimal TotalDiscount)> LineItems) : IntegrationEvent
{
}
