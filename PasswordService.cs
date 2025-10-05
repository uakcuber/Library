using System;
using System.Security.Cryptography;
using System.Text;
using Konscious.Security.Cryptography;

namespace Library
{
    public class PasswordService
    {
        private static readonly int SaltSize = 16; // 128 bit salt
        private static readonly int HashSize = 32; // 256 bit hash
        private static readonly int Iterations = 4;
        private static readonly int MemorySize = 1024 * 1024; // 1 GB
        private static readonly int Parallelism = 8;

        /// <summary>
        /// Şifreyi Argon2 ile hash'ler ve salt ile birlikte döner
        /// </summary>
        /// <param name="password">Hashlenecek şifre</param>
        /// <returns>Base64 formatında: salt$hash</returns>
        public static string HashPassword(string password)
        {
            // Random salt oluştur
            byte[] salt = new byte[SaltSize];
            using (var rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Argon2id ile hash'le
            using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
            {
                argon2.Salt = salt;
                argon2.DegreeOfParallelism = Parallelism;
                argon2.Iterations = Iterations;
                argon2.MemorySize = MemorySize;

                byte[] hash = argon2.GetBytes(HashSize);

                // Salt ve hash'i Base64 ile encode et
                string saltBase64 = Convert.ToBase64String(salt);
                string hashBase64 = Convert.ToBase64String(hash);

                // Format: salt$hash
                return $"{saltBase64}${hashBase64}";
            }
        }

        /// <summary>
        /// Şifreyi doğrular
        /// </summary>
        /// <param name="password">Kontrol edilecek şifre</param>
        /// <param name="hashedPassword">Veritabanındaki hash'lenmiş şifre (salt$hash formatında)</param>
        /// <returns>Şifre doğru mu?</returns>
        public static bool VerifyPassword(string password, string hashedPassword)
        {
            try
            {
                // Hash'lenmiş şifreyi ayır
                var parts = hashedPassword.Split('$');
                if (parts.Length != 2)
                    return false;

                byte[] salt = Convert.FromBase64String(parts[0]);
                byte[] storedHash = Convert.FromBase64String(parts[1]);

                // Girilen şifreyi aynı salt ile hash'le
                using (var argon2 = new Argon2id(Encoding.UTF8.GetBytes(password)))
                {
                    argon2.Salt = salt;
                    argon2.DegreeOfParallelism = Parallelism;
                    argon2.Iterations = Iterations;
                    argon2.MemorySize = MemorySize;

                    byte[] computedHash = argon2.GetBytes(HashSize);

                    // Hash'leri karşılaştır
                    return CompareHashes(storedHash, computedHash);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// İki hash'i güvenli şekilde karşılaştırır (timing attack'a karşı)
        /// </summary>
        private static bool CompareHashes(byte[] hash1, byte[] hash2)
        {
            return CryptographicOperations.FixedTimeEquals(hash1, hash2);
        }
    }
}