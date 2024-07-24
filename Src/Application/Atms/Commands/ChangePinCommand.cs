using Application.Admins.Commands;
using Application.Admins.Queries;
using Application.Common.ResultsModel;
using Application.Users.Commands;
using Application.Users.Queries;
using Application.Interfaces;
using MediatR;

namespace Application.Atms.Commands
{
    public class ChangePinCommand : IRequest<Result>
    {
        public long AccountNumber { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangePinCommandHandler : IRequestHandler<ChangePinCommand, Result>
    {
        private readonly IDataContext _context;

        public ChangePinCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ChangePinCommand request, CancellationToken cancellationToken)
        {
            
                var user = await new GetUserByAccountNumberQueryHandler(_context).Handle(new GetUserByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken);

                var admin = user == null ? await new GetAdminByAccountNumberQueryHandler(_context).Handle(new GetAdminByAccountNumberQuery { AccountNumber = request.AccountNumber }, cancellationToken) : null;

                if (user == null && admin == null)
                    return Result.Failure<ChangePinCommand>( "Unauthorized");

                if (user != null)
                {
                    await new ChangeUserPinCommandHandler(_context).Handle(new ChangeUserPinCommand { Id = user.Id, NewPin = request.NewPin }, cancellationToken);
                }
                else if (admin != null)
                {
                    _ = await new ChangeAdminPinCommandHandler(_context).Handle(new ChangeAdminPinCommand { Id = admin.Id, NewPin = request.NewPin }, cancellationToken);
                }

                return Result.Success("PIN changed successfully");
            }
        
        }
}
