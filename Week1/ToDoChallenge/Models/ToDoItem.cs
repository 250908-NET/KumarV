namespace ToDoChallenge.Models;

public class ToDoItem : ToDoItemBase
{
    //fields

    //constructor
    public ToDoItem(int id, string toDo)
        : base(id, toDo) { }

    //functions
    public override string GetDisplayText() =>
        $"[{Id}] {ToDo} Created: {CreatedAt} Complete: {Completed}";
}
