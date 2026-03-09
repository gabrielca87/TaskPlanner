using FluentValidation;
using FluentValidation.AspNetCore;
using TaskPlanner.Api.Extensions;
using TaskPlanner.Api.Middlewares;
using TaskPlanner.Application.Mappings;
using TaskPlanner.Application.Validators.Users;

namespace TaskPlanner.Api;

public partial class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("frontend", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                    .AllowAnyHeader()
                    .AllowAnyMethod();
            });
        });

        builder.Services.AddFluentValidationAutoValidation();
        builder.Services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();
        builder.Services.AddAutoMapper(typeof(TaskItemProfile).Assembly);
        builder.Services.AddSwagger();
        builder.Services.AddJwtAuthentication(builder.Configuration);
        builder.Services.AddDependencyInjection(builder.Configuration);

        var app = builder.Build();
        await app.InitializeDatabaseAsync();

        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseMiddleware<ExceptionHandlingMiddleware>();
        app.UseHttpsRedirection();

        app.UseCors("frontend");

        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }
}
