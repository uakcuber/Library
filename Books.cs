using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Library;

public class Books
{
    public int Id { get; set; }
    public string Name { get; set; }
    public string Author { get; set; }
    
    private static List<Books> booksList = new List<Books>();

    public Books(int id, string name, string author)
    {
        Id = id;
        Name = name;
        Author = author;
    }

    public static void Alert()
    {
        if (!SessionService.IsLoggedIn())
            return;

        var overdueBooks = GetOverdueBooks();
        
        if (overdueBooks.Count == 0)
            return;

        Console.WriteLine("\n🚨 OVERDUE BOOKS ALERT 🚨");
        Console.WriteLine("═══════════════════════════");
        
        foreach (var overdue in overdueBooks)
        {
            string urgencyLevel = GetUrgencyLevel(overdue.DaysOverdue);
            string urgencyIcon = GetUrgencyIcon(overdue.DaysOverdue);
            
            Console.ForegroundColor = GetUrgencyColor(overdue.DaysOverdue);
            Console.WriteLine($"{urgencyIcon} Book: {overdue.BookName}");
            Console.WriteLine($"   Borrowed: {overdue.BorrowDate:yyyy-MM-dd}");
            Console.WriteLine($"   Days Overdue: {overdue.DaysOverdue} days");
            Console.WriteLine($"   Urgency: {urgencyLevel}");
            Console.WriteLine();
            Console.ResetColor();
        }
        
        Console.WriteLine("Please return these books as soon as possible!");
        Console.WriteLine("═══════════════════════════════════════════════");
        Console.WriteLine();
    }

