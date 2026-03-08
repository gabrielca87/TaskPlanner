using System.Security.Claims;
using TaskPlanner.Application.Exceptions;

namespace TaskPlanner.Api.Extensions;

public static class ClaimsPrincipalExtensions
{
    public static Guid GetCurrentUserId(this ClaimsPrincipal user)
    {
        var value = user?.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!Guid.TryParse(value, out var userId))
        {
            throw new UnauthorizedException("Authenticated user identity is invalid.");
        }

        return userId;
    }
}
