namespace Identity.Application.Features.ToggleLikedProducts;
public record ToggleLikedProductsCommand : ICommand<Fin<Unit>>
{
    public IEnumerable<ProductId> ProductIds { get; set; } = [];

}
internal class ToggleLikedProductsCommandHandler(IUserContext userContext, IdentityDbContext dbContext) : ICommandHandler<ToggleLikedProductsCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(ToggleLikedProductsCommand command, CancellationToken cancellationToken)
    {
        var db = from userId in GetCurrentUserId()
                 from a in GetUpdateEntity<IdentityDbContext, User>(
                     u => u.Id == userId,
                     NotFoundError.New($"User with id '{userId}' does not exist."),
                     opt =>
                     {
                         opt = opt.AddInclude(u => u.LikedProducts);
                         opt = opt.AddAsTracking();
                         return opt;
                     },
                     user => user.ToggleLikedProducts([.. command.ProductIds])
                 )
                 select unit;
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Fin<UserId> GetCurrentUserId() => userContext.GetCurrentUserF<Fin>()
        .As().MapFail(_ => UnAuthorizedError.New("You are not logged in to like products."))
        .Map(u => UserId.From(u.Id));
}

