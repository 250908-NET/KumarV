using System.Text.Json;
using System.Text.Json.Serialization;
using Deadlock.Api.Data;
using Deadlock.Api.DTO;
using Deadlock.Api.Models;
using Deadlock.Api.Repository;
using Deadlock.Api.Service;
using Microsoft.EntityFrameworkCore;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

string CS = File.ReadAllText("../connection_string.env");

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DeadlockDbContext>(options => options.UseSqlServer(CS));

builder.Services.AddScoped<IHeroRepository, HeroRepository>();
builder.Services.AddScoped<IHeroService, HeroService>();

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

//serilogger
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(builder.Configuration).CreateLogger(); // read from appsettings.json
builder.Host.UseSerilog();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.UseSwagger();
    app.UseSwaggerUI();
}

//string ConnectionString =
//   ""; //this is temporary to make sure we can actually join the db.

app.MapGet(
    "/",
    () =>
    {
        return "Hello world";
    }
);

/* ******************************************************************************** */
//testing HERO apis
app.MapGet(
    "/heroes",
    async (ILogger<Program> logger, IHeroService service) =>
    {
        logger.LogInformation("Getting all heroes");
        return Results.Ok(await service.GetAllAsync());
    }
);

app.MapPost(
    "/heroes",
    async (ILogger<Program> logger, IHeroService service, HeroCreateDto dto) =>
    {
        logger.LogInformation($"Creating hero  named {dto.Name}");
        var hero = new Hero { Name = dto.Name };
        Hero createdHero = await service.CreateAsync(hero);
        return Results.Created($"/heroes/{hero.Id}", createdHero);
    }
);

/* ******************************************************************************** */
app.UseHttpsRedirection();

app.Run();
