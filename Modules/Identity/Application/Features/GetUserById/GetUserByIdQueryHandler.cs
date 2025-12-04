using Identity.Application.Contracts;

namespace Identity.Application.Features.GetUserById;

public class GetUserByIdQueryHandler(IdentityDbContext dbContext, ISender sender)
    : IQueryHandler<GetUserByIdQuery, Fin<UserResult>>
{
    public Task<Fin<UserResult>> Handle(GetUserByIdQuery query, CancellationToken cancellationToken)
    {
        var db = GetEntity<IdentityDbContext, User>(
            user => user.Id == query.UserId,
            NotFoundError.New($"User with id: '{query.UserId}' does not exist"),
            opt =>
            {
                opt.AddInclude(u => u.LikedProducts);
                opt.AddInclude(u => u.Addresses);
                opt.AddInclude(u => u.Roles);
                opt.AddInclude(u => u.Permissions);
                return opt;
            }
        ).Map(u => u.ToResult());
        //from ps in IO.liftAsync(async e =>
        //{
        //    return await sender.Send(new GetProductsByIdsQuery
        //    {
        //        ProductIds = u.LikedProducts.Select(id => id.ProductId),
        //        Include = "reviews,variants,images",
        //        PageNumber = 1,
        //        PageSize = 20
        //    }, e.Token);
        //})
        //from p in ps
        //select u.ToResult() with
        //{
        //    LikedProducts = p.Items.ToList()
        //};


        return db.RunAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}