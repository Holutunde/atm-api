using MediatR;
using Infrastructure.Data;


namespace Application.Users.Commands
{
    public class ChangeAdminPinCommand : IRequest<int>
    {
        public int Id { get; set; }
        public int NewPin { get; set; }
    }

    public class ChangeAdminPinCommandHandler : IRequestHandler<ChangeAdminPinCommand, int>
    {
        private readonly DataContext _context;

        public ChangeAdminPinCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<int> Handle(ChangeAdminPinCommand request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.FindAsync(request.Id);
            if (admin != null)
            {
                admin.Pin = request.NewPin;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return admin.Pin;
        }
    }
}