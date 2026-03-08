namespace TaskPlanner.Domain.Entities;

public class User : BaseEntity
{
    public required string Email { get; set; }
    public required string DisplayName { get; set; }
    public required string PasswordHash { get; set; }
}
