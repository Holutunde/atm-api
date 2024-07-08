using Application.Admins.Queries;
using Application.Users.Queries;
using Infrastructure.Data;
using Infrastructure.Services;
using MediatR;

namespace Application.Online.Commands
{
    public class CheckBalanceOnlineCommand : IRequest<(double? Balance, string ErrorMessage)>
    {
        public required string Email { get; set; }
    }

    public class CheckBalanceCommandHandler : IRequestHandler<CheckBalanceOnlineCommand, (double? Balance, string ErrorMessage)>
    {
        private readonly DataContext _context;


        public CheckBalanceCommandHandler(DataContext context)
        {
            _context = context;

        }

        public async Task<(double? Balance, string ErrorMessage)> Handle(CheckBalanceOnlineCommand request, CancellationToken cancellationToken)
        {

            var user = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery { Email = request.Email }, cancellationToken);
            if (user != null)
            {
                return (user.Balance, null);
            }

            var admin = await new GetAdminByEmailQueryHandler(_context).Handle(new GetAdminByEmailQuery { Email = request.Email }, cancellationToken);

            if (admin != null)
            {
                return (admin.Balance, null);

            }
            

            return (null, "Unauthorized");
        }
    }
}


