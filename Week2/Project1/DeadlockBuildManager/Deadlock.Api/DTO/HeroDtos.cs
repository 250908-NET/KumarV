using System.ComponentModel.DataAnnotations;

namespace Deadlock.Api.DTO;

public record HeroCreateDto([Required, MaxLength(50)] string Name);
