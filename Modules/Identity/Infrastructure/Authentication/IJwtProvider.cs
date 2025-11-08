namespace Identity.Infrastructure.Authentication;


public interface IJwtProvider
{
    Task<string> GenerateAsync(User user);
}
