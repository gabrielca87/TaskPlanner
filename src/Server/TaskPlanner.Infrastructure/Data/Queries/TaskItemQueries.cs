namespace TaskPlanner.Infrastructure.Data.Queries;

internal static class TaskItemQueries
{
    public const string Insert = """
        INSERT INTO TaskItems (Id, UserId, Title, Description, CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy)
        VALUES (@Id, @UserId, @Title, @Description, @CreatedAtUtc, @UpdatedAtUtc, @CreatedBy, @UpdatedBy);
        """;

    public const string SelectById = """
        SELECT Id, UserId, Title, Description, CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy
        FROM TaskItems
        WHERE Id = @Id;
        """;

    public const string SelectByUserId = """
        SELECT Id, UserId, Title, Description, CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy
        FROM TaskItems
        WHERE UserId = @UserId
        ORDER BY CreatedAtUtc;
        """;

    public const string Update = """
        UPDATE TaskItems
        SET Title = @Title,
            Description = @Description,
            UpdatedAtUtc = @UpdatedAtUtc,
            UpdatedBy = @UpdatedBy
        WHERE Id = @Id;
        """;

    public const string Delete = """
        DELETE FROM TaskItems
        WHERE Id = @Id;
        """;
}
