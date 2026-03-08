using AutoMapper;
using Microsoft.AspNetCore.Identity;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Exceptions;
using TaskPlanner.Application.Interfaces.Services;
using TaskPlanner.Application.Requests.Users;
using TaskPlanner.Domain.Entities;
using TaskPlanner.Domain.Interfaces.Repositories;

namespace TaskPlanner.Application.Services;

public class AuthService : IAuthService
{
    private const string InvalidCredentialsMessage = "Invalid email or password.";

    private readonly IUserService _userService;
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher<User> _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;
    private readonly IMapper _mapper;

    public AuthService(
        IUserService userService,
        IUserRepository userRepository,
        IPasswordHasher<User> passwordHasher,
        IJwtTokenService jwtTokenService,
        IMapper mapper)
    {
        _userService = userService;
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
        _mapper = mapper;
    }

    public async Task<AuthResponseDto> RegisterAsync(CreateUserRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userService.CreateAsync(request, cancellationToken);
        return BuildAuthResponse(user);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByEmailAsync(request.Email, cancellationToken);
        if (user is null)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var passwordVerification = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password);
        if (passwordVerification == PasswordVerificationResult.Failed)
        {
            throw new UnauthorizedException(InvalidCredentialsMessage);
        }

        var userDto = _mapper.Map<UserDto>(user);
        return BuildAuthResponse(userDto);
    }

    private AuthResponseDto BuildAuthResponse(UserDto user)
    {
        return new AuthResponseDto
        {
            AccessToken = _jwtTokenService.GenerateToken(user),
            UserId = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName
        };
    }
}
