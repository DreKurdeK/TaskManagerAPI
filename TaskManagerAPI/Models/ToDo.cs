using System;

namespace TaskManagerAPI.Models;

public class ToDo
{
    public Guid Id { get; set; }
    public required string Title { get; set; }
    public required string Description { get; set; }
    public DateTimeOffset Expiry { get; set; }
    public int PercentComplete { get; set; } = 0;
    public bool? IsDone { get; set; } = false;
}