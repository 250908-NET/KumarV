using Deadlock.Api.Data;
using Deadlock.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Deadlock.Api.Repository;

public class ItemRepository : IItemRepository
{
    private readonly DeadlockDbContext _context;

    public ItemRepository(DeadlockDbContext context)
    {
        _context = context;
    }

    public async Task<List<Item>> GetAllAsync()
    {
        return await _context.Items.Include(h => h.Builds).ToListAsync();
    }

    public async Task<Item?> GetByIdAsync(int id) =>
        await _context.Items.Include(h => h.Builds).FirstOrDefaultAsync(h => h.Id == id); //h for item

    public async Task<Item> AddAsync(Item Item)
    {
        _context.Items.Add(Item);
        await _context.SaveChangesAsync();
        return Item;
    }

    public async Task<bool> UpdateAsync(int id, Item Item)
    {
        if (!await Exists(id))
        {
            return false; //Item id dont exist
        }
        if (id != Item.Id)
        {
            return false; //Item id doesnt match given Item's Item id
        }

        _context.Items.Update(Item);
        await _context.SaveChangesAsync();
        return true; //changed this to a bool task return
        //return Item;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var Item = await _context.Items.FindAsync(id);
        if (Item == null)
        {
            return false; //Item cant be found based on id
        }
        _context.Items.Remove(Item);
        await _context.SaveChangesAsync();
        return true;
    }

    public async Task<bool> Exists(int id) =>
        (await _context.Items.FindAsync(id) != null ? true : false);
}
