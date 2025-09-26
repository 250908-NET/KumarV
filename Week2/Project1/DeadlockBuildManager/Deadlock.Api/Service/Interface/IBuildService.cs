using Deadlock.Api.Models;

namespace Deadlock.Api.Service;

public interface IBuildService
{
    public Task<List<Build>> GetAllAsync();
    public Task<Build?> GetByIdAsync(int id);
    public Task<Build> CreateAsync(Build Build);

    public Task<bool> UpdateAsync(int id, Build Build);
    public Task<bool> DeleteAsync(int id);
    public Task<bool> Exists(int id);
}
