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

builder.Services.AddScoped<IBuildRepository, BuildRepository>();
builder.Services.AddScoped<IBuildService, BuildService>();

builder.Services.AddScoped<IItemRepository, ItemRepository>();
builder.Services.AddScoped<IItemService, ItemService>();

/*builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
}); */

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
        var heroes = await service.GetAllAsync();

        // Select maps everything in a list to a new var shape
        var result = heroes.Select(h => new HeroReadDto(h.Id, h.Name));
        return Results.Ok(result);
    }
);

app.MapPost(
    "/heroes",
    async (ILogger<Program> logger, IHeroService service, HeroCreateDto dto) =>
    {
        logger.LogInformation($"Creating hero  named {dto.Name}");
        var hero = new Hero { Name = dto.Name };
        Hero createdHero = await service.CreateAsync(hero);
        //return Results.Created($"/heroes/{hero.Id}", createdHero);

        var readDto = new HeroReadDto(createdHero.Id, createdHero.Name);
        return Results.Created($"/heroes/{createdHero.Id}", readDto);
    }
);

/* ******************************************************************************** */
//testing Build Apis
/*
app.MapPost(
    "/builds",
    async (ILogger<Program> logger, IBuildService service, BuildCreateDto dto) =>
    {
        logger.LogInformation($"Creating build named {dto.Name}");

        // Map DTO entity
        var build = new Build
        {
            Name = dto.Name,
            Desc = dto.Desc,
            HeroId = dto.HeroId,
        };

        // Save
        var created = await service.CreateAsync(build);
        var read = new BuildReadDto(created.Id, created.Name, created.Desc, created.HeroId);

        return Results.Created($"/builds/{created.Id}", read);
    }
);
*/

app.MapPost(
    "/builds",
    async (
        BuildCreateDto dto,
        IBuildService svc, //builer service
        ILogger<Program> logger,
        IHeroService hsvc, //hero service
        IItemService itemSvc
    ) =>
    {
        logger.LogInformation("Creating build {Name}", dto.Name);

        //var heroExists = Exists(dto.HeroId);
        if (!await hsvc.Exists(dto.HeroId))
            return Results.BadRequest(new { error = $"Hero {dto.HeroId} not found." });

        var build = new Build
        {
            Name = dto.Name,
            Desc = dto.Desc,
            HeroId = dto.HeroId,
        };

        //If Itemids provided lets load and attach them
        if (dto.ItemIds is { Count: > 0 })
        {
            var distinctIds = dto.ItemIds.Distinct().ToList();
            var items = new List<Item>(distinctIds.Count);

            foreach (var itemId in distinctIds)
            {
                var item = await itemSvc.GetByIdAsync(itemId);
                if (item is null)
                    return Results.BadRequest(new { error = $"Item {itemId} not found." }); //sanity check item exists

                items.Add(item);
            }

            build.Items.AddRange(items);
        }

        var created = await svc.CreateAsync(build);

        //we can add hero name and items later
        var read = new BuildReadDto(created.Id, created.Name, created.Desc, created.HeroId);
        return Results.Created($"/builds/{created.Id}", read);
    }
);

app.MapGet(
    "/builds/{id}",
    async (int id, IBuildService svc) =>
    {
        var build = await svc.GetByIdAsync(id);
        if (build is null)
            return Results.NotFound();

        var dto = new BuildReadWithItemsDto(
            build.Id,
            build.Name,
            build.Desc,
            build.HeroId,
            build
                .Items.Select(i => new ItemReadDto(i.Id, i.Name, i.Price, i.Color.ToString()))
                .ToList()
        );

        return Results.Ok(dto);
    }
);

app.MapGet(
    "/heroes/{heroId}/builds",
    async (int heroId, IBuildService buildSvc, IHeroService heroSvc) =>
    {
        // if hero doesn't exist
        if (!await heroSvc.Exists(heroId))
            return Results.NotFound(new { error = $"Hero {heroId} not found." });

        var builds = await buildSvc.GetAllAsync();
        var filtered = builds.Where(b => b.HeroId == heroId);

        //can uncomment this if we dont want items in the output
        // var result = filtered.Select(b => new BuildReadDto(b.Id, b.Name, b.Desc, b.HeroId));

        var result = filtered.Select(b => new BuildReadWithItemsDto(
            b.Id,
            b.Name,
            b.Desc,
            b.HeroId,
            b.Items.Select(i => new ItemReadDto(i.Id, i.Name, i.Price, i.Color.ToString())).ToList()
        ));

        return Results.Ok(result);
    }
);

/* ********************************************************************************************************************************* */
//Item endpoints
//testing Item Apis

// GET all items
app.MapGet(
    "/items",
    async (IItemService svc) =>
    {
        var items = await svc.GetAllAsync();
        return Results.Ok(items.Select(ItemMap.ToRead));
    }
);

// GET item by id
app.MapGet(
    "/items/{id}",
    async (int id, IItemService svc) =>
    {
        var item = await svc.GetByIdAsync(id);
        return item is null ? Results.NotFound() : Results.Ok(ItemMap.ToRead(item));
    }
);

// POST create item
app.MapPost(
    "/items",
    async (ItemCreateDto dto, IItemService svc) =>
    {
        // Parse color string to enum
        if (!Enum.TryParse<Color>(dto.Color, ignoreCase: true, out var color)) //wrong color
            return Results.BadRequest(new { error = "Color must be Orange, Purple, or Green." });

        var item = new Item
        {
            Name = dto.Name,
            Price = dto.Price,
            Color = color,
        };

        var created = await svc.CreateAsync(item);
        var read = ItemMap.ToRead(created);
        return Results.Created($"/items/{created.Id}", read);
    }
);

// PUT update item
app.MapPut(
    "/items/{id}",
    async (int id, ItemUpdateDto dto, IItemService svc) =>
    {
        var existing = await svc.GetByIdAsync(id);
        if (existing is null)
            return Results.NotFound();

        if (dto.Name is not null)
            existing.Name = dto.Name;
        if (dto.Price is not null)
            existing.Price = dto.Price.Value;

        if (dto.Color is not null)
        {
            if (!Enum.TryParse<Color>(dto.Color, ignoreCase: true, out var color))
                return Results.BadRequest(
                    new { error = "Color must be Orange, Purple, or Green." }
                );
            existing.Color = color;
        }

        var ok = await svc.UpdateAsync(id, existing);
        return ok ? Results.NoContent() : Results.NotFound();
    }
);

// DELETE item
app.MapDelete(
    "/items/{id}",
    async (int id, IItemService svc) =>
    {
        var ok = await svc.DeleteAsync(id);
        return ok ? Results.NoContent() : Results.NotFound();
    }
);

/* ******************************************************************************** */
app.UseHttpsRedirection();

app.Run();

public partial class Program { }
