namespace Identity.Infrastructure.Authentication;

internal sealed class JwtProvider : IJwtProvider
{

    public string Generate(User user, JwtOptions options, TimeSpan? duration = null)
    {
        var claims = new List<Claim>
    {
        new(JwtRegisteredClaimNames.Sub, user.Id.Value.ToString()),
        new(JwtRegisteredClaimNames.Email, user.Email.Value),

    };

        var roles = user.Roles.Select(r => r.Name).Distinct().ToList();

        var permissions = user.Permissions.Select(p => p.Name).Concat(
            user.Roles.SelectMany(r => r.Permissions)
                .Select(p => p.Name)
        ).Distinct().ToList();

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
                Encoding.UTF8.GetBytes(options.SecretKey)),
            SecurityAlgorithms.HmacSha256);

        var d = Optional(duration).Match(span => DateTime.UtcNow.Add(span),
            () => DateTime.UtcNow.AddDays(2));
        var token = new JwtSecurityToken(
            options.Issuer,
            options.Audience,
            claims,
            null,
            d,
            signingCredentials);

        Console.WriteLine($"The Token Is going to expire in: {(d - DateTime.Now).TotalMinutes} minutes");

        string tokenValue = new JwtSecurityTokenHandler()
            .WriteToken(token);

        return tokenValue;
    }
}


