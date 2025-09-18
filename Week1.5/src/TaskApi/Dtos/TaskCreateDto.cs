using System.ComponentModel.DataAnnotations;
using TaskApi.Models;

namespace TaskApi.Dtos;

public class TaskCreateDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty; //handles null input

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsComplete { get; set; } //undecided if this should be in here tbh

    [EnumDataType(typeof(Priority))]
    public Priority? Priority { get; set; } //if null default medium

    public DateTime? DueDate { get; set; }
}
