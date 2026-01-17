using Identity.Application.Contracts;

namespace Identity.Application.Features.GetUser;

public class GetUserByIdQueryHandler(IdentityDbContext dbContext, ISender sender)
    : IQueryHandler<GetUserByIdQuery, Fin<UserResult>>
{
    public async Task<Fin<UserResult>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var db = GetEntity<IdentityDbContext, User>(
            user => user.Id == query.UserId,
            NotFoundError.New($"User with id: '{query.UserId}' does not exist"),
            opt =>
            {
                opt.AddInclude(u => u.LikedProductIds);
                return opt;
            }
        ).Map(u => u.ToResult());


        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}