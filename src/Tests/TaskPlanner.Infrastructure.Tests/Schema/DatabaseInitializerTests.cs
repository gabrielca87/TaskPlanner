using TaskPlanner.Infrastructure.Data.Schema;
using TaskPlanner.Infrastructure.Tests.TestSupport;

namespace TaskPlanner.Infrastructure.Tests.Schema;

public class DatabaseInitializerTests
{
    [Fact]
    public async Task InitializeAsync_WhenCalledMultipleTimes_CreatesSchemaWithoutFailing()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        var initializer = new DatabaseInitializer(database.ConnectionFactory);

        // Act
        await initializer.InitializeAsync();
        await initializer.InitializeAsync();

        // Assert
        var usersTableCount = await database.CommandExecutor.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'Users';");
        var taskItemsTableCount = await database.CommandExecutor.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM sqlite_master WHERE type = 'table' AND name = 'TaskItems';");

        Assert.Equal(1, usersTableCount);
        Assert.Equal(1, taskItemsTableCount);
    }
}
