using ATMAPI.Models;

namespace ATMAPI.Services
{
    public interface IRegisteredAccountsService
    {
        void AddAccount(Account account); 
        void AddAdmin(Admin account); 
        ICollection<Account> GetAccounts();
        Account GetAccountByNumber(long accountNumber);
        void UpdateAccount(Account  updatedAccount);
        void DeleteAccount(long accountNumber);
    }
}