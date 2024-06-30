using Infrastructure.Data;
using MediatR;

namespace Application.Users.Commands
{
    public class DeleteUserCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteUserCommandHandler : IRequestHandler<DeleteUserCommand, bool>
    {
        private readonly DataContext _context;

        public DeleteUserCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var user = await _context.Users.FindAsync(request.Id);

            if (user != null)
            {
                _context.Users.Remove(user);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }
    }
}
