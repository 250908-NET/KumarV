using Deadlock.Api.Models;

namespace Deadlock.Api.Repository;

public interface IItemRepository
{
    public Task<List<Item>> GetAllAsync();
    public Task<Item?> GetByIdAsync(int id);
    public Task<Item> AddAsync(Item Item);
    public Task<bool> UpdateAsync(int id, Item Item);
    public Task<bool> DeleteAsync(int id);
    public Task<bool> Exists(int id);
}
