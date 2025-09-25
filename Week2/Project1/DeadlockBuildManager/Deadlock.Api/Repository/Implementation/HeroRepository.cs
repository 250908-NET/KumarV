using Deadlock.Api.Data;
using Deadlock.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deadlock.Api.Repository;

public class HeroRepository : IHeroRepository
{
    private readonly DeadlockDbContext _context;

    public HeroRepository(DeadlockDbContext context)
    {
        _context = context;
    }

    public async Task<List<Hero>> GetAllAsync()
    {
        return await _context.Heroes.Include(h => h.Builds).ToListAsync();
    }

    public async Task<Hero?> GetByIdAsync(int id) =>
        await _context.Heroes.Include(h => h.Builds).FirstOrDefaultAsync(h => h.Id == id);

    public async Task<Hero> AddAsync(Hero hero)
    {
        _context.Heroes.Add(hero);
        await _context.SaveChangesAsync();
        return hero;
    }

    public async Task<bool> UpdateAsync(int id, Hero hero)
    {
        if (!await Exists(id))
        {
            return false; //hero id dont exist
        }
        if (id != hero.Id)
        {
            return false; //hero id doesnt match given hero's hero id
        }

        _context.Heroes.Update(hero);
        await _context.SaveChangesAsync();
        return true; //changed this to a bool task return
        //return hero;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var hero = await _context.Heroes.FindAsync(id);
        if (hero == null)
        {
            return false; //hero cant be found based on id
        }
        _context.Heroes.Remove(hero);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Exists(int id) =>
        (await _context.Heroes.FindAsync(id) != null ? true : false);
}
