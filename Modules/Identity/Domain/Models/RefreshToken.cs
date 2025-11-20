
namespace Identity.Domain.Models;
public record RefreshToken
{
    private RefreshToken()
    {
    }

    public Guid Id { get; init; }

    public UserId UserId { get; init; }

    public string TokenHash { get; init; }
    [NotMapped]
    public string RawToken { get; init; }

    public DateTime CreatedAt { get; init; }
    public DateTime ExpiresAt { get; init; }

    public DateTime? RevokedAt { get; private set; }
    public string? RevokedReason { get; private set; }

    public bool IsExpired(DateTime utcNow) => utcNow >= ExpiresAt;
    public bool IsRevoked { get; private set; }


    public static RefreshToken Generate(UserId userId, string salt, DateTime dateTime)
    {
        var token = Helpers.GenerateRefreshToken();
        var hashed = Helpers.Hash(token, salt);

        return new RefreshToken()
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            TokenHash = hashed,
            RawToken = token,
            CreatedAt = dateTime,
            ExpiresAt = dateTime.AddDays(7),
        };
    }


    public Unit Revoke(string reason, DateTime dateTime)
    {
        RevokedAt = dateTime;
        RevokedReason = reason;
        IsRevoked = true;
        return unit;
    }
    public virtual bool Equals(RefreshToken? other)
    {
        return other is { } && TokenHash == other.TokenHash && UserId == other.UserId;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(TokenHash.GetHashCode(), UserId.Value.GetHashCode());
    }
}