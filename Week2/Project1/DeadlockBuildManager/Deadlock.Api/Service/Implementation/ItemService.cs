using Deadlock.Api.Models;
using Deadlock.Api.Repository;

namespace Deadlock.Api.Service;

public class ItemService : IItemService
{
    private readonly IItemRepository _repo;

    public ItemService(IItemRepository repo) //constructor for our service
    {
        if (repo == null)
            throw new ArgumentNullException(nameof(repo)); //uhoh
        _repo = repo;
    }

    public async Task<List<Item>> GetAllAsync() => await _repo.GetAllAsync();

    public async Task<Item?> GetByIdAsync(int id) => await _repo.GetByIdAsync(id);

    public async Task<Item> CreateAsync(Item Item) => await _repo.AddAsync(Item); //might have to initialize a Item for this but lets try anonymous func

    public async Task<bool> UpdateAsync(int id, Item Item) => await _repo.UpdateAsync(id, Item);

    public async Task<bool> DeleteAsync(int id) => await _repo.DeleteAsync(id);

    public async Task<bool> Exists(int id) => await _repo.Exists(id);
}
