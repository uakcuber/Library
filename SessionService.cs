using System;

namespace Library
{
    public class SessionService
    {
        private static Users? currentUser = null;

        public static void SetCurrentUser(Users user)
        {
            currentUser = user;
        }

        public static Users? GetCurrentUser()
        {
            return currentUser;
        }

        public static int GetCurrentUserId()
        {
            return currentUser?.Id ?? 0;
        }

        public static string GetCurrentUserName()
        {
            return currentUser?.Name ?? "Unknown";
        }

        public static string GetCurrentUserRole()
        {
            return currentUser?.Role ?? "guest";
        }

        public static bool IsAdmin()
        {
            return currentUser?.Role?.Equals("admin", StringComparison.OrdinalIgnoreCase) ?? false;
        }

        public static bool IsLoggedIn()
        {
            return currentUser != null;
        }

        public static void Logout()
        {
            currentUser = null;
        }
    }
}