    private static List<OverdueBook> GetOverdueBooks()
    {
        List<OverdueBook> overdueBooks = new List<OverdueBook>();
        
        if (!File.Exists("borrow.txt"))
            return overdueBooks;

        int currentUserId = SessionService.GetCurrentUserId();
        string[] lines = File.ReadAllLines("borrow.txt");
        Dictionary<int, DateTime> borrowedBooks = new Dictionary<int, DateTime>();
        Dictionary<int, string> bookNames = new Dictionary<int, string>();

        // Kullanıcının ödünç aldığı kitapları bul
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    if (int.TryParse(parts[0].Trim(), out int userId) && 
                        int.TryParse(parts[1].Trim(), out int bookId) &&
                        DateTime.TryParse(parts[2].Trim(), out DateTime date) &&
                        userId == currentUserId)
                    {
                        string action = parts[3].Trim().ToUpper();
                        string bookName = parts[5].Trim();
                        
                        if (action == "BORROWED" || action == "BORROW")
                        {
                            borrowedBooks[bookId] = date;
                            bookNames[bookId] = bookName;
                        }
                        else if (action == "RETURNED")
                        {
                            borrowedBooks.Remove(bookId);
                            bookNames.Remove(bookId);
                        }
                    }
                }
            }
        }

        // Gecikmiş kitapları kontrol et (15 gün limit)
        DateTime today = DateTime.Now;
        foreach (var borrowed in borrowedBooks)
        {
            int bookId = borrowed.Key;
            DateTime borrowDate = borrowed.Value;
            int daysElapsed = (today - borrowDate).Days;
            
            if (daysElapsed > 15) // 15 günden fazla ise gecikmiş
            {
                int daysOverdue = daysElapsed - 15;
                string bookName = bookNames.ContainsKey(bookId) ? bookNames[bookId] : "Unknown Book";
                
                overdueBooks.Add(new OverdueBook
                {
                    BookId = bookId,
                    BookName = bookName,
                    BorrowDate = borrowDate,
                    DaysOverdue = daysOverdue
                });
            }
        }

        return overdueBooks.OrderByDescending(x => x.DaysOverdue).ToList();
    }

    private static string GetUrgencyLevel(int daysOverdue)
    {
        if (daysOverdue <= 7) return "NORMAL";
        if (daysOverdue <= 14) return "WARNING";
        if (daysOverdue <= 30) return "URGENT";
        return "CRITICAL";
    }

    private static string GetUrgencyIcon(int daysOverdue)
    {
        if (daysOverdue <= 7) return "⚠️";
        if (daysOverdue <= 14) return "🔶";
        if (daysOverdue <= 30) return "🔴";
        return "💀";
    }

    private static ConsoleColor GetUrgencyColor(int daysOverdue)
    {
        if (daysOverdue <= 7) return ConsoleColor.Yellow;
        if (daysOverdue <= 14) return ConsoleColor.DarkYellow;
        if (daysOverdue <= 30) return ConsoleColor.Red;
        return ConsoleColor.DarkRed;
    }

    public class OverdueBook
    {
        public int BookId { get; set; }
        public string BookName { get; set; } = "";
        public DateTime BorrowDate { get; set; }
        public int DaysOverdue { get; set; }
    }

    public static void AddBook()
    {
        Console.WriteLine("Enter Book Id-->");
        int id = Convert.ToInt32(Console.ReadLine());
        Console.WriteLine("Enter Book Name-->");
        string? name = Console.ReadLine();
        Console.WriteLine("Enter Book Author-->");
        string? author = Console.ReadLine();

        if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(author))
        {
            Console.WriteLine("Name and author cannot be empty!");
            return;
        }

        Books book = new Books(id, name, author);
        booksList.Add(book);

        // Dosyayı güncelle
        SaveBooksToFile();
        Console.WriteLine("Book added successfully!");
    }

    public static void LoadBooks()
    {
        booksList.Clear(); // Önceki verileri temizle (duplicate önlemi)
        
        if (File.Exists("books.txt"))
        {
            string[] lines = File.ReadAllLines("books.txt");
            foreach (string line in lines)
            {
                if (!string.IsNullOrWhiteSpace(line)) // Boş satırları atla
                {
                    string[] parts = line.Split(",");
                    if (parts.Length == 3)
                    {
                        int id = int.Parse(parts[0].Trim());
                        string name = parts[1].Trim();
                        string author = parts[2].Trim();
                        
                        // Aynı ID'li kitap zaten var mı kontrol et
                        bool exists = booksList.Any(b => b.Id == id);
                        if (!exists)
                        {
                            Books book = new Books(id, name, author);
                            booksList.Add(book);
                        }
                    }
                }
            }
        }
    }

    public static void DisplayBooks()
    {
        Console.WriteLine("------------BOOK LIST-------------");
        if (booksList.Count == 0)
        {
            Console.WriteLine("No books found.");
        }
        else
        {
            foreach (var book in booksList)
            {
                Console.WriteLine($"ID: {book.Id} | NAME: {book.Name} | AUTHOR: {book.Author}");
            }
        }
        Console.WriteLine("-----------------------------------");
    }

    public static void UpdateBook()
    {
        if (!SessionService.IsLoggedIn())
        {
            Console.WriteLine("You must be logged in to update books!");
            return;
        }

        DisplayBooks(); // Önce mevcut kitapları göster
        Console.WriteLine("\nEnter book name to search (partial match supported):");
        string? searchTerm = Console.ReadLine();

        if (string.IsNullOrEmpty(searchTerm))
        {
            Console.WriteLine("Search term cannot be empty!");
            return;
        }

        else if (searchTerm == "delete")
        {
            Console.Write("Enter Book ID to delete: ");
            string? bookId = Console.ReadLine();
            if (!string.IsNullOrEmpty(bookId))
            {
                DeleteBook(bookId);
            }
            else
            {
                Console.WriteLine("Book ID cannot be empty!");
            }
            return;
        }

        // Fuzzy search ile kitapları bul
        List<Books> matchingBooks = FindBooks(searchTerm);
        
        if (matchingBooks.Count == 0)
        {
            Console.WriteLine($"No books found matching '{searchTerm}'");
            return;
        }
        else if (matchingBooks.Count == 1)
        {
            // Tek sonuç, direkt update et
            Books selectedBook = matchingBooks[0];
            UpdateBookRecord(selectedBook);
        }
        else
        {
            // Birden fazla sonuç, kullanıcıya seçim yaptır
            Console.WriteLine($"\nFound {matchingBooks.Count} matching books:");
            for (int i = 0; i < matchingBooks.Count; i++)
            {
                var book = matchingBooks[i];
                Console.WriteLine($"{i + 1}. ID: {book.Id} | NAME: {book.Name} | AUTHOR: {book.Author}");
            }
            
            Console.Write("Select book number to update (1-" + matchingBooks.Count + "): ");
            if (int.TryParse(Console.ReadLine(), out int choice) && choice >= 1 && choice <= matchingBooks.Count)
            {
                Books selectedBook = matchingBooks[choice - 1];
                DeleteBook(selectedBook.Id.ToString());
                UpdateBookRecord(selectedBook);
            }
            else
            {
                Console.WriteLine("Invalid selection!");
            }
        }
    }

    private static List<Books> FindBooks(string searchTerm)
    {
        List<Books> matchingBooks = new List<Books>();
        string searchLower = searchTerm.ToLower().Trim();
        
        foreach (var book in booksList)
        {
            string bookNameLower = book.Name.ToLower();
            string bookAuthorLower = book.Author.ToLower();
            
            // Farklı arama yöntemleri:
            
            // 1. Tam eşleşme (case insensitive)
            if (bookNameLower == searchLower)
            {
                matchingBooks.Insert(0, book); // En üste ekle
                continue;
            }
            
            // 2. Başlangıç eşleşmesi
            if (bookNameLower.StartsWith(searchLower))
            {
                matchingBooks.Add(book);
                continue;
            }
            
            // 3. İçerik eşleşmesi (kitap adında)
            if (bookNameLower.Contains(searchLower))
            {
                matchingBooks.Add(book);
                continue;
            }
            
            // 4. Yazar adında eşleşme
            if (bookAuthorLower.Contains(searchLower))
            {
                matchingBooks.Add(book);
                continue;
            }
            
            // 5. Kelime kelime eşleşme (boşluklarla ayrılmış)
            string[] searchWords = searchLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            string[] bookWords = bookNameLower.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            bool hasWordMatch = false;
            foreach (string searchWord in searchWords)
            {
                foreach (string bookWord in bookWords)
                {
                    if (bookWord.Contains(searchWord) || searchWord.Contains(bookWord))
                    {
                        hasWordMatch = true;
                        break;
                    }
                }
                if (hasWordMatch) break;
            }
            
            if (hasWordMatch)
            {
                matchingBooks.Add(book);
            }
        }
        
        return matchingBooks;
    }

    private static void UpdateBookRecord(Books book)
    {
        Console.WriteLine($"\n📖 Current Book Info:");
        Console.WriteLine($"ID: {book.Id}");
        Console.WriteLine($"Name: {book.Name}");
        Console.WriteLine($"Author: {book.Author}");
        
        Console.WriteLine("\n🔄 Enter new information (press Enter to keep current value):");
        
        // Yeni kitap adı
        Console.Write($"New Book Name [{book.Name}]: ");
        string? newName = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newName))
        {
            book.Name = newName.Trim();
        }
        
        // Yeni yazar adı
        Console.Write($"New Author [{book.Author}]: ");
        string? newAuthor = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(newAuthor))
        {
            book.Author = newAuthor.Trim();
        }
        
        // Değişiklikleri kaydet
        SaveBooksToFile();
        
        // Activity log (borrow.txt sadece borrow işlemleri için)
        int userId = SessionService.GetCurrentUserId();
        string userName = SessionService.GetCurrentUserName();
        Console.WriteLine($"Book updated by {userName} (ID: {userId})");
        
        Console.WriteLine($"Book updated successfully!");
        Console.WriteLine($"New Info - Name: {book.Name}, Author: {book.Author}");
    }

    private static void SaveBooksToFile()
    {
        using (StreamWriter sw = new StreamWriter("books.txt", false)) // false = overwrite
        {
            foreach (var book in booksList)
            {
                sw.WriteLine($"{book.Id}, {book.Name}, {book.Author}");
            }
            sw.Flush();
        }
    }

    public static void BorrowBook()
    {
        DisplayBooks();
        Console.WriteLine("Enter Book ID to borrow:");
        string? input = Console.ReadLine();

        int userId = SessionService.GetCurrentUserId();
        string userName = SessionService.GetCurrentUserName();

        var bookToBorrow = booksList.FirstOrDefault(b => b.Id.ToString() == input);
        if (bookToBorrow != null)
        {
            using (StreamWriter sw = new StreamWriter("borrow.txt", true))
            {
                sw.WriteLine($"{userId}, {bookToBorrow.Id}, {DateTime.Now:yyyy-MM-dd HH:mm:ss}, BORROW, {userName}");
                sw.Flush();
            }
        }

        Console.WriteLine($"Book with ID {input} borrowed successfully by {userName}!");

    }

    public static void ReturnBook()
    {
        if (!SessionService.IsLoggedIn())
        {
            Console.WriteLine("You must be logged in to return books!");
            return;
        }

        // Kullanıcının ödünç aldığı kitapları göster
        ShowBorrowedBooksByUser();
        
        Console.WriteLine("\nEnter Book ID or Name to return:");
        string? input = Console.ReadLine();
        
        if (string.IsNullOrEmpty(input))
        {
            Console.WriteLine("Input cannot be empty!");
            return;
        }

        int userId = SessionService.GetCurrentUserId();
        string userName = SessionService.GetCurrentUserName();
        
        Books? bookToReturn = null;
        
        // ID ile dene
        if (int.TryParse(input, out int bookId))
        {
            bookToReturn = booksList.FirstOrDefault(b => b.Id == bookId);
        }
        else
        {
            // İsim ile ara
            bookToReturn = booksList.FirstOrDefault(b => 
                b.Name.Equals(input, StringComparison.OrdinalIgnoreCase));
        }
        
        if (bookToReturn == null)
        {
            Console.WriteLine($"Book '{input}' not found!");
            return;
        }
        
        // Kitabın bu kullanıcı tarafından ödünç alınıp alınmadığını kontrol et
        if (!IsBookBorrowedByUser(bookToReturn.Id, userId))
        {
            Console.WriteLine($"You haven't borrowed the book '{bookToReturn.Name}'!");
            return;
        }
        
        // Return kaydı oluştur
        using (StreamWriter sw = new StreamWriter("borrow.txt", true))
        {
            sw.WriteLine($"{userId}, {bookToReturn.Id}, {DateTime.Now:yyyy-MM-dd HH:mm:ss}, RETURNED, {userName}, {bookToReturn.Name}");
            sw.Flush();
        }
        
        Console.WriteLine($"✅ Book '{bookToReturn.Name}' returned successfully!");
        Console.WriteLine($"📅 Returned on: {DateTime.Now:yyyy-MM-dd HH:mm:ss}");
    }

    private static void ShowBorrowedBooksByUser()
    {
        if (!File.Exists("borrow.txt"))
        {
            Console.WriteLine("No borrow records found.");
            return;
        }

        int currentUserId = SessionService.GetCurrentUserId();
        string[] lines = File.ReadAllLines("borrow.txt");
        Dictionary<int, string> borrowedBooks = new Dictionary<int, string>();
        
        Console.WriteLine("\n📚 Your Borrowed Books:");
        Console.WriteLine("------------------------");
        
        bool hasBorrowedBooks = false;
        
        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 6)
                {
                    if (int.TryParse(parts[0].Trim(), out int userId) && 
                        int.TryParse(parts[1].Trim(), out int bookId) &&
                        userId == currentUserId)
                    {
                        string action = parts[3].Trim().ToUpper();
                        string bookName = parts[5].Trim();
                        
                        if (action == "BORROWED" || action == "BORROW")
                        {
                            borrowedBooks[bookId] = bookName;
                        }
                        else if (action == "RETURNED")
                        {
                            borrowedBooks.Remove(bookId);
                        }
                    }
                }
            }
        }
        
        foreach (var book in borrowedBooks)
        {
            Console.WriteLine($"ID: {book.Key} | Name: {book.Value}");
            hasBorrowedBooks = true;
        }
        
        if (!hasBorrowedBooks)
        {
            Console.WriteLine("You have no borrowed books.");
        }
    }

    private static bool IsBookBorrowedByUser(int bookId, int userId)
    {
        if (!File.Exists("borrow.txt"))
            return false;

        string[] lines = File.ReadAllLines("borrow.txt");
        string lastAction = "";

        foreach (string line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                string[] parts = line.Split(',');
                if (parts.Length >= 4)
                {
                    if (int.TryParse(parts[0].Trim(), out int lineUserId) && 
                        int.TryParse(parts[1].Trim(), out int lineBookId) &&
                        lineUserId == userId && lineBookId == bookId)
                    {
                        lastAction = parts[3].Trim().ToUpper();
                    }
                }
            }
        }

        return lastAction == "BORROWED" || lastAction == "BORROW";
    }

    public static void DeleteBook(string bookId)
    {
        if (!int.TryParse(bookId, out int id))
        {
            Console.WriteLine("Invalid book ID format.");
            return;
        }

        var bookToDelete = booksList.FirstOrDefault(b => b.Id == id);
        if (bookToDelete != null)
        {
            booksList.Remove(bookToDelete);
            SaveBooksToFile();
            Console.WriteLine($"Book with ID {bookId} deleted successfully.");
        }
        else
        {
            Console.WriteLine($"Book with ID {bookId} not found.");
        }
    }

    
}
