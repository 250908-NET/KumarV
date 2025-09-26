using System.ComponentModel.DataAnnotations;

namespace Deadlock.Api.DTO;

public record BuildCreateDto(
    [Required, MaxLength(50)] string Name,
    string? Desc,
    [Required] int HeroId,
    List<int>? ItemIds
);

public record BuildReadDto(int Id, string Name, string? Desc, int HeroId);

public record BuildReadWithItemsDto(
    int Id,
    string Name,
    string? Desc,
    int HeroId,
    List<ItemReadDto> Items
);

//public record ItemSummaryDto(int Id, string Name, int Price, string Color);
