using TaskPlanner.Domain.Entities;
using TaskPlanner.Infrastructure.Data.Repositories;
using TaskPlanner.Infrastructure.Tests.TestSupport;

namespace TaskPlanner.Infrastructure.Tests.Repositories;

public class TaskItemRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenTaskDoesNotExist_ReturnsNull()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var repository = new TaskItemRepository(database.CommandExecutor);

        // Act
        var persistedTask = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(persistedTask);
    }

    [Fact]
    public async Task AddAsync_WhenTaskItemIsPersisted_GetByIdAsyncReturnsMatchingTask()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var userRepository = new UserRepository(database.CommandExecutor);
        var taskRepository = new TaskItemRepository(database.CommandExecutor);
        var user = CreateUser("owner@taskplanner.dev");
        await userRepository.AddAsync(user);
        var task = CreateTask(user.Id, "Draft persistence tests");
        
        // Act
        await taskRepository.AddAsync(task);

        // Assert
        var persistedTask = await taskRepository.GetByIdAsync(task.Id);
        Assert.NotNull(persistedTask);
        Assert.Equal(task.Id, persistedTask.Id);
        Assert.Equal(task.Title, persistedTask.Title);
        Assert.Equal(task.Description, persistedTask.Description);
        Assert.Equal(task.CreatedAtUtc, persistedTask.CreatedAtUtc);
        Assert.Equal(task.UpdatedAtUtc, persistedTask.UpdatedAtUtc);
        Assert.Equal(task.CreatedBy, persistedTask.CreatedBy);
        Assert.Equal(task.UpdatedBy, persistedTask.UpdatedBy);
    }

    [Fact]
    public async Task GetByUserIdAsync_WhenOtherUsersHaveTasks_ReturnsOnlyTasksForRequestedUser()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var userRepository = new UserRepository(database.CommandExecutor);
        var taskRepository = new TaskItemRepository(database.CommandExecutor);
        var primaryUser = CreateUser("primary@taskplanner.dev");
        var secondaryUser = CreateUser("secondary@taskplanner.dev");
        var primaryTaskOne = CreateTask(primaryUser.Id, "Primary task one");
        var primaryTaskTwo = CreateTask(primaryUser.Id, "Primary task two");
        var secondaryTask = CreateTask(secondaryUser.Id, "Secondary task");
        await userRepository.AddAsync(primaryUser);
        await userRepository.AddAsync(secondaryUser);
        await taskRepository.AddAsync(primaryTaskOne);
        await taskRepository.AddAsync(primaryTaskTwo);
        await taskRepository.AddAsync(secondaryTask);

        // Act
        var persistedTasks = await taskRepository.GetByUserIdAsync(primaryUser.Id);

        // Assert
        Assert.Equal(2, persistedTasks.Count);
        Assert.All(persistedTasks, task => Assert.Equal(primaryUser.Id, task.UserId));
    }

    [Fact]
    public async Task UpdateAsync_WhenTaskItemExists_PersistsNewValuesAndReturnsTrue()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var userRepository = new UserRepository(database.CommandExecutor);
        var taskRepository = new TaskItemRepository(database.CommandExecutor);
        var user = CreateUser("updater@taskplanner.dev");
        var task = CreateTask(user.Id, "Initial title");
        await userRepository.AddAsync(user);
        await taskRepository.AddAsync(task);
        task.Title = "Updated title";
        task.Description = "Updated description";
        task.UpdatedAtUtc = new DateTime(2026, 3, 8, 17, 0, 0, DateTimeKind.Utc);
        task.UpdatedBy = user.Id;

        // Act
        var wasUpdated = await taskRepository.UpdateAsync(task);

        // Assert
        var persistedTask = await taskRepository.GetByIdAsync(task.Id);
        Assert.True(wasUpdated);
        Assert.NotNull(persistedTask);
        Assert.Equal("Updated title", persistedTask.Title);
        Assert.Equal("Updated description", persistedTask.Description);
        Assert.Equal(new DateTime(2026, 3, 8, 17, 0, 0, DateTimeKind.Utc), persistedTask.UpdatedAtUtc);
        Assert.Equal(user.Id, persistedTask.UpdatedBy);
    }

    [Fact]
    public async Task DeleteAsync_WhenTaskItemExists_RemovesTaskAndReturnsTrue()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var userRepository = new UserRepository(database.CommandExecutor);
        var taskRepository = new TaskItemRepository(database.CommandExecutor);
        var user = CreateUser("deleter@taskplanner.dev");
        var task = CreateTask(user.Id, "Task to delete");
        await userRepository.AddAsync(user);
        await taskRepository.AddAsync(task);

        // Act
        var wasDeleted = await taskRepository.DeleteAsync(task.Id);

        // Assert
        var persistedTask = await taskRepository.GetByIdAsync(task.Id);
        Assert.True(wasDeleted);
        Assert.Null(persistedTask);
    }

    private User CreateUser(string email)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = email.Split('@')[0],
            PasswordHash = "hash-value",
            CreatedAtUtc = new DateTime(2026, 3, 7, 8, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 7, 8, 0, 0, DateTimeKind.Utc),
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        };
    }

    private TaskItem CreateTask(Guid userId, string title)
    {
        return new TaskItem
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            Title = title,
            Description = null,
            CreatedAtUtc = new DateTime(2026, 3, 7, 9, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 7, 9, 0, 0, DateTimeKind.Utc),
            CreatedBy = userId,
            UpdatedBy = userId
        };
    }
}
