namespace TaskPlanner.Infrastructure.Data.Queries;

internal static class UserQueries
{
    public const string Insert = """
        INSERT INTO Users (Id, Email, DisplayName, PasswordHash, CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy)
        VALUES (@Id, @Email, @DisplayName, @PasswordHash, @CreatedAtUtc, @UpdatedAtUtc, @CreatedBy, @UpdatedBy);
        """;

    public const string SelectById = """
        SELECT Id, Email, DisplayName, PasswordHash, CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy
        FROM Users
        WHERE Id = @Id;
        """;

    public const string SelectByEmail = """
        SELECT Id, Email, DisplayName, PasswordHash, CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy
        FROM Users
        WHERE Email = @Email COLLATE NOCASE;
        """;
}
