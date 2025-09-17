using TaskApi.Dtos;
using TaskApi.Models;

//for mapping dtos to the actual task objects
namespace TaskApi.Mappers;

public static class TaskMappers
{
    public static TaskItem CreateTask(this TaskCreateDto Dto) //This entire thing is completely unfinished and temporary
    {
        var now = DateTime.UtcNow;
        return new TaskItem
        {
            Title = Dto.Title,
            Description = Dto.Description,
            IsComplete = Dto.IsComplete,
            Priority = Dto.Priority ?? Priority.Medium,
            DueDate = Dto.DueDate,
            CreatedAt = now,
            UpdatedAt = now,
        };
    }

    public static void UpdateTask(this TaskItem entity, TaskUpdateDto Dto)
    {
        var now = DateTime.UtcNow;

        entity.Title = Dto.Title;
        entity.Description = Dto.Description;
        entity.IsComplete = Dto.IsComplete;
        if (Dto.Priority.HasValue)
        {
            entity.Priority = Dto.Priority.Value;
        }

        // overwrite due date
        entity.DueDate = Dto.DueDate;

        // make sure to update UpdatedAt and not CreatedAt
        entity.UpdatedAt = now;
    }

    public static TaskReadDto ToReadDto(this TaskItem entity) =>
        new()
        {
            Id = entity.Id,
            Title = entity.Title,
            Description = entity.Description,
            IsComplete = entity.IsComplete,
            Priority = entity.Priority,
            DueDate = entity.DueDate,
            CreatedAt = entity.CreatedAt,
            UpdatedAt = entity.UpdatedAt,
        };

    //might need a helper function for reading a list of tasks
    //TODO
}
