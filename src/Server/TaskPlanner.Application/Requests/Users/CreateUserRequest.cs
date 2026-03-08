namespace TaskPlanner.Application.Requests.Users;

public class CreateUserRequest
{
    public string Email { get; init; } = string.Empty;
    public string DisplayName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}
