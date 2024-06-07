using ATMAPI.Models;

namespace ATMAPI.Interfaces
{
    public interface ITransaction
    {
        void CheckBalance();
        void DepositMoney();
        void WithdrawMoney();
        void UpdatePin(List<User> Users);
        void TransferMoney(List<User> Users);
    }
}
