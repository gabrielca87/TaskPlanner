namespace TaskPlanner.Infrastructure.Data.Seed;

internal static class TaskItemSeedScript
{
    public static IReadOnlyList<SeedTaskItemDefinition> TaskItems =>
    [
        new(
            Id: Guid.Parse("25d86bcf-b2aa-4134-abab-f2ddab415e23"),
            UserId: Guid.Parse("8a563ceb-b084-4f26-9984-7f44f5d87ad0"),
            Title: "Prepare sprint board",
            Description: "Create stories for onboarding flow",
            CreatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy: Guid.Empty,
            UpdatedBy: Guid.Empty),
        new(
            Id: Guid.Parse("65f4de50-17bc-47a4-8f4d-a38f735b7f72"),
            UserId: Guid.Parse("8a563ceb-b084-4f26-9984-7f44f5d87ad0"),
            Title: "Review pull request",
            Description: "Persistence slice review",
            CreatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy: Guid.Empty,
            UpdatedBy: Guid.Empty),
        new(
            Id: Guid.Parse("f20f576b-bff0-414f-bb15-8db099afe9d1"),
            UserId: Guid.Parse("278d6af5-8f2f-46e2-a9ed-1f4d3f0878ec"),
            Title: "Publish release notes",
            Description: "Share changelog with stakeholders",
            CreatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy: Guid.Empty,
            UpdatedBy: Guid.Empty)
    ];

    public const string InsertTaskItem = """
        INSERT OR IGNORE INTO TaskItems (Id, UserId, Title, Description, CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy)
        VALUES (@Id, @UserId, @Title, @Description, @CreatedAtUtc, @UpdatedAtUtc, @CreatedBy, @UpdatedBy);
        """;
}

internal sealed record SeedTaskItemDefinition(
    Guid Id,
    Guid UserId,
    string Title,
    string? Description,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    Guid CreatedBy,
    Guid UpdatedBy);
