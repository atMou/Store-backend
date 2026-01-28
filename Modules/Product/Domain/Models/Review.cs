namespace Product.Domain.Models;

public class Review : Entity<ReviewId>
{
    private Review() : base(ReviewId.New)
    {
    }
    private Review(UserId userId, ProductId productId, Comment comment, Rating rating, string userName, string avatarUrl) : base(ReviewId.New)
    {
        UserId = userId;
        ProductId = productId;
        Comment = comment;
        Rating = rating;
        UserName = userName;
        AvatarUrl = avatarUrl;
    }

    public UserId UserId { get; private init; }
    public string UserName { get; private init; }
    public string AvatarUrl { get; private init; }
    public Comment Comment { get; private set; }

    public Rating Rating { get; private set; }

    public ProductId ProductId { get; private init; }

    public Product Product { get; private init; }

    public static Fin<Review> Create(UserId userId, ProductId productId, string comment, double rating, string userName, string avatarUrl)
    {
        return (Comment.From(comment), Rating.From(rating))
            .Apply((c, r) => new Review(userId, productId, c, r, userName, avatarUrl)).As();
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