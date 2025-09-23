using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Deadlock.Api.Models;

public class Hero
{
    public int Id { get; set; }

    [Required, MaxLength(50)]
    public string Name { get; set; }

    public List<Build> Builds { get; set; } = new();
}
