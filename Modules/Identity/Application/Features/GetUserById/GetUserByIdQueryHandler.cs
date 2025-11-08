

using Identity.Domain.Contracts;
using Identity.Persistence;

namespace Identity.Application.Features.GetUserById;


public record GetUserByIdQuery(UserId UserId) : IQuery<Fin<GetUserByIdQueryResult>>;
public class GetUserByIdQueryHandler(IUserRepository userRepository)
    : IQueryHandler<GetUserByIdQuery, Fin<GetUserByIdQueryResult>>
{

    public Task<Fin<GetUserByIdQueryResult>> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        var db = from u in Db<UserDbContext>.liftIO((ctx) => userRepository.GetUserById(request.UserId, ctx))
                 select new GetUserByIdQueryResult(u.ToDto());

        return db.RunAsync(null, EnvIO.New(null, cancellationToken));
    }
}
