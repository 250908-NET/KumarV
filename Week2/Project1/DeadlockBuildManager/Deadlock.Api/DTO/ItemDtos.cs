using System.ComponentModel.DataAnnotations;
using Deadlock.Api.Models;

namespace Deadlock.Api.DTO;

public record ItemCreateDto(
    [Required, MaxLength(50)] string Name,
    [Range(0, int.MaxValue)] int Price,
    [Required] string Color // "Orange" "Purple" "Green"
);

public record ItemUpdateDto(
    [MaxLength(50)] string? Name,
    int? Price,
    string? Color // if provided, must be one of the enum names
);

public record ItemReadDto(int Id, string Name, int Price, string Color);

//using Deadlock.Api.DTO;

public static class ItemMap
{
    public static ItemReadDto ToRead(Item e) => new(e.Id, e.Name, e.Price, e.Color.ToString());
}
