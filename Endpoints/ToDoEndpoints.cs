using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.TaskManagerAPI.Data;
using TaskManagerAPI.TaskManagerAPI.Models;

namespace TaskManagerAPI.Endpoints;

public static class ToDoEndpoints
{
    public static void MapToDoEndpoints(this IEndpointRouteBuilder app)
    {  
        // Get All Todo's
        app.MapGet("/todos", async (ToDoDbContext db) =>
            await db.ToDos.ToListAsync());
        
        // Get Todo by id 
        app.MapGet("/todos/{id}", async (ToDoDbContext db, Guid id) =>
            await db.ToDos.FindAsync(id)
                is ToDo todo
                ? Results.Ok(todo)
                : Results.NotFound());
                
        // Search Todo's by Title 
        app.MapGet("/todos/search/{title}", async (ToDoDbContext db, string title) =>
        {
            var todos = await db.ToDos
                .Where(t => t.Title.Contains(title, StringComparison.OrdinalIgnoreCase))
                .ToListAsync();
            
            if (todos.Count == 0)
                return Results.NotFound(new {message = "No Todos found with this title."});

            return Results.Ok(todos);
        });
        
        // Get Todo's for today
        app.MapGet("/todos/upcoming/today", async (ToDoDbContext db) =>
            await db.ToDos.Where(t => t.Expiry.Date == DateTime.Today).ToListAsync());
        
        // Get Todo's for tomorrow
        app.MapGet("/todos/upcoming/tomorrow", async (ToDoDbContext db) =>
            await db.ToDos.Where(t => t.Expiry.Date == DateTime.Today.AddDays(1)).ToListAsync());
        
        // Get Todo's for current week
        app.MapGet("/todos/upcoming/tomorrow", async (ToDoDbContext db) =>
            await db.ToDos.Where(
                t => t.Expiry.Date >= DateTime.Today && t.Expiry.Date <= DateTime.Today.AddDays(7)).ToListAsync());
        
        // Get Todo's for specifif number of days
        app.MapGet("/todos/upcoming/{days:int}", async (ToDoDbContext db, int days) =>
        {
            var dateRange = DateTime.Today.AddDays(days);
            return await db.ToDos.Where(t => t.Expiry.Date <= dateRange).ToListAsync();
        });

    }
}