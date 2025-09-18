using TaskApi.Contracts;
using TaskApi.Dtos;
//using TaskApi.Endpoints;
using TaskApi.Mappers;
using TaskApi.Models;
using TaskApi.Validation;

namespace TaskApi.Endpoints;

public static class MinimalEndpoints
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        //testing if my endpoints work and if everything is functional
        // GET /api/tasks
        app.MapGet(
            "/api/tasks",
            (List<TaskItem> store) =>
            {
                // Just return the list mapped using our dto, maybe add this as helper in mappers
                return Results.Ok(store.Select(t => t.ToReadDto()));
            }
        );

        // POST /api/tasks
        app.MapPost(
            "/api/tasks",
            (TaskCreateDto dto, List<TaskItem> store) =>
            {
                var (ok, errors) = TaskValidator.Validate(dto);
                if (!ok)
                    return Results.ValidationProblem(errors);

                var entity = dto.CreateTask();
                entity.Id = store.Count == 0 ? 1 : store.Max(t => t.Id) + 1; //id incrementation

                store.Add(entity);
                return Results.Created($"/api/tasks/{entity.Id}", entity.ToReadDto());
            }
        );

        //PUT /api/tasks/{id}
        app.MapPut(
            "/api/tasks/{id}",
            (int id, TaskUpdateDto dto, List<TaskItem> store) =>
            {
                var (ok, errors) = TaskValidator.Validate(dto);
                if (!ok)
                    return Results.ValidationProblem(errors);

                var entity = store.FirstOrDefault(t => t.Id == id); //FirstOrDefault loops through and checks if check else null (for each t in list)
                if (entity is null)
                    return Results.NotFound();

                entity.UpdateTask(dto);
                return Results.Ok(entity.ToReadDto());
            }
        );

        //GET /api/tasks/{id}
        app.MapGet(
            "/api/tasks/{id}",
            (int id, List<TaskItem> store) =>
            {
                var entity = store.FirstOrDefault(t => t.Id == id);
                return Results.Ok(entity.ToReadDto());
            }
        );

        //DELETE /api/tasks/{id}
        app.MapDelete(
            "/api/tasks/{id}",
            (int id, List<TaskItem> store) =>
            {
                var entity = store.FirstOrDefault(t => t.Id == id);
                if (entity is null)
                {
                    return Results.NotFound();
                }
                store.Remove(entity);
                return Results.NoContent();
            }
        );

        return app;
    }
}
