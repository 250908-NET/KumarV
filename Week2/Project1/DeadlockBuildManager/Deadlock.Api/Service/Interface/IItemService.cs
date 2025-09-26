using Deadlock.Api.Models;

namespace Deadlock.Api.Service;

public interface IItemService
{
    public Task<List<Item>> GetAllAsync();
    public Task<Item?> GetByIdAsync(int id);
    public Task<Item> CreateAsync(Item Item);

    public Task<bool> UpdateAsync(int id, Item Item);
    public Task<bool> DeleteAsync(int id);
    public Task<bool> Exists(int id);
}
