using System;
using System.Security.Cryptography.X509Certificates;

namespace Library
{
    public class AuthService
    {
        public static void Login()
        {
            Console.Write("Enter Your Name-->");
            string? name = Console.ReadLine();
            Console.Write("Enter Your Password-->");
            string? pass = Console.ReadLine();

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                Console.WriteLine("Name and password cannot be empty!");
                return;
            }

            Users? user = Users.AuthenticateUser(name, pass);
            if (user != null)
            {
                Console.WriteLine($"Welcome {user.Name}!");
                SessionService.SetCurrentUser(user); // Current user'ı set et
                
                if (user.Role.Equals("admin", StringComparison.OrdinalIgnoreCase))
                {
                    AdminPage.Admin();
                }
                else if (user.Role.Equals("user", StringComparison.OrdinalIgnoreCase))
                {
                    UserPage.User();
                }
                
                SessionService.Logout(); // Çıkış yaptığında session'ı temizle
            }
            else
            {
                Console.WriteLine("Invalid username or password!");
            }
        }

        public static void Register()
        {
            Console.Write("Enter Name-->");
            string? name = Console.ReadLine();
            Console.Write("Enter Password-->");
            string? pass = Console.ReadLine();
            
            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass))
            {
                Console.WriteLine("Name and password cannot be empty!");
                return;
            }

            Users.RegisterUser(name, pass);
        }
    }
}