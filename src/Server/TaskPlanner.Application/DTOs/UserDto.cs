namespace TaskPlanner.Application.DTOs;

public class UserDto : BaseDto
{
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
}
