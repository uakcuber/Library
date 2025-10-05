using System;

namespace Library
{
    public class MainClass
    {
        public static void Main(string[] args)
        {
            Users.LoadUsers();
            Books.LoadBooks();
            Console.WriteLine("Do you have an account? (yes/no) or y/n");
            Console.Write("-->");
            string? response = Console.ReadLine()?.Trim().ToLower();

            switch (response)
            {
                case "yes":
                case "y":
                    AuthService.Login();
                    break;
                case "no":
                case "n":
                    AuthService.Register();
                    break;
                default:
                    Console.WriteLine("Invalid option!");
                    break;
            }
        }
    }
}