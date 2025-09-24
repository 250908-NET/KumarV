using Deadlock.Api.Models;

namespace Deadlock.Api.Service;

public interface IHeroService
{
    public Task<List<Hero>> GetAllAsync();
    public Task<Hero?> GetByIdAsync(int id);
    public Task CreateAsync(Hero hero);
}
