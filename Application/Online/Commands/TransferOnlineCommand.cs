using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Atms.Commands;
using Application.Common.ResultsModel;
using Application.Transactions.Commands;
using Application.Users.Commands;
using Application.Users.Queries;
using Application.Interfaces;
using MediatR;

namespace Application.Online.Commands
{
    public class TransferOnlineCommand : IRequest<Result>
    {
        public string SenderEmail { get; set; }
        public long ReceiverAccountNumber { get; set; }
        public double Amount { get; set; }
    }

    public class TransferOnlineCommandHandler : IRequestHandler<TransferOnlineCommand, Result>
    {
        private readonly IDataContext _context;

        public TransferOnlineCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(TransferOnlineCommand request, CancellationToken cancellationToken)
        {
            var sender = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery { Email = request.SenderEmail }, cancellationToken);
            var adminSender = sender == null ? await new GetAdminByEmailQueryHandler(_context).Handle(new GetAdminByEmailQuery { Email = request.SenderEmail }, cancellationToken) : null;

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
                await new UpdateUserBalanceCommandHandler(_context).Handle(new UpdateUserBalanceCommand { Id = sender.Id, NewBalance = sender.Balance - request.Amount }, cancellationToken);
            }
            else if (adminSender != null)
            {
                await new UpdateAdminBalanceCommandHandler(_context).Handle(new UpdateAdminBalanceCommand { Id = adminSender.Id, NewBalance = adminSender.Balance - request.Amount }, cancellationToken);
            }

            if (receiverUser != null)
            {
                await new UpdateUserBalanceCommandHandler(_context).Handle(new UpdateUserBalanceCommand { Id = receiverUser.Id, NewBalance = receiverUser.Balance + request.Amount }, cancellationToken);
            }
            else if (receiverAdmin != null)
            {
                await new UpdateAdminBalanceCommandHandler(_context).Handle(new UpdateAdminBalanceCommand { Id = receiverAdmin.Id, NewBalance = receiverAdmin.Balance + request.Amount }, cancellationToken);
            }
           

            await new CreateTransactionCommandHandler(_context).Handle(new CreateTransactionCommand  {
                SenderAccountNumber = sender?.AccountNumber ?? adminSender.AccountNumber,
                ReceiverAccountNumber = request.ReceiverAccountNumber,
                Amount = request.Amount,
                TransactionType = "Transfer"
            }, cancellationToken);

            return Result.Success(sender?.Balance ?? adminSender.Balance);
        }
    }
}
