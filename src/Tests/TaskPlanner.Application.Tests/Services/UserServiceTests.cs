using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Moq;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Exceptions;
using TaskPlanner.Application.Requests.Users;
using TaskPlanner.Application.Services;
using TaskPlanner.Domain.Entities;
using TaskPlanner.Domain.Interfaces.Repositories;

namespace TaskPlanner.Application.Tests.Services;

public class UserServiceTests
{
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IPasswordHasher<User>> _passwordHasherMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _passwordHasherMock = new Mock<IPasswordHasher<User>>();
        _mapperMock = new Mock<IMapper>();
        _userService = new UserService(
           _userRepositoryMock.Object,
           _passwordHasherMock.Object,
           _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserExists_ReturnsMappedUserDto()
    {
        // Arrange
        var user = CreateUser();
        var expectedDto = new UserDto
        {
            Id = user.Id,
            Email = user.Email,
            DisplayName = user.DisplayName
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(user.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(user);

        _mapperMock
            .Setup(m => m.Map<UserDto>(user))
            .Returns(expectedDto);

        // Act
        var result = await _userService.GetByIdAsync(user.Id);

        // Assert
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Email, result.Email);
        Assert.Equal(expectedDto.DisplayName, result.DisplayName);

        _mapperMock.Verify(m => m.Map<UserDto>(user), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _userService.GetByIdAsync(Guid.NewGuid()));

        _mapperMock.Verify(m => m.Map<UserDto>(It.IsAny<User>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenEmailAlreadyExists_ThrowsConflictException()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "existing@taskplanner.dev",
            DisplayName = "Existing User",
            Password = "hash"
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync(CreateUser());

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(
            () => _userService.CreateAsync(request));

        _passwordHasherMock.Verify(
            h => h.HashPassword(It.IsAny<User>(), It.IsAny<string>()),
            Times.Never);

        _userRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_HashesPasswordPersistsUserAndReturnsDto()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "new@taskplanner.dev",
            DisplayName = "New User",
            Password = "plain-text-password"
        };
        var mappedUser = new User
        {
            Email = request.Email,
            DisplayName = request.DisplayName,
            PasswordHash = string.Empty
        };

        const string expectedPasswordHash = "hashed-password";

        var expectedDto = new UserDto
        {
            Id = Guid.NewGuid(),
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        _userRepositoryMock
            .Setup(r => r.GetByEmailAsync(request.Email, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        _mapperMock
            .Setup(m => m.Map<User>(request))
            .Returns(mappedUser);

        _passwordHasherMock
            .Setup(h => h.HashPassword(It.IsAny<User>(), request.Password))
            .Returns(expectedPasswordHash);

        User? persistedUser = null;
        _userRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()))
            .Callback<User, CancellationToken>((user, _) => persistedUser = user)
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<UserDto>(It.IsAny<User>()))
            .Returns(expectedDto);
        // Act
        var result = await _userService.CreateAsync(request);

        // Assert
        Assert.NotNull(persistedUser);
        Assert.NotEqual(Guid.Empty, persistedUser!.Id);
        Assert.Equal(request.Email, persistedUser.Email);
        Assert.Equal(request.DisplayName, persistedUser.DisplayName);
        Assert.Equal(expectedPasswordHash, persistedUser.PasswordHash);

        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Email, result.Email);

        _passwordHasherMock.Verify(
            h => h.HashPassword(It.IsAny<User>(), request.Password),
            Times.Once);

        _userRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<User>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            m => m.Map<UserDto>(It.IsAny<User>()),
            Times.Once);
    }

    private static User CreateUser()
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = "user@taskplanner.dev",
            DisplayName = "Planner User",
            PasswordHash = "hash-value",
            CreatedAtUtc = new DateTime(2026, 3, 8, 9, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 8, 9, 0, 0, DateTimeKind.Utc),
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        };
    }
}
