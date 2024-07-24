using Application.Interfaces;
using Domain.Entities;
using Domain.Enum;

namespace Infrastructure.Services
{
    public class AccountService(IPasswordHasher passwordHasher) : IAccountService
    {
        private readonly IPasswordHasher _passwordHasher = passwordHasher;
        public T CreateAccount<T>(string email, string password, string firstName, string lastName, int pin, Roles role, string roleDesc) where T : BaseEntity, new()
        {
            Random random = new();
            T account = new T
            {
                Email = email,
                Password = _passwordHasher.HashPassword(password),
                FirstName = firstName,
                LastName = lastName,
                Pin = pin,
                Balance = 0,
                AccountNumber = (long)(random.NextDouble() * 9000000000L) + 1000000000L,
                OpeningDate = DateTime.Now,
                Role = role,
                RoleDesc =  roleDesc
            };

            return account;
        }
    }
}