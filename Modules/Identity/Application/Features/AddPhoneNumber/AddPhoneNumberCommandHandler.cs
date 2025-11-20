namespace Identity.Application.Features.AddPhoneNumber;

public record AddPhoneNumberCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; }
    public string PhoneNumber { get; init; }
}


public class AddPhoneNumberCommandHandler(
    IOptions<JwtOptions> options,
    IClock clock,
    IdentityDbContext dbContext)
    : ICommandHandler<AddPhoneNumberCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(AddPhoneNumberCommand command, CancellationToken cancellationToken)
    {
        var db = from phone in Phone.From(command.PhoneNumber)
                 from user in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                     await ctx.Users.FirstOrDefaultAsync(user => user.Id == command.UserId, e.Token))
                 from _1 in when(user is null,
                     IO.fail<Unit>(NotFoundError.New($"User with id: '{command.UserId}' does not exists"))).As()

                 from userUpdated in IO.lift(() => user.SetPhone(phone, clock.UtcNow))
                 from _2 in Db<IdentityDbContext>.lift(ctx =>
                     {
                         ctx.Users.Entry(user).CurrentValues.SetValues(userUpdated);
                         return unit;
                     })
                 select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }


}

