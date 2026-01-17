namespace Product.Domain.Models;

public class Review : Entity<ReviewId>
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

    public Comment Comment { get; private set; }

    public Rating Rating { get; private set; }

    public ProductId ProductId { get; private init; }

    public Product Product { get; private init; }

    public static Fin<Review> Create(UserId userId, ProductId productId, string comment, double rating)
    {
        return (Comment.From(comment), Rating.From(rating))
            .Apply((c, r) => new Review(userId, productId, c, r)).As();
    }


    public Fin<Review> Update(string? comment = null, double? rating = null)
    {
        var newComment = Optional(comment).Match(Comment.From, FinSucc(Comment));
        var newRating = Optional(rating).Match(Rating.From, FinSucc(Rating));
        return (newComment, newRating)
            .Apply((c, r) =>
            {
                Comment = c;
                Rating = r;
                return this;
            }).As();
    }


    public virtual bool Equals(Review? other)
    {
        return other is not null && Id == other.Id;
    }

    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }

}