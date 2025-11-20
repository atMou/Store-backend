namespace Identity.Presentation.Requests;

public record DeleteRoleRequest
{
    public Guid UserId { get; init; }
    public string Role { get; init; }
}