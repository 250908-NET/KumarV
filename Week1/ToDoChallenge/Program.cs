// See https://aka.ms/new-console-template for more information
using ToDoChallenge.Models;
using ToDoChallenge.Services;

void PrintAll(IToDoService svc)
{
    var items = svc.GetAll();
    if (items.Count == 0)
    {
        Console.WriteLine("(no items)");
        return;
    }
    foreach (var it in items)
        Console.WriteLine(it);
}

var service = new ToDoService();

while (true)
{
    Console.WriteLine(
        "=== TO-DO LIST MANAGER ===\n"
            + "1. Add new item\n"
            + "2. View all items\n"
            + "3. Mark item complete\n"
            + "4. Mark item incomplete\n"
            + "5. Delete item\n"
            + "6. Exit\n"
            + "Choose an option (1-6):"
    );
    var line = Console.ReadLine();

    switch (line)
    {
        case "1":
            Console.Write("What do you want to add to your To Do List?\n");
            var title = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                Console.WriteLine("Title cannot be empty.\n");
                Console.WriteLine("Let's try this again.\n");
                continue;
            }
            service.AddItem(title.Trim());
            Console.WriteLine("Task added!");
            break; //might have to change this to continue

        case "2":
            Console.WriteLine("=== YOUR TO-DO ITEMS ===\n");
            var items = service.GetAll();

            if (items.Count == 0)
            {
                Console.WriteLine("(no items yet)");
            }
            else
            {
                foreach (var it in items)
                    Console.WriteLine(it);
            }
            continue;

        case "3":
            Console.Write("What is the ID of the item you want to complete?\n");
            var IdComplete = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                Console.WriteLine("ID cannot be empty.\n");
                Console.WriteLine("Let's try this again.\n");
                continue;
            }
            else if (!int.TryParse(IdComplete, out int badId))
            {
                Console.WriteLine("Make sure to only enter numbers!\n");
                Console.WriteLine("Let's try this again.\n");
                continue;
            }

            int.TryParse(IdComplete, out int idC);
            if (service.GetById(idC) is null)
            {
                Console.WriteLine("ID not found.\n");
                continue;
            }
            service.CompleteItem(idC);
            Console.WriteLine($"Item {idC} has been completed\n");
            continue;

        case "4":
            Console.Write("What is the ID of the item you want incompleted?\n");
            var IdIncomplete = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(line))
            {
                Console.WriteLine("ID cannot be empty.\n");
                Console.WriteLine("Let's try this again.\n");
                continue;
            }
            else if (!int.TryParse(IdIncomplete, out int badIdInc))
            {
                Console.WriteLine("Make sure to only enter numbers!\n");
                Console.WriteLine("Let's try this again.\n");
                continue;
            }

            int.TryParse(IdIncomplete, out int idI);
            if (service.GetById(idI) is null)
            {
                Console.WriteLine("ID not found.\n");
                continue;
            }
            service.IncompleteItem(idI);
            Console.WriteLine($"Item {idI} has been Incompleted\n");
            continue;

        case "5":
            Console.Write("What is the ID of the item you want to remove?\n");
            var IdToRemove = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(IdToRemove))
            {
                Console.WriteLine("ID cannot be empty.\n");
                Console.WriteLine("Let's try this again.\n");
                continue;
            }
            else if (!int.TryParse(IdToRemove, out int badIdRemoval))
            {
                Console.WriteLine("Make sure to only enter numbers!\n");
                Console.WriteLine("Let's try this again.\n");
                continue;
            }
            int.TryParse(IdToRemove, out int idR);
            service.DeleteItem(idR);
            Console.WriteLine($"Item {idR} has been removed\n");
            continue;

        case "6":
            Console.Write("Have a nice day!");
            return;
    }

    if (string.IsNullOrWhiteSpace(line))
    {
        Console.WriteLine("Title cannot be empty.");
        continue;
    }
}
