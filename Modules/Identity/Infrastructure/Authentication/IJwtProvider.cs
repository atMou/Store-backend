namespace Identity.Infrastructure.Authentication;


public interface IJwtProvider
{
    string Generate(User user, JwtOptions options, TimeSpan? duration = null);
}
