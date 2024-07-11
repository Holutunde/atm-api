using Application.Common.ResultsModel;
using Application.Admins.Queries;
using Application.Users.Queries;
using Application.Interfaces;
using MediatR;

namespace Application.Atms.Commands
{
    public class CheckBalanceCommand : IRequest<Result>
    {
        public long AccountNumber { get; set; }
    }

    public class CheckBalanceCommandHandler : IRequestHandler<CheckBalanceCommand, Result>
    {
        private readonly IDataContext _context;

        public CheckBalanceCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(CheckBalanceCommand request, CancellationToken cancellationToken)
        {
            var user = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);
            if (user != null)
            {
                return Result.Success(user.Balance, "Balance retrieved successfully.");
            }

            var admin = await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);

            if (admin != null)
            {
                return Result.Success(admin.Balance, "Balance retrieved successfully.");
            }

            return Result.Failure<CheckBalanceCommand>("Unauthorized");
        }
    }
}
