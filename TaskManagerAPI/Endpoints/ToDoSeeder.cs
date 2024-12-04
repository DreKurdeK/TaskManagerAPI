using TaskManagerAPI.DAL;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Endpoints;

public static class ToDoSeeder
{
    public static void MapToDoSeederEndpoints(this IEndpointRouteBuilder endpoints)
    {
        var environment = endpoints.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

        if (environment.IsDevelopment() || environment.IsEnvironment("Test"))
        {
            endpoints.MapGet("/todos/seed", async (ToDoDbContext dbContext) =>
            {
                if (dbContext.ToDos.Any())
                {
                    return Results.Ok(new { message = "Todos already exist in the database." });
                }

                var seedData = new List<ToDo>
                {
                    new() { Id = Guid.NewGuid(), Title = "Buy groceries", Description = "Milk, Eggs, Bread, and Vegetables", Expiry = DateTimeOffset.UtcNow.AddDays(1) },
                    new() { Id = Guid.NewGuid(), Title = "Complete C# project", Description = "Finish the REST API project for task manager", Expiry = DateTimeOffset.UtcNow.AddDays(2) },
                    new() { Id = Guid.NewGuid(), Title = "Attend meeting", Description = "Discuss project progress with the team", Expiry = DateTimeOffset.UtcNow.AddDays(3) },
                    new() { Id = Guid.NewGuid(), Title = "Doctor appointment", Description = "Annual health checkup", Expiry = DateTimeOffset.UtcNow.AddDays(4) },
                    new() { Id = Guid.NewGuid(), Title = "Clean the house", Description = "Tidy up the living room and kitchen", Expiry = DateTimeOffset.UtcNow.AddDays(5) },
                    new() { Id = Guid.NewGuid(), Title = "Study for exam", Description = "Review study materials for upcoming exam", Expiry = DateTimeOffset.UtcNow.AddDays(6) },
                    new() { Id = Guid.NewGuid(), Title = "Pay bills", Description = "Pay utility and phone bills", Expiry = DateTimeOffset.UtcNow.AddDays(7) },
                    new() { Id = Guid.NewGuid(), Title = "Plan vacation", Description = "Book flights and hotels for summer trip", Expiry = DateTimeOffset.UtcNow.AddDays(8) },
                    new() { Id = Guid.NewGuid(), Title = "Update resume", Description = "Add latest work experience to my CV", Expiry = DateTimeOffset.UtcNow.AddDays(9) },
                    new() { Id = Guid.NewGuid(), Title = "Watch a movie", Description = "Watch the new Marvel movie", Expiry = DateTimeOffset.UtcNow.AddDays(10) },
                    new() { Id = Guid.NewGuid(), Title = "Read a book", Description = "Finish reading 'Clean Code'", Expiry = DateTimeOffset.UtcNow.AddDays(11) },
                    new() { Id = Guid.NewGuid(), Title = "Prepare presentation", Description = "Prepare slides for the client presentation", Expiry = DateTimeOffset.UtcNow.AddDays(12) },
                    new() { Id = Guid.NewGuid(), Title = "Clean car", Description = "Wash and vacuum the car", Expiry = DateTimeOffset.UtcNow.AddDays(13) },
                    new() { Id = Guid.NewGuid(), Title = "Organize files", Description = "Organize digital files on the computer", Expiry = DateTimeOffset.UtcNow.AddDays(14) },
                    new() { Id = Guid.NewGuid(), Title = "Reply to emails", Description = "Respond to work and personal emails", Expiry = DateTimeOffset.UtcNow.AddDays(15) },
                };

                dbContext.ToDos.AddRange(seedData);
                await dbContext.SaveChangesAsync();

                return Results.Ok(new { message = "Seed data added to Todos successfully." });
            });

            endpoints.MapDelete("/todos/unseed", async (ToDoDbContext dbContext) =>
            {
                var existingTodos = dbContext.ToDos.ToList();

                if (!existingTodos.Any())
                {
                    return Results.NotFound(new { message = "No Todos available to remove." });
                }

                dbContext.ToDos.RemoveRange(existingTodos);
                await dbContext.SaveChangesAsync();

                return Results.Ok(new { message = "All Todos have been deleted successfully." });
            });
        }
    }
}
