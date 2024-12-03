using TaskManagerAPI.Models;
using TaskManagerAPI.Services;
using FluentValidation;

namespace TaskManagerAPI.Endpoints;

public static class ToDoEndpoints
{
    public static void MapToDoEndpoints(this IEndpointRouteBuilder app)
    {
        app.MapGet("/todos", async (ToDoService toDoService) =>
            await toDoService.GetAllToDosAsync());

        app.MapGet("/todos/{id:guid}", async (Guid todoId, ToDoService toDoService) =>
        {
            if (todoId == Guid.Empty) return Results.BadRequest();
            var toDoItem = await toDoService.GetToDoByIdAsync(todoId);
            return toDoItem is not null ? Results.Ok(toDoItem) : Results.NotFound();
        });

        app.MapGet("/todos/search/{title}", async (string searchTitle, ToDoService toDoService) =>
        {
            var matchingToDos = await toDoService.GetToDosByTitleAsync(searchTitle);
            return matchingToDos.Count > 0 
                ? Results.Ok(matchingToDos) 
                : Results.NotFound(new { message = "No ToDo items found with the specified title." });
        });

        app.MapGet("/todos/upcoming/{days:int}", async (int daysAhead, ToDoService toDoService) =>
        {
            if (daysAhead < 1) return Results.BadRequest();
            var endDate = DateTimeOffset.UtcNow.AddDays(daysAhead).Date;
            var upcomingToDos = await toDoService.GetToDosForDateRangeAsync(DateTimeOffset.UtcNow, endDate);
            return Results.Ok(upcomingToDos);
        });

        app.MapGet("/todos/today", async (ToDoService toDoService) =>
        {
            var startOfDay = DateTimeOffset.UtcNow.Date;
            var endOfDay = startOfDay.AddDays(1).AddTicks(-1);
            var todayToDos = await toDoService.GetToDosForDateRangeAsync(startOfDay, endOfDay);
            return Results.Ok(todayToDos);
        });

        app.MapGet("/todos/next-day", async (ToDoService toDoService) =>
        {
            var startOfNextDay = DateTimeOffset.UtcNow.AddDays(1).Date;
            var endOfNextDay = startOfNextDay.AddDays(1).AddTicks(-1);
            var nextDayToDos = await toDoService.GetToDosForDateRangeAsync(startOfNextDay, endOfNextDay);
            return Results.Ok(nextDayToDos);
        });

        app.MapGet("/todos/current-week", async (ToDoService toDoService) =>
        {
            var today = DateTimeOffset.UtcNow;
            var startOfWeek = today.AddDays(-(int)today.DayOfWeek + (int)DayOfWeek.Monday).Date;
            var endOfWeek = startOfWeek.AddDays(6);
            var currentWeekToDos = await toDoService.GetToDosForDateRangeAsync(startOfWeek, endOfWeek);
            return Results.Ok(currentWeekToDos);
        });

        app.MapPost("/todos", async (ToDo newToDo, IValidator<ToDo> toDoValidator, ToDoService toDoService) =>
        {
            var validation = await toDoValidator.ValidateAsync(newToDo);
            if (!validation.IsValid)
            {
                return Results.BadRequest(validation.Errors);
            }

            await toDoService.CreateToDoAsync(newToDo);
            return Results.Created($"/todos/{newToDo.Id}", newToDo);
        });

        app.MapPut("/todos/{id:guid}", async (Guid todoId, ToDo updatedToDo, ToDoService toDoService) =>
        {
            var existingToDo = await toDoService.GetToDoByIdAsync(todoId);
            if (existingToDo == null)
            {
                return Results.NotFound(new { message = "ToDo item not found." });
            }

            await toDoService.UpdateToDoAsync(todoId, updatedToDo);
            return Results.NoContent();
        });

        app.MapPatch("/todos/{id:guid}/done", async (Guid todoId, ToDoService toDoService) =>
        {
            var existingToDo = await toDoService.GetToDoByIdAsync(todoId);
            if (existingToDo == null)
            {
                return Results.NotFound(new { message = "ToDo item not found." });
            }

            await toDoService.MarkToDoAsDoneAsync(todoId);
            return Results.NoContent();
        });

        app.MapDelete("/todos/{id:guid}", async (Guid todoId, ToDoService toDoService) =>
        {
            var existingToDo = await toDoService.GetToDoByIdAsync(todoId);
            if (existingToDo == null)
            {
                return Results.NotFound(new { message = "ToDo item not found." });
            }

            await toDoService.DeleteToDoAsync(todoId);
            return Results.NoContent();
        });
    }
}
