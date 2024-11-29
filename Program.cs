using FluentValidation;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Endpoints;
using TaskManagerAPI.Middleware;
using TaskManagerAPI.Data;
using TaskManagerAPI.Validators;

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

app.Run();