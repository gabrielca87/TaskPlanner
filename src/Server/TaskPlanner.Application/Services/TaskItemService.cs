using AutoMapper;
using TaskPlanner.Application.DTOs;
using TaskPlanner.Application.Exceptions;
using TaskPlanner.Application.Interfaces.Services;
using TaskPlanner.Application.Requests.Tasks;
using TaskPlanner.Domain.Entities;
using TaskPlanner.Domain.Interfaces.Repositories;

namespace TaskPlanner.Application.Services;

public class TaskItemService : ITaskItemService
{
    private readonly ITaskItemRepository _taskItemRepository;
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public TaskItemService(
        ITaskItemRepository taskItemRepository,
        IUserRepository userRepository,
        IMapper mapper)
    {
        _taskItemRepository = taskItemRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public async Task<TaskItemDto> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var taskItem = await _taskItemRepository.GetByIdAsync(id, cancellationToken);
        if (taskItem is null)
        {
            throw new NotFoundException("TaskItem", id.ToString());
        }

        return _mapper.Map<TaskItemDto>(taskItem);
    }

    public async Task<IReadOnlyList<TaskItemDto>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var taskItems = await _taskItemRepository.GetByUserIdAsync(userId, cancellationToken);
        return _mapper.Map<IReadOnlyList<TaskItemDto>>(taskItems);
    }

    public async Task<TaskItemDto> CreateAsync(CreateTaskItemRequest request, CancellationToken cancellationToken = default)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user is null)
        {
            throw new NotFoundException("User", request.UserId.ToString());
        }

        var taskItem = _mapper.Map<TaskItem>(request);
        var utcNow = DateTime.UtcNow;

        taskItem.Id = Guid.NewGuid();
        taskItem.CreatedAtUtc = utcNow;
        taskItem.UpdatedAtUtc = utcNow;
        taskItem.CreatedBy = request.UserId;
        taskItem.UpdatedBy = request.UserId;

        await _taskItemRepository.AddAsync(taskItem, cancellationToken);

        return _mapper.Map<TaskItemDto>(taskItem);
    }

    public async Task<TaskItemDto> UpdateAsync(UpdateTaskItemRequest request, CancellationToken cancellationToken = default)
    {
        var existingTaskItem = await _taskItemRepository.GetByIdAsync(request.Id, cancellationToken);
        if (existingTaskItem is null)
        {
            throw new NotFoundException("TaskItem", request.Id.ToString());
        }

        existingTaskItem.Title = request.Title;
        existingTaskItem.Description = request.Description;
        existingTaskItem.UpdatedAtUtc = DateTime.UtcNow;
        existingTaskItem.UpdatedBy = existingTaskItem.UserId;

        var updated = await _taskItemRepository.UpdateAsync(existingTaskItem, cancellationToken);
        if (!updated)
        {
            throw new NotFoundException("TaskItem", request.Id.ToString());
        }

        return _mapper.Map<TaskItemDto>(existingTaskItem);
    }

    public async Task DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var existingTaskItem = await _taskItemRepository.GetByIdAsync(id, cancellationToken);
        if (existingTaskItem is null)
        {
            throw new NotFoundException("TaskItem", id.ToString());
        }

        var deleted = await _taskItemRepository.DeleteAsync(id, cancellationToken);
        if (!deleted)
        {
            throw new NotFoundException("TaskItem", id.ToString());
        }
    }
}
