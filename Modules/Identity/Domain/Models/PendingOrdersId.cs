namespace Identity.Domain.Models;

public record PendingOrderId
{
    public UserId UserId { get; init; }
    public OrderId OrderId { get; init; }


    private PendingOrderId(UserId userId, OrderId orderId)
    {
        UserId = userId;
    }

    public static PendingOrderId Create(UserId userId, OrderId orderId)
    {
        return new PendingOrderId(userId, orderId);
    }

}