using FluentValidation;
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
        {
            var todo = await GetTodoByIdAsync(db, id);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        });
                
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
            await db.ToDos.Where(t => t.Expiry.Date == DateTimeOffset.UtcNow.Date).ToListAsync());
        
        // Get Todo's for tomorrow
        app.MapGet("/todos/upcoming/tomorrow", async (ToDoDbContext db) =>
            await db.ToDos.Where(t => t.Expiry.Date == DateTimeOffset.UtcNow.AddDays(1).Date).ToListAsync());
        
        // Get Todo's for current week
        app.MapGet("/todos/upcoming/week", async (ToDoDbContext db) =>
            await db.ToDos.Where(
                t => t.Expiry.Date >= DateTimeOffset.UtcNow.Date && t.Expiry.Date <= DateTimeOffset.UtcNow.AddDays(7).Date).ToListAsync());
        
        // Get Todo's for specifif number of days
        app.MapGet("/todos/upcoming/{days:int}", async (ToDoDbContext db, int days) =>
        {
            var dateRange = DateTimeOffset.UtcNow.AddDays(days).Date;
            return await db.ToDos.Where(t => t.Expiry.Date <= dateRange).ToListAsync();
        });
        
        // Creating Todo
        app.MapPost("/todos", async (ToDoDbContext db, ToDo todo, IValidator<ToDo> validator) =>
        {
            var validationResult = await validator.ValidateAsync(todo);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }
            
            todo.Id = Guid.NewGuid();

            db.ToDos.Add(todo);
            await db.SaveChangesAsync();

            return Results.Created($"/todos/{todo.Id}", todo);
        });

        // Update Todo
        app.MapPut("/todos/{id}", async (ToDoDbContext db, Guid id, ToDo updatedTodo) =>
        {
            var todo = await GetTodoByIdAsync(db, id);
            if (todo is null) return Results.NotFound();

            // Skiping guid - Updated doens't need it
            // Only update provided fields
            if (!string.IsNullOrEmpty(updatedTodo.Title))
                todo.Title = updatedTodo.Title;

            if (!string.IsNullOrEmpty(updatedTodo.Description))
                todo.Description = updatedTodo.Description;

            if (updatedTodo.Expiry != default)
                todo.Expiry = updatedTodo.Expiry;

            if (updatedTodo.PercentComplete >= 0 && updatedTodo.PercentComplete <= 100)
                todo.PercentComplete = updatedTodo.PercentComplete;

            if (updatedTodo.IsDone != null)
                todo.IsDone = updatedTodo.IsDone;

            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        // Set Todo complition percentage by id
        app.MapPatch("/todos/{id:guid}/percent", async (ToDoDbContext db, Guid id, int percent) =>
        {
            var todo = await GetTodoByIdAsync(db, id);
            if (todo is null) return Results.NotFound();

            todo.PercentComplete = percent;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        // Set Todo complition percentage by Title
        app.MapPatch("/todos/{title}/percent", async (ToDoDbContext db, string title, int percent) =>
        {
            var todo = await GetTodoByTitleAsync(db, title);
            if (todo is null) return Results.NotFound();

            todo.PercentComplete = percent;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        // Mark Todo as done by Id
        app.MapPatch("/todos/{id:guid}/done", async (ToDoDbContext db, Guid id) =>
        {
            var todo = await GetTodoByIdAsync(db, id);
            if (todo is null) return Results.NotFound();

            todo.IsDone = true;
            todo.PercentComplete = 100;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        // Mark Todo as done by Title
        app.MapPatch("/todos/{title}/done", async (ToDoDbContext db, string title, int percent) =>
        {
            var todo = await GetTodoByTitleAsync(db, title);
            if (todo is null) return Results.NotFound();

            todo.IsDone = true;
            todo.PercentComplete = 100;
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        // Delete Todo by Id
        app.MapDelete("/todos/{id}", async (ToDoDbContext db, Guid id) =>
        {
            var todo = await GetTodoByIdAsync(db, id);
            if (todo == null) return Results.NotFound();

            db.ToDos.Remove(todo);
            await db.SaveChangesAsync();
            return Results.NoContent();
        });
        
        static async Task<ToDo?> GetTodoByIdAsync(ToDoDbContext db, Guid id)
        {
            return await db.ToDos.FindAsync(id);
        }

        static async Task<ToDo?> GetTodoByTitleAsync(ToDoDbContext db, string title)
        {
            return await db.ToDos
                .FirstOrDefaultAsync(t => t.Title.Equals(title, StringComparison.OrdinalIgnoreCase));
        }
    }
}