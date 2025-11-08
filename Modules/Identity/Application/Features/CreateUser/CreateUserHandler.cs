using Identity.Domain.Contracts;
using Identity.Persistence;

using Microsoft.AspNetCore.Identity;

namespace Identity.Application.Features.CreateUser;

public record CreateUserCommand(
    CreateUserDto CreateUserDto) : ICommand<Fin<CreateUserCommandResult>>;

public record CreateUserCommandResult(UserId UserId, string Email);

internal class CreateUserCommandHandler(
    UserDbContext userDbContext,
    IUserRepository userRepository
    ) : ICommandHandler<CreateUserCommand, Fin<CreateUserCommandResult>>
{
    public Task<Fin<CreateUserCommandResult>> Handle(CreateUserCommand command, CancellationToken cancellationToken)
    {
        var db = from user in User.Create(command.CreateUserDto)
                 from _ in Db<UserDbContext>.liftIO(ctx => userRepository.CheckIfUserExists(user.Email, ctx))
                 let userWithHashedPassword = HashPassword(user)
                 from _1 in Db<UserDbContext>.liftIO(ctx => userRepository.AddUser(userWithHashedPassword, ctx))
                 select new CreateUserCommandResult(userWithHashedPassword.Id, userWithHashedPassword.Email.Value);
        return db.RunSave(userDbContext, EnvIO.New(null, cancellationToken));
    }

    private static User HashPassword(User user)
    {
        var hasher = new PasswordHasher<User>();
        return user.SetHashedPassword(hasher.HashPassword(user, user.Password.Value));

    }

}
