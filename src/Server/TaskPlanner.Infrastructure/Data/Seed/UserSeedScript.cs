namespace TaskPlanner.Infrastructure.Data.Seed;

internal static class UserSeedScript
{
    public static IReadOnlyList<SeedUserDefinition> Users =>
    [
        new(
            Id: Guid.Parse("8a563ceb-b084-4f26-9984-7f44f5d87ad0"),
            Email: "planner@taskplanner.dev",
            DisplayName: "Planner User",
            PlainPassword: "Planner@123",
            CreatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy: Guid.Empty,
            UpdatedBy: Guid.Empty),
        new(
            Id: Guid.Parse("278d6af5-8f2f-46e2-a9ed-1f4d3f0878ec"),
            Email: "admin@taskplanner.dev",
            DisplayName: "Admin User",
            PlainPassword: "Admin@123",
            CreatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            UpdatedAtUtc: new DateTime(2026, 3, 7, 0, 0, 0, DateTimeKind.Utc),
            CreatedBy: Guid.Empty,
            UpdatedBy: Guid.Empty)
    ];

    public const string InsertUser = """
        INSERT OR IGNORE INTO Users (Id, Email, DisplayName, PasswordHash, CreatedAtUtc, UpdatedAtUtc, CreatedBy, UpdatedBy)
        VALUES (@Id, @Email, @DisplayName, @PasswordHash, @CreatedAtUtc, @UpdatedAtUtc, @CreatedBy, @UpdatedBy);
        """;
}

internal sealed record SeedUserDefinition(
    Guid Id,
    string Email,
    string DisplayName,
    string PlainPassword,
    DateTime CreatedAtUtc,
    DateTime UpdatedAtUtc,
    Guid CreatedBy,
    Guid UpdatedBy);
