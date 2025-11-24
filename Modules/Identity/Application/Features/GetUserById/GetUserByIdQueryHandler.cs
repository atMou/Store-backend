

using Identity.Application.Contracts;

namespace Identity.Application.Features.GetUserById;


//public record GetUserByIdQuery(UserId UserId) : IQuery<Fin<GetUserByIdQueryResult>>;
public class GetUserByIdQueryHandler(IUserRepository userRepository, IdentityDbContext dbContext)
    : IQueryHandler<GetUserByIdQuery, Fin<UserResult>>
{

    public Task<Fin<UserResult>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var db = GetEntity<IdentityDbContext, User, UserResult>(
            user => user.Id == query.UserId,
            user => user.ToResult(),
            NotFoundError.New($"User with id: '{query.UserId}' does not exists")
        );

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
