using TaskPlanner.Domain.Entities;

namespace TaskPlanner.Domain.Interfaces.Repositories;

public interface ITaskItemRepository
{
    Task<TaskItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItem>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task AddAsync(TaskItem taskItem, CancellationToken cancellationToken = default);

    Task<bool> UpdateAsync(TaskItem taskItem, CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
