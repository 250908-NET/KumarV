namespace ToDoChallenge.Models;

public abstract class ToDoItemBase
{
    //fields
    public int Id { get; }
    public string ToDo { get; set; }
    public bool Completed { get; private set; }
    public DateTime CreatedAt { get; }

    //constructor
    protected ToDoItemBase(int id, string toDo)
    {
        Id = id;
        ToDo = toDo;
        Completed = false;
        CreatedAt = DateTime.Now;
    }

    //methods
    public void MarkComplete() => Completed = true;

    public void MarkIncomplete() => Completed = false;

    public abstract string GetDisplayText();

    public override string ToString() => GetDisplayText();
}
