using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using TaskApi.Contracts;
using TaskApi.Dtos;
using TaskApi.Endpoints;
using TaskApi.Mappers;
using TaskApi.Models;
using TaskApi.Repositories;
using TaskApi.Services;

static string GetUserId(HttpContext ctx) =>
    ctx.User.FindFirstValue(ClaimTypes.NameIdentifier)
    ?? throw new Exception("Missing user id claim");

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration().Enrich.FromLogContext().WriteTo.Console().CreateLogger(); //makes logger pretty

builder.Host.UseSerilog();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "TaskApi", Version = "v1" });

    // Define the Bearer security scheme
    c.AddSecurityDefinition(
        "Bearer",
        new OpenApiSecurityScheme
        {
            Name = "Authorization",
            Description = "Enter: {your token}",
            In = ParameterLocation.Header,
            Type = SecuritySchemeType.Http,
            Scheme = "bearer",
            BearerFormat = "JWT",
        }
    );

    // Require the Bearer scheme for all operations
    c.AddSecurityRequirement(
        new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer",
                    },
                },
                Array.Empty<string>()
            },
        }
    );
});

// TEMPORARY trying out using singleton in memory store for taskitem list, seems pretty cool
builder.Services.AddSingleton<List<TaskItem>>();

// JWT bearer validation
builder
    .Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        var cfg = builder.Configuration;
        o.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidIssuer = cfg["Jwt:Issuer"],
            ValidateAudience = true,
            ValidAudience = cfg["Jwt:Audience"],
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cfg["Jwt:Key"]!)),
            ValidateLifetime = true,
            ClockSkew = TimeSpan.FromMinutes(2),
        };
    });

builder.Services.AddAuthorization();

// DI for auth-related services
builder.Services.AddSingleton<IUserRepository, InMemUserRepository>();
builder.Services.AddSingleton<ITokenService, JwtTokenService>();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints(); //calls our endpoints on our app to access singleton

if (app.Environment.IsDevelopment()) //only runs if not in prod and enables swagger
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapGet("/", () => "Hello World!");

app.Run();

public partial class Program { }
