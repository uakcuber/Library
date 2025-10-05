namespace Library;

public class UserPage
{
    public static void User()
    {
        Books.Alert();
        Console.WriteLine("-----LIBRARY MANAGEMENT SYSTEM-----");
        Console.WriteLine("1. Display Books");
        Console.WriteLine("2. Borrow Book");
        Console.WriteLine("3. Return Book");
        Console.WriteLine("4. Exit");
        Console.WriteLine("-----------------------------------");
        Console.Write("Choose an option: ");
        string? choice = Console.ReadLine();
        switch (choice)
        {
            case "1":
                Books.DisplayBooks();
                break;
            case "2":
                Books.BorrowBook();
                break;
            case "3":
                Books.ReturnBook();
                break;
            case "4":
                Console.WriteLine("Exiting...");
                break;
            default:
                Console.WriteLine("Invalid choice. Please try again.");
                break;
        }

    }
}
