using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Requests.Users;

namespace TaskPlanner.Application.Interfaces.Services;

public interface IAuthService
{
    Task<AuthResponseDto> RegisterAsync(CreateUserRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);
}
