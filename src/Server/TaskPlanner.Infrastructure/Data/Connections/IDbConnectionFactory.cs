using System.Data.Common;

namespace TaskPlanner.Infrastructure.Data.Connections;

public interface IDbConnectionFactory
{
    Task<DbConnection> CreateOpenConnectionAsync(CancellationToken cancellationToken = default);
}
