using Deadlock.Api.Models;
using Deadlock.Api.Repository;

namespace Deadlock.Api.Service;

public class BuildService : IBuildService
{
    private readonly IBuildRepository _repo;

    public BuildService(IBuildRepository repo)
    {
        _repo = repo;
    }

    public async Task<List<Build>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Build?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task CreateAsync(Build build)
    {
        await _repo.AddAsync(build);
        await _repo.SaveChangesAsync();
    }
}
