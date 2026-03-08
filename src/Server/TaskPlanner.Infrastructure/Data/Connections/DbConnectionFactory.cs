using System.Data.Common;
using Microsoft.Data.Sqlite;

namespace TaskPlanner.Infrastructure.Data.Connections;

public class DbConnectionFactory : IDbConnectionFactory
{
    private readonly string _connectionString;

    public DbConnectionFactory(string connectionString)
    {
        var connectionStringBuilder = new SqliteConnectionStringBuilder(connectionString)
        {
            ForeignKeys = true
        };

        _connectionString = connectionStringBuilder.ToString();
    }

    public async Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default)
    {
        var connection = new SqliteConnection(_connectionString);
        await connection.OpenAsync(cancellationToken);
        return connection;
    }
}
