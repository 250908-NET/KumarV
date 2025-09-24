using Deadlock.Api.Data;
using Deadlock.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deadlock.Api.Repository;

public class BuildRepository : IBuildRepository
{
    private readonly DeadlockDbContext _context;

    private BuildRepository(DeadlockDbContext context)
    {
        _context = context;
    }

    public Task<List<Build>> GetAllAsync()
    {
        throw new NotImplementedException();
    }

    public Task<Build?> GetByIdAsync(int id)
    {
        throw new NotImplementedException();
    }

    public Task AddAsync(Build build)
    {
        throw new NotImplementedException();
    }

    public Task SaveChangesAsync()
    {
        throw new NotImplementedException();
    }
}
