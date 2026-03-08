using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Exceptions;
using TaskPlanner.Application.Interfaces.Services;
using TaskPlanner.Application.Requests.Users;
using TaskPlanner.Application.Services;
using TaskPlanner.Domain.Entities;
using TaskPlanner.Domain.Interfaces.Repositories;

namespace TaskPlanner.Application.Tests.Services;

public class AuthServiceTests
{
    private readonly Mock<IUserService> _userServiceMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly Mock<IJwtTokenService> _jwtTokenServiceMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _userServiceMock = new Mock<IUserService>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _jwtTokenServiceMock = new Mock<IJwtTokenService>();
        _mapperMock = new Mock<IMapper>();
        _authService = new AuthService(
            _userServiceMock.Object,
            _userRepositoryMock.Object,
            _passwordHasherMock.Object,
            _jwtTokenServiceMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenUserIsCreated_ReturnsAuthResponseWithAccessToken()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "new@taskplanner.dev",
            DisplayName = "New User",
            Password = "P@ssw0rd!"
        };

        var createdUser = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        _userServiceMock
            .Setup(service => service.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdUser);
        _jwtTokenServiceMock
            .Setup(service => service.GenerateToken(createdUser))
            .Returns("generated-jwt-token");

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.Equal("generated-jwt-token", result.AccessToken);
        Assert.Equal(createdUser.Id, result.UserId);
        Assert.Equal(createdUser.Email, result.Email);
        Assert.Equal(createdUser.DisplayName, result.DisplayName);
        _userServiceMock.Verify(service => service.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
        _jwtTokenServiceMock.Verify(service => service.GenerateToken(createdUser), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenUserDoesNotExist_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "missing@taskplanner.dev",
            Password = "P@ssw0rd!"
        };

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));

        _jwtTokenServiceMock.Verify(service => service.GenerateToken(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenPasswordDoesNotMatch_ThrowsUnauthorizedException()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "planner@taskplanner.dev",
            Password = "wrong-password"
        };

        var user = CreatePersistedUser();

        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password))
            .Returns(PasswordVerificationResult.Failed);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(() => _authService.LoginAsync(request));

        _jwtTokenServiceMock.Verify(service => service.GenerateToken(It.IsAny<UserDto>()), Times.Never);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsAuthResponseWithAccessToken()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "planner@taskplanner.dev",
            Password = "Planner@123"
        };

        var user = CreatePersistedUser();
        var userDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName
        };
        _userRepositoryMock
            .Setup(repository => repository.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);
        _passwordHasherMock
            .Setup(hasher => hasher.VerifyHashedPassword(user, user.PasswordHash, request.Password))
            .Returns(PasswordVerificationResult.Success);
        _mapperMock
            .Setup(mapper => mapper.Map<UserDto>(user))
            .Returns(userDto);
        _jwtTokenServiceMock
            .Setup(service => service.GenerateToken(userDto))
            .Returns("valid-jwt-token");

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Equal("valid-jwt-token", result.AccessToken);
        Assert.Equal(user.Id, result.UserId);
        Assert.Equal(user.Email, result.Email);
        _jwtTokenServiceMock.Verify(service => service.GenerateToken(userDto), Times.Once);
    }

    private static User CreatePersistedUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = "planner@taskplanner.dev",
            DisplayName = "Planner",
            PasswordHash = "persisted-hash",
            CreatedAtUtc = new DateTime(2026, 3, 8, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 8, 10, 0, 0, DateTimeKind.Utc),
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        };
    }
}
