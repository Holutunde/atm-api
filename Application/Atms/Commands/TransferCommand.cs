using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Transactions.Commands;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;

namespace Application.Atms.Commands
{
    public class TransferCommand : IRequest<(double SenderBalance, double ReceiverBalance, string ErrorMessage)>
    {
        public long SenderAccountNumber { get; set; }
        public long ReceiverAccountNumber { get; set; }
        public double Amount { get; set; }
    }

    public class TransferCommandHandler : IRequestHandler<TransferCommand, (double SenderBalance, double ReceiverBalance, string ErrorMessage)>
    {
        private readonly DataContext _context;

        public TransferCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<(double SenderBalance, double ReceiverBalance, string ErrorMessage)> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            var sender = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.SenderAccountNumber }, cancellationToken);
            var adminSender = sender == null ? await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.SenderAccountNumber }, cancellationToken) : null;

            if (sender == null && adminSender == null)
                return (0, 0, "Unauthorized");

            var senderBalance = sender?.Balance ?? adminSender.Balance;

            if (sender?.AccountNumber == request.ReceiverAccountNumber || adminSender?.AccountNumber == request.ReceiverAccountNumber)
            {
                return (0, 0, "Sender and receiver account numbers cannot be the same.");
            }

            if (senderBalance < request.Amount)
            {
                return (0, 0, "Insufficient balance.");
            }

            var receiverUser = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.ReceiverAccountNumber }, cancellationToken);
            var receiverAdmin = receiverUser == null ? await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.ReceiverAccountNumber }, cancellationToken) : null;

            if (receiverUser == null && receiverAdmin == null)
            {
                return (0, 0, "Receiver account not found.");
            }

            if (sender != null)
            {
                sender.Balance -= request.Amount;
                await new UpdateUserBalanceCommandHandler(_context).Handle(new UpdateUserBalanceCommand { Id = sender.Id, NewBalance = sender.Balance }, cancellationToken);
            }
            else if (adminSender != null)
            {
                adminSender.Balance -= request.Amount;
                await new UpdateAdminBalanceCommandHandler(_context).Handle(new UpdateAdminBalanceCommand { Id = adminSender.Id, NewBalance = adminSender.Balance }, cancellationToken);
            }

            if (receiverUser != null)
            {
                receiverUser.Balance += request.Amount;
                await new UpdateUserBalanceCommandHandler(_context).Handle(new UpdateUserBalanceCommand { Id = receiverUser.Id, NewBalance = receiverUser.Balance }, cancellationToken);
            }
            else if (receiverAdmin != null)
            {
                receiverAdmin.Balance += request.Amount;
                await new UpdateAdminBalanceCommandHandler(_context).Handle(new UpdateAdminBalanceCommand { Id = receiverAdmin.Id, NewBalance = receiverAdmin.Balance }, cancellationToken);
            }

            Transaction transaction = new()
            {
                SenderAccountNumber = sender?.AccountNumber ?? adminSender.AccountNumber,
                ReceiverAccountNumber = request.ReceiverAccountNumber,
                Amount = request.Amount,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "Transfer"
            };

            await new CreateTransactionCommandHandler(_context).Handle(new CreateTransactionCommand { Transaction = transaction }, cancellationToken);

            return (sender?.Balance ?? adminSender.Balance, receiverUser?.Balance ?? receiverAdmin.Balance, null);
        }
    }
}
