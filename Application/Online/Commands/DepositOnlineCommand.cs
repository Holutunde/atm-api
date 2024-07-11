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
    public class DepositOnlineCommand : IRequest<Result>
    {
        public required string  Email { get; set; }
        public double Amount { get; set; }
    }

    public class DepositOnlineCommandHandler : IRequestHandler<DepositOnlineCommand, Result>
    {
        private readonly IDataContext _context;

        public DepositOnlineCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DepositOnlineCommand request, CancellationToken cancellationToken)
        {

            var user = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery { Email = request.Email }, cancellationToken);

            var admin = await new GetAdminByEmailQueryHandler(_context).Handle(new GetAdminByEmailQuery { Email = request.Email }, cancellationToken);


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

      
            await new CreateTransactionCommandHandler(_context).Handle(new CreateTransactionCommand {  ReceiverAccountNumber = accountNumber,
                Amount = request.Amount,
                TransactionType = "Deposit" }, cancellationToken);


            return Result.Success(user?.Balance ?? admin.Balance, "Deposit successful.");
        }
    }
}

