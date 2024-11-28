using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Endpoints;
using TaskManagerAPI.TaskManagerAPI.Data;
using TaskManagerAPI.Validators;

var builder = WebApplication.CreateBuilder(args);

// DbContext
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FluentValidation
builder.Services.AddValidatorsFromAssemblyContaining<ToDoValidator>();

var app = builder.Build();

// Use endpoints for ToDo from different file
app.MapToDoEndpoints();

// Swagger and seeder for development only
if (app.Environment.IsDevelopment())
{
    app.MapToDoSeederEndpoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Default map
app.MapGet("/", () => "Hello World!");

app.Run();
