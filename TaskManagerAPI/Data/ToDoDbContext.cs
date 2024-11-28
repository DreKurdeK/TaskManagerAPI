using Microsoft.EntityFrameworkCore;
using TaskManagerAPI.TaskManagerAPI.Models;

namespace TaskManagerAPI.TaskManagerAPI.Data;

public class ToDoDbContext(DbContextOptions<ToDoDbContext> options) : DbContext(options)
{
    public DbSet<ToDo> ToDos { get; set; }
}