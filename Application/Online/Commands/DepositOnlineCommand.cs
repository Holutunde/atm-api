using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Transactions.Commands;
using Application.Users.Commands;
using Application.Users.Queries;
using Domain.Entities;
using Infrastructure.Data;
using MediatR;


namespace Application.Online.Commands
{
    public class DepositOnlineCommand : IRequest<(double? Balance, string ErrorMessage)>
    {
        public string  Email { get; set; }
        public double Amount { get; set; }
    }

    public class DepositOnlineCommandHandler : IRequestHandler<DepositOnlineCommand, (double? Balance, string ErrorMessage)>
    {
        private readonly DataContext _context;

        public DepositOnlineCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<(double? Balance, string ErrorMessage)> Handle(DepositOnlineCommand request, CancellationToken cancellationToken)
        {

            var user = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery { Email = request.Email }, cancellationToken);

            var admin = await new GetAdminByEmailQueryHandler(_context).Handle(new GetAdminByEmailQuery { Email = request.Email }, cancellationToken);


            if (user == null && admin == null)
                return (null, "Unauthorized");

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

            Transaction transaction = new()
            {

                ReceiverAccountNumber = accountNumber,
                Amount = request.Amount,
                TransactionDate = DateTime.UtcNow,
                TransactionType = "Transfer"
            };

            await new CreateTransactionCommandHandler(_context).Handle(new CreateTransactionCommand { Transaction = transaction }, cancellationToken);


            return (user?.Balance ?? admin?.Balance, null);
        }
    }
}

