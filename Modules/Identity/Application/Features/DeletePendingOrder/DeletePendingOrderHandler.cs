namespace Identity.Application.Features.DeletePendingOrder;
public record DeletePendingOrderCommand : ICommand<Fin<Unit>>
{
    public OrderId OrderId { get; set; }
}
internal class DeletePendingOrderHandler(IUserContext userContext, IdentityDbContext dbContext) : ICommandHandler<DeletePendingOrderCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeletePendingOrderCommand command, CancellationToken cancellationToken)
    {
        var db = from userId in GetCurrentUserId()
                 from a in GetUpdateEntityA<IdentityDbContext, User>(
                     u => u.Id == userId,
                     NotFoundError.New($"User with id '{userId}' does not exist."),
                     opt =>
                     {
                         opt.AddInclude(u => u.LikedProducts);
                         return opt;
                     },
                     user => user.RemovePendingOrder(PendingOrderId.Create(user.Id, command.OrderId))
                 )
                 select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private Fin<UserId> GetCurrentUserId() => userContext.GetCurrentUserF<Fin>()
        .As().MapFail(_ => UnAuthorizedError.New("You are not logged in to delete pending orders."))
        .Map(u => UserId.From(u.Id));
}

