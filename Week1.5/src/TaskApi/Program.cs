using Serilog;
using TaskApi.Contracts;
using TaskApi.Dtos;
using TaskApi.Endpoints;
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

app.MapEndpoints(); //calls our endpoints on our app to access singleton

if (app.Environment.IsDevelopment()) //only runs if not in prod and enables swagger
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.Run();
