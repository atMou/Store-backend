namespace Identity.Application.Features.GetUser;

public record GetUserByEmailQuery : IQuery<Fin<Unit>>
{
    public string Email { get; set; }
}

public class GetUserByEmailQueryHandler(IdentityDbContext dbContext, ISender sender)
    : IQueryHandler<GetUserByEmailQuery, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(GetUserByEmailQuery query, CancellationToken cancellationToken)
    {
        var db =
            from email in Email.From(query.Email)
            from result in GetEntity<IdentityDbContext, User>(
                user => user.Email == email,
                NotFoundError.New($"User with email: '{query.Email}' does not exist")
            )
            select unit;

        return await db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}