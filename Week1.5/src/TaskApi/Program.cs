using Serilog;
using TaskApi.Contracts;
using TaskApi.Dtos;
using TaskApi.Mappers;
using TaskApi.Models;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger(); //makes logger pretty

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// TEMPORARY trying out using singleton in memory store for taskitem list, seems pretty cool
builder.Services.AddSingleton<List<TaskItem>>();

var app = builder.Build();

if (app.Environment.IsDevelopment()) //only runs if not in prod and enables swagger
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

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
        var entity = dto.CreateTask();
        entity.Id = store.Count == 0 ? 1 : store.Max(t => t.Id) + 1; //id incrementation

        store.Add(entity);
        return Results.Created($"/api/tasks/{entity.Id}", entity.ToReadDto());
    }
);

app.Run();
