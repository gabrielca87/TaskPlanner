using AutoMapper;
using Moq;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Exceptions;
using TaskPlanner.Application.Requests.Tasks;
using TaskPlanner.Application.Services;
using TaskPlanner.Domain.Entities;
using TaskPlanner.Domain.Interfaces.Repositories;

namespace TaskPlanner.Application.Tests.Services;

public class TaskItemServiceTests
{
    private readonly Mock<ITaskItemRepository> _taskItemRepositoryMock;
    private readonly Mock<IUserRepository> _userRepositoryMock;
    private readonly Mock<IMapper> _mapperMock;
    private readonly TaskItemService _taskItemService;

    public TaskItemServiceTests()
    {
        _taskItemRepositoryMock = new Mock<ITaskItemRepository>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _mapperMock = new Mock<IMapper>();

        _taskItemService = new TaskItemService(
            _taskItemRepositoryMock.Object,
            _userRepositoryMock.Object,
            _mapperMock.Object);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskItemExists_ReturnsMappedTaskItemDto()
    {
        // Arrange
        var taskItem = CreateTaskItem();
        var expectedDto = new TaskItemDto
        {
            Id = taskItem.Id,
            Title = taskItem.Title,
            UserId = taskItem.UserId
        };

        _taskItemRepositoryMock
            .Setup(r => r.GetByIdAsync(taskItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskItem);

        _mapperMock
            .Setup(m => m.Map<TaskItemDto>(taskItem))
            .Returns(expectedDto);

        // Act
        var result = await _taskItemService.GetByIdAsync(taskItem.Id);

        // Assert
        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Title, result.Title);
        Assert.Equal(expectedDto.UserId, result.UserId);

        _taskItemRepositoryMock.Verify(r => r.GetByIdAsync(taskItem.Id, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TaskItemDto>(taskItem), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_WhenTaskItemDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _taskItemRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskItemService.GetByIdAsync(id));

        _taskItemRepositoryMock.Verify(r => r.GetByIdAsync(id, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<TaskItemDto>(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenTaskItemsExist_ReturnsMappedTaskItemDtos()
    {
        // Arrange
        var userId = Guid.NewGuid();
        IReadOnlyList<TaskItem> taskItems =
        [
            CreateTaskItem(userId: userId),
            CreateTaskItem(userId: userId)
        ];

        var expectedDtos = taskItems
            .Select(t => new TaskItemDto { Id = t.Id, Title = t.Title, UserId = t.UserId })
            .ToList();

        _taskItemRepositoryMock
            .Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskItems);

        _mapperMock
            .Setup(m => m.Map<IReadOnlyList<TaskItemDto>>(taskItems))
            .Returns(expectedDtos);

        // Act
        var result = await _taskItemService.GetByUserIdAsync(userId);

        // Assert
        Assert.Equal(2, result.Count);
        Assert.All(result, t => Assert.Equal(userId, t.UserId));

        _taskItemRepositoryMock.Verify(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()), Times.Once);
        _mapperMock.Verify(m => m.Map<IReadOnlyList<TaskItemDto>>(taskItems), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_WhenUserDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new CreateTaskItemRequest
        {
            UserId = Guid.NewGuid(),
            Title = "Draft interview tests",
            Description = "Persist business layer behaviors"
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(request.UserId, It.IsAny<CancellationToken>()))
            .ReturnsAsync((User?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskItemService.CreateAsync(request));

        _userRepositoryMock.Verify(r => r.GetByIdAsync(request.UserId, It.IsAny<CancellationToken>()), Times.Once);
        _taskItemRepositoryMock.Verify(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),Times.Never);
    }

    [Fact]
    public async Task CreateAsync_WhenRequestIsValid_PersistsTaskItemAndReturnsDto()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new CreateTaskItemRequest
        {
            UserId = userId,
            Title = "Create task",
            Description = "Task description"
        };
        var mappedTaskItem = new TaskItem
        {
            UserId = request.UserId,
            Title = request.Title,
            Description = request.Description
        };

        var expectedDto = new TaskItemDto
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            UserId = userId
        };

        _userRepositoryMock
            .Setup(r => r.GetByIdAsync(userId, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new User
            {
                Id = userId,
                Email = "user@taskplanner.dev",
                DisplayName = "Planner User",
                PasswordHash = "hash"
            });

        TaskItem? persistedTaskItem = null;
        _taskItemRepositoryMock
            .Setup(r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()))
            .Callback<TaskItem, CancellationToken>((t, _) => persistedTaskItem = t)
            .Returns(Task.CompletedTask);

        _mapperMock
            .Setup(m => m.Map<TaskItem>(request))
            .Returns(mappedTaskItem);

        _mapperMock
            .Setup(m => m.Map<TaskItemDto>(It.IsAny<TaskItem>()))
            .Returns(expectedDto);

        // Act
        var result = await _taskItemService.CreateAsync(request);

        // Assert
        Assert.NotNull(persistedTaskItem);
        Assert.NotEqual(Guid.Empty, persistedTaskItem!.Id);
        Assert.Equal(request.UserId, persistedTaskItem.UserId);
        Assert.Equal(request.Title, persistedTaskItem.Title);
        Assert.Equal(request.Description, persistedTaskItem.Description);
        Assert.Equal(userId, persistedTaskItem.CreatedBy);
        Assert.Equal(userId, persistedTaskItem.UpdatedBy);

        Assert.Equal(expectedDto.Id, result.Id);
        Assert.Equal(expectedDto.Title, result.Title);

        _taskItemRepositoryMock.Verify(
            r => r.AddAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            m => m.Map<TaskItemDto>(It.IsAny<TaskItem>()),
            Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskItemDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        var request = new UpdateTaskItemRequest
        {
            Id = Guid.NewGuid(),
            Title = "Updated title",
            Description = "Updated description"
        };

        _taskItemRepositoryMock
            .Setup(r => r.GetByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskItemService.UpdateAsync(request));

        _taskItemRepositoryMock.Verify(
            r => r.UpdateAsync(It.IsAny<TaskItem>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskItemExists_PersistsChangesAndReturnsDto()
    {
        // Arrange
        var taskItem = CreateTaskItem();
        var request = new UpdateTaskItemRequest
        {
            Id = taskItem.Id,
            Title = "Updated title",
            Description = "Updated description"
        };

        var expectedDto = new TaskItemDto
        {
            Id = taskItem.Id,
            Title = request.Title,
            Description = request.Description,
            UserId = taskItem.UserId
        };

        _taskItemRepositoryMock
            .Setup(r => r.GetByIdAsync(request.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskItem);

        _taskItemRepositoryMock
            .Setup(r => r.UpdateAsync(taskItem, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        _mapperMock
            .Setup(m => m.Map<TaskItemDto>(taskItem))
            .Returns(expectedDto);

        // Act
        var result = await _taskItemService.UpdateAsync(request);

        // Assert
        Assert.Equal(request.Title, result.Title);
        Assert.Equal(request.Description, result.Description);

        _taskItemRepositoryMock.Verify(
            r => r.UpdateAsync(taskItem, It.IsAny<CancellationToken>()),
            Times.Once);

        _mapperMock.Verify(
            m => m.Map<TaskItemDto>(taskItem),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskItemDoesNotExist_ThrowsNotFoundException()
    {
        // Arrange
        _taskItemRepositoryMock
            .Setup(r => r.GetByIdAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((TaskItem?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            () => _taskItemService.DeleteAsync(Guid.NewGuid()));

        _taskItemRepositoryMock.Verify(
            r => r.DeleteAsync(It.IsAny<Guid>(), It.IsAny<CancellationToken>()),
            Times.Never);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskItemExists_DeletesTaskItem()
    {
        // Arrange
        var taskItem = CreateTaskItem();

        _taskItemRepositoryMock
            .Setup(r => r.GetByIdAsync(taskItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(taskItem);

        _taskItemRepositoryMock
            .Setup(r => r.DeleteAsync(taskItem.Id, It.IsAny<CancellationToken>()))
            .ReturnsAsync(true);

        // Act
        await _taskItemService.DeleteAsync(taskItem.Id);

        // Assert
        _taskItemRepositoryMock.Verify(
            r => r.DeleteAsync(taskItem.Id, It.IsAny<CancellationToken>()),
            Times.Once);
    }

    private static TaskItem CreateTaskItem(Guid? userId = null)
    {
        var ownerId = userId ?? Guid.NewGuid();
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            UserId = ownerId,
            Title = "Task title",
            Description = "Task description",
            CreatedAtUtc = new DateTime(2026, 3, 8, 10, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 8, 10, 0, 0, DateTimeKind.Utc),
            CreatedBy = ownerId,
            UpdatedBy = ownerId
        };
    }
}
