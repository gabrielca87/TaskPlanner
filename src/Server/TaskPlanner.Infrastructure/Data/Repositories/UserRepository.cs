using TaskPlanner.Domain.Entities;
using TaskPlanner.Domain.Interfaces.Repositories;
using TaskPlanner.Infrastructure.Data.Commands;
using TaskPlanner.Infrastructure.Data.Mappers;
using TaskPlanner.Infrastructure.Data.Queries;

namespace TaskPlanner.Infrastructure.Data.Repositories;

public sealed class UserRepository : IUserRepository
{
    private readonly IDbCommandExecutor _commandExecutor;

    public UserRepository(IDbCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor ?? throw new ArgumentNullException(nameof(commandExecutor));
    }

    public Task<User?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["@Id"] = id
        };

        return _commandExecutor.QuerySingleAsync(
            UserQueries.SelectById,
            UserDataMapper.Map,
            parameters,
            cancellationToken);
    }

    public Task<User?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["@Email"] = email
        };

        return _commandExecutor.QuerySingleAsync(
            UserQueries.SelectByEmail,
            UserDataMapper.Map,
            parameters,
            cancellationToken);
    }

    public async Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["@Id"] = user.Id,
            ["@Email"] = user.Email,
            ["@DisplayName"] = user.DisplayName,
            ["@PasswordHash"] = user.PasswordHash,
            ["@CreatedAtUtc"] = user.CreatedAtUtc,
            ["@UpdatedAtUtc"] = user.UpdatedAtUtc,
            ["@CreatedBy"] = user.CreatedBy,
            ["@UpdatedBy"] = user.UpdatedBy
        };

        await _commandExecutor.ExecuteNonQueryAsync(
            UserQueries.Insert,
            parameters,
            cancellationToken);
    }
}
