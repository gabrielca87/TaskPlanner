using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Requests.Users;

namespace TaskPlanner.Application.Interfaces.Services;

public interface IUserService
{
    Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default);
}
