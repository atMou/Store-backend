namespace Identity.Application.Features.AddPendingOrder;
public record AddPendingOrderCommand : ICommand<Fin<Unit>>
{
    public OrderId OrderId { get; set; }
    public UserId UserId { get; set; }
}
internal class AddPendingOrderCommandHandler(IUserContext userContext, IdentityDbContext dbContext)
    : ICommandHandler<AddPendingOrderCommand, Fin<Unit>>
{
    public async Task<Fin<Unit>> Handle(AddPendingOrderCommand command, CancellationToken cancellationToken)
    {
        var db = from _ in userContext.IsSameUser<IO>(command.UserId, UnAuthorizedError.New("You are not authorized to continue this process")).As()
                 from a in GetUpdateEntity<IdentityDbContext, User>(
                     u => u.Id == command.UserId,
                     NotFoundError.New($"User with id '{command.UserId}' does not exist."),
                     opt =>
                     {
                         opt.AddInclude(u => u.PendingOrderIds);
                         return opt;
                     },
                     user => user.AddPendingOrder(PendingOrderId.Create(command.UserId, command.OrderId))
                 )
                 select unit;
        return await db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

