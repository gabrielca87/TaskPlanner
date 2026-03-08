using System.Data;
using System.Data.Common;
using System.Globalization;
using TaskPlanner.Infrastructure.Data.Connections;

namespace TaskPlanner.Infrastructure.Data.Commands;

public class DbCommandExecutor : IDbCommandExecutor
{
    private readonly IDbConnectionFactory _connectionFactory;

    public DbCommandExecutor(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory ?? throw new ArgumentNullException(nameof(connectionFactory));
    }

    public async Task<int> ExecuteNonQueryAsync(
        string sql,
        IReadOnlyDictionary<string, object?>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = CreateCommand(connection, sql, parameters);
        return await command.ExecuteNonQueryAsync(cancellationToken);
    }

    public async Task<T?> ExecuteScalarAsync<T>(
        string sql,
        IReadOnlyDictionary<string, object?>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = CreateCommand(connection, sql, parameters);
        var value = await command.ExecuteScalarAsync(cancellationToken);
        if (value is null || value is DBNull)
        {
            return default;
        }

        return ConvertValue<T>(value);
    }

    public async Task<T?> QuerySingleAsync<T>(
        string sql,
        Func<IDataRecord, T> mapper,
        IReadOnlyDictionary<string, object?>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = CreateCommand(connection, sql, parameters);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);
        if (!await reader.ReadAsync(cancellationToken))
        {
            return default;
        }

        return mapper(reader);
    }

    public async Task<IReadOnlyList<T>> QueryListAsync<T>(
        string sql,
        Func<IDataRecord, T> mapper,
        IReadOnlyDictionary<string, object?>? parameters = null,
        CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var command = CreateCommand(connection, sql, parameters);
        await using var reader = await command.ExecuteReaderAsync(cancellationToken);

        var results = new List<T>();
        while (await reader.ReadAsync(cancellationToken))
        {
            results.Add(mapper(reader));
        }

        return results;
    }

    private DbCommand CreateCommand(
        DbConnection connection,
        string sql,
        IReadOnlyDictionary<string, object?>? parameters)
    {
        var command = connection.CreateCommand();
        command.CommandText = sql;
        AddParameters(command, parameters);
        return command;
    }

    private void AddParameters(DbCommand command, IReadOnlyDictionary<string, object?>? parameters)
    {
        if (parameters is null || parameters.Count == 0)
        {
            return;
        }

        foreach (var parameter in parameters)
        {
            var dbParameter = command.CreateParameter();
            dbParameter.ParameterName = parameter.Key;
            dbParameter.Value = parameter.Value ?? DBNull.Value;
            if (parameter.Value is Guid)
            {
                dbParameter.DbType = DbType.Guid;
            }
            else if (parameter.Value is DateTime)
            {
                dbParameter.DbType = DbType.DateTime;
            }

            command.Parameters.Add(dbParameter);
        }
    }

    private T ConvertValue<T>(object value)
    {
        if (value is T castValue)
        {
            return castValue;
        }

        var targetType = Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T);
        if (targetType == typeof(Guid))
        {
            return (T)(object)Guid.Parse(value.ToString()!);
        }

        if (targetType == typeof(DateTime))
        {
            return (T)(object)DateTime.Parse(
                value.ToString()!,
                CultureInfo.InvariantCulture,
                DateTimeStyles.RoundtripKind);
        }

        return (T)Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
    }
}
