﻿// Program.cs

using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TaskManagerAPI.Data;
using TaskManagerAPI.Endpoints;
using TaskManagerAPI.Middleware;
using TaskManagerAPI.Validators;

public class Program
{
    public static WebApplication CreateApp(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Logger
        builder.Logging.ClearProviders()
            .AddConsole()
            .AddDebug();

        // DbContext
        builder.Services.AddDbContext<ToDoDbContext>(options =>
            options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

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
            app.MapToDoSeederEndpoint();
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