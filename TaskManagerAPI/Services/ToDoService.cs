using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.Data;
using TaskManagerAPI.Models;

namespace TaskManagerAPI.Services;

public class ToDoService(ToDoDbContext dbContext)
{
    public async Task<List<ToDo>> GetAllToDosAsync()
    {
        return await dbContext.ToDos.ToListAsync();
    }

    public async Task<ToDo?> GetToDoByIdAsync(Guid todoId)
    {
        return await dbContext.ToDos.FindAsync(todoId);
    }

    public async Task<List<ToDo>> GetToDosByTitleAsync(string searchTitle)
    {
        return await dbContext.ToDos
            .Where(todo => EF.Functions.Like(todo.Title, $"%{searchTitle}%"))
            .ToListAsync();
    }

    public async Task<List<ToDo>> GetToDosForDateRangeAsync(DateTimeOffset startDate, DateTimeOffset endDate)
    {
        return await dbContext.ToDos
            .Where(todo => todo.Expiry.Date >= startDate.Date && todo.Expiry.Date <= endDate.Date)
            .ToListAsync();
    }

    public async Task CreateToDoAsync(ToDo newToDo)
    {
        newToDo.Id = Guid.NewGuid();
        newToDo.IsDone ??= false;
        newToDo.PercentComplete ??= 0;

        dbContext.ToDos.Add(newToDo);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateToDoAsync(Guid todoId, ToDo updatedToDo)
    {
        var existingToDo = await GetToDoByIdAsync(todoId);
        if (existingToDo is not null)
        {
            if (!string.IsNullOrEmpty(updatedToDo.Title)) existingToDo.Title = updatedToDo.Title;
            if (!string.IsNullOrEmpty(updatedToDo.Description)) existingToDo.Description = updatedToDo.Description;
            if (updatedToDo.Expiry != default) existingToDo.Expiry = updatedToDo.Expiry;
            if (updatedToDo.PercentComplete is >= 0 and <= 100) existingToDo.PercentComplete = updatedToDo.PercentComplete;
            if (updatedToDo.IsDone != null) existingToDo.IsDone = updatedToDo.IsDone;

            await dbContext.SaveChangesAsync();
        }
    }

    public async Task MarkToDoAsDoneAsync(Guid todoId)
    {
        var existingToDo = await GetToDoByIdAsync(todoId);
        if (existingToDo is not null)
        {
            existingToDo.IsDone = true;
            existingToDo.PercentComplete = 100;
            await dbContext.SaveChangesAsync();
        }
    }

    public async Task DeleteToDoAsync(Guid todoId)
    {
        var existingToDo = await GetToDoByIdAsync(todoId);
        if (existingToDo is not null)
        {
            dbContext.ToDos.Remove(existingToDo);
            await dbContext.SaveChangesAsync();
        }
    }
}
