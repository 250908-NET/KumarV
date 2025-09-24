using Deadlock.Api.Data;
using Deadlock.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deadlock.Api.Data;

public class DeadlockDbContext : DbContext
{
    //fields of data
    public DbSet<Hero> Heroes { get; set; }
    public DbSet<Build> Builds { get; set; }
    public DbSet<Item> Items { get; set; }

    //methods
    public DeadlockDbContext(DbContextOptions<DeadlockDbContext> options)
        : base(options) { }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder
            .Entity<Item>() //change the enum to store as string color
            .Property(i => i.Color)
            .HasConversion<string>();
    }
}
