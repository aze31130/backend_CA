using System;
using System.Security.Cryptography;
using System.Text;

namespace backend_CA.Utils
{
    public class HashUtils
    {
        //Length of the random salt
        public const int SALT_LENGTH = 10;

        //-----
        //Generate a dual hashed layer password to register into the database
        //-----
        public static string HashPassword(string inputPassword, string salt)
        {
            return (getSHA256Hash(getSHA256Hash(inputPassword + salt) + salt));
        }

        //-----
        //Authenticates a given password with a hashed one
        //-----
        public static bool AuthPassword(string inputPassword, string hashedPassword, string salt)
        {
            return (getSHA256Hash(getSHA256Hash(inputPassword + salt) + salt).Equals(hashedPassword));
        }

        //-----
        //Calculates the SHA256 hash of a string
        //-----
        public static string getSHA256Hash(string password)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(password));
                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < bytes.Length; i++)
                {
                    builder.Append(bytes[i].ToString("x2"));
                }
                return builder.ToString();
            }
        }

        //-----
        //Generates a random string
        //-----
        public static string getRandomSalt(int lenght)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            char[] saltBuilder = new char[lenght];
            Random random = new Random();
            for (int i = 0; i < saltBuilder.Length; i++)
            {
                saltBuilder[i] = chars[random.Next(chars.Length)];
            }
            return new String(saltBuilder);
        }
    }
}
