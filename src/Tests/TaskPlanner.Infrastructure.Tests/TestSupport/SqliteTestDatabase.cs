using TaskPlanner.Infrastructure.Data.Commands;
using TaskPlanner.Infrastructure.Data.Connections;
using TaskPlanner.Infrastructure.Data.Schema;
using TaskPlanner.Infrastructure.Data.Seed;

namespace TaskPlanner.Infrastructure.Tests.TestSupport;

internal sealed class SqliteTestDatabase : IAsyncDisposable
{
    private readonly string _databasePath;

    public SqliteTestDatabase()
    {
        _databasePath = Path.Combine(Path.GetTempPath(), $"taskplanner-tests-{Guid.NewGuid():N}.db");
        ConnectionString = $"Data Source={_databasePath};Pooling=False";
        ConnectionFactory = new DbConnectionFactory(ConnectionString);
        CommandExecutor = new DbCommandExecutor(ConnectionFactory);
    }

    public string ConnectionString { get; }

    public IDbConnectionFactory ConnectionFactory { get; }

    public IDbCommandExecutor CommandExecutor { get; }

    public async Task InitializeSchemaAsync(CancellationToken cancellationToken = default)
    {
        var initializer = new DatabaseInitializer(ConnectionFactory);
        await initializer.InitializeAsync(cancellationToken);
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        var seeder = new DataSeeder(ConnectionFactory);
        await seeder.SeedAsync(cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        if (!File.Exists(_databasePath))
        {
            return;
        }

        for (var attempt = 0; attempt < 3; attempt++)
        {
            try
            {
                File.Delete(_databasePath);
                return;
            }
            catch (IOException) when (attempt < 2)
            {
                await Task.Delay(25);
            }
        }
    }
}
