namespace Product.Domain.Models;

public record Review : Entity<ReviewId>, IEqualityComparer<Review>
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


    public Fin<Review> Update(string? comment = null, double? rating = null)
    {
        var newComment = Optional(comment).Match(Comment.From, FinSucc(Comment));
        var newRating = Optional(rating).Match(Rating.From, FinSucc(Rating));
        return (newComment, newRating)
            .Apply((c, r) => this with
            {
                Comment = c,
                Rating = r
            }).As();
    }

    public bool Equals(Review? x, Review? y)
    {

        return (x, y) switch
        {
            (null, null) => false,
            (null, _) => false,
            (_, null) => false,
            var (_x, _y) when _x.GetType() != _y.GetType() => false,
            var (_x, _y) when ReferenceEquals(_x, _y) => true,
            var (_x, _y) when _x.Id.Value == _y.Id.Value => true,
            _ => false
        };

    }

    public int GetHashCode(Review obj)
    {
        return obj.Id.GetHashCode();
    }
}