using System;
using System.Security.Cryptography;
using System.Text;

namespace AuthServiceSGC.Domain.Utilities
{
    public static class EncryptedPassword
    {
        // Hashes the password using SHA256
        public static string Hash(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = Encoding.UTF8.GetBytes(password);
                var hash = sha256.ComputeHash(bytes);
                return Convert.ToBase64String(hash);
            }
        }

        // Verifies the provided password against the hashed password
        public static bool Verify(string password, string hashedPassword)
        {
            var hashedInput = Hash(password);
            return hashedInput == hashedPassword;
        }
    }
}
