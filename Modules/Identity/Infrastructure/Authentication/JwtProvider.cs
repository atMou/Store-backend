using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

using Identity.Identity.Infrastructure.Authentication;

using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

using Shared.Infrastructure.Authentication;

namespace Identity.Infrastructure.Authentication;

internal sealed class JwtProvider : IJwtProvider
{
    private readonly JwtOptions _options;

    public JwtProvider(IOptions<JwtOptions> options)
    {
        _options = options.Value;
    }

    public async Task<string> GenerateAsync(User user)
    {
        var claims = new List<Claim>
        {
            new(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
            new(JwtRegisteredClaimNames.Email, user.Email.Value),

        };

        var roles = user.Roles.Select(r => r.Name).Distinct().ToList();

        var permissions = user.Roles
            .SelectMany(r => r.Permissions)
            .Select(p => p.Name)
            .Distinct().ToList();


        foreach (var role in roles)
        {
            claims.Add(new(ClaimTypes.Role, role));
        }

        foreach (string permission in permissions)
        {
            claims.Add(new(Claims.Permissions, permission));
        }

        var signingCredentials = new SigningCredentials(
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(_options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            _options.Issuer,
            _options.Audience,
            claims,
            null,
            DateTime.UtcNow.AddHours(1),
            signingCredentials);

        string tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }
}
