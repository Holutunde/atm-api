using Application.Admins.Queries;
using Application.Users.Commands;
using Application.Users.Queries;
using Infrastructure.Data;
using MediatR;

namespace Application.Online.Commands
{
    public class ChangePinOnlineCommand : IRequest<string>
    {
        public string Email { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangePinOnlineCommandHandler : IRequestHandler<ChangePinOnlineCommand, string>
    {
        private readonly DataContext _context;

        public ChangePinOnlineCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<string> Handle(ChangePinOnlineCommand request, CancellationToken cancellationToken)
        {
            var user = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery { Email = request.Email }, cancellationToken);

            var admin = await new GetAdminByEmailQueryHandler(_context).Handle(new GetAdminByEmailQuery { Email = request.Email }, cancellationToken);


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
