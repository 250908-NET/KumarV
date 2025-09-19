using System.Security.Claims;
using TaskApi.Contracts;
using TaskApi.Dtos;
using TaskApi.Mappers;
using TaskApi.Models;
using TaskApi.Repositories;
using TaskApi.Services;
using TaskApi.Validation;

namespace TaskApi.Endpoints;

public static class MinimalEndpoints
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder app)
    {
        MapAuth(app);
        MapTasks(app);
        return app;
    }

    // AUTH ENDPOINTS

    private static void MapAuth(IEndpointRouteBuilder app)
    {
        // POST /api/auth/register
        app.MapPost(
                "/api/auth/register",
                async (RegisterDto dto, IUserRepository users, CancellationToken ct) =>
                {
                    if (
                        string.IsNullOrWhiteSpace(dto.UserName)
                        || string.IsNullOrWhiteSpace(dto.Password)
                    )
                        return Results.BadRequest(
                            new ApiError { Errors = { "Username and password are required" } }
                        );

                    if (
                        dto.UserName.Length < 3
                        || dto.UserName.Length > 50
                        || dto.Password.Length < 6
                        || dto.Password.Length > 128
                    )
                        return Results.BadRequest(
                            new ApiError { Errors = { "Invalid username or password length" } }
                        );

                    var existing = await users.GetByUsernameAsync(dto.UserName, ct);
                    if (existing is not null)
                        return Results.BadRequest(
                            new ApiError { Errors = { "Username already taken" } }
                        );

                    var user = new User
                    {
                        UserName = dto.UserName.Trim(),
                        Password = dto.Password.Trim(), // plaintext for now; swap to hashing later
                        CreatedAt = DateTime.UtcNow,
                    };

                    var saved = await users.CreateAsync(user, ct);

                    return Results.Ok(
                        new ApiResponse<object>
                        {
                            Data = new
                            {
                                saved.Id,
                                saved.UserName,
                                saved.CreatedAt,
                            },
                            Message = "Registered",
                        }
                    );
                }
            )
            .WithTags("Auth");

        // POST /api/auth/login
        app.MapPost(
                "/api/auth/login",
                async (
                    LoginDto dto,
                    IUserRepository users,
                    ITokenService tokens,
                    CancellationToken ct
                ) =>
                {
                    if (
                        string.IsNullOrWhiteSpace(dto.UserName)
                        || string.IsNullOrWhiteSpace(dto.Password)
                    )
                        return Results.BadRequest(
                            new ApiError { Errors = { "Username and password are required" } }
                        );

                    var user = await users.GetByUsernameAsync(dto.UserName, ct);
                    if (user is null || user.Password != dto.Password)
                        return Results.Unauthorized();

                    var jwt = tokens.CreateToken(user.Id, user.UserName);

                    return Results.Ok(
                        new ApiResponse<AuthResponseDto>
                        {
                            Data = new AuthResponseDto { Token = jwt },
                            Message = "Login ok",
                        }
                    );
                }
            )
            .WithTags("Auth");
    }

    // TASK ENDPOINTS JWT required

    private static void MapTasks(IEndpointRouteBuilder app)
    {
        static string GetUserId(HttpContext ctx) =>
            ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new Exception("Missing user id claim");

        var tasks = app.MapGroup("/api/tasks").RequireAuthorization().WithTags("Tasks");

        // GET /api/tasks (current user's tasks)
        tasks.MapGet(
            "/",
            (HttpContext ctx, List<TaskItem> store) =>
            {
                var userId = GetUserId(ctx);
                var mine = store.Where(t => t.UserId == userId).Select(t => t.ToReadDto());

                return Results.Ok(
                    new ApiResponse<IEnumerable<TaskReadDto>>
                    {
                        Data = mine,
                        Message = "Tasks retrieved",
                    }
                );
            }
        );

        // GET /api/tasks/{id}
        tasks.MapGet(
            "/{id}",
            (HttpContext ctx, int id, List<TaskItem> store) =>
            {
                var userId = GetUserId(ctx);
                var entity = store.FirstOrDefault(t => t.Id == id && t.UserId == userId);
                if (entity is null)
                    return Results.NotFound(new ApiError { Errors = { "Task not found" } });

                return Results.Ok(new ApiResponse<TaskReadDto> { Data = entity.ToReadDto() });
            }
        );

        // POST /api/tasks
        tasks.MapPost(
            "/",
            (HttpContext ctx, TaskCreateDto dto, List<TaskItem> store) =>
            {
                var (ok, errors) = TaskValidator.Validate(dto);
                if (!ok)
                    return Results.BadRequest(
                        new ApiError
                        {
                            Errors = errors.SelectMany(kv => kv.Value).ToList(),
                            Message = "Validation failed",
                        }
                    );

                var userId = GetUserId(ctx);
                var now = DateTime.UtcNow;

                var entity = dto.CreateTask();
                entity.Id = store.Count == 0 ? 1 : store.Max(t => t.Id) + 1;
                entity.UserId = userId;
                entity.IsComplete = false;
                entity.CreatedAt = now;
                entity.UpdatedAt = now;

                store.Add(entity);

                return Results.Created(
                    $"/api/tasks/{entity.Id}",
                    new ApiResponse<TaskReadDto> { Data = entity.ToReadDto(), Message = "Created" }
                );
            }
        );

        // PUT /api/tasks/{id}
        tasks.MapPut(
            "/{id}",
            (HttpContext ctx, int id, TaskUpdateDto dto, List<TaskItem> store) =>
            {
                var (ok, errors) = TaskValidator.Validate(dto);
                if (!ok)
                    return Results.BadRequest(
                        new ApiError
                        {
                            Errors = errors.SelectMany(kv => kv.Value).ToList(),
                            Message = "Validation failed",
                        }
                    );

                var userId = GetUserId(ctx);
                var entity = store.FirstOrDefault(t => t.Id == id && t.UserId == userId);
                if (entity is null)
                    return Results.NotFound(new ApiError { Errors = { "Task not found" } });

                entity.UpdateTask(dto);
                entity.UpdatedAt = DateTime.UtcNow;

                return Results.Ok(
                    new ApiResponse<TaskReadDto> { Data = entity.ToReadDto(), Message = "Updated" }
                );
            }
        );

        // DELETE /api/tasks/{id}
        tasks.MapDelete(
            "/{id}",
            (HttpContext ctx, int id, List<TaskItem> store) =>
            {
                var userId = GetUserId(ctx);
                var idx = store.FindIndex(t => t.Id == id && t.UserId == userId);
                if (idx < 0)
                    return Results.NotFound(new ApiError { Errors = { "Task not found" } });

                store.RemoveAt(idx);
                return Results.Ok(
                    new ApiResponse<object> { Data = new { id }, Message = "Deleted" }
                );
            }
        );
    }
}
