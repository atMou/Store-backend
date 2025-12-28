namespace Shared.Application.Contracts.User.Results;

public record RoleResult
{
	public string Name { get; init; }
	public IEnumerable<string> Permissions { get; init; }
}