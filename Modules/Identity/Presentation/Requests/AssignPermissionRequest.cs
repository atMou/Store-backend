namespace Identity.Presentation.Requests;

public record AssignPermissionRequest
{
	public Guid UserId { get; init; }
	public IEnumerable<string> Permissions { get; init; }
}