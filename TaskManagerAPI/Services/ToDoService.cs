using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services;

public class ToDoService(ToDoDbContext dbContext)
{
    public async Task<List<ToDo>> GetAllToDosAsync()
    {
        // get all the todo items from db
        return await dbContext.ToDos.ToListAsync();
    }

    public async Task<ToDo?> GetToDoByIdAsync(Guid id)
    {
        // This function fetch a todo by its unique ID
        return await dbContext.ToDos.FindAsync(id);
    }

    public async Task<List<ToDo>> GetToDosByTitleAsync(string title)
    {
        // search todos by Title using the title keyword
        return await dbContext.ToDos
            .Where(t => EF.Functions.Like(t.Title, $"%{title}%"))
            .ToListAsync();
    }

    public async Task<List<ToDo>> GetToDosForDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        // Return all todos within the date range
        return await dbContext.ToDos
            .Where(t => t.Expiry.Date >= startDate.Date && t.Expiry.Date <= endDate.Date)
            .ToListAsync();
    }
    

    public async Task CreateToDoAsync(ToDo todo)
    {
        // Add todo to db if it does not exists already
        todo.Id = Guid.NewGuid();
        todo.IsDone ??= false;
        todo.PercentComplete ??= 0;

        dbContext.ToDos.Add(todo);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateToDoAsync(Guid id, ToDo updatedTodo)
    {
        var todo = await GetToDoByIdAsync(id);
        if (todo is not null)
        {
            // Update only the properties that are set
            if (!string.IsNullOrEmpty(updatedTodo.Title)) todo.Title = updatedTodo.Title;
            if (!string.IsNullOrEmpty(updatedTodo.Description)) todo.Description = updatedTodo.Description;
            if (updatedTodo.Expiry != default) todo.Expiry = updatedTodo.Expiry;
            if (updatedTodo.PercentComplete >= 0 && updatedTodo.PercentComplete <= 100) todo.PercentComplete = updatedTodo.PercentComplete;
            if (updatedTodo.IsDone != null) todo.IsDone = updatedTodo.IsDone;

            await dbContext.SaveChangesAsync();
        }
    }

    public async Task MarkToDoAsDoneAsync(Guid id)
    {
        var todo = await GetToDoByIdAsync(id);
        if (todo is not null)
        {
            todo.IsDone = true;
            todo.PercentComplete = 100;
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteToDoAsync(Guid id)
    {
        var todo = await GetToDoByIdAsync(id);
        if (todo is not null)
        {
            dbContext.ToDos.Remove(todo);
            await dbContext.SaveChangesAsync();
        }
    }
}
