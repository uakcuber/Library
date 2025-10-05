using System;
using System.Collections.Generic;
using System.IO;

namespace Library;

public class Users
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Pass { get; set; }
    public string Role { get; set; }
    
    private static List<Users> usersList = new List<Users>();

    public Users(int id, string name, string pass, string role)
    {
        Id = id;
        Name = name;
        Pass = pass;
        Role = role;
    }

    public static void AddUser()
    {
        int id = usersList.Count + 1;
        Console.WriteLine("Enter User Name-->");
        string? name = Console.ReadLine();
        Console.WriteLine("Enter User Password-->");
        string? pass = Console.ReadLine();
        Console.WriteLine("Enter User Role-->");
        string? role = Console.ReadLine();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(pass) || string.IsNullOrEmpty(role))
        {
            Console.WriteLine("Name, password and role cannot be empty!");
            return;
        }

        // Şifreyi hash'le
        string hashedPassword = PasswordService.HashPassword(pass);
        Users user = new Users(id, name, hashedPassword, role);
        usersList.Add(user);

        using (StreamWriter sw = new StreamWriter("users.txt", true))
        {
            sw.WriteLine($"{user.Id}, {user.Name}, {user.Pass}, {user.Role}");
            sw.Flush();
        }
        Console.WriteLine("User added successfully!");
    }

    public static void LoadUsers()
    {
        if (File.Exists("users.txt"))
        {
            string[] lines = File.ReadAllLines("users.txt");
            foreach (string line in lines)
            {
                string[] parts = line.Split(',');
                if (parts.Length == 4)
                {
                    int id = int.Parse(parts[0].Trim());
                    string name = parts[1].Trim();
                    string pass = parts[2].Trim();
                    string role = parts[3].Trim();
                    Users user = new Users(id, name, pass, role);
                    usersList.Add(user);
                }
            }
        }
    }

    public static Users? AuthenticateUser(string name, string pass)
    {
        foreach (var user in usersList)
        {
            if (user.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && PasswordService.VerifyPassword(pass, user.Pass))
            {
                return user;
            }
        }
        return null;
    }

    public static void RegisterUser(string name, string pass)
    {
        int id = usersList.Count + 1;
        string role = "user";

        // Şifreyi hash'le
        string hashedPassword = PasswordService.HashPassword(pass);
        Users user = new Users(id, name, hashedPassword, role);
        usersList.Add(user);

        using (StreamWriter sw = new StreamWriter("users.txt", true))
        {
            sw.WriteLine($"{user.Id}, {user.Name}, {user.Pass}, {user.Role}");
            sw.Flush();
        }
        Console.WriteLine("Registration successful!");
    }
}


