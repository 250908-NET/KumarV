using Deadlock.Api.Models;

namespace Deadlock.Api.Service;

public interface IHeroService
{
    public Task<List<Hero>> GetAllAsync();
    public Task<Hero?> GetByIdAsync(int id);
    public Task<Hero> CreateAsync(Hero hero);

    public Task<bool> UpdateAsync(int id, Hero hero);
    public Task<bool> DeleteAsync(int id);
    public Task<bool> Exists(int id);
}
