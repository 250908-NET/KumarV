using Deadlock.Api.Models;

namespace Deadlock.Api.Service;

public interface IItemService
{
    public Task<List<Item>> GetAllAsync();
    public Task<Item?> GetByIdAsync(int id);
    public Task CreateAsync(Item item);
}
