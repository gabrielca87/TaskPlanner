namespace TaskPlanner.Application.DTOs;

public class BaseDto
{
    public Guid Id { get; init; }
    public DateTime CreatedAtUtc { get; init; }
    public DateTime UpdatedAtUtc { get; init; }
    public Guid CreatedBy { get; init; }
    public Guid UpdatedBy { get; init; }
}
