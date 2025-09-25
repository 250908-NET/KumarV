using Deadlock.Api.Models;
using Deadlock.Api.Repository;

namespace Deadlock.Api.Service;

public class HeroService : IHeroService
{
    private readonly IHeroRepository _repo;

    public HeroService(IHeroRepository repo) //constructor for our service
    {
        if (repo == null)
            throw new ArgumentNullException(nameof(repo)); //uhoh
        _repo = repo;
    }

    public async Task<List<Hero>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Hero?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Hero> CreateAsync(Hero hero) => await _repo.AddAsync(hero); //might have to initialize a hero for this but lets try anonymous func

    public async Task<bool> UpdateAsync(int id, Hero hero) => await _repo.UpdateAsync(id, hero);

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    public async Task<bool> Exists(int id) => await _repo.Exists(id);
}
