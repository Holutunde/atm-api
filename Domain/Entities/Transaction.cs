
namespace Domain.Entities
{
    public class Transaction
    {
        public int Id { get; set; }
        public long SenderAccountNumber { get; set; }
        public long ReceiverAccountNumber { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; }
        public string TransactionType { get; set; }
    }
}