
using Domain.Entities;

namespace Application.Interfaces
{
    public interface IAccountFactory
    {
        T CreateAccount<T>(string email, string password, string firstName, string lastName, int pin, string role) where T : IAccount, new();
    }
}