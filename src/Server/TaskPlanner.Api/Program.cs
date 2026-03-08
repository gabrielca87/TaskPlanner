using FluentValidation;
using FluentValidation.AspNetCore;
using TaskPlanner.Application.Validators.Users;
using TaskPlanner.Infrastructure.Extensions;

namespace TaskPlanner.Api;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("TaskPlannerDatabase")
            ?? throw new InvalidOperationException("Connection string 'TaskPlannerDatabase' is missing.");

        builder.Services.AddControllers();
        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        builder.Services.AddOpenApi();
        builder.Services.AddInfrastructure(connectionString);

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
