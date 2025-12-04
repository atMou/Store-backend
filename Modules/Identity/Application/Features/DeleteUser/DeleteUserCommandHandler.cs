namespace Identity.Application.Features.DeleteUser;
public record DeleteUserCommand(Guid UserId) : ICommand<Fin<Unit>>;

public class SetUserCartIdCommandHandler(IOptions<JwtOptions> options, IClock clock, IdentityDbContext dbContext, IUserContext userContext)
    : ICommandHandler<DeleteUserCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(DeleteUserCommand command, CancellationToken cancellationToken)
    {
        var db =

            from _ in EnsureIsSameUserOrAdmin(UserId.From(command.UserId))
            from _1 in GetUpdateEntity<IdentityDbContext, User>(
                user => user.Id == UserId.From(command.UserId),
                NotFoundError.New($"User with id: '{command.UserId}' does not exists"),
                opt =>
                {
                    opt.AddInclude(user => user.PendingOrderIds);
                    return opt;
                }, user => user.HasNoPendingOrders(),
                user => user.MarkAsDeleted())

            select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }

    private IO<Unit> EnsureIsSameUserOrAdmin(UserId userId) =>
        userContext.IsSameUser<IO>(userId, UnAuthorizedError.New($"You are not Authorized to Delete User Cart Id"))
        | userContext.IsInRole<IO>(Role.Admin, UnAuthorizedError.New($"You are not Authorized to Delete User Cart Id"))
            .As();


}
