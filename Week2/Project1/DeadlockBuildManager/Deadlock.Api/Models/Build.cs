using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Deadlock.Api.Models;

public class Build
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }

    public string? Desc { get; set; }

    public List<Item> Items { get; set; } = new();

    public Hero Hero { get; set; }
}
