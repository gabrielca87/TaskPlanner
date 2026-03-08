using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Exceptions;
using TaskPlanner.Application.Interfaces.Services;
using TaskPlanner.Application.Requests.Users;
using TaskPlanner.Domain.Entities;
using TaskPlanner.Domain.Interfaces.Repositories;

namespace TaskPlanner.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IMapper _mapper;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<UserDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(id, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", id.ToString());
        }

        return _mapper.Map<UserDto>(user);
    }

    public async Task<UserDto> CreateAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var existingUser = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (existingUser is not null)
        {
            throw new ConflictException($"User with email '{request.Email}' already exists.");
        }

        var user = _mapper.Map<User>(request);
        var utcNow = DateTime.UtcNow;
        user.Id = Guid.NewGuid();
        user.CreatedAtUtc = utcNow;
        user.UpdatedAtUtc = utcNow;
        user.CreatedBy = Guid.Empty; // TODO: Replace with actual user ID when authentication is implemented
        user.UpdatedBy = Guid.Empty; // TODO: Replace with actual user ID when authentication is implemented
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);

        await _userRepository.AddAsync(user, cancellationToken);

        return _mapper.Map<UserDto>(user);
    }
}
