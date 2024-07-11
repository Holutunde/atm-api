using Application.Admins.Queries;
using Application.Atms.Commands;
using Application.Common.ResultsModel;
using Application.Users.Queries;

using Application.Interfaces;
using MediatR;

namespace Application.Online.Commands
{
    public class CheckBalanceOnlineCommand : IRequest<Result>
    {
        public required string Email { get; set; }
    }

    public class CheckBalanceCommandHandler : IRequestHandler<CheckBalanceOnlineCommand, Result>
    {
        private readonly IDataContext _context;


        public CheckBalanceCommandHandler(IDataContext context)
        {
            _context = context;

        }

        public async Task<Result> Handle(CheckBalanceOnlineCommand request, CancellationToken cancellationToken)
        {

            var user = await new GetUserByEmailQueryHandler(_context).Handle(new GetUserByEmailQuery { Email = request.Email }, cancellationToken);
            if (user != null)
            {
                return Result.Success(user.Balance, "Balance retrieved successfully.");
            }

            var admin = await new GetAdminByEmailQueryHandler(_context).Handle(new GetAdminByEmailQuery { Email = request.Email }, cancellationToken);

            if (admin != null)
            {

                return Result.Success(admin.Balance, "Balance retrieved successfully.");

            }


            return Result.Failure<CheckBalanceCommand>("Unauthorized");
        }
    }
}


