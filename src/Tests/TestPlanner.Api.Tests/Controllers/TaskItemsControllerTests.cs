using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using TaskPlanner.Api.Controllers;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Exceptions;
using TaskPlanner.Application.Interfaces.Services;
using TaskPlanner.Application.Requests.Tasks;

namespace TaskPlanner.Api.Tests.Controllers;

public class TaskItemsControllerTests
{
    private readonly Guid _currentUserId;
    private readonly Mock<ITaskItemService> _taskItemServiceMock;
    private TaskItemsController? _taskItemsController;

    public TaskItemsControllerTests()
    {
        _currentUserId = Guid.NewGuid();
        _taskItemServiceMock = new Mock<ITaskItemService>();
    }

    [Fact]
    public async Task GetByCurrentUserAsync_WhenUserClaimIsValid_ReturnsCurrentUserTasks()
    {
        // Arrange
        IReadOnlyList<TaskItemDto> expectedTasks =
        [
            new TaskItemDto { Id = Guid.NewGuid(), UserId = _currentUserId, Title = "Task one" },
            new TaskItemDto { Id = Guid.NewGuid(), UserId = _currentUserId, Title = "Task two" }
        ];

        _taskItemServiceMock
            .Setup(s => s.GetByUserIdAsync(_currentUserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTasks);

        _taskItemsController = CreateController(_currentUserId);

        // Act
        var actionResult = await _taskItemsController.GetByCurrentUserAsync(CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var payload = Assert.IsAssignableFrom<IReadOnlyList<TaskItemDto>>(okResult.Value);

        Assert.Equal(2, payload.Count);

        _taskItemServiceMock.Verify(s => s.GetByUserIdAsync(_currentUserId, It.IsAny<CancellationToken>()),Times.Once);
    }

    [Fact]
    public async Task GetByCurrentUserAsync_WhenUserClaimIsMissing_ThrowsUnauthorizedException()
    {
        // Arrange
        _taskItemsController = CreateController(userId: null);

        // Act & Assert
        await Assert.ThrowsAsync<UnauthorizedException>(
            () => _taskItemsController.GetByCurrentUserAsync(CancellationToken.None));

        _taskItemServiceMock.Verify(s => s.GetByUserIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskExists_ReturnsOkWithTaskItem()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var expectedTask = new TaskItemDto
        {
            Id = taskId,
            UserId = _currentUserId,
            Title = "Task title"
        };

        _taskItemServiceMock
            .Setup(s => s.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(expectedTask);

        _taskItemsController = CreateController(_currentUserId);

        // Act
        var actionResult = await _taskItemsController.GetByIdAsync(taskId, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var payload = Assert.IsType<TaskItemDto>(okResult.Value);

        Assert.Equal(expectedTask.Id, payload.Id);
        Assert.Equal(expectedTask.Title, payload.Title);

        _taskItemServiceMock.Verify(s => s.GetByIdAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ExceptionPropagates()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _taskItemServiceMock
            .Setup(s => s.GetByIdAsync(taskId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Task not found.", _currentUserId.ToString()));

        var controller = CreateController(_currentUserId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => controller.GetByIdAsync(taskId, CancellationToken.None));
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_ReturnsCreatedAtAction()
    {
        // Arrange
        var request = new CreateTaskItemRequest
        {
            UserId = _currentUserId,
            Title = "Task title",
            Description = "Task description"
        };

        var createdTask = new TaskItemDto
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description
        };

        _taskItemServiceMock
            .Setup(s => s.CreateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(createdTask);

        _taskItemsController = CreateController(_currentUserId);

        // Act
        var actionResult = await _taskItemsController.CreateAsync(request, CancellationToken.None);

        // Assert
        var createdResult = Assert.IsType<CreatedAtActionResult>(actionResult.Result);

        Assert.Equal(nameof(TaskItemsController.GetByIdAsync), createdResult.ActionName);
        Assert.Equal(createdTask.Id, createdResult.RouteValues!["id"]);

        _taskItemServiceMock.Verify(s => s.CreateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenRouteIdMatchesRequestId_ReturnsOkWithUpdatedTask()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var request = new UpdateTaskItemRequest
        {
            Id = taskId,
            Title = "Updated title"
        };

        var updatedTask = new TaskItemDto
        {
            Id = taskId,
            UserId = _currentUserId,
            Title = request.Title
        };

        _taskItemServiceMock
            .Setup(s => s.UpdateAsync(request, It.IsAny<CancellationToken>()))
            .ReturnsAsync(updatedTask);

        _taskItemsController = CreateController(_currentUserId);

        // Act
        var actionResult = await _taskItemsController.UpdateAsync(taskId, request, CancellationToken.None);

        // Assert
        var okResult = Assert.IsType<OkObjectResult>(actionResult.Result);
        var payload = Assert.IsType<TaskItemDto>(okResult.Value);

        Assert.Equal(request.Title, payload.Title);

        _taskItemServiceMock.Verify(s => s.UpdateAsync(request, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskExists_ReturnsNoContent()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _taskItemServiceMock
            .Setup(s => s.DeleteAsync(taskId, It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _taskItemsController = CreateController(_currentUserId);

        // Act
        var result = await _taskItemsController.DeleteAsync(taskId, CancellationToken.None);

        // Assert
        Assert.IsType<NoContentResult>(result);

        _taskItemServiceMock.Verify(s => s.DeleteAsync(taskId, It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskDoesNotExist_ExceptionPropagates()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _taskItemServiceMock
            .Setup(s => s.DeleteAsync(taskId, It.IsAny<CancellationToken>()))
            .ThrowsAsync(new NotFoundException("Task not found.", taskId.ToString()));

        _taskItemsController = CreateController(_currentUserId);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskItemsController.DeleteAsync(taskId, CancellationToken.None));
    }

    private TaskItemsController CreateController(Guid? userId)
    {
        var identity = userId.HasValue
            ? new ClaimsIdentity(
                [new Claim(ClaimTypes.NameIdentifier, userId.Value.ToString())],
                "Test")
            : new ClaimsIdentity();

        var controller = new TaskItemsController(_taskItemServiceMock.Object)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(identity)
                }
            }
        };

        return controller;
    }
}