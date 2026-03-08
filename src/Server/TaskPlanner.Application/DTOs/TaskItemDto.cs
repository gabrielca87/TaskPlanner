namespace TaskPlanner.Application.DTOs;

public class TaskItemDto : BaseDto
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
}
