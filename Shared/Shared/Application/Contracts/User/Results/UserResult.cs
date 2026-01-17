namespace Shared.Application.Contracts.User.Results;
public record UserResult
{
    public Guid Id { get; init; }
    public string Email { get; init; }
    public string? Phone { get; init; }
    public string FirstName { get; init; }
    public string LastName { get; init; }
    public int? Age { get; init; }
    public string? Avatar { get; init; }
    public string? Gender { get; init; }
    public bool IsVerified { get; init; }
    public Guid? CartId { get; init; }
    public IEnumerable<AddressResult> Addresses { get; init; }
    public IEnumerable<RoleResult> Roles { get; init; }
    public IEnumerable<string> Permissions { get; init; }

    public IEnumerable<string> ProductSubscriptions { get; init; }

    public IEnumerable<Guid> LikedProductIds { get; init; }
}