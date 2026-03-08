using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Requests.Tasks;

namespace TaskPlanner.Application.Interfaces.Services;

public interface ITaskItemService
{
    Task<TaskItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<TaskItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<TaskItemDto> CreateAsync(CreateTaskItemRequest request, CancellationToken cancellationToken = default);

    Task<TaskItemDto> UpdateAsync(UpdateTaskItemRequest request, CancellationToken cancellationToken = default);

    Task DeleteAsync(Guid id, CancellationToken cancellationToken = default);
}
