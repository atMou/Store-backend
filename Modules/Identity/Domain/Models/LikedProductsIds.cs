namespace Identity.Domain.Models;

public record LikedProductId
{
    public UserId UserId { get; init; }
    public ProductId ProductId { get; init; }


    private LikedProductId(UserId userId, ProductId productId)
    {
        UserId = userId;
        ProductId = productId;
    }

    public static LikedProductId Create(UserId userId, ProductId productId)
    {
        return new LikedProductId(userId, productId);
    }

}