using TaskPlanner.Domain.Entities;
using TaskPlanner.Domain.Interfaces.Repositories;
using TaskPlanner.Infrastructure.Data.Commands;
using TaskPlanner.Infrastructure.Data.Mappers;
using TaskPlanner.Infrastructure.Data.Queries;

namespace TaskPlanner.Infrastructure.Data.Repositories;

public sealed class TaskItemRepository : ITaskItemRepository
{
    private readonly IDbCommandExecutor _commandExecutor;

    public TaskItemRepository(IDbCommandExecutor commandExecutor)
    {
        _commandExecutor = commandExecutor;
    }

    public Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["@Id"] = id
        };

        return _commandExecutor.QuerySingleAsync(
            TaskItemQueries.SelectById,
            TaskItemDataMapper.Map,
            parameters,
            cancellationToken);
    }

    public Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["@UserId"] = userId
        };

        return _commandExecutor.QueryListAsync(
            TaskItemQueries.SelectByUserId,
            TaskItemDataMapper.Map,
            parameters,
            cancellationToken);
    }

    public async Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
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

        await _commandExecutor.ExecuteNonQueryAsync(
            TaskItemQueries.Insert,
            parameters,
            cancellationToken);
    }

    public async Task<bool> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["@Id"] = taskItem.Id,
            ["@Title"] = taskItem.Title,
            ["@Description"] = taskItem.Description,
            ["@UpdatedAtUtc"] = taskItem.UpdatedAtUtc,
            ["@UpdatedBy"] = taskItem.UpdatedBy
        };

        var affectedRows = await _commandExecutor.ExecuteNonQueryAsync(
            TaskItemQueries.Update,
            parameters,
            cancellationToken);

        return affectedRows > 0;
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var parameters = new Dictionary<string, object?>
        {
            ["@Id"] = id
        };

        var affectedRows = await _commandExecutor.ExecuteNonQueryAsync(
            TaskItemQueries.Delete,
            parameters,
            cancellationToken);

        return affectedRows > 0;
    }
}
