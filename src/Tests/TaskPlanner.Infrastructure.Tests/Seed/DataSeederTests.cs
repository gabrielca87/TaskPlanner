using Microsoft.AspNetCore.Identity;
using TaskPlanner.Infrastructure.Data.Seed;
using TaskPlanner.Infrastructure.Tests.TestSupport;

namespace TaskPlanner.Infrastructure.Tests.Seed;

public class DataSeederTests
{
    [Fact]
    public async Task SeedAsync_WhenCalledMultipleTimes_InsertsDefaultDataOnlyOnce()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var seeder = new DataSeeder(database.ConnectionFactory);

        // Act
        await seeder.SeedAsync();
        await seeder.SeedAsync();

        // Assert
        var userCount = await database.CommandExecutor.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM Users;");
        var taskCount = await database.CommandExecutor.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM TaskItems;");
        var plannerUserTaskCount = await database.CommandExecutor.ExecuteScalarAsync<long>(
            "SELECT COUNT(*) FROM TaskItems WHERE UserId = @UserId;",
            new Dictionary<string, object?>
            {
                ["@UserId"] = Guid.Parse("8a563ceb-b084-4f26-9984-7f44f5d87ad0")
            });

        Assert.Equal(2, userCount);
        Assert.Equal(3, taskCount);
        Assert.Equal(2, plannerUserTaskCount);
    }

    [Fact]
    public async Task SeedAsync_WhenUsersAreSeeded_StoresPasswordAsHash()
    {
        // Arrange
        await using var database = new SqliteTestDatabase();
        await database.InitializeSchemaAsync();
        var seeder = new DataSeeder(database.ConnectionFactory);
        var passwordHasher = new PasswordHasher<object>();

        // Act
        await seeder.SeedAsync();

        // Assert
        var passwordHash = await database.CommandExecutor.ExecuteScalarAsync<string>(
            "SELECT PasswordHash FROM Users WHERE Email = 'planner@taskplanner.dev';");

        Assert.False(string.IsNullOrWhiteSpace(passwordHash));
        Assert.NotEqual("Planner@123", passwordHash);
        var verification = passwordHasher.VerifyHashedPassword(new object(), passwordHash!, "Planner@123");
        Assert.True(
            verification == PasswordVerificationResult.Success ||
            verification == PasswordVerificationResult.SuccessRehashNeeded);
    }
}
