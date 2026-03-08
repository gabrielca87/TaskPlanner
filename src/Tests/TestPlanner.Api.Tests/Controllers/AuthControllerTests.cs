using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskPlanner.Api.Controllers;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Exceptions;
using TaskPlanner.Application.Interfaces.Services;
using TaskPlanner.Application.Requests.Users;

namespace TaskPlanner.Api.Tests.Controllers;

public class AuthControllerTests
{
    private readonly Mock<IAuthService> _authServiceMock;
    private readonly AuthController _authController;

    public AuthControllerTests()
    {
        _authServiceMock = new Mock<IAuthService>();
        _authController = new AuthController(_authServiceMock.Object);
    }

    [Fact]
    public async Task RegisterAsync_WhenRequestIsValid_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "new@taskplanner.dev",
            DisplayName = "New User",
            Password = "P@ssw0rd!"
        };

        var expectedResponse = new AuthResponseDto
        {
            AccessToken = "access-token",
            UserId = Guid.NewGuid(),
            Email = request.Email,
            DisplayName = request.DisplayName
        };

        _authServiceMock
            .Setup(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var actionResult = await _authController.RegisterAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var payload = Assert.IsType<AuthResponseDto>(okResult.Value);

        Assert.Equal(expectedResponse.AccessToken, payload.AccessToken);
        Assert.Equal(expectedResponse.UserId, payload.UserId);
        Assert.Equal(expectedResponse.Email, payload.Email);

        _authServiceMock.Verify(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task RegisterAsync_WhenServiceThrowsConflictException_ExceptionPropagates()
    {
        // Arrange
        var request = new CreateUserRequest
        {
            Email = "existing@taskplanner.dev",
            DisplayName = "Existing User",
            Password = "P@ssw0rd!"
        };

        _authServiceMock
            .Setup(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new ConflictException("Email already in use."));

        // Act & Assert
        await Assert.ThrowsAsync<ConflictException>(
            () => _authController.RegisterAsync(request, CancellationToken.None));

        _authServiceMock.Verify(s => s.RegisterAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreValid_ReturnsOkWithAuthResponse()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "planner@taskplanner.dev",
            Password = "Planner@123"
        };

        var expectedResponse = new AuthResponseDto
        {
            AccessToken = "access-token",
            UserId = Guid.NewGuid(),
            Email = request.Email,
            DisplayName = "Planner"
        };

        _authServiceMock
            .Setup(s => s.LoginAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var actionResult = await _authController.LoginAsync(request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var payload = Assert.IsType<AuthResponseDto>(okResult.Value);

        Assert.Equal(expectedResponse.AccessToken, payload.AccessToken);
        Assert.Equal(expectedResponse.Email, payload.Email);
        Assert.Equal(expectedResponse.DisplayName, payload.DisplayName);

        _authServiceMock.Verify(s => s.LoginAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task LoginAsync_WhenCredentialsAreInvalid_ExceptionPropagates()
    {
        // Arrange
        var request = new LoginRequest
        {
            Email = "planner@taskplanner.dev",
            Password = "wrong-password"
        };

        _authServiceMock
            .Setup(s => s.LoginAsync(request, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new UnauthorizedException("Invalid credentials."));

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _authController.LoginAsync(request, CancellationToken.None));

        _authServiceMock.Verify(s => s.LoginAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }
}