using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace MoneyBanks.Service
{
    internal class PasswordHelper
    {
        public static (string Hash, string Salt) HashPasswordWithSalt(string password)
        {
            // Generate a random salt
            using (var rng = new RNGCryptoServiceProvider())
            {
                byte[] salt = new byte[16];
                rng.GetBytes(salt);

                // Hash the password with the salt
                using (var sha256 = SHA256.Create())
                {
                    byte[] passwordBytes = Encoding.UTF8.GetBytes(password);
                    byte[] saltedPassword = new byte[passwordBytes.Length + salt.Length];
                    passwordBytes.CopyTo(saltedPassword, 0);
                    salt.CopyTo(saltedPassword, passwordBytes.Length);

                    byte[] hash = sha256.ComputeHash(saltedPassword);
                    return (Convert.ToBase64String(hash), Convert.ToBase64String(salt));
                }
            }
        }

        public static bool VerifyPassword(string enteredPassword, string storedHash, string storedSalt)
        {
            // Recreate the hash from the entered password and stored salt
            byte[] salt = Convert.FromBase64String(storedSalt);
            using (var sha256 = SHA256.Create())
            {
                byte[] enteredPasswordBytes = Encoding.UTF8.GetBytes(enteredPassword);
                byte[] saltedPassword = new byte[enteredPasswordBytes.Length + salt.Length];
                enteredPasswordBytes.CopyTo(saltedPassword, 0);
                salt.CopyTo(saltedPassword, enteredPasswordBytes.Length);

                byte[] enteredHash = sha256.ComputeHash(saltedPassword);
                string enteredHashString = Convert.ToBase64String(enteredHash);

                // Compare the entered hash with the stored hash
                return enteredHashString == storedHash;
            }
        }
    }
}
