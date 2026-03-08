namespace TaskPlanner.Infrastructure.Data.Schema;

public interface IDatabaseInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
