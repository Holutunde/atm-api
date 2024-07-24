using Domain.Entities;

namespace Infrastructure.Helpers
{
    public static class TransactionExtension
    {
        public static Transaction CreateTransaction(long senderAccountNumber, long receiverAccountNumber, double amount, string transactionType)
        {
            return new Transaction()
            {
                SenderAccountNumber = senderAccountNumber,
                ReceiverAccountNumber = receiverAccountNumber,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                TransactionType = transactionType
            };
        }
        
        public static Transaction CreateDepositTransaction(long receiverAccountNumber, double amount, string transactionType)
        {
            return new Transaction()
            {
                ReceiverAccountNumber = receiverAccountNumber,
                Amount = amount,
                TransactionDate = DateTime.UtcNow,
                TransactionType = transactionType
            };
        }
    }
}