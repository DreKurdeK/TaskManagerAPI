using TaskManagerAPI.Models;
using TaskManagerAPI.Services;
using FluentValidation;

namespace TaskManagerAPI.Endpoints;

public static class ToDoEndpoints
{
    public static void MapToDoEndpoints(this IEndpointRouteBuilder app)
    {
        // Map Get all ToDo's
        app.MapGet("/todos", async (ToDoService toDoService) =>
            await toDoService.GetAllToDosAsync());

        // Map Get Todo by id
        app.MapGet("/todos/{id:guid}", async (Guid id, ToDoService toDoService) =>
        {
            var todo = await toDoService.GetToDoByIdAsync(id);
            return todo is not null ? Results.Ok(todo) : Results.NotFound();
        });

        // Map Search Todo's by Title
        app.MapGet("/todos/search/{title}", async (string title, ToDoService toDoService) =>
        {
            var todos = await toDoService.GetToDosByTitleAsync(title);
            if (todos.Count == 0)
                return Results.NotFound(new { message = "No Todos found with this title." });

            return Results.Ok(todos);
        });

        // Map Get for todos within a specific number of days
        app.MapGet("/todos/upcoming/{days:int}", async (int days, ToDoService toDoService) =>
        {
            var dateRange = DateTimeOffset.UtcNow.AddDays(days).Date;
            var todos = await toDoService.GetToDosForDateRangeAsync(dateRange, DateTimeOffset.UtcNow);
            return Results.Ok(todos);
        });

        // Map Get for todos for today
        app.MapGet("/todos/today", async (ToDoService toDoService) =>
        {
            var today = DateTimeOffset.UtcNow.Date;
            var todos = await toDoService.GetToDosForDateRangeAsync(today, today.AddDays(1).AddTicks(-1));
            return Results.Ok(todos);
        });

        // Map Get for todos for tommorow
        app.MapGet("/todos/next-day", async (ToDoService toDoService) =>
        {
            var tomorrow = DateTimeOffset.UtcNow.AddDays(1).Date;
            var todos = await toDoService.GetToDosForDateRangeAsync(tomorrow, tomorrow.AddDays(1).AddTicks(-1));
            return Results.Ok(todos);
        });

        // Map Get for todos for this week
        app.MapGet("/todos/current-week", async (ToDoService toDoService) =>
        {
            var currentDate = DateTimeOffset.UtcNow;
    
            // Calculate start of the week and end of the week
            var startOfWeek = currentDate.AddDays(-(int)currentDate.DayOfWeek + (int)DayOfWeek.Monday).Date;
            var endOfWeek = startOfWeek.AddDays(6);

            // Return todos for the current week
            var todos = await toDoService.GetToDosForDateRangeAsync(startOfWeek, endOfWeek);
            return Results.Ok(todos);
        });


        // Map Creating Todo -
        app.MapPost("/todos", async (ToDo todo, IValidator<ToDo> validator, ToDoService toDoService) =>
        {
            var validationResult = await validator.ValidateAsync(todo);
            if (!validationResult.IsValid)
            {
                return Results.BadRequest(validationResult.Errors);
            }

            await toDoService.CreateToDoAsync(todo);
            return Results.Created($"/todos/{todo.Id}", todo);
        });

        // Map Update Todo -
        app.MapPut("/todos/{id:guid}", async (Guid id, ToDo updatedTodo, ToDoService toDoService) =>
        {
            await toDoService.UpdateToDoAsync(id, updatedTodo);
            return Results.NoContent();
        });

        // Map Mark Todo as done by Id
        app.MapPatch("/todos/{id:guid}/done", async (Guid id, ToDoService toDoService) =>
        {
            await toDoService.MarkToDoAsDoneAsync(id);
            return Results.NoContent();
        });

        // Map Delete Todo by Id
        app.MapDelete("/todos/{id:guid}", async (Guid id, ToDoService toDoService) =>
        {
            await toDoService.DeleteToDoAsync(id);
            return Results.NoContent();
        });
    }
}
