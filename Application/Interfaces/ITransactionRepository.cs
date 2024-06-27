using Domain.Entities;

namespace Application.Interfaces
{
    public interface ITransactionRepository
    {
        Task AddTransaction(Transaction transaction);
    }
}