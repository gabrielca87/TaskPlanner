using System.Data.Common;
using Microsoft.AspNetCore.Identity;
using TaskPlanner.Infrastructure.Data.Connections;

namespace TaskPlanner.Infrastructure.Data.Seed;

public sealed class DataSeeder : IDataSeeder
{
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly PasswordHasher<SeedUserDefinition> _passwordHasher = new();

    public DataSeeder(IDbConnectionFactory connectionFactory)
    {
        _connectionFactory = connectionFactory;
    }

    public async Task SeedAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
        await using var transaction = await connection.BeginTransactionAsync(cancellationToken);

        await SeedUSers(connection, transaction, cancellationToken);
        await SeedTaskItems(connection, transaction, cancellationToken);

        await transaction.CommitAsync(cancellationToken);
    }

    private async Task SeedUSers(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        foreach (var user in UserSeedScript.Users)
        {
            var parameters = new Dictionary<string, object?>
            {
                ["@Id"] = user.Id,
                ["@Email"] = user.Email,
                ["@DisplayName"] = user.DisplayName,
                ["@PasswordHash"] = _passwordHasher.HashPassword(user, user.PlainPassword),
                ["@CreatedAtUtc"] = user.CreatedAtUtc,
                ["@UpdatedAtUtc"] = user.UpdatedAtUtc,
                ["@CreatedBy"] = user.CreatedBy,
                ["@UpdatedBy"] = user.UpdatedBy
            };

            await ExecuteNonQueryAsync(
                connection,
                transaction,
                UserSeedScript.InsertUser,
                parameters,
                cancellationToken);
        }
    }

    private async Task SeedTaskItems(DbConnection connection, DbTransaction transaction, CancellationToken cancellationToken)
    {
        foreach (var taskItem in TaskItemSeedScript.TaskItems)
        {
            var parameters = new Dictionary<string, object?>
            {
                ["@Id"] = taskItem.Id,
                ["@UserId"] = taskItem.UserId,
                ["@Title"] = taskItem.Title,
                ["@Description"] = taskItem.Description,
                ["@CreatedAtUtc"] = taskItem.CreatedAtUtc,
                ["@UpdatedAtUtc"] = taskItem.UpdatedAtUtc,
                ["@CreatedBy"] = taskItem.CreatedBy,
                ["@UpdatedBy"] = taskItem.UpdatedBy
            };

            await ExecuteNonQueryAsync(
                connection,
                transaction,
                TaskItemSeedScript.InsertTaskItem,
                parameters,
                cancellationToken);
        }
    }

    private static async Task ExecuteNonQueryAsync(
        System.Data.Common.DbConnection connection,
        System.Data.Common.DbTransaction transaction,
        string sql,
        IReadOnlyDictionary<string, object?> parameters,
        CancellationToken cancellationToken)
    {
        await using var command = connection.CreateCommand();
        command.Transaction = transaction;
        command.CommandText = sql;
        foreach (var parameter in parameters)
        {
            var dbParameter = command.CreateParameter();
            dbParameter.ParameterName = parameter.Key;
            dbParameter.Value = parameter.Value ?? DBNull.Value;
            if (parameter.Value is Guid)
            {
                dbParameter.DbType = System.Data.DbType.Guid;
            }
            else if (parameter.Value is DateTime)
            {
                dbParameter.DbType = System.Data.DbType.DateTime;
            }

            command.Parameters.Add(dbParameter);
        }

        await command.ExecuteNonQueryAsync(cancellationToken);
    }
}
