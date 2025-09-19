namespace TaskApi.Models;

public enum Priority
{
    Low = 0,
    Medium = 1,
    High = 2,
    Critical = 3,
}

public class TaskItem
{
    public int Id { get; set; }

    public string Title { get; set; } = default!;

    public string? Description { get; set; }

    public bool IsComplete { set; get; } = false;

    public Priority Priority { set; get; } = Priority.Medium;

    public DateTime? DueDate { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public DateTime UpdatedAt { get; set; } = DateTime.Now;

    public string UserId { get; set; } = default!; //new userId
}
