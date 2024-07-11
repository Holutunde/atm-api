using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Common.ResultsModel;
using Application.Users.Commands;
using Application.Users.Queries;
using Application.Interfaces;
using MediatR;

namespace Application.Online.Commands
{
    public class ChangePinOnlineCommand : IRequest<Result>
    {
        public required string Email { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangePinOnlineCommandHandler(IDataContext context) : IRequestHandler<ChangePinOnlineCommand, Result>
    {
        private readonly IDataContext _context = context;

        public async Task<Result> Handle(ChangePinOnlineCommand request, CancellationToken cancellationToken)
        {
            var user = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery { Email = request.Email }, cancellationToken);

            var admin = await new GetAdminByEmailQueryHandler(_context).Handle(new GetAdminByEmailQuery { Email = request.Email }, cancellationToken);


            if (user == null && admin == null)
                return Result.Failure<ChangePinOnlineCommand>("Unauthorized");

            if (user != null)
            {
                await new ChangeUserPinCommandHandler(_context).Handle(new ChangeUserPinCommand{ Id = user.Id, NewPin = request.NewPin }, cancellationToken);
            }
            else if (admin != null)
            {
                await new ChangeAdminPinCommandHandler(_context).Handle(new ChangeAdminPinCommand { Id = user.Id, NewPin = request.NewPin }, cancellationToken);
            }

            return Result.Success("PIN changed successfully");
        }
    }
}
