using TaskApi.Dtos;
using TaskApi.Models;

//for mapping dtos to the actual task objects
namespace TaskApi.Mappers;

public static class TaskMappers
{
    public static TaskItem CreateTask() //This entire thing is completely unfinished and temporary
    {
        var task = new TaskItem
        {
            Title = "",
            Description = "",
            Priority = Priority.High,
        };
        return task;
    }
}
