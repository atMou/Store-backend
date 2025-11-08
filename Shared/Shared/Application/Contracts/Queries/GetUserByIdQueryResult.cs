using Shared.Domain.Contracts.User;

namespace Shared.Application.Contracts.Queries;

public record GetUserByIdQueryResult(UserDto userDto)
{
}

