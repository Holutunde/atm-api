using MediatR;
using Application.Interfaces;
using Application.Common.ResultsModel;

namespace Application.Users.Commands
{
    public class UpdateUserBalanceCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public double NewBalance { get; set; }
    }
    public class UpdateUserBalanceCommandHandler : IRequestHandler<UpdateUserBalanceCommand, Result>
    {
        private readonly IDataContext _context;

        public UpdateUserBalanceCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(UpdateUserBalanceCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(new object?[] { request.Id }, cancellationToken: cancellationToken);
            if (user == null)
            {
                return Result.Failure<UpdateUserBalanceCommand>($"User with id {request.Id} not found.");
            }

            user.Balance = request.NewBalance;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(user.Balance, "User balance updated successfully.");
        }
    }

}
