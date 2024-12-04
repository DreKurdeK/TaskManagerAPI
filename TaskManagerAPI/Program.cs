using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.DAL;
using TaskManagerAPI.Endpoints;
using TaskManagerAPI.Middleware;
using TaskManagerAPI.Services;
using TaskManagerAPI.Validators;

namespace TaskManagerAPI;

public class Program
{
    private static WebApplication CreateApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Logger
        builder.Logging.ClearProviders()
            .AddConsole()
            .AddDebug();

        // DbContext
        builder.Services.AddDbContext<ToDoDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));
        
        // Register services
        builder.Services.AddScoped<ToDoService>();

        // Swagger
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        // FluentValidation
        builder.Services.AddValidatorsFromAssemblyContaining<ToDoValidator>();

        var app = builder.Build();

        // Middleware
        app.UseErrorHandling();

        // Swagger and seeder for development only
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
            app.MapToDoSeederEndpoints();
        }

        // Use endpoints for ToDo from different file
        app.MapToDoEndpoints();

        // Default map
        app.MapGet("/", () => "Hello World!");

        return app;
    }

    public static void Main(string[] args)
    {
        var app = CreateApp(args);
        app.Run();
    }
}