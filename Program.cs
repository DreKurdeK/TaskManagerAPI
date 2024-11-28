using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Endpoints;
using TaskManagerAPI.TaskManagerAPI.Data;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<ToDoDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Use endpoints for ToDo from different file
app.MapToDoEndpoints();

if (app.Environment.IsDevelopment())
{
    app.MapToDoSeederEndpoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.Run();
