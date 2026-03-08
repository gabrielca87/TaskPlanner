using TaskPlanner.Domain.Entities;
using TaskPlanner.Infrastructure.Data.Repositories;
using TaskPlanner.Infrastructure.Tests.TestSupport;

namespace TaskPlanner.Infrastructure.Tests.Repositories;

public class UserRepositoryTests
{
    [Fact]
    public async Task GetByIdAsync_WhenUserDoesNotExist_ReturnsNull()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var repository = new UserRepository(database.CommandExecutor);

        // Act
        var persistedUser = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        Assert.Null(persistedUser);
    }

    [Fact]
    public async Task AddAsync_WhenUserIsPersisted_GetByIdAsyncReturnsTheSameUser()
    {
        //Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var repository = new UserRepository(database.CommandExecutor);
        var user = CreateUser("owner@taskplanner.dev", "Owner");
        await repository.AddAsync(user);

        // Act
        var persistedUser = await repository.GetByIdAsync(user.Id);

        // Assert
        Assert.NotNull(persistedUser);
        Assert.Equal(user.Id, persistedUser.Id);
        Assert.Equal(user.Email, persistedUser.Email);
        Assert.Equal(user.DisplayName, persistedUser.DisplayName);
        Assert.Equal(user.CreatedAtUtc, persistedUser.CreatedAtUtc);
        Assert.Equal(user.UpdatedAtUtc, persistedUser.UpdatedAtUtc);
        Assert.Equal(user.CreatedBy, persistedUser.CreatedBy);
        Assert.Equal(user.UpdatedBy, persistedUser.UpdatedBy);
    }   

    [Fact]
    public async Task GetByEmailAsync_WhenUserExistsWithEmail_ReturnsMatchingUser()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var repository = new UserRepository(database.CommandExecutor);
        var user = CreateUser("planner@taskplanner.dev", "Planner");
        await repository.AddAsync(user);

        // Act
        var persistedUser = await repository.GetByEmailAsync(user.Email);

        // Assert
        Assert.NotNull(persistedUser);
        Assert.Equal(user.Email, persistedUser.Email);
    }

    private static User CreateUser(string email, string displayName)
    {
        return new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            DisplayName = displayName,
            PasswordHash = "hash-value",
            CreatedAtUtc = new DateTime(2026, 3, 7, 11, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc = new DateTime(2026, 3, 7, 11, 0, 0, DateTimeKind.Utc),
            CreatedBy = Guid.Empty,
            UpdatedBy = Guid.Empty
        };
    }
}
