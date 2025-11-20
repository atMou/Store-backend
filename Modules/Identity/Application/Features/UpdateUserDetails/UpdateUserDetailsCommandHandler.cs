namespace Identity.Application.Features.UpdateUserDetails;

public record UpdateUserDetailsCommand : ICommand<Fin<Unit>>
{
    public UserId UserId { get; init; } = null!;
    public IFormFile? Image { get; init; }
    public string? FirstName { get; init; }
    public string? LastName { get; init; }
    public Address? Address { get; init; }
    public string? Phone { get; init; }
    public string? Email { get; init; }
    public string? Password { get; init; }
    public string? Gender { get; init; }
    public byte? Age { get; init; }
}

public class UpdateUserDetailsCommandHandler(
    IOptions<JwtOptions> options,
    IClock clock,
    IdentityDbContext dbContext,
    IImageService imageService,
    IUserContext userContext)
    : ICommandHandler<UpdateUserDetailsCommand, Fin<Unit>>
{
    public Task<Fin<Unit>> Handle(UpdateUserDetailsCommand command, CancellationToken cancellationToken)
    {
        var db =

            from user in Db<IdentityDbContext>.liftIO(async (ctx, e) =>
                await ctx.Users.FirstOrDefaultAsync(user => user.Id == command.UserId, e.Token))
            from _1 in when(user is null,
                IO.fail<Unit>(NotFoundError.New($"User with id: '{command.UserId}' does not exists"))).As()
            from image in Optional(command.Image).Match<IO<ImageUrl>>(file =>
                imageService.UploadImage(file, user.Id.Value.ToString()), () => IO.pure<ImageUrl>(null!))
            from userUpdated in IO.lift(() => user.Update(command.Email,
                command.FirstName, command.LastName, command.Age, command.Password, command.Gender, command.Address,
                command.Phone, image))
            from _2 in Db<IdentityDbContext>.lift(ctx =>
            {
                ctx.Users.Entry(user).CurrentValues.SetValues(userUpdated);
                return unit;
            })
            select unit;
        return db.RunSaveAsync(dbContext, EnvIO.New(null, cancellationToken));
    }
}