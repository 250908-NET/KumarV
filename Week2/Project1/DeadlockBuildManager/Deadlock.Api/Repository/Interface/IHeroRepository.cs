using Deadlock.Api.Models;

namespace Deadlock.Api.Repository;

public interface IHeroRepository
{
    public Task<List<Hero>> GetAllAsync();
    public Task<Hero?> GetByIdAsync(int id);
    public Task<Hero> AddAsync(Hero Hero);
    public Task<bool> UpdateAsync(int id, Hero hero);
    public Task<bool> DeleteAsync(int id);
    public Task<bool> Exists(int id);
}
