namespace Identity.Presentation.Requests;

public record AssignRoleRequest
{
    public Guid UserId { get; init; }
    public string Role { get; init; }
}