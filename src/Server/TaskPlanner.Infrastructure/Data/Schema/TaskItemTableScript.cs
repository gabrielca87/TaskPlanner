namespace TaskPlanner.Infrastructure.Data.Schema;

internal static class TaskItemTableScript
{
    public const string CreateTable = """
        CREATE TABLE IF NOT EXISTS TaskItems (
            Id UNIQUEIDENTIFIER NOT NULL PRIMARY KEY,
            UserId UNIQUEIDENTIFIER NOT NULL,
            Title NVARCHAR(250) NOT NULL,
            Description NVARCHAR(1000) NULL,
            CreatedAtUtc DATETIME NOT NULL,
            UpdatedAtUtc DATETIME NOT NULL,
            CreatedBy UNIQUEIDENTIFIER NOT NULL,
            UpdatedBy UNIQUEIDENTIFIER NOT NULL,
            FOREIGN KEY (UserId) REFERENCES Users(Id) ON DELETE CASCADE
        );
        """;

    public const string CreateUserIndex = """
        CREATE INDEX IF NOT EXISTS IX_TaskItems_UserId
        ON TaskItems (UserId);
        """;

}
