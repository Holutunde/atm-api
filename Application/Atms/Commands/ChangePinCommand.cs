using Application.Admins.Queries;
using Application.Users.Commands;
using Application.Users.Queries;
using Infrastructure.Data;
using MediatR;

namespace Application.Atms.Commands
{
    public class ChangePinCommand : IRequest<string>
    {
        public long AccountNumber { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangePinCommandHandler : IRequestHandler<ChangePinCommand, string>
    {
        private readonly DataContext _context;

        public ChangePinCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(ChangePinCommand request, CancellationToken cancellationToken)
        {
            var user = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);

            var admin = user == null ? await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken) : null;

            if (user == null && admin == null)
                return "Unauthorized";

            if (user != null)
            {
                await new ChangeUserPinCommandHandler(_context).Handle(new ChangeUserPinCommand{ Id = user.Id, NewPin = request.NewPin }, cancellationToken);
            }
            else if (admin != null)
            {
                await new ChangeAdminPinCommandHandler(_context).Handle(new ChangeAdminPinCommand { Id = user.Id, NewPin = request.NewPin }, cancellationToken);
            }

            return null;
        }
    }
}
