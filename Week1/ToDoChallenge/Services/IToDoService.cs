using ToDoChallenge.Models;

namespace ToDoChallenge.Services;

public interface IToDoService
{
    ToDoItemBase AddItem(string title);

    IReadOnlyList<ToDoItemBase> GetAll();
    ToDoItemBase? GetById(int id);

    bool DeleteItem(int id);

    bool CompleteItem(int id);
    bool IncompleteItem(int id);
}
