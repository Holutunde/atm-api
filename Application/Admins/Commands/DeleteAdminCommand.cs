using Infrastructure.Data;
using MediatR;

namespace Application.Admins.Commands
{
    public class DeleteAdminCommand : IRequest<bool>
    {
        public int Id { get; set; }
    }

    public class DeleteAdminCommandHandler : IRequestHandler<DeleteAdminCommand, bool>
    {
        private readonly DataContext _context;

        public DeleteAdminCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> Handle(DeleteAdminCommand request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.FindAsync(request.Id);

            if (admin != null)
            {
                _context.Admins.Remove(admin);
                await _context.SaveChangesAsync(cancellationToken);
                return true;
            }

            return false;
        }
    }
}