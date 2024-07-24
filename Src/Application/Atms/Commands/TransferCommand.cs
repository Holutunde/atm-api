using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Common.ResultsModel;
using Application.Transactions.Commands;
using Application.Users.Commands;
using Application.Users.Queries;
using Application.Interfaces;
using MediatR;

namespace Application.Atms.Commands
{
    public class TransferCommand : IRequest<Result>
    {
        public long SenderAccountNumber { get; set; }
        public long ReceiverAccountNumber { get; set; }
        public double Amount { get; set; }
    }

    public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
    {
        private readonly IDataContext _context;

        public TransferCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
        {
            var sender = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.SenderAccountNumber }, cancellationToken);
            var adminSender = sender == null ? await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.SenderAccountNumber }, cancellationToken) : null;

            if (sender == null && adminSender == null)
                return Result.Failure<TransferCommand>("Unauthorized");

            var senderBalance = sender?.Balance ?? adminSender.Balance;

            if (sender?.AccountNumber == request.ReceiverAccountNumber || adminSender?.AccountNumber == request.ReceiverAccountNumber)
            {
                return Result.Failure<TransferCommand>("Sender and receiver account numbers cannot be the same.");
            }

            if (senderBalance < request.Amount)
            {
                return Result.Failure<TransferCommand>("Insufficient balance.");
            }

            var receiverUser = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.ReceiverAccountNumber }, cancellationToken);
            var receiverAdmin = receiverUser == null ? await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.ReceiverAccountNumber }, cancellationToken) : null;

            if (receiverUser == null && receiverAdmin == null)
            {
                return Result.Failure<TransferCommand>("Receiver account not found.");
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

      

            await new CreateTransactionCommandHandler(_context).Handle(new CreateTransactionCommand
            {
                SenderAccountNumber = sender?.AccountNumber ?? adminSender.AccountNumber,
                ReceiverAccountNumber = request.ReceiverAccountNumber,
                Amount = request.Amount,
                TransactionType = "Transfer"
            }, cancellationToken);

            return Result.Success(sender?.Balance ?? adminSender.Balance, "New Balance");
        }
    }
}
