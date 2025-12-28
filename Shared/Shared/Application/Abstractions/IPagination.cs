namespace Shared.Application.Abstractions;

public interface IPagination
{
	int PageNumber { get; init; }
	int PageSize { get; init; }
}