using System;
using System.ComponentModel.DataAnnotations;

namespace TaskManagerAPI.Models;

public class ToDo
{
    public Guid Id { get; set; }
    
    [MaxLength(100)]
    public required string Title { get; set; }
    
    [MaxLength(500)]
    public required string Description { get; set; }
    public DateTimeOffset Expiry { get; set; }
    public int? PercentComplete { get; set; } = 0;
    public bool? IsDone { get; set; } = false;
}