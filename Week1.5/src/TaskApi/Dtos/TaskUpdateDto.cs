using TaskApi.Models;

namespace TaskApi.Dtos;

public class TaskUpdateDto
{
    public string Title { get; set; } = string.Empty; //incase null input

    public string? Description { get; set; }

    public bool IsComplete { get; set; } //undecided if this should be in here tbh

    public Priority? Priority { get; set; } //if null default medium

    public DateTime? DueDate { get; set; }
}
