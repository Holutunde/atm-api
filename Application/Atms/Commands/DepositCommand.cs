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
    public class DepositCommand : IRequest<Result>
    {
        public long AccountNumber { get; set; }
        public double Amount { get; set; }
        public string Email { get; internal set; }
    }

    public class DepositCommandHandler : IRequestHandler<DepositCommand, Result>
    {
        private readonly IDataContext _context;

        public DepositCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DepositCommand request, CancellationToken cancellationToken)
        {
            var user = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);
             var admin = await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);

            if (user == null && admin == null)
                return Result.Failure<DepositCommand>("Unauthorized");

            if (user != null)
            {
                user.Balance += request.Amount;
                await new UpdateUserBalanceCommandHandler(_context).Handle(new UpdateUserBalanceCommand { Id = user.Id, NewBalance = user.Balance }, cancellationToken);
            }
            else if (admin != null)
            {
                admin.Balance += request.Amount;
                await new UpdateAdminBalanceCommandHandler(_context).Handle(new UpdateAdminBalanceCommand { Id = admin.Id, NewBalance = admin.Balance }, cancellationToken);
            }

            var accountNumber = user?.AccountNumber ?? admin.AccountNumber;
          

            await new CreateTransactionCommandHandler(_context).Handle(new CreateTransactionCommand   {

                ReceiverAccountNumber = accountNumber,
                Amount = request.Amount,
                TransactionType = "Transfer"
            }, cancellationToken);

            return Result.Success(user?.Balance ?? admin.Balance, "Deposit successful.");
        }
    }
}