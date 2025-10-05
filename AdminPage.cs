using System.IO.Compression;

namespace Library;

public class AdminPage
{
    public static void Admin()
    {
        Console.WriteLine("-----LIBRARY MANAGEMENT SYSTEM-----");
        Console.WriteLine("1. Add User");
        Console.WriteLine("2. Add Book");
        Console.WriteLine("3. Display Books");
        Console.WriteLine("4. Update Book");
        Console.WriteLine("5. Exit");
        Console.WriteLine("-----------------------------------");
        Console.Write("Choose an option: ");
        string? choice = Console.ReadLine();

        switch (choice)
        {
            case "1":
                Users.AddUser();
                break;
            case "2":
                Books.AddBook();
                break;
            case "3":
                Books.DisplayBooks();
                break;
            case "4":
                Books.UpdateBook();
                break;
            case "5":
                Console.WriteLine("Exiting...");
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;

            }
    }
}
