using Microsoft.Extensions.DependencyInjection;
using TaskPlanner.Domain.Interfaces.Repositories;
using TaskPlanner.Infrastructure.Data.Commands;
using TaskPlanner.Infrastructure.Data.Connections;
using TaskPlanner.Infrastructure.Data.Repositories;
using TaskPlanner.Infrastructure.Data.Schema;
using TaskPlanner.Infrastructure.Data.Seed;

namespace TaskPlanner.Infrastructure.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionString)
    {
        services.AddSingleton<IDbConnectionFactory>(_ => new DbConnectionFactory(connectionString));
        services.AddScoped<IDbCommandExecutor, DbCommandExecutor>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<ITaskItemRepository, TaskItemRepository>();
        services.AddScoped<IDatabaseInitializer, DatabaseInitializer>();
        services.AddScoped<IDataSeeder, DataSeeder>();

        return services;
    }
}
