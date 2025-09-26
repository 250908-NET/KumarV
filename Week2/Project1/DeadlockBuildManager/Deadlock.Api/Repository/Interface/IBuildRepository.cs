using Deadlock.Api.Models;

namespace Deadlock.Api.Repository;

public interface IBuildRepository
{
    public Task<List<Build>> GetAllAsync();
    public Task<Build?> GetByIdAsync(int id);
    public Task<Build> AddAsync(Build Build);
    public Task<bool> UpdateAsync(int id, Build Build);
    public Task<bool> DeleteAsync(int id);
    public Task<bool> Exists(int id);
}
