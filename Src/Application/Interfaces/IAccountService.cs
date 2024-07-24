
using Domain.Entities;
using Domain.Enum;

namespace Application.Interfaces
{
    public interface IAccountService
    {
        T CreateAccount<T>(string email, string password, string firstName, string lastName, int pin,Roles role, string roleDesc) where T : BaseEntity, new();
    }
}