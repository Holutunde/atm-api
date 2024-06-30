using Infrastructure.Data;
using MediatR;

namespace Application.Admins.Commands
{
    public class UpdateAdminBalanceCommand : IRequest<double>
    {
        public int Id { get; set; }
        public double NewBalance { get; set; }
    }

    public class UpdateAdminBalanceCommandHandler : IRequestHandler<UpdateAdminBalanceCommand, double>
    {
        private readonly DataContext _context;

        public UpdateAdminBalanceCommandHandler(DataContext context)
        {
            _context = context;
        }

        public async Task<double> Handle(UpdateAdminBalanceCommand request, CancellationToken cancellationToken)
        {
            var admin = await _context.Admins.FindAsync(request.Id);
            if (admin != null)
            {
                admin.Balance = request.NewBalance;
                await _context.SaveChangesAsync(cancellationToken);
            }

            return admin.Balance;
        }
    }
}