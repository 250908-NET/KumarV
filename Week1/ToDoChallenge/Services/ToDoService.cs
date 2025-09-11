using ToDoChallenge.Models;

namespace ToDoChallenge.Services;

public class ToDoService : IToDoService
{
    private readonly List<ToDoItemBase> _items = new();
    private int _nextId = 1;

    // 3) Implement the contract
    public ToDoItemBase AddItem(string title)
    {
        // validate title
        // create new ToDoItem
        var item = new ToDoItem(_nextId++, title);
        _items.Add(item);

        // return item
        return item;
    }

    public IReadOnlyList<ToDoItemBase> GetAll()
    {
        /*foreach (var item in _items)
        {
            Console.WriteLine($"{item}");
        }*/

        return _items.AsReadOnly();
    }

    public ToDoItemBase? GetById(int id)
    {
        // find item by id
        foreach (var item in _items)
        {
            if (item.Id == id)
            {
                return item;
            }
        }
        return null; //make sure to output at console that we didnt find the item.
    }

    public bool CompleteItem(int id)
    {
        // call item.MarkComplete()
        var item = GetById(id);
        if (item is null)
        {
            return false;
        }
        else
        {
            item.MarkComplete();
            return true;
        }
    }

    public bool IncompleteItem(int id)
    {
        // call item.MarkComplete()
        var item = GetById(id);
        if (item is null)
        {
            return false;
        }
        else
        {
            item.MarkIncomplete();
            return true;
        }
    }

    public bool DeleteItem(int id)
    {
        // remove by id
        var item = GetById(id);
        if (item is null)
        {
            return false;
        }
        else
        {
            _items.Remove(item);
            return true;
        }
        //return true;
    }
}
