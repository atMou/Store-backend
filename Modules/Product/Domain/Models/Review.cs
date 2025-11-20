namespace Product.Domain.Models;

public record Review : Entity<ReviewId>
{
    private Review() : base(ReviewId.New)
    {

    }
    private Review(UserId userId, ProductId productId, Comment comment, Rating rating) : base(ReviewId.New)
    {
        UserId = userId;
        ProductId = productId;
        Comment = comment;
        Rating = rating;
    }

    public UserId UserId { get; private init; }

    public Comment Comment { get; private init; }

    public Rating Rating { get; private init; }

    public ProductId ProductId { get; private init; }

    public Product Product { get; private init; }

    public static Fin<Review> Create(UserId userId, ProductId productId, string comment, double rating)
    {
        return (Comment.From(comment), Rating.From(rating))
            .Apply((c, r) => new Review(userId, productId, c, r)).As();
    }


}