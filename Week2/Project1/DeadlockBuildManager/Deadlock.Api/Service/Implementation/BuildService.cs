using Deadlock.Api.Models;
using Deadlock.Api.Repository;

namespace Deadlock.Api.Service;

public class BuildService : IBuildService
{
    private readonly IBuildRepository _repo;

    public BuildService(IBuildRepository repo) //constructor for our service
    {
        if (repo == null)
            throw new ArgumentNullException(nameof(repo)); //uhoh
        _repo = repo;
    }

    public async Task<List<Build>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Build?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Build> CreateAsync(Build Build) => await _repo.AddAsync(Build); //might have to initialize a Build for this but lets try anonymous func

    public async Task<bool> UpdateAsync(int id, Build Build) => await _repo.UpdateAsync(id, Build);

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    public async Task<bool> Exists(int id) => await _repo.Exists(id);
}
