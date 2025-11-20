

using Identity.Domain.Contracts;

namespace Identity.Application.Features.GetUserById;


//public record GetUserByIdQuery(UserId UserId) : IQuery<Fin<GetUserByIdQueryResult>>;
public class GetUserByIdQueryHandler(IUserRepository userRepository, IdentityDbContext dbContext)
    : IQueryHandler<GetUserByIdQuery, Fin<UserDto>>
{

    public Task<Fin<UserDto>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var db = from u in Db<IdentityDbContext>.liftIO((ctx)
                => userRepository.GetUserById(query.UserId, ctx))
                 select u.ToDto();

        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}
