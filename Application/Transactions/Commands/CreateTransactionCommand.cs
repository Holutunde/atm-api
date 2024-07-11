using Domain.Entities;
using Application.Interfaces;
using MediatR;


namespace Application.Transactions.Commands
{
    public class CreateTransactionCommand : IRequest
    {
        public long? SenderAccountNumber { get; set; }
        public long? ReceiverAccountNumber { get; set; }
        public double Amount { get; set; }
        public DateTime TransactionDate { get; set; } = DateTime.UtcNow;
        public string TransactionType { get; set; }
    }

    public class CreateTransactionCommandHandler(IDataContext context) : IRequestHandler<CreateTransactionCommand>
    {
        public async Task<Unit> Handle(CreateTransactionCommand request, CancellationToken cancellationToken)
        {
            var newTransaction = new Transaction
            {
                SenderAccountNumber = request.SenderAccountNumber,
                ReceiverAccountNumber = request.ReceiverAccountNumber,
                Amount = request.Amount,
                TransactionDate = request.TransactionDate,
                TransactionType = request.TransactionType
            };

            context.Transactions.Add(newTransaction);
            await context.SaveChangesAsync(cancellationToken);

            return Unit.Value;
        }
    }
}