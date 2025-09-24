using Deadlock.Api.Models;

namespace Deadlock.Api.Repository;

public interface IBuildRepository
{
    public Task<List<Build>> GetAllAsync();
    public Task<Build?> GetByIdAsync(int id);
    public Task AddAsync(Build build);
    public Task SaveChangesAsync();
}
