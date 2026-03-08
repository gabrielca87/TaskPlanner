using TaskPlanner.Infrastructure.Data.Connections;

namespace TaskPlanner.Infrastructure.Data.Schema;

public sealed class DatabaseInitializer : IDatabaseInitializer
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly IEnumerable<string> _scriptsToRun = new List<string>
    {
        UserTableScript.CreateTable,
        UserTableScript.CreateEmailIndex,
        TaskItemTableScript.CreateTable,
        TaskItemTableScript.CreateUserIndex
    };

    public DatabaseInitializer(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        foreach (var script in _scriptsToRun)
        {
            await using var command = connection.CreateCommand();
            command.CommandText = script;
            await command.ExecuteNonQueryAsync(cancellationToken);
        }
    }
}
