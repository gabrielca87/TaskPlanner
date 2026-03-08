namespace TaskPlanner.Infrastructure.Data.Schema;

internal static class UserTableScript
{
    public const string CreateTable = """
        CREATE TABLE IF NOT EXISTS Users (
            Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            Email NVARCHAR(256) NOT NULL,
            DisplayName NVARCHAR(200) NOT NULL,
            PasswordHash NVARCHAR(1000) NOT NULL,
            CreatedAtUtc DATETIME NOT NULL,
            UpdatedAtUtc DATETIME NOT NULL,
            CreatedBy UNIQUEIDENTIFIER NOT NULL,
            UpdatedBy UNIQUEIDENTIFIER NOT NULL
        );
        """;

    public const string CreateEmailIndex = """
        CREATE UNIQUE INDEX IF NOT EXISTS IX_Users_Email
        ON Users (Email COLLATE NOCASE);
        """;
}
