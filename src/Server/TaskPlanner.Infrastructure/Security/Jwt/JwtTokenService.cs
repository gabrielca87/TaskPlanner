using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Interfaces.Services;

namespace TaskPlanner.Infrastructure.Security.Jwt;

public sealed class JwtTokenService : IJwtTokenService
{
    private readonly JwtOptions _options;
    private readonly JwtSecurityTokenHandler _tokenHandler;

    public JwtTokenService(IOptions<JwtOptions> options)
    {
        _options = options.Value;
        _tokenHandler = new JwtSecurityTokenHandler();
    }

    public string GenerateToken(UserDto user)
    {
        var now = DateTime.UtcNow;
        var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_options.Key));
        var signingCredentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
            new Claim(ClaimTypes.Name, user.DisplayName),
            new Claim(ClaimTypes.Email, user.Email)
        };

        var token = new JwtSecurityToken(
            issuer: _options.Issuer,
            audience: _options.Audience,
            claims: claims,
            notBefore: now,
            expires: now.AddMinutes(_options.ExpiresMinutes),
            signingCredentials: signingCredentials);

        return _tokenHandler.WriteToken(token);
    }
}
