using Deadlock.Api.Data;
using Deadlock.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deadlock.Api.Repository;

public class BuildRepository : IBuildRepository
{
    private readonly DeadlockDbContext _context;

    public BuildRepository(DeadlockDbContext context)
    {
        _context = context;
    }

    public async Task<List<Build>> GetAllAsync()
    {
        return await _context.Builds.Include(b => b.Hero).Include(b => b.Items).ToListAsync();
    }

    public Task<Build?> GetByIdAsync(int id) =>
        _context.Builds.Include(b => b.Items).FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Build> AddAsync(Build build)
    {
        _context.Builds.Add(build);
        await _context.SaveChangesAsync();
        return build;
    }

    public async Task<bool> UpdateAsync(int id, Build build)
    {
        if (!await Exists(id))
        {
            return false; //Build id dont exist
        }
        if (id != build.Id)
        {
            return false; //Build id doesnt match given Build's Build id
        }

        _context.Builds.Update(build);
        await _context.SaveChangesAsync();
        return true; //changed this to a bool task return
        //return Build;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var Build = await _context.Builds.FindAsync(id);
        if (Build == null)
        {
            return false; //Build cant be found based on id
        }
        _context.Builds.Remove(Build);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Exists(int id) =>
        await _context.Builds.FindAsync(id) != null ? true : false;
}
