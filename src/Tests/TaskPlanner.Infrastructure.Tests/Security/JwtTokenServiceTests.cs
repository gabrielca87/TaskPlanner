using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Options;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Infrastructure.Security.Jwt;

namespace TaskPlanner.Infrastructure.Tests.Security;

public class JwtTokenServiceTests
{
    [Fact]
    public void GenerateToken_WhenUserIsProvided_ContainsIdentityClaimsAndExpiration()
    {
        var options = Options.Create(new JwtOptions
        {
            Issuer = "TaskPlanner.Tests",
            Audience = "TaskPlanner.Tests.Client",
            Key = "TaskPlanner-Tests-Signing-Key-That-Is-Long-Enough-2026",
            ExpiresMinutes = 30
        });
        var service = new JwtTokenService(options);
        var user = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = "planner@taskplanner.dev",
            DisplayName = "Planner"
        };

        var token = service.GenerateToken(user);

        var parsedToken = new JwtSecurityTokenHandler().ReadJwtToken(token);
        Assert.Equal(options.Value.Issuer, parsedToken.Issuer);
        Assert.Contains(options.Value.Audience, parsedToken.Audiences);
        Assert.Equal(user.Id.ToString(), parsedToken.Claims.Single(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
        Assert.Equal(user.Email, parsedToken.Claims.Single(claim => claim.Type == ClaimTypes.Email).Value);
        Assert.Equal(user.DisplayName, parsedToken.Claims.Single(claim => claim.Type == ClaimTypes.Name).Value);

        var expiresAt = parsedToken.ValidTo;
        Assert.True(expiresAt > DateTime.UtcNow);
        Assert.True(expiresAt <= DateTime.UtcNow.AddMinutes(options.Value.ExpiresMinutes + 1));
    }
}
