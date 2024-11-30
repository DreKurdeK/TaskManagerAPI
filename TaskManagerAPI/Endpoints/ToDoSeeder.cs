using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Endpoints;

public static class ToDoSeeder
{
    public static void MapToDoSeederEndpoint(this IEndpointRouteBuilder app)
    {
        var env = app.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
        
        // Only seed data in development or test environments
        if (env.IsDevelopment() || env.IsEnvironment("Test"))
        {
            // Seed with todos for tests
            app.MapGet("/todos/seed", async (ToDoDbContext db) =>
            {
                // Skip seeding when there are already some todos
                if (db.ToDos.Any())
                {
                    return Results.Ok(new { message = "There are already some Todos" });
                }

                var todos = new List<ToDo>
                {
                    new ToDo { Id = Guid.NewGuid(), Title = "Buy groceries", Description = "Milk, Eggs, Bread, and Vegetables", Expiry = DateTimeOffset.UtcNow.AddDays(1)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Complete C# project", Description = "Finish the REST API project for task manager", Expiry = DateTimeOffset.UtcNow.AddDays(2)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Attend meeting", Description = "Discuss project progress with the team", Expiry = DateTimeOffset.UtcNow.AddDays(3)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Doctor appointment", Description = "Annual health checkup", Expiry = DateTimeOffset.UtcNow.AddDays(4)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Clean the house", Description = "Tidy up the living room and kitchen", Expiry = DateTimeOffset.UtcNow.AddDays(5)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Study for exam", Description = "Review study materials for upcoming exam", Expiry = DateTimeOffset.UtcNow.AddDays(6)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Pay bills", Description = "Pay utility and phone bills", Expiry = DateTimeOffset.UtcNow.AddDays(7)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Plan vacation", Description = "Book flights and hotels for summer trip", Expiry = DateTimeOffset.UtcNow.AddDays(8)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Update resume", Description = "Add latest work experience to my CV", Expiry = DateTimeOffset.UtcNow.AddDays(9)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Watch a movie", Description = "Watch the new Marvel movie", Expiry = DateTimeOffset.UtcNow.AddDays(10)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Read a book", Description = "Finish reading 'Clean Code'", Expiry = DateTimeOffset.UtcNow.AddDays(11)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Prepare presentation", Description = "Prepare slides for the client presentation", Expiry = DateTimeOffset.UtcNow.AddDays(12)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Clean car", Description = "Wash and vacuum the car", Expiry = DateTimeOffset.UtcNow.AddDays(13)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Organize files", Description = "Organize digital files on the computer", Expiry = DateTimeOffset.UtcNow.AddDays(14)},
                    new ToDo { Id = Guid.NewGuid(), Title = "Reply to emails", Description = "Respond to work and personal emails", Expiry = DateTimeOffset.UtcNow.AddDays(15)},
                };

                db.ToDos.AddRange(todos);
                await db.SaveChangesAsync();


                return Results.Ok(new { message = "Todos seeded successfully" });
            });
            
            // Delete everything (unseed) 
            app.MapDelete("/todos/unseed", async (ToDoDbContext db) =>
            {
                var allToDos = db.ToDos.ToList();
        
                if (!allToDos.Any())
                    return Results.NotFound(new { message = "No Todos to remove." });

                db.ToDos.RemoveRange(allToDos);
                await db.SaveChangesAsync();
        
                return Results.Ok(new { message = "All Todos have been removed." });
            });
        }
    }
}