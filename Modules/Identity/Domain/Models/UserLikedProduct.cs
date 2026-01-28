namespace Identity.Domain.Models;

public class UserLikedProduct
{
    public UserId UserId { get; set; }
    public ProductId ProductId { get; set; }
    public DateTime LikedAt { get; set; }
    
    public User User { get; set; }
}
