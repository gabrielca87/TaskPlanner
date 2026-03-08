using TaskPlanner.Infrastructure.Data.Schema;
using TaskPlanner.Infrastructure.Data.Seed;

namespace TaskPlanner.Api.Extensions;

public static class ApplicationBuilderExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app, CancellationToken cancellationToken = default)
    {
        using var scope = app.Services.CreateScope();
        var initializer = scope.ServiceProvider.GetRequiredService<IDatabaseInitializer>();
        var seeder = scope.ServiceProvider.GetRequiredService<IDataSeeder>();
        await initializer.InitializeAsync(cancellationToken);
        await seeder.SeedAsync(cancellationToken);
    }
}
