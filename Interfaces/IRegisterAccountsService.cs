using ATMAPI.Models;

namespace ATMAPI.Services
{
    public interface IRegisteredAccountsService
    {
        void AddAccount(Account account); 
        ICollection<Account> GetAccounts();
        Account GetAccountByNumber(long accountNumber);
        void UpdateAccount(Account  updatedAccount);


    }
}