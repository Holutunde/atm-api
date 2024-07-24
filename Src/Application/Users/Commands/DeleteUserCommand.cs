using Application.Common.ResultsModel;
using Application.Interfaces;
using MediatR;

namespace Application.Users.Commands
{
    public class DeleteUserCommand : IRequest<Result>
    {
        public int Id { get; set; }
    }
    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, Result>
    {
        private readonly IDataContext _context;

        public DeleteUserCommandHandler(IDataContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);

            if (user == null)
            {
                return Result.Failure<DeleteUserCommand>($"User with ID {request.Id} not found.");
            }

            _context.Users.Remove(user);
            await _context.SaveChangesAsync(cancellationToken);
            return Result.Success($"User with ID {request.Id} deleted successfully.");
        }
    }

}
