using System;
using System.Security.Cryptography;
using System.Text;
using Application.Interfaces;

namespace Infrastructure.Services
{
    public class PasswordService: IPasswordHasher
    {
        public string HashPassword(string password)
        {
            var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(password));
            StringBuilder builder = new();
            foreach (byte b in bytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        public  bool VerifyPassword(string enteredPassword, string storedHashedPassword)
        {
            var hashedPassword = HashPassword(enteredPassword);
            return hashedPassword.Equals(storedHashedPassword);
        }
    }
}