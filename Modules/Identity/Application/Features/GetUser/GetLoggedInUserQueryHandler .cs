using Identity.Application.Contracts;

namespace Identity.Application.Features.GetUser;
public record GetLoggedInUserQuery : IQuery<Fin<UserResult>>;
public class GetLoggedInQueryHandler(IdentityDbContext dbContext, ISender sender, IUserContext userContext)
    : IQueryHandler<GetLoggedInUserQuery, Fin<UserResult>>
{
    public async Task<Fin<UserResult>> Handle(GetLoggedInUserQuery query, CancellationToken cancellationToken)
    {
        var db =
            from uc in userContext.GetCurrentUserF<Fin>()
                .As()
                .MapFail(_ => UnAuthorizedError.New("You are not logged in to get user profile."))


            from u in GetEntity<IdentityDbContext, User>(
                user => user.Id == UserId.From(uc.Id),
                NotFoundError.New($"User with id '{uc.Id}' does not exist"),
                opt =>
                {
                    opt = opt.AddInclude(u => u.LikedProductIds);
                    opt = opt.AddInclude(u => u.Addresses);
                    opt = opt.AddInclude(u => u.Roles);
                    opt = opt.AddInclude(u => u.Permissions);
                    return opt;
                }
            ).Map(u => u.ToResult())
            select u;


        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}