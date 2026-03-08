namespace TaskPlanner.Domain.Entities;

public class TaskItem : BaseEntity
{
    public Guid UserId { get; set; }
    public required string Title { get; set; }
    public string? Description { get; set; }
}
