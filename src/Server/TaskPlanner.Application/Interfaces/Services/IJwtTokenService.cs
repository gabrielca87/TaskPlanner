using TaskPlanner.Application.DTOs;

namespace TaskPlanner.Application.Interfaces.Services;

public interface IJwtTokenService
{
    string GenerateToken(UserDto user);
}
