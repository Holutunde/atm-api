using MediatR;
using Application.Interfaces;
using Application.Common.ResultsModel;


namespace Application.Users.Commands
{
    public class ChangeUserPinCommand : IRequest<Result>
    {
        public int Id { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangeUserPinCommandHandler : IRequestHandler<ChangeUserPinCommand, Result>
    {
        private readonly IDataContext _context;

        public ChangeUserPinCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ChangeUserPinCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);
            if (user == null)
            {
                return Result.Failure<ChangeUserPinCommand>($"User with id {request.Id} not found.");
            }

            user.Pin = request.NewPin;
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(user.Pin);
        }
    }

}