using Application.Interfaces;
using Domain.Entities;

namespace Infrastructure.Services
{
    public class AccountFactory(IPasswordHasher passwordHasher) : IAccountFactory
    {
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        public T CreateAccount<T>(string email, string password, string firstName, string lastName, int pin, string role) where T : IAccount, new()
        {
            Random random = new();
            var account = new T
            {
                Email = email,
                Password = _passwordHasher.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Pin = pin,
                Balance = 0,
                AccountNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L,
                OpeningDate = DateTime.Now,
                Role = role
            };

            return account;
        }
    }
}