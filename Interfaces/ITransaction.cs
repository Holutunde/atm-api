using ATMAPI.Models;

namespace ATMAPI.Interfaces
{
    public interface ITransaction
    {
        void CheckBalance();
        void DepositMoney();
        void WithdrawMoney();
        void UpdatePin(List<Account> accounts);
        void TransferMoney(List<Account> accounts);
    }
}
