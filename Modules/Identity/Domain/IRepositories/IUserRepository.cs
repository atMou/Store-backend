using Identity.Persistence;

namespace Identity.Domain.IRepositories;

public interface IUserRepository
{
    IO<User> GetUserById(UserId userId, UserDbContext ctx);
    IO<Unit> AddUser(User user, UserDbContext ctx);
    IO<User> GetUserByEmail(Email email, UserDbContext ctx);
    IO<Unit> CheckIfUserExists(Email email, UserDbContext ctx);

}
