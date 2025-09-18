using System.ComponentModel.DataAnnotations;
using TaskApi.Models;

namespace TaskApi.Dtos;

public class TaskUpdateDto
{
    [Required, MaxLength(100)]
    public string Title { get; set; } = string.Empty; //incase null input

    [MaxLength(500)]
    public string? Description { get; set; }

    [Required]
    public bool IsComplete { get; set; } //undecided if this should be in here tbh

    [EnumDataType(typeof(Priority))]
    public Priority? Priority { get; set; } //if null default medium

    public DateTime? DueDate { get; set; }
}
