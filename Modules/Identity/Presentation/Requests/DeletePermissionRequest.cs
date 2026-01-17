namespace Identity.Presentation.Requests;

public record DeletePermissionRequest
{
    public Guid UserId { get; init; }
    public IEnumerable<string> Permissions { get; init; }
}