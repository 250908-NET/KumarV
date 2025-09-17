using TaskApi.Models;

namespace TaskApi.Dtos;

public class TaskReadDto
{
    public int Id { get; set; }

    public string Title { get; set; } = string.Empty; //handles null input

    public string? Description { get; set; }

    public bool IsComplete { get; set; } //undecided if this should be in here tbh

    public Priority Priority { get; set; } //not nullable since default medium

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; }

    public DateTime UpdatedAt { get; set; }
}
