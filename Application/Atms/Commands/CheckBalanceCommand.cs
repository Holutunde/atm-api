using Application.Admins.Queries;
using Application.Users.Queries;
using Infrastructure.Data;
using MediatR;

namespace Application.Atms.Commands
{
    public class CheckBalanceCommand : IRequest<(double? Balance, string ErrorMessage)>
    {
        public long AccountNumber { get; set; }
    }

    public class CheckBalanceCommandHandler : IRequestHandler<CheckBalanceCommand, (double? Balance, string ErrorMessage)>
    {
        private readonly DataContext _context;

        public CheckBalanceCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<(double? Balance, string ErrorMessage)> Handle(CheckBalanceCommand request, CancellationToken cancellationToken)
        {
            var user = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);
            if (user != null)
            {
                return (user.Balance, null);
            }

            var admin = await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);

            if (admin != null)
            {
                return (admin.Balance, null);
            }

            return (null, "Unauthorized");
        }
    }
}
