using Deadlock.Api.Models;

namespace Deadlock.Api.Service;

public interface IBuildService
{
    public Task<List<Build>> GetAllAsync();
    public Task<Build?> GetByIdAsync(int id);
    public Task CreateAsync(Build build);
}
