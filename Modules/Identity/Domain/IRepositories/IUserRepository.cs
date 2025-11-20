using Identity.Domain.ValueObjects;

namespace Identity.Domain.IRepositories;

public interface IUserRepository
{
    IO<User> GetUserById(UserId userId, IdentityDbContext ctx);
    IO<Unit> AddUser(User user, IdentityDbContext ctx);
    IO<User> GetUserByEmail(Email email, IdentityDbContext ctx);
    IO<Unit> CheckIfUserExists(Email email, IdentityDbContext ctx);

}
