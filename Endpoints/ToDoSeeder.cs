using TaskManagerAPI.TaskManagerAPI.Data;
using TaskManagerAPI.TaskManagerAPI.Models;

namespace TaskManagerAPI.Endpoints;

public static class ToDoSeeder
{
    public static void MapToDoSeederEndpoint(this IEndpointRouteBuilder app)
    {
        // Seed with todos for tests
        app.MapPost("/todos/seed", async (ToDoDbContext db) =>
        {
            // Skip seeding when there are already some todos
            if (db.ToDos.Any())
            {
                return Results.Ok(new { message = "There are already some Todos" });
            }

            var todos = new List<ToDo>
            {
                new ToDo { Id = Guid.NewGuid(), Title = "Buy groceries", Description = "Milk, Eggs, Bread, and Vegetables", Expiry = DateTime.Today.AddDays(1).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Complete C# project", Description = "Finish the REST API project for task manager", Expiry = DateTime.Today.AddDays(2).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Attend meeting", Description = "Discuss project progress with the team", Expiry = DateTime.Today.AddDays(3).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Doctor appointment", Description = "Annual health checkup", Expiry = DateTime.Today.AddDays(4).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Clean the house", Description = "Tidy up the living room and kitchen", Expiry = DateTime.Today.AddDays(5).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Study for exam", Description = "Review study materials for upcoming exam", Expiry = DateTime.Today.AddDays(6).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Pay bills", Description = "Pay utility and phone bills", Expiry = DateTime.Today.AddDays(7).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Plan vacation", Description = "Book flights and hotels for summer trip", Expiry = DateTime.Today.AddDays(8).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Update resume", Description = "Add latest work experience to my CV", Expiry = DateTime.Today.AddDays(9).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Watch a movie", Description = "Watch the new Marvel movie", Expiry = DateTime.Today.AddDays(10).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Read a book", Description = "Finish reading 'Clean Code'", Expiry = DateTime.Today.AddDays(11).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Prepare presentation", Description = "Prepare slides for the client presentation", Expiry = DateTime.Today.AddDays(12).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Clean car", Description = "Wash and vacuum the car", Expiry = DateTime.Today.AddDays(13).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Organize files", Description = "Organize digital files on the computer", Expiry = DateTime.Today.AddDays(14).ToUniversalTime() },
                new ToDo { Id = Guid.NewGuid(), Title = "Reply to emails", Description = "Respond to work and personal emails", Expiry = DateTime.Today.AddDays(15).ToUniversalTime() },
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