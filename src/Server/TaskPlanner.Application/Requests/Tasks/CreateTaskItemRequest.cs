namespace TaskPlanner.Application.Requests.Tasks;

public class CreateTaskItemRequest
{
    public Guid UserId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
}
