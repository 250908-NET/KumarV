using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Deadlock.Api.Models;

public enum Color
{
    Orange = 0,
    Purple = 1,
    Green = 2,
}

public class Item
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }

    [Required]
    public int Price { get; set; }

    [Required]
    public Color Color { get; set; }

    public List<Build> Builds { get; set; } = new();
}